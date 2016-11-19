using SFXEngine.AudioEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SFXEngine.AudioEngine.Effects;
using SFXPlayer;

namespace PantomimePlayer {

    public partial class frmPantomime : Form, SFXPlayerGUI {

        private SFXPlayerControl player;

        public frmPantomime() {
            InitializeComponent();
            // player = new SFXPlayer(_displayMessage, statusBarUpdateTime, statusBarUpdateFinished);
            player = new SFXPlayerControl(this);
            cmbIntervalTime.Items.AddRange(Announcements.GetIntervalTimes());
            selectIntervalTime(IntervalTime.I20);
            TS_Tick = new TimeSpan(0, 0, 0, 0, tShow.Interval);

            if (dlgOpen.ShowDialog() == DialogResult.OK) {
                player.onRegisterEffect(dlgOpen.FileName);
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

        private SortedDictionary<uint, RegisteredEffect> registeredEffects = new SortedDictionary<uint, RegisteredEffect>();
        private object registeredEffects_lock = new object();

        public bool isRegisteredEffect(uint regID) {
            lock (registeredEffects_lock) {
                return registeredEffects.ContainsKey(regID);
            }
        }

        public RegisteredEffect getRegisteredEffect(uint regID) {
            lock (registeredEffects_lock) {
                return registeredEffects[regID];
            }
        }

        private uint _nextEffect = 0;
        public long registerEffect(SoundFile fx) {
            lock (registeredEffects_lock) {
                // first check the effect is not already registered
                foreach (RegisteredEffect e in registeredEffects.Values) {
                    if (e.Filename == fx.filename) {
                        _displayMessage("This sound file has already been loaded at: " + e.SourceID, "Error loading Sound File");
                        return e.SourceID;
                    }
                }
                while (isRegisteredEffect(_nextEffect)) _nextEffect++;
                updateEffect(_nextEffect, fx);
                uint regID = _nextEffect;
                _nextEffect++;
                return regID;
            }
        }

        public bool updateEffect(uint reqIndex, SoundFile fx) {
            lock (registeredEffects_lock) {
                if (isRegisteredEffect(reqIndex)) {
                    if (fx == null) {
                        registeredEffects.Remove(reqIndex);
                    } else {
                        RegisteredEffect row = registeredEffects[reqIndex];
                        row.fx = fx;
                        registeredEffects[reqIndex] = row;
                        datRegisteredEffects.Rows.Clear();
                        foreach (RegisteredEffect r in registeredEffects.Values) {
                            datRegisteredEffects.Rows.Add(r.toRow());
                        }
                    }
                    return true;
                } else {
                    if (fx == null) return false;
                    RegisteredEffect row = new RegisteredEffect(reqIndex, fx);
                    registeredEffects[reqIndex] = row;
                    datRegisteredEffects.Rows.Add(row.toRow());
                    return false;
                }
            }
        }

        private void exitToolStripMenuItem_Click(Object sender, EventArgs e) {
            onExit();
        }

        private bool onExit() {
            bool _result = player.onExit();
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
            bool _result = player.onExit();
            if (!_result) e.Cancel = true;
        }

        private void btnPreshow_Click(Object sender, EventArgs e) {
            player.onBeginPreshow();
        }

        private void bStartShow_Click(Object sender, EventArgs e) {
            player.onStartShow(chkSpecial.Checked, getIntervalTime());
        }

        private void _displayMessage(string msg, string title) {
            MessageBox.Show(msg, title);
        }

        private readonly TimeSpan TS_Tick;

        DisplayMessage SFXPlayerGUI.displayMessage {
            get {
                return _displayMessage;
            }
        }

        public String actionStatusText {
            get {
                return lblCurrentAction.Text;
            }

            set {
                this.Invoke(new Action(() => lblCurrentAction.Text = value));
            }
        }

        public String runtimeStatusText {
            get {
                return lblRunningTime.Text;
            }

            set {
                this.Invoke(new Action(() => lblRunningTime.Text = value));
            }
        }

        public String showRuntimeStatusText {
            get {
                return lblShowRunTime.Text;
            }
            set {
                this.Invoke(new Action(() => lblShowRunTime.Text = value));
            }
        }

        public Boolean enableShowTimer {
            get {
                return tShow.Enabled;
            }

            set {
                this.Invoke(new Action(() => tShow.Enabled = value));
            }
        }

        private void tShow_Tick(Object sender, EventArgs e) {
            player.onTimerTick(TS_Tick);
        }

        private void bStopAll_Click(Object sender, EventArgs e) {
            player.onStopAll();
        }

        private void bEndShow_Click(Object sender, EventArgs e) {
            player.onEndShow();
        }

        private void btnInterval_Click(Object sender, EventArgs e) {
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

        private void bAddEffect_Click(Object sender, EventArgs e) {
            if (dlgOpen.ShowDialog() == DialogResult.OK) {
                player.onRegisterEffect(dlgOpen.FileName);
            }
        }

        private void bRemoveEffect_Click(Object sender, EventArgs e) {
            lock (registeredEffects_lock) {
                if (datRegisteredEffects.SelectedRows.Count == 0) {
                    _displayMessage("Please select a row to remove first.", "No Row Selected");
                } else {
                    List<uint> idsToDelete = new List<uint>();
                    List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in datRegisteredEffects.Rows) {
                        if (datRegisteredEffects.SelectedRows.Contains(row)) {
                            idsToDelete.Add((uint)row.Cells[0].Value);
                            rowsToDelete.Add(row);
                        }
                    }
                    foreach (uint i in idsToDelete) {
                        updateEffect(i, null);
                    }
                    foreach (DataGridViewRow row in rowsToDelete) {
                        datRegisteredEffects.Rows.Remove(row);
                    }
                }
            }
        }

        private void bRenumber_Click(Object sender, EventArgs e) {
            lock (registeredEffects_lock) {
                RegisteredEffect[] effects = registeredEffects.Values.ToArray();
                registeredEffects.Clear();
                _nextEffect = 0;
                long maxID = effects.Length - 1;
                foreach (RegisteredEffect ef in effects) {
                    // First include all the effects which will be inside the new numbering range already
                    // By not renumbering these, we avoid the possibility of numbering a new cue to the same as an existing one,
                    // updating all of the references to point to the first cue registered.
                    if (ef.SourceID <= maxID) {
                        registeredEffects[ef.SourceID] = ef;
                    }
                }
                foreach (RegisteredEffect ef in effects) {
                    // Now we've registered all the effects which are in the existing numbering, register the effects which
                    // are outside this numbering to fill in any gaps.
                    if (ef.SourceID > maxID) {
                        while (isRegisteredEffect(_nextEffect)) _nextEffect++;
                        RegisteredEffect nEffect = new RegisteredEffect(_nextEffect, ef.fx);
                        registeredEffects[nEffect.SourceID] = nEffect;
                        player.updateEffectNumber(ef.SourceID, nEffect.SourceID);
                    }
                }
                datRegisteredEffects.Rows.Clear();
                foreach (RegisteredEffect r in registeredEffects.Values) {
                    datRegisteredEffects.Rows.Add(r.toRow());
                }
            }
        }

        private void bPlayNow_Click(Object sender, EventArgs e) {
            lock (registeredEffects_lock) {
                if (datRegisteredEffects.SelectedRows.Count == 0) {
                    _displayMessage("No cue selected to play.", "No Row Selected");
                } else {
                    List<uint> idsToPlay = new List<uint>();
                    foreach (DataGridViewRow row in datRegisteredEffects.Rows) {
                        if (datRegisteredEffects.SelectedRows.Contains(row)) idsToPlay.Add((uint)row.Cells[0].Value);
                    }
                    player.onPlayCueCollection(idsToPlay);
                }
            }
        }
    }
}