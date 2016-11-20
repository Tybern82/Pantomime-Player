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
            player = new SFXPlayerControl(this);
            cmbIntervalTime.Items.AddRange(Announcements.GetIntervalTimes());
            selectIntervalTime(IntervalTime.I20);
            TS_Tick = new TimeSpan(0, 0, 0, 0, tShow.Interval);
            frmInitialLoad dlg = new frmInitialLoad();
            if (dlg.ShowDialog() != DialogResult.OK) this.DialogResult = DialogResult.Abort;
            else {
                player.onOpenFile(dlg.SelectedShow.FullName);
                player.currentShow.showDetails.Name = "1001 Arabian Nights... and a Matinee";
                player.currentShow.showDetails.Organisation = "ZPAC Theatre";
                player.currentShow.showDetails.FXDesign = "Jeff Sweeney";
            }
            dlg.Dispose();
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
                        _displayMessage("This sound file <" + e.Filename + "> has already been loaded at: " + e.SourceID, "Error loading Sound File");
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
                log4net.LogManager.Shutdown();
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

        public string titleBarText {
            get {
                return this.Text;
            }
            set {
                try {
                    this.Invoke(new Action(() => this.Text = value));
                } catch (InvalidOperationException) {
                    // thrown when the window hasn't been created yet, should therefore be on the GUI thread...
                    this.Text = value;
                }
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
            fDialog.ShowNewFolderButton = false;
            if (fDialog.ShowDialog() == DialogResult.OK) {
                DirectoryInfo dInfo = new DirectoryInfo(fDialog.SelectedPath);
                if (!SFXShowFile.verifyShowFile(dInfo)) MessageBox.Show("Unable to locate a show file at this location.");
                else {
                    player.onOpenFile(dInfo.FullName);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(Object sender, EventArgs e) {
            AboutBox about = new AboutBox();
            about.ShowDialog(this);
        }

        private void bAddEffect_Click(Object sender, EventArgs e) {
            if (dlgOpen.ShowDialog() == DialogResult.OK) {
                string[] files = dlgOpen.FileNames;
                foreach (string str in files) player.onRegisterEffect(str);
            }
        }

        private void effectToolStripMenuItem_Click(Object sender, EventArgs e) {
            if (dlgOpen.ShowDialog() == DialogResult.OK) {
                string[] files = dlgOpen.FileNames;
                foreach (string str in files) player.onRegisterEffect(str);
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

        private void newToolStripMenuItem_Click(Object sender, EventArgs e) {
            fDialog.ShowNewFolderButton = true;
            if (fDialog.ShowDialog() == DialogResult.OK) {
                DirectoryInfo dInfo = new DirectoryInfo(fDialog.SelectedPath);
                if (SFXShowFile.verifyShowFile(dInfo)) {
                    var _result = MessageBox.Show("The requested show exists, do you want to overwrite it?\nSelect 'Yes' to overwrite, 'No' to open the existing show file or 'Cancel' to go back.", "Overwrite?", MessageBoxButtons.YesNoCancel);
                    switch (_result) {
                        case DialogResult.Yes:
                            try {
                                foreach (DirectoryInfo d in dInfo.EnumerateDirectories()) d.Delete(true);
                                foreach (FileInfo f in dInfo.EnumerateFiles()) f.Delete();
                            } catch (Exception) {
                                MessageBox.Show("Unable to erase the existing show files completely. Please delete manually and try again.");
                                break;
                            }
                            goto case DialogResult.No;
                        case DialogResult.No:
                            player.onOpenFile(dInfo.FullName);
                            break;
                    }
                }
            }
        }
    }
}