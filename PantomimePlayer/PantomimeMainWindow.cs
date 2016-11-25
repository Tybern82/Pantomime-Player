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
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(frmPantomime));

        private SFXPlayerControl player;

        public frmPantomime() {
            InitializeComponent();
            tCueList.CanExpandGetter = delegate (object o) {
                return (o is CueGroup);
            };
            tCueList.ChildrenGetter = delegate (object o) {
                CueGroup g = o as CueGroup;
                return g.cueItems.Values;
            };
            tCueList.ParentGetter = delegate (object o) {
                return player.currentShow.getParent(o as Cue);
            };


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
                    if (original.FullName == dlgSave.FileName) return original; // don't copy over itself
                    FileInfo nFile = new FileInfo(dlgSave.FileName);
                    if (nFile.Exists) {
                        try {
                            deleteFile(nFile);
                        } catch (IOException) {
                            _displayMessage("Unable to save this sound over the existing file.", "Overwrite Failed");
                            return original;
                        }
                    }
                    FileInfo _result = original.CopyTo(dlgSave.FileName, true);
                    _result.Attributes = FileAttributes.Normal;
                    return _result;
                } catch (Exception) {
                    return original;
                }
            } else {
                return original;
            }
        }

        public void addRegisteredEffect(RegisteredEffect row) {
            lock (lstSoundEffects_lock) {
                // datRegisteredEffects.Rows.Add(row.toRow());
                lstSoundEffects.AddObject(row);
            }
        }

        public void updateRegisteredEffects() {
            lock (lstSoundEffects_lock) {
                foreach (RegisteredEffect r in player.currentShow.getRegisteredEffects()) lstSoundEffects.UpdateObject(r);
                /*
                datRegisteredEffects.Rows.Clear();
                foreach (RegisteredEffect r in player.currentShow.getRegisteredEffects()) {
                    datRegisteredEffects.Rows.Add(r.toRow());
                    lstSoundEffects.AddObject(r);
                }
                */
            }
        }

        public void updateCues() {
            lock (tCueList_lock) {
                // tCueList.UpdateObject(player.currentShow.rootCues);
                tCueList.ClearObjects();
                foreach (Cue c in player.currentShow.getRootCues()) {
                    tCueList.UpdateObject(c);
                }
                tCueList.ExpandAll();
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

        public object lstSoundEffects_lock = new object();
        public object tCueList_lock = new object();

        private void bRemoveEffect_Click(Object sender, EventArgs e) {
            lock (lstSoundEffects_lock) {
                var selected = lstSoundEffects.SelectedObjects;
                foreach (RegisteredEffect eff in selected) {
                    player.currentShow.updateEffect(eff.SourceID, null);
                    lstSoundEffects.RemoveObject(eff);
                    // TODO: Delete file being removed from local storage
                    // logger.Debug("Deleting stored SFX: <" + eff.Filename + ">");
                    // eff.fx.close();
                    // deleteFile(new FileInfo(RegisteredEffect.getAbsolutePath(eff.Filename)));
                }
                // updateRegisteredEffects();
                /*
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
                */
            }
        }

        private void bRenumber_Click(Object sender, EventArgs e) {
            lock (lstSoundEffects_lock) {
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
            lock (lstSoundEffects_lock) {
                System.Collections.IList selected = lstSoundEffects.SelectedObjects;
                if (selected.Count == 0) {
                    _displayMessage("No cue selected to play.", "No Row Selected");
                } else {
                    List<RegisteredEffect> playlist = new List<RegisteredEffect>(selected.Count);
                    foreach (object o in selected) playlist.Add((RegisteredEffect)o);
                    player.onPlayCueCollection(playlist);
                    /*
                    List<uint> idsToPlay = new List<uint>();
                    foreach (DataGridViewRow row in datRegisteredEffects.Rows) {
                        if (datRegisteredEffects.SelectedRows.Contains(row)) idsToPlay.Add((uint)row.Cells[0].Value);
                    }
                    player.onPlayCueCollection(idsToPlay);
                    */
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
            if (targetFile.Exists) {
                targetFile.Attributes = FileAttributes.Normal;
                targetFile.Delete();
            }
        }

        private uint _nextCueID = 0;
        private uint getCueID() {
            while (player.currentShow.isCue(_nextCueID)) _nextCueID++;
            return _nextCueID;
        }

        private void insertCue(Cue c) {
            player.currentShow.registerCue(c);
            lock (tCueList_lock) {
                object o = tCueList.SelectedObject;
                if (o != null) {
                    if (o is CueGroup) {
                        // add the new object into the selected group (at the end)
                        ((CueGroup)o).addElement(c);
                        tCueList.UpdateObject(o);
                    } else {
                        CueGroup g = player.currentShow.getParent((Cue)o);
                        g.insertAfter((Cue)o, c);
                        if (g == player.currentShow.rootCues)
                            updateCues();
                        else tCueList.UpdateObject(g);
                    }
                } else {
                    player.currentShow.rootCues.addElement(c);
                    tCueList.AddObject(c);
                }
                if (c is CueGroup) tCueList.Expand(c);
            }
        }

        private void bSilence_Click(Object sender, EventArgs e) {
            SilenceCue c = new SilenceCue();
            c.CueID = getCueID();
            c.Length = SFXUtilities.TimeInSeconds(2);
            insertCue(c);
        }

        private void bSequence_Click(Object sender, EventArgs e) {
            CueGroup g = new CueGroup();
            g.CueID = getCueID();
            g.Type = GroupElementType.SEQUENCE;
            insertCue(g);
        }

        private void bCollection_Click(Object sender, EventArgs e) {
            CueGroup g = new CueGroup();
            g.CueID = getCueID();
            g.Type = GroupElementType.COLLECTION;
            insertCue(g);
        }

        private void bRemoveCue_Click(Object sender, EventArgs e) {
            lock (tCueList_lock) {
                var selected = tCueList.SelectedObjects;
                foreach (Cue c in selected) {
                    player.currentShow.removeCue(c);
                    tCueList.RemoveObject(c);
                }
            }
        }
    }
}