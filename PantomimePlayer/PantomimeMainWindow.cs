using SFXEngine.AudioEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SFXEngine.AudioEngine.Effects;
using SFXEngine.AudioEngine.Groups;

namespace PantomimePlayer {

    public partial class frmPantomime : Form {

        private SFXPlayer player;

        public frmPantomime() {
            InitializeComponent();
            player = new SFXPlayer(displayMessage, statusBarUpdateTime, statusBarUpdateFinished);
            cmbIntervalTime.Items.AddRange(Announcements.GetIntervalTimes());
            selectIntervalTime(IntervalTime.I20);
            TS_Tick = new TimeSpan(0, 0, 0, 0, tShow.Interval);

            if (dlgOpen.ShowDialog() == DialogResult.OK) {
                SoundFile fx = new SoundFile(dlgOpen.FileName);
                RegisteredEffectRow row = new RegisteredEffectRow(0, fx);
                datRegisteredEffects.Rows.Add(row.toRow());
            } else {
                datRegisteredEffects.Rows.Add(new object[] {
                    0,
                    "SampleFilename.mp3",
                    new TimeSpan(0, 0, 30),
                    "Cached"
                });
            }
        }

        private void selectIntervalTime(IntervalTime i) {
            foreach(object o in cmbIntervalTime.Items) {
                IntervalTimeComboItem curr = (IntervalTimeComboItem)o;
                if (curr.intervalTime == i) cmbIntervalTime.SelectedItem = curr;
            }
        }

        private IntervalTime getIntervalTime() {
            if (cmbIntervalTime.SelectedItem == null) return IntervalTime.I20;
            return ((IntervalTimeComboItem)cmbIntervalTime.SelectedItem).intervalTime;
        }

        private void exitToolStripMenuItem_Click(Object sender, EventArgs e) {
            onExit();
        }

        private bool onExitPrepare() {
            return true;
        }

        private bool onExit() {
            bool _result = onExitPrepare();
            if (_result) {
                // Stop all currently playing audio...
                AudioPlaybackEngine.Instance.stop();
                this.Close();
                this.Dispose();
            }
            return _result;
        }

        private void frmPantomime_FormClosing(Object sender, FormClosingEventArgs e) {
            // TODO: Update to use ShutdownBlockReasonCreate, etc?
                        
            if (e.CloseReason == CloseReason.WindowsShutDown) return;   // if machine is shutting down, just quit
            bool _result = onExitPrepare();
            if (!_result) e.Cancel = true;
        }

        private void btnPreshow_Click(Object sender, EventArgs e) {
            lblCurrentAction.Text = "Preshow Audience Warnings...";
            player.onBeginPreshow();
        }

        private void bStartShow_Click(Object sender, EventArgs e) {
            SoundFX showStart = Announcements.ShowBeginning;
            showStart = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                showStart,
                SFXUtilities.GenerateSilence(2),
                Announcements.PreshowInterval(getIntervalTime())
            });
            if (chkSpecial.Checked) {
                showStart = new SoundFXSequence(AudioPlaybackEngine.Instance.WaveFormat, new SoundFX[] {
                    showStart,
                    SFXUtilities.GenerateSilence(2),
                    Announcements.SpecialNotice_AgathaChristie
                });
            }
            lblCurrentAction.Text = "Show Announcements...";
            showStart.onSample.addEventTrigger(statusBarUpdateTime);
            showStart.onStop.addEventTrigger(statusBarUpdateFinished);
            AudioPlaybackEngine.Instance.play(showStart);
            tShow.Enabled = true;
            // TODO: add trigger action to the onStop of the announcement to trigger selecting the first cue for the show
        }

        private void displayMessage(string msg, string title) {
            MessageBox.Show(msg, title);
        }

        private void statusBarUpdateTime(SoundFX fx) {
            TimeSpan time = fx.length - fx.currentTime + SFXPlayer.TS_1Second;
            string timeStr;
            if (time.Hours != 0) {
                timeStr = time.ToString(SFXPlayer.LongTimeFormat);
            } else {
                timeStr = time.ToString(SFXPlayer.ShortTimeFormat);
            }
            try {
                this.Invoke(new Action(() => lblRunningTime.Text = timeStr));
            } catch (Exception) {}  // ignore exceptions in updating the status bar
        }

        private void statusBarUpdateFinished(SoundFX fx) {
            try {
                this.Invoke(new Action(() => {
                    lblRunningTime.Text = "00:00";
                    lblCurrentAction.Text = "ready";
                }));
            } catch (Exception) {}  // ignore exceptions updating the status bar
        }

        private TimeSpan totalRuntime = TimeSpan.Zero;
        private readonly TimeSpan TS_Tick;

        private void tShow_Tick(Object sender, EventArgs e) {
            totalRuntime += TS_Tick;
            lblShowRunTime.Text = totalRuntime.ToString(SFXPlayer.LongTimeFormat);
        }

        private void bStopAll_Click(Object sender, EventArgs e) {
            AudioPlaybackEngine.Instance.stop();
            statusBarUpdateFinished(null);
        }

        private void bEndShow_Click(Object sender, EventArgs e) {
            AudioPlaybackEngine.Instance.stop();
            statusBarUpdateFinished(null);
            tShow.Enabled = false;
        }

        private void btnInterval_Click(Object sender, EventArgs e) {
            lblCurrentAction.Text = "Interval...";
            player.onBeginInterval(getIntervalTime());
        }

        private void openToolStripMenuItem_Click(Object sender, EventArgs e) {
            DialogResult _result = dlgFolderBrowser.ShowDialog();
            if (_result == DialogResult.OK) player.onOpenFile(dlgFolderBrowser.SelectedPath);
        }

        private void aboutToolStripMenuItem_Click(Object sender, EventArgs e) {
            AboutBox about = new AboutBox();
            about.ShowDialog(this);
        }
    }

    class RegisteredEffectRow {
        public uint SourceID;
        public string Filename { get { return fx.filename; } }
        public TimeSpan Length { get { return fx.length; } }
        public string CacheMode { get { return (fx.isCachable ? "Cached" : "Buffered"); } }

        public SoundFile fx { get; private set; }

        public RegisteredEffectRow(uint id, SoundFile fx) {
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
