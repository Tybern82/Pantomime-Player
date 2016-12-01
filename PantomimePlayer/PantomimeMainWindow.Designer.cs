namespace PantomimePlayer {
    partial class frmPantomime {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.tMainWindow = new System.Windows.Forms.TabControl();
            this.pgEffects = new System.Windows.Forms.TabPage();
            this.lstSoundEffects = new BrightIdeasSoftware.ObjectListView();
            this.cSoundFXID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cFilename = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cMode = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.grpEffectsControls = new System.Windows.Forms.GroupBox();
            this.bPlayNow = new System.Windows.Forms.Button();
            this.bRenumber = new System.Windows.Forms.Button();
            this.bRemoveEffect = new System.Windows.Forms.Button();
            this.bAddEffect = new System.Windows.Forms.Button();
            this.pgCues = new System.Windows.Forms.TabPage();
            this.gCues = new System.Windows.Forms.GroupBox();
            this.bSwitchMode = new System.Windows.Forms.Button();
            this.bRemoveCue = new System.Windows.Forms.Button();
            this.bSequence = new System.Windows.Forms.Button();
            this.bCollection = new System.Windows.Forms.Button();
            this.bSoundFX = new System.Windows.Forms.Button();
            this.bSilence = new System.Windows.Forms.Button();
            this.tCueList = new BrightIdeasSoftware.TreeListView();
            this.cCueID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cCueName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cSourceID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cCueLength = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cCueSeekTo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cFadeIn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cHasAutoFade = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cFadeOut = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cFadeOutDuration = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cVolume = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.mbarMainMenu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.effectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpPreshow = new System.Windows.Forms.GroupBox();
            this.bEndShow = new System.Windows.Forms.Button();
            this.bStartShow = new System.Windows.Forms.Button();
            this.chkSpecial = new System.Windows.Forms.CheckBox();
            this.btnPreshow = new System.Windows.Forms.Button();
            this.cmbIntervalTime = new System.Windows.Forms.ComboBox();
            this.lblIntervalTime = new System.Windows.Forms.Label();
            this.tShow = new System.Windows.Forms.Timer(this.components);
            this.grpInterval = new System.Windows.Forms.GroupBox();
            this.btnInterval = new System.Windows.Forms.Button();
            this.grpControls = new System.Windows.Forms.GroupBox();
            this.bFadeOut = new System.Windows.Forms.Button();
            this.bPlayCue = new System.Windows.Forms.Button();
            this.bStopAll = new System.Windows.Forms.Button();
            this.fDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.sbarMainStatus = new System.Windows.Forms.StatusStrip();
            this.lblShowRunTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTotalTimeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRunningTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCurrentAction = new System.Windows.Forms.ToolStripStatusLabel();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.tMainWindow.SuspendLayout();
            this.pgEffects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstSoundEffects)).BeginInit();
            this.grpEffectsControls.SuspendLayout();
            this.pgCues.SuspendLayout();
            this.gCues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tCueList)).BeginInit();
            this.mbarMainMenu.SuspendLayout();
            this.grpPreshow.SuspendLayout();
            this.grpInterval.SuspendLayout();
            this.grpControls.SuspendLayout();
            this.sbarMainStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // tMainWindow
            // 
            this.tMainWindow.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tMainWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tMainWindow.Controls.Add(this.pgEffects);
            this.tMainWindow.Controls.Add(this.pgCues);
            this.tMainWindow.Location = new System.Drawing.Point(14, 88);
            this.tMainWindow.Margin = new System.Windows.Forms.Padding(5);
            this.tMainWindow.Name = "tMainWindow";
            this.tMainWindow.SelectedIndex = 0;
            this.tMainWindow.ShowToolTips = true;
            this.tMainWindow.Size = new System.Drawing.Size(1176, 438);
            this.tMainWindow.TabIndex = 0;
            // 
            // pgEffects
            // 
            this.pgEffects.BackColor = System.Drawing.SystemColors.Control;
            this.pgEffects.Controls.Add(this.lstSoundEffects);
            this.pgEffects.Controls.Add(this.grpEffectsControls);
            this.pgEffects.Location = new System.Drawing.Point(4, 4);
            this.pgEffects.Name = "pgEffects";
            this.pgEffects.Padding = new System.Windows.Forms.Padding(3);
            this.pgEffects.Size = new System.Drawing.Size(1168, 412);
            this.pgEffects.TabIndex = 0;
            this.pgEffects.Text = "Registered Sound FX";
            // 
            // lstSoundEffects
            // 
            this.lstSoundEffects.AllColumns.Add(this.cSoundFXID);
            this.lstSoundEffects.AllColumns.Add(this.cFilename);
            this.lstSoundEffects.AllColumns.Add(this.cLength);
            this.lstSoundEffects.AllColumns.Add(this.cMode);
            this.lstSoundEffects.AllowColumnReorder = true;
            this.lstSoundEffects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSoundEffects.CellEditUseWholeCell = false;
            this.lstSoundEffects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cSoundFXID,
            this.cFilename,
            this.cLength,
            this.cMode});
            this.lstSoundEffects.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstSoundEffects.FullRowSelect = true;
            this.lstSoundEffects.Location = new System.Drawing.Point(102, 7);
            this.lstSoundEffects.Name = "lstSoundEffects";
            this.lstSoundEffects.ShowItemCountOnGroups = true;
            this.lstSoundEffects.Size = new System.Drawing.Size(1060, 399);
            this.lstSoundEffects.SortGroupItemsByPrimaryColumn = false;
            this.lstSoundEffects.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstSoundEffects.TabIndex = 2;
            this.lstSoundEffects.UseCompatibleStateImageBehavior = false;
            this.lstSoundEffects.UseNotifyPropertyChanged = true;
            this.lstSoundEffects.View = System.Windows.Forms.View.Details;
            // 
            // cSoundFXID
            // 
            this.cSoundFXID.AspectName = "SourceID";
            this.cSoundFXID.Groupable = false;
            this.cSoundFXID.MinimumWidth = 100;
            this.cSoundFXID.Text = "Sound FX ID";
            this.cSoundFXID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.cSoundFXID.Width = 100;
            // 
            // cFilename
            // 
            this.cFilename.AspectName = "Filename";
            this.cFilename.FillsFreeSpace = true;
            this.cFilename.Text = "Filename";
            // 
            // cLength
            // 
            this.cLength.AspectName = "Length";
            this.cLength.AspectToStringFormat = "{0:h\\:mm\\:ss\\.FFF}";
            this.cLength.Groupable = false;
            this.cLength.IsEditable = false;
            this.cLength.MinimumWidth = 100;
            this.cLength.Sortable = false;
            this.cLength.Text = "Total Time";
            this.cLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.cLength.Width = 100;
            // 
            // cMode
            // 
            this.cMode.AspectName = "CacheMode";
            this.cMode.IsEditable = false;
            this.cMode.MinimumWidth = 120;
            this.cMode.Text = "Cached / Buffered";
            this.cMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cMode.Width = 120;
            // 
            // grpEffectsControls
            // 
            this.grpEffectsControls.Controls.Add(this.bPlayNow);
            this.grpEffectsControls.Controls.Add(this.bRenumber);
            this.grpEffectsControls.Controls.Add(this.bRemoveEffect);
            this.grpEffectsControls.Controls.Add(this.bAddEffect);
            this.grpEffectsControls.Location = new System.Drawing.Point(7, 7);
            this.grpEffectsControls.Name = "grpEffectsControls";
            this.grpEffectsControls.Size = new System.Drawing.Size(89, 162);
            this.grpEffectsControls.TabIndex = 1;
            this.grpEffectsControls.TabStop = false;
            this.grpEffectsControls.Text = "Effects";
            // 
            // bPlayNow
            // 
            this.bPlayNow.Location = new System.Drawing.Point(7, 130);
            this.bPlayNow.Name = "bPlayNow";
            this.bPlayNow.Size = new System.Drawing.Size(75, 23);
            this.bPlayNow.TabIndex = 3;
            this.bPlayNow.Text = "Play Now";
            this.bPlayNow.UseVisualStyleBackColor = true;
            this.bPlayNow.Click += new System.EventHandler(this.bPlayNow_Click);
            // 
            // bRenumber
            // 
            this.bRenumber.Location = new System.Drawing.Point(7, 100);
            this.bRenumber.Name = "bRenumber";
            this.bRenumber.Size = new System.Drawing.Size(75, 23);
            this.bRenumber.TabIndex = 2;
            this.bRenumber.Text = "Renumber";
            this.bRenumber.UseVisualStyleBackColor = true;
            this.bRenumber.Click += new System.EventHandler(this.bRenumber_Click);
            // 
            // bRemoveEffect
            // 
            this.bRemoveEffect.Location = new System.Drawing.Point(7, 50);
            this.bRemoveEffect.Name = "bRemoveEffect";
            this.bRemoveEffect.Size = new System.Drawing.Size(75, 23);
            this.bRemoveEffect.TabIndex = 1;
            this.bRemoveEffect.Text = "Remove";
            this.bRemoveEffect.UseVisualStyleBackColor = true;
            this.bRemoveEffect.Click += new System.EventHandler(this.bRemoveEffect_Click);
            // 
            // bAddEffect
            // 
            this.bAddEffect.Location = new System.Drawing.Point(7, 20);
            this.bAddEffect.Name = "bAddEffect";
            this.bAddEffect.Size = new System.Drawing.Size(75, 23);
            this.bAddEffect.TabIndex = 0;
            this.bAddEffect.Text = "Add";
            this.bAddEffect.UseVisualStyleBackColor = true;
            this.bAddEffect.Click += new System.EventHandler(this.bAddEffect_Click);
            // 
            // pgCues
            // 
            this.pgCues.BackColor = System.Drawing.SystemColors.Control;
            this.pgCues.Controls.Add(this.gCues);
            this.pgCues.Controls.Add(this.tCueList);
            this.pgCues.Location = new System.Drawing.Point(4, 4);
            this.pgCues.Name = "pgCues";
            this.pgCues.Padding = new System.Windows.Forms.Padding(3);
            this.pgCues.Size = new System.Drawing.Size(1168, 412);
            this.pgCues.TabIndex = 1;
            this.pgCues.Text = "Sound FX Cues";
            // 
            // gCues
            // 
            this.gCues.Controls.Add(this.bSwitchMode);
            this.gCues.Controls.Add(this.bRemoveCue);
            this.gCues.Controls.Add(this.bSequence);
            this.gCues.Controls.Add(this.bCollection);
            this.gCues.Controls.Add(this.bSoundFX);
            this.gCues.Controls.Add(this.bSilence);
            this.gCues.Location = new System.Drawing.Point(6, 6);
            this.gCues.Name = "gCues";
            this.gCues.Size = new System.Drawing.Size(89, 293);
            this.gCues.TabIndex = 2;
            this.gCues.TabStop = false;
            this.gCues.Text = "Cues";
            // 
            // bSwitchMode
            // 
            this.bSwitchMode.Location = new System.Drawing.Point(7, 163);
            this.bSwitchMode.Name = "bSwitchMode";
            this.bSwitchMode.Size = new System.Drawing.Size(75, 23);
            this.bSwitchMode.TabIndex = 5;
            this.bSwitchMode.Text = "Switch";
            this.bSwitchMode.UseVisualStyleBackColor = true;
            this.bSwitchMode.Click += new System.EventHandler(this.bSwitchMode_Click);
            // 
            // bRemoveCue
            // 
            this.bRemoveCue.Location = new System.Drawing.Point(6, 224);
            this.bRemoveCue.Name = "bRemoveCue";
            this.bRemoveCue.Size = new System.Drawing.Size(75, 23);
            this.bRemoveCue.TabIndex = 4;
            this.bRemoveCue.Text = "Remove";
            this.bRemoveCue.UseVisualStyleBackColor = true;
            this.bRemoveCue.Click += new System.EventHandler(this.bRemoveCue_Click);
            // 
            // bSequence
            // 
            this.bSequence.Location = new System.Drawing.Point(7, 133);
            this.bSequence.Name = "bSequence";
            this.bSequence.Size = new System.Drawing.Size(75, 23);
            this.bSequence.TabIndex = 3;
            this.bSequence.Text = "Sequence";
            this.bSequence.UseVisualStyleBackColor = true;
            this.bSequence.Click += new System.EventHandler(this.bSequence_Click);
            // 
            // bCollection
            // 
            this.bCollection.Location = new System.Drawing.Point(7, 103);
            this.bCollection.Name = "bCollection";
            this.bCollection.Size = new System.Drawing.Size(75, 23);
            this.bCollection.TabIndex = 2;
            this.bCollection.Text = "Collection";
            this.bCollection.UseVisualStyleBackColor = true;
            this.bCollection.Click += new System.EventHandler(this.bCollection_Click);
            // 
            // bSoundFX
            // 
            this.bSoundFX.Location = new System.Drawing.Point(7, 50);
            this.bSoundFX.Name = "bSoundFX";
            this.bSoundFX.Size = new System.Drawing.Size(75, 23);
            this.bSoundFX.TabIndex = 1;
            this.bSoundFX.Text = "Sound FX";
            this.bSoundFX.UseVisualStyleBackColor = true;
            this.bSoundFX.Click += new System.EventHandler(this.bSoundFX_Click);
            // 
            // bSilence
            // 
            this.bSilence.Location = new System.Drawing.Point(7, 20);
            this.bSilence.Name = "bSilence";
            this.bSilence.Size = new System.Drawing.Size(75, 23);
            this.bSilence.TabIndex = 0;
            this.bSilence.Text = "Silence";
            this.bSilence.UseVisualStyleBackColor = true;
            this.bSilence.Click += new System.EventHandler(this.bSilence_Click);
            // 
            // tCueList
            // 
            this.tCueList.AllColumns.Add(this.cCueID);
            this.tCueList.AllColumns.Add(this.cCueName);
            this.tCueList.AllColumns.Add(this.cSourceID);
            this.tCueList.AllColumns.Add(this.cCueLength);
            this.tCueList.AllColumns.Add(this.cCueSeekTo);
            this.tCueList.AllColumns.Add(this.cFadeIn);
            this.tCueList.AllColumns.Add(this.cHasAutoFade);
            this.tCueList.AllColumns.Add(this.cFadeOut);
            this.tCueList.AllColumns.Add(this.cFadeOutDuration);
            this.tCueList.AllColumns.Add(this.cVolume);
            this.tCueList.AllowColumnReorder = true;
            this.tCueList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tCueList.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.tCueList.CellEditUseWholeCell = false;
            this.tCueList.CheckedAspectName = "";
            this.tCueList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cCueID,
            this.cCueName,
            this.cSourceID,
            this.cCueLength,
            this.cCueSeekTo,
            this.cFadeIn,
            this.cHasAutoFade,
            this.cFadeOut,
            this.cFadeOutDuration,
            this.cVolume});
            this.tCueList.Cursor = System.Windows.Forms.Cursors.Default;
            this.tCueList.EmptyListMsg = "No Cues Recorded";
            this.tCueList.FullRowSelect = true;
            this.tCueList.Location = new System.Drawing.Point(101, 7);
            this.tCueList.MultiSelect = false;
            this.tCueList.Name = "tCueList";
            this.tCueList.ShowGroups = false;
            this.tCueList.ShowImagesOnSubItems = true;
            this.tCueList.Size = new System.Drawing.Size(1061, 399);
            this.tCueList.TabIndex = 0;
            this.tCueList.UseCompatibleStateImageBehavior = false;
            this.tCueList.UseNotifyPropertyChanged = true;
            this.tCueList.UseSubItemCheckBoxes = true;
            this.tCueList.View = System.Windows.Forms.View.Details;
            this.tCueList.VirtualMode = true;
            this.tCueList.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.tCueList_CellEditFinishing);
            this.tCueList.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.tCueList_CellEditStarting);
            // 
            // cCueID
            // 
            this.cCueID.AspectName = "CueID";
            this.cCueID.IsEditable = false;
            this.cCueID.IsVisible = false;
            this.cCueID.Text = "Cue ID";
            this.cCueID.Width = 100;
            // 
            // cCueName
            // 
            this.cCueName.AspectName = "Name";
            this.cCueName.FillsFreeSpace = true;
            this.cCueName.Text = "Name";
            // 
            // cSourceID
            // 
            this.cSourceID.AspectName = "";
            this.cSourceID.FillsFreeSpace = true;
            this.cSourceID.MinimumWidth = 80;
            this.cSourceID.Text = "Source ID";
            this.cSourceID.Width = 80;
            // 
            // cCueLength
            // 
            this.cCueLength.AspectName = "Length";
            this.cCueLength.AspectToStringFormat = "{0:h\\:mm\\:ss\\.FFF}";
            this.cCueLength.MinimumWidth = 100;
            this.cCueLength.Text = "Cue Length";
            this.cCueLength.Width = 100;
            // 
            // cCueSeekTo
            // 
            this.cCueSeekTo.AspectName = "SeekTo";
            this.cCueSeekTo.AspectToStringFormat = "{0:h\\:mm\\:ss\\.FFF}";
            this.cCueSeekTo.MinimumWidth = 100;
            this.cCueSeekTo.Text = "Start Cue At";
            this.cCueSeekTo.Width = 100;
            // 
            // cFadeIn
            // 
            this.cFadeIn.AspectName = "FadeInDuration";
            this.cFadeIn.AspectToStringFormat = "{0:h\\:mm\\:ss\\.FFF}";
            this.cFadeIn.MinimumWidth = 100;
            this.cFadeIn.Text = "Fade In Length";
            this.cFadeIn.Width = 100;
            // 
            // cHasAutoFade
            // 
            this.cHasAutoFade.AspectName = "hasAutoFade";
            this.cHasAutoFade.AspectToStringFormat = "";
            this.cHasAutoFade.CheckBoxes = true;
            this.cHasAutoFade.Text = "Auto?";
            // 
            // cFadeOut
            // 
            this.cFadeOut.AspectName = "AutoFadeOutAt";
            this.cFadeOut.AspectToStringFormat = "{0:h\\:mm\\:ss\\.FFF}";
            this.cFadeOut.MinimumWidth = 100;
            this.cFadeOut.Text = "Fade Out At";
            this.cFadeOut.Width = 100;
            // 
            // cFadeOutDuration
            // 
            this.cFadeOutDuration.AspectName = "FadeOutDuration";
            this.cFadeOutDuration.AspectToStringFormat = "{0:h\\:mm\\:ss\\.FFF}";
            this.cFadeOutDuration.MinimumWidth = 100;
            this.cFadeOutDuration.Text = "Fade Out Length";
            this.cFadeOutDuration.Width = 100;
            // 
            // cVolume
            // 
            this.cVolume.AspectName = "Volume";
            this.cVolume.AspectToStringFormat = "{0:P}";
            this.cVolume.MinimumWidth = 80;
            this.cVolume.Text = "Volume Adjust";
            this.cVolume.Width = 80;
            // 
            // mbarMainMenu
            // 
            this.mbarMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp,
            this.addToolStripMenuItem});
            this.mbarMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mbarMainMenu.Name = "mbarMainMenu";
            this.mbarMainMenu.Size = new System.Drawing.Size(1204, 24);
            this.mbarMainMenu.TabIndex = 1;
            this.mbarMainMenu.Text = "mnuMainMenu";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(111, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.mnuHelp.RightToLeftAutoMirrorImage = true;
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.effectToolStripMenuItem,
            this.cueToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.addToolStripMenuItem.Text = "&Add";
            // 
            // effectToolStripMenuItem
            // 
            this.effectToolStripMenuItem.Name = "effectToolStripMenuItem";
            this.effectToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.effectToolStripMenuItem.Text = "&Effect";
            this.effectToolStripMenuItem.Click += new System.EventHandler(this.effectToolStripMenuItem_Click);
            // 
            // cueToolStripMenuItem
            // 
            this.cueToolStripMenuItem.Name = "cueToolStripMenuItem";
            this.cueToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.cueToolStripMenuItem.Text = "&Cue";
            // 
            // grpPreshow
            // 
            this.grpPreshow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPreshow.Controls.Add(this.bEndShow);
            this.grpPreshow.Controls.Add(this.bStartShow);
            this.grpPreshow.Controls.Add(this.chkSpecial);
            this.grpPreshow.Controls.Add(this.btnPreshow);
            this.grpPreshow.Controls.Add(this.cmbIntervalTime);
            this.grpPreshow.Controls.Add(this.lblIntervalTime);
            this.grpPreshow.Location = new System.Drawing.Point(14, 27);
            this.grpPreshow.Name = "grpPreshow";
            this.grpPreshow.Size = new System.Drawing.Size(613, 53);
            this.grpPreshow.TabIndex = 2;
            this.grpPreshow.TabStop = false;
            this.grpPreshow.Text = "Preshow";
            // 
            // bEndShow
            // 
            this.bEndShow.Location = new System.Drawing.Point(528, 20);
            this.bEndShow.Name = "bEndShow";
            this.bEndShow.Size = new System.Drawing.Size(75, 23);
            this.bEndShow.TabIndex = 4;
            this.bEndShow.Text = "End Show";
            this.bEndShow.UseVisualStyleBackColor = true;
            this.bEndShow.Click += new System.EventHandler(this.bEndShow_Click);
            // 
            // bStartShow
            // 
            this.bStartShow.Location = new System.Drawing.Point(447, 20);
            this.bStartShow.Name = "bStartShow";
            this.bStartShow.Size = new System.Drawing.Size(75, 23);
            this.bStartShow.TabIndex = 0;
            this.bStartShow.Text = "Start Show";
            this.bStartShow.UseVisualStyleBackColor = true;
            this.bStartShow.Click += new System.EventHandler(this.bStartShow_Click);
            // 
            // chkSpecial
            // 
            this.chkSpecial.AutoSize = true;
            this.chkSpecial.Location = new System.Drawing.Point(305, 23);
            this.chkSpecial.Name = "chkSpecial";
            this.chkSpecial.Size = new System.Drawing.Size(136, 17);
            this.chkSpecial.TabIndex = 3;
            this.chkSpecial.Text = "Special Announcement";
            this.chkSpecial.UseVisualStyleBackColor = true;
            // 
            // btnPreshow
            // 
            this.btnPreshow.Location = new System.Drawing.Point(223, 20);
            this.btnPreshow.Name = "btnPreshow";
            this.btnPreshow.Size = new System.Drawing.Size(75, 23);
            this.btnPreshow.TabIndex = 2;
            this.btnPreshow.Text = "Play Preshow";
            this.btnPreshow.UseVisualStyleBackColor = true;
            this.btnPreshow.Click += new System.EventHandler(this.btnPreshow_Click);
            // 
            // cmbIntervalTime
            // 
            this.cmbIntervalTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIntervalTime.FormattingEnabled = true;
            this.cmbIntervalTime.Location = new System.Drawing.Point(95, 20);
            this.cmbIntervalTime.Name = "cmbIntervalTime";
            this.cmbIntervalTime.Size = new System.Drawing.Size(121, 21);
            this.cmbIntervalTime.TabIndex = 1;
            // 
            // lblIntervalTime
            // 
            this.lblIntervalTime.AutoSize = true;
            this.lblIntervalTime.Location = new System.Drawing.Point(7, 20);
            this.lblIntervalTime.Name = "lblIntervalTime";
            this.lblIntervalTime.Size = new System.Drawing.Size(81, 13);
            this.lblIntervalTime.TabIndex = 0;
            this.lblIntervalTime.Text = "Interval Length:";
            // 
            // tShow
            // 
            this.tShow.Interval = 1000;
            this.tShow.Tick += new System.EventHandler(this.tShow_Tick);
            // 
            // grpInterval
            // 
            this.grpInterval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpInterval.Controls.Add(this.btnInterval);
            this.grpInterval.Location = new System.Drawing.Point(633, 28);
            this.grpInterval.Name = "grpInterval";
            this.grpInterval.Size = new System.Drawing.Size(92, 52);
            this.grpInterval.TabIndex = 3;
            this.grpInterval.TabStop = false;
            this.grpInterval.Text = "Interval";
            // 
            // btnInterval
            // 
            this.btnInterval.Location = new System.Drawing.Point(6, 19);
            this.btnInterval.Name = "btnInterval";
            this.btnInterval.Size = new System.Drawing.Size(75, 23);
            this.btnInterval.TabIndex = 0;
            this.btnInterval.Text = "Play Interval";
            this.btnInterval.UseVisualStyleBackColor = true;
            this.btnInterval.Click += new System.EventHandler(this.btnInterval_Click);
            // 
            // grpControls
            // 
            this.grpControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpControls.Controls.Add(this.bFadeOut);
            this.grpControls.Controls.Add(this.bPlayCue);
            this.grpControls.Controls.Add(this.bStopAll);
            this.grpControls.Location = new System.Drawing.Point(731, 28);
            this.grpControls.Name = "grpControls";
            this.grpControls.Size = new System.Drawing.Size(455, 52);
            this.grpControls.TabIndex = 4;
            this.grpControls.TabStop = false;
            this.grpControls.Text = "Cue Controls";
            // 
            // bFadeOut
            // 
            this.bFadeOut.Location = new System.Drawing.Point(168, 19);
            this.bFadeOut.Name = "bFadeOut";
            this.bFadeOut.Size = new System.Drawing.Size(75, 23);
            this.bFadeOut.TabIndex = 2;
            this.bFadeOut.Text = "Fade Out";
            this.bFadeOut.UseVisualStyleBackColor = true;
            this.bFadeOut.Click += new System.EventHandler(this.bFadeOut_Click);
            // 
            // bPlayCue
            // 
            this.bPlayCue.Location = new System.Drawing.Point(87, 19);
            this.bPlayCue.Name = "bPlayCue";
            this.bPlayCue.Size = new System.Drawing.Size(75, 23);
            this.bPlayCue.TabIndex = 1;
            this.bPlayCue.Text = "Play Cue";
            this.bPlayCue.UseVisualStyleBackColor = true;
            this.bPlayCue.Click += new System.EventHandler(this.bPlayCue_Click);
            // 
            // bStopAll
            // 
            this.bStopAll.Location = new System.Drawing.Point(6, 19);
            this.bStopAll.Name = "bStopAll";
            this.bStopAll.Size = new System.Drawing.Size(75, 23);
            this.bStopAll.TabIndex = 0;
            this.bStopAll.Text = "Stop ALL";
            this.bStopAll.UseVisualStyleBackColor = true;
            this.bStopAll.Click += new System.EventHandler(this.bStopAll_Click);
            // 
            // fDialog
            // 
            this.fDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // sbarMainStatus
            // 
            this.sbarMainStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblShowRunTime,
            this.lblTotalTimeLabel,
            this.lblRunningTime,
            this.lblCurrentAction});
            this.sbarMainStatus.Location = new System.Drawing.Point(0, 531);
            this.sbarMainStatus.Name = "sbarMainStatus";
            this.sbarMainStatus.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.sbarMainStatus.Size = new System.Drawing.Size(1204, 22);
            this.sbarMainStatus.SizingGrip = false;
            this.sbarMainStatus.TabIndex = 5;
            this.sbarMainStatus.Text = "statusStrip1";
            // 
            // lblShowRunTime
            // 
            this.lblShowRunTime.Name = "lblShowRunTime";
            this.lblShowRunTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblShowRunTime.Size = new System.Drawing.Size(43, 17);
            this.lblShowRunTime.Text = "0:00:00";
            // 
            // lblTotalTimeLabel
            // 
            this.lblTotalTimeLabel.Margin = new System.Windows.Forms.Padding(0, 3, 20, 2);
            this.lblTotalTimeLabel.Name = "lblTotalTimeLabel";
            this.lblTotalTimeLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblTotalTimeLabel.Size = new System.Drawing.Size(117, 17);
            this.lblTotalTimeLabel.Text = "Show Running Time:";
            // 
            // lblRunningTime
            // 
            this.lblRunningTime.Name = "lblRunningTime";
            this.lblRunningTime.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblRunningTime.RightToLeftAutoMirrorImage = true;
            this.lblRunningTime.Size = new System.Drawing.Size(34, 17);
            this.lblRunningTime.Text = "00:00";
            // 
            // lblCurrentAction
            // 
            this.lblCurrentAction.Margin = new System.Windows.Forms.Padding(10, 3, 0, 2);
            this.lblCurrentAction.Name = "lblCurrentAction";
            this.lblCurrentAction.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCurrentAction.Size = new System.Drawing.Size(36, 17);
            this.lblCurrentAction.Text = "ready";
            // 
            // dlgOpen
            // 
            this.dlgOpen.Multiselect = true;
            this.dlgOpen.RestoreDirectory = true;
            this.dlgOpen.SupportMultiDottedExtensions = true;
            // 
            // dlgSave
            // 
            this.dlgSave.AddExtension = false;
            this.dlgSave.RestoreDirectory = true;
            this.dlgSave.SupportMultiDottedExtensions = true;
            // 
            // frmPantomime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 553);
            this.Controls.Add(this.sbarMainStatus);
            this.Controls.Add(this.grpControls);
            this.Controls.Add(this.grpInterval);
            this.Controls.Add(this.grpPreshow);
            this.Controls.Add(this.mbarMainMenu);
            this.Controls.Add(this.tMainWindow);
            this.Name = "frmPantomime";
            this.Text = "1001 Arabian Nights.... and a Matinee (2016)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPantomime_FormClosing);
            this.tMainWindow.ResumeLayout(false);
            this.pgEffects.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstSoundEffects)).EndInit();
            this.grpEffectsControls.ResumeLayout(false);
            this.pgCues.ResumeLayout(false);
            this.gCues.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tCueList)).EndInit();
            this.mbarMainMenu.ResumeLayout(false);
            this.mbarMainMenu.PerformLayout();
            this.grpPreshow.ResumeLayout(false);
            this.grpPreshow.PerformLayout();
            this.grpInterval.ResumeLayout(false);
            this.grpControls.ResumeLayout(false);
            this.sbarMainStatus.ResumeLayout(false);
            this.sbarMainStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tMainWindow;
        private System.Windows.Forms.TabPage pgEffects;
        private System.Windows.Forms.TabPage pgCues;
        private System.Windows.Forms.MenuStrip mbarMainMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem effectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cueToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpPreshow;
        private System.Windows.Forms.ComboBox cmbIntervalTime;
        private System.Windows.Forms.Label lblIntervalTime;
        private System.Windows.Forms.Button btnPreshow;
        private System.Windows.Forms.Timer tShow;
        private System.Windows.Forms.GroupBox grpInterval;
        private System.Windows.Forms.Button btnInterval;
        private System.Windows.Forms.GroupBox grpControls;
        private System.Windows.Forms.FolderBrowserDialog fDialog;
        private System.Windows.Forms.StatusStrip sbarMainStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblRunningTime;
        private System.Windows.Forms.ToolStripStatusLabel lblCurrentAction;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.Button bStartShow;
        private System.Windows.Forms.CheckBox chkSpecial;
        private System.Windows.Forms.ToolStripStatusLabel lblShowRunTime;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalTimeLabel;
        private System.Windows.Forms.Button bStopAll;
        private System.Windows.Forms.Button bEndShow;
        private System.Windows.Forms.GroupBox grpEffectsControls;
        private System.Windows.Forms.Button bAddEffect;
        private System.Windows.Forms.Button bRemoveEffect;
        private System.Windows.Forms.Button bRenumber;
        private System.Windows.Forms.Button bPlayNow;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private BrightIdeasSoftware.TreeListView tCueList;
        private BrightIdeasSoftware.ObjectListView lstSoundEffects;
        private BrightIdeasSoftware.OLVColumn cSoundFXID;
        private BrightIdeasSoftware.OLVColumn cFilename;
        private BrightIdeasSoftware.OLVColumn cLength;
        private BrightIdeasSoftware.OLVColumn cMode;
        private System.Windows.Forms.GroupBox gCues;
        private System.Windows.Forms.Button bSequence;
        private System.Windows.Forms.Button bCollection;
        private System.Windows.Forms.Button bSoundFX;
        private System.Windows.Forms.Button bSilence;
        private System.Windows.Forms.Button bRemoveCue;
        private BrightIdeasSoftware.OLVColumn cCueID;
        private BrightIdeasSoftware.OLVColumn cCueName;
        private BrightIdeasSoftware.OLVColumn cCueLength;
        private BrightIdeasSoftware.OLVColumn cCueSeekTo;
        private BrightIdeasSoftware.OLVColumn cFadeIn;
        private BrightIdeasSoftware.OLVColumn cFadeOut;
        private BrightIdeasSoftware.OLVColumn cFadeOutDuration;
        private BrightIdeasSoftware.OLVColumn cVolume;
        private BrightIdeasSoftware.OLVColumn cSourceID;
        private System.Windows.Forms.Button bPlayCue;
        private System.Windows.Forms.Button bFadeOut;
        private System.Windows.Forms.Button bSwitchMode;
        private BrightIdeasSoftware.OLVColumn cHasAutoFade;
    }
}

