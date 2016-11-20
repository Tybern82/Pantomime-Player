using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFXEngine.Events;
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;
using SFXEngine.AudioEngine.Groups;

namespace SFXPlayer {
    public delegate void DisplayMessage(string message, string title);

    public interface SFXPlayerGUI {
        DisplayMessage displayMessage { get; }

        string actionStatusText { get; set; }
        string runtimeStatusText { get; set; }
        string showRuntimeStatusText { get; set; }
        string titleBarText { get; set; }

        bool enableShowTimer { get; set; }

        bool isRegisteredEffect(uint regID);
        RegisteredEffect getRegisteredEffect(uint regID);
        long registerEffect(SoundFile fx);
        bool updateEffect(uint reqIndex, SoundFile fx);
    }

    public class SFXPlayerControl {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SFXPlayerControl));
        
        public static readonly TimeSpan TS_5Minutes = new TimeSpan(0, 5, 0);
        public static readonly TimeSpan TS_1Second = new TimeSpan(0, 0, 1);
        public static readonly string ShortTimeFormat = @"mm\:ss";
        public static readonly string LongTimeFormat = @"h\:mm\:ss";

        private SFXShowFile _currentShow;
        public SFXShowFile currentShow {
            get { return _currentShow; }
            set { _currentShow = value;  SFXEngineProperties.ShowProperties = value.showDetails; }
        }

        private SFXPlayerGUI gui;
        private TimeSpan totalRuntime = TimeSpan.Zero;

        public SFXPlayerControl(SFXPlayerGUI gui) {
            this.gui = gui;
        }

        public void onBeginPreshow() {
            gui.actionStatusText = "Preshow Audience Warnings...";
            SoundFXSequence seq = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                currentShow.Announcements.PreshowEntry_10,
                // SFXUtilities.GenerateSilence(10),
                SFXUtilities.GenerateSilence(TS_5Minutes),
                currentShow.Announcements.PreshowEntry_5,
                // SFXUtilities.GenerateSilence(10),
                SFXUtilities.GenerateSilence(TS_5Minutes)
            });
            seq.onSample.addEventTrigger(updateStatusTimer);
            seq.onStop.addEventTrigger(statusTimerComplete);
            seq.onStop.addEventTrigger(cue => gui.displayMessage("Ready to begin performance", "Preshow"));
            AudioPlaybackEngine.Instance.play(seq);
        }

        public void onStartShow(bool needSpecialAnnouncement, IntervalTime iTime) {
            gui.actionStatusText = "Show Announcements...";
            gui.enableShowTimer = true;     // activate the show timer
            SoundFX showStart = Announcements.ShowBeginning;
            showStart = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                showStart,
                SFXUtilities.GenerateSilence(2),
                currentShow.Announcements.PreshowInterval(iTime)
            });
            if (needSpecialAnnouncement) {
                showStart = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                    showStart,
                    SFXUtilities.GenerateSilence(2),
                    currentShow.Announcements.SpecialNotice
                });
            }
            showStart.onSample.addEventTrigger(updateStatusTimer);
            showStart.onStop.addEventTrigger(statusTimerComplete);
            AudioPlaybackEngine.Instance.play(showStart);
            // TODO: add trigger action to the onStop of the announcement to trigger selecting the first cue for the show
        }

        public void onTimerTick(TimeSpan TS_Tick) {
            totalRuntime += TS_Tick;
            gui.showRuntimeStatusText = totalRuntime.ToString(LongTimeFormat);
        }

        public void onEndShow() {
            AudioPlaybackEngine.Instance.stop();
            statusTimerComplete(null);
            gui.enableShowTimer = false;
        }

        public long onRegisterEffect(string filename) {
            try {
                SoundFile fx = new SoundFile(filename);
                long _result = gui.registerEffect(fx);
                if (_result == -1) {
                    gui.displayMessage("Unable to register this sound effect: <" + filename + ">", "Error");
                }
                return _result;
            } catch (Exception e) when ((e is UnsupportedAudioException) || (e is System.Runtime.InteropServices.COMException)) {
                gui.displayMessage("Unsupported audio file: <" + filename + ">", "Unable to Open File");
            }
            return -1;
        }

        public void onBeginInterval(IntervalTime iTime) {
            gui.actionStatusText = "Interval...";
            SoundFX fx = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                currentShow.Announcements.BeginInterval(iTime),
                SFXUtilities.GenerateSilence((int)iTime-5, 0),
                currentShow.Announcements.Interval_5MinuteWarning,
                SFXUtilities.GenerateSilence(TS_5Minutes)
            });
            fx.onSample.addEventTrigger(updateStatusTimer);
            fx.onStop.addEventTrigger(statusTimerComplete);
            fx.onStop.addEventTrigger(cue => gui.displayMessage("Interval completed", "Interval"));
            AudioPlaybackEngine.Instance.play(fx);
        }

        public void onOpenFile(string filename) {
            logger.Info("onOpenFile: Requesting to open <" + filename + ">");
            currentShow = new SFXShowFile(filename);
            currentShow.showDetails.onChange.addEventTrigger(updateShowName);
            gui.titleBarText = currentShow.showDetails.Name;
        }

        public void onStopAll() {
            AudioPlaybackEngine.Instance.stop();
            statusTimerComplete(null);
        }

        public bool onExit() {
            // TODO: Prepare and save open file
            return true;
        }

        public void onPlayCueCollection(IEnumerable<uint> cueList) {
            SoundFXCollection cueCollection = new SoundFXCollection();
            foreach (uint i in cueList) {
                RegisteredEffect e = gui.getRegisteredEffect(i);
                if (e != null) cueCollection.addSoundFX(e.fx.dup());
            }
            AudioPlaybackEngine.Instance.play(cueCollection);
        }

        public void updateEffectNumber(uint oNumber, uint nNumber) {
            // TODO: Modify current references to the old index, to match the new index.
        }

        public void updateStatusTimer(SoundFX fx) {
            TimeSpan time = fx.length - fx.currentTime + TS_1Second;
            string timeStr;
            if (fx.length == TimeSpan.Zero) {
                timeStr = "--:--";
            } else if (time.Hours != 0) {
                timeStr = time.ToString(LongTimeFormat);
            } else {
                timeStr = time.ToString(ShortTimeFormat);
            }
            try {
                gui.runtimeStatusText = timeStr;
            } catch (Exception) { } // ignore exceptions in updating the status bar
        }

        public void statusTimerComplete(SoundFX fx) {
            try {
                gui.actionStatusText = "ready";
                gui.runtimeStatusText = "00:00";
            } catch (Exception) { } // ignore exceptions in updating the status bar
        }

        public void updateShowName(string property, object value) {
            if (property == "Name") {
                string name = (string)value;
                gui.titleBarText = name;
            } else if (property == "") {
                SFXShowProperties props = (SFXShowProperties)value;
                gui.titleBarText = props.Name;
            }
        }
    }

    public class RegisteredEffect {
        public uint SourceID { get; private set; }
        public string Filename { get { return fx.filename; } }
        public TimeSpan Length { get { return fx.length; } }
        public string CacheMode { get { return (fx.isCachable ? "Cached" : "Buffered"); } }

        public SoundFile fx { get; set; }

        public RegisteredEffect(uint id, SoundFile fx) {
            this.SourceID = id;
            this.fx = fx;
        }

        public object[] toRow() {
            return new object[] {
                SourceID, Filename, Length, CacheMode
            };
        }
    }
}