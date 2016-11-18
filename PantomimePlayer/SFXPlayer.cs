using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFXEngine.Events;
using SFXEngine.AudioEngine;
using SFXEngine.AudioEngine.Effects;
using SFXEngine.AudioEngine.Groups;

namespace PantomimePlayer {
    public delegate void DisplayMessage(string message, string title);

    public interface SFXPlayerGUI {
        DisplayMessage displayMessage { get; }
        SoundEventCallback updateStatusTimer { get; }
        SoundEventCallback statusTimerComplete { get; }
    }

    public class SFXPlayer {
        public static readonly TimeSpan TS_5Minutes = new TimeSpan(0, 5, 0);
        public static readonly TimeSpan TS_1Second = new TimeSpan(0, 0, 1);
        public static readonly string ShortTimeFormat = @"mm\:ss";
        public static readonly string LongTimeFormat = @"h\:mm\:ss";

        private DisplayMessage msg;
        private SoundEventCallback updateStatusTimer;
        private SoundEventCallback statusTimerComplete;

        public SFXPlayer(DisplayMessage msg, SoundEventCallback update, SoundEventCallback complete) {
            this.msg = msg;
            this.updateStatusTimer = update;
            this.statusTimerComplete = complete;
        }

        public void onBeginPreshow() {
            SoundFXSequence seq = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                Announcements.PreshowEntry_10,
                // SFXUtilities.GenerateSilence(10),
                SFXUtilities.GenerateSilence(TS_5Minutes),
                Announcements.PreshowEntry_5,
                // SFXUtilities.GenerateSilence(10),
                SFXUtilities.GenerateSilence(TS_5Minutes)
            });
            seq.onSample.addEventTrigger(updateStatusTimer);
            seq.onStop.addEventTrigger(statusTimerComplete);
            seq.onStop.addEventTrigger(cue => msg("Ready to begin performance", "Preshow"));
            AudioPlaybackEngine.Instance.play(seq);

        }

        public void onBeginInterval(IntervalTime iTime) {
            SoundFX fx = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                Announcements.BeginInterval(iTime),
                SFXUtilities.GenerateSilence((int)iTime-5, 0),
                Announcements.Interval_5MinuteWarning,
                SFXUtilities.GenerateSilence(TS_5Minutes)
            });
            fx.onSample.addEventTrigger(updateStatusTimer);
            fx.onStop.addEventTrigger(statusTimerComplete);
            fx.onStop.addEventTrigger(cue => msg("Interval completed", "Interval"));
            AudioPlaybackEngine.Instance.play(fx);
        }

        public void onOpenFile(string filename) {
            msg("Requesting to open <" + filename + ">", "Open Show");

            // TODO: Implement opening
        }
    }
}
