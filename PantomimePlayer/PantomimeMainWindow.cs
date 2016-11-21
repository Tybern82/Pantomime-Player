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

        private FileInfo _saveSoundFile(DirectoryInfo baseDir, FileInfo original) {
            if (!original.Exists) return null;  // original file does not exist... cannot save it
            dlgSave.InitialDirectory = baseDir.FullName;
            dlgSave.FileName = original.Name;
            if (dlgSave.ShowDialog() == DialogResult.OK) {
                try {
                    return original.CopyTo(dlgSave.FileName, true);
                } catch (Exception) {
                    return null;
                }
            } else {
                return null;
            }
        }

        public void addRegisteredEffect(RegisteredEffect row) {
            lock (datRegisteredEffects_lock) {
                datRegisteredEffects.Rows.Add(row.toRow());
            }
        }

        public void updateRegisteredEffects() {
            lock (datRegisteredEffects_lock) {
                datRegisteredEffects.Rows.Clear();
                foreach (RegisteredEffect r in player.currentShow.getRegisteredEffects()) {
                    datRegisteredEffects.Rows.Add(r.toRow());
                }
            }
        }

        private readonly TimeSpan TS_Tick;

        public DisplayMessage displayMessage {
            get {
                return _displayMessage;
            }
        }

        public SaveFile saveSoundFile {
            get {
                return _saveSoundFile;
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

        public object datRegisteredEffects_lock = new object();

        private void bRemoveEffect_Click(Object sender, EventArgs e) {
            lock (datRegisteredEffects_lock) {
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
                        player.currentShow.updateEffect(i, null);
                    }
                    foreach (DataGridViewRow row in rowsToDelete) {
                        datRegisteredEffects.Rows.Remove(row);
                    }
                }
            }
        }

        private void bRenumber_Click(Object sender, EventArgs e) {
            lock (datRegisteredEffects_lock) {
                var effects = player.currentShow.getRegisteredEffects();
                uint _nextEffect = 0;
                long maxID = effects.Count - 1;
                foreach (RegisteredEffect ef in effects) {
                    // Only change cues which are outside the new numbering range
                    if (ef.SourceID > maxID) {
                        while (player.currentShow.isRegisteredEffect(_nextEffect)) _nextEffect++;
                        RegisteredEffect nEffect = new RegisteredEffect(_nextEffect, ef.fx);
                        player.currentShow.updateEffect(ef.SourceID, null);     // remove the old mapping
                        player.currentShow.updateEffect(_nextEffect, ef.fx);    // set the new mapping
                        player.updateEffectNumber(ef.SourceID, nEffect.SourceID);
                    }
                }
                updateRegisteredEffects();
            }
        }

        private void bPlayNow_Click(Object sender, EventArgs e) {
            lock (datRegisteredEffects_lock) {
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
                if (isNotEmpty(dInfo)) {
                    var _result = MessageBox.Show("There are files in the requested location, do you want to overwrite them?\nSelect 'Yes' to overwrite, 'No' to open the existing show file or 'Cancel' to go back.", "Overwrite?", MessageBoxButtons.YesNoCancel);
                    switch (_result) {
                        case DialogResult.Yes:
                            try {
                                // only delete the contents - we're just going to recreate the directory anyway...
                                foreach (DirectoryInfo d in dInfo.EnumerateDirectories()) deleteDirectory(d);
                                foreach (FileInfo f in dInfo.EnumerateFiles()) deleteFile(f);
                            } catch (Exception) {
                                MessageBox.Show("Unable to erase the existing show files completely. Please delete manually and try again.");
                                break;
                            }
                            goto case DialogResult.No;
                        case DialogResult.No:
                            player.onOpenFile(dInfo.FullName);
                            break;
                    }
                } else {
                    player.onOpenFile(dInfo.FullName);
                }
            }
        }

        private bool isNotEmpty(DirectoryInfo dInfo) {
            return (dInfo.Exists && (dInfo.EnumerateDirectories().Count() != 0) && (dInfo.EnumerateFiles().Count() != 0));
        }

        private void deleteDirectory(DirectoryInfo targetDir) {
            targetDir.Attributes = FileAttributes.Normal;
            foreach (FileInfo f in targetDir.EnumerateFiles()) deleteFile(f);
            foreach (DirectoryInfo d in targetDir.EnumerateDirectories()) deleteDirectory(d);
            targetDir.Delete();
        }

        private void deleteFile(FileInfo targetFile) {
            targetFile.Attributes = FileAttributes.Normal;
            targetFile.Delete();
        }
    }
}