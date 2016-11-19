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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tMainWindow = new System.Windows.Forms.TabControl();
            this.pgEffects = new System.Windows.Forms.TabPage();
            this.datRegisteredEffects = new System.Windows.Forms.DataGridView();
            this.SourceID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Filename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CacheMode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pgCues = new System.Windows.Forms.TabPage();
            this.mbarMainMenu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.bStopAll = new System.Windows.Forms.Button();
            this.dlgFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.sbarMainStatus = new System.Windows.Forms.StatusStrip();
            this.lblShowRunTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTotalTimeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRunningTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCurrentAction = new System.Windows.Forms.ToolStripStatusLabel();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.grpEffectsControls = new System.Windows.Forms.GroupBox();
            this.bAddEffect = new System.Windows.Forms.Button();
            this.bRemoveEffect = new System.Windows.Forms.Button();
            this.bRenumber = new System.Windows.Forms.Button();
            this.bPlayNow = new System.Windows.Forms.Button();
            this.tMainWindow.SuspendLayout();
            this.pgEffects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datRegisteredEffects)).BeginInit();
            this.mbarMainMenu.SuspendLayout();
            this.grpPreshow.SuspendLayout();
            this.grpInterval.SuspendLayout();
            this.grpControls.SuspendLayout();
            this.sbarMainStatus.SuspendLayout();
            this.grpEffectsControls.SuspendLayout();
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
            this.pgEffects.Controls.Add(this.grpEffectsControls);
            this.pgEffects.Controls.Add(this.datRegisteredEffects);
            this.pgEffects.Location = new System.Drawing.Point(4, 4);
            this.pgEffects.Name = "pgEffects";
            this.pgEffects.Padding = new System.Windows.Forms.Padding(3);
            this.pgEffects.Size = new System.Drawing.Size(1168, 412);
            this.pgEffects.TabIndex = 0;
            this.pgEffects.Text = "Registered Sound FX";
            // 
            // datRegisteredEffects
            // 
            this.datRegisteredEffects.AllowUserToAddRows = false;
            this.datRegisteredEffects.AllowUserToDeleteRows = false;
            this.datRegisteredEffects.AllowUserToOrderColumns = true;
            this.datRegisteredEffects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datRegisteredEffects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.datRegisteredEffects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.datRegisteredEffects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SourceID,
            this.Filename,
            this.Length,
            this.CacheMode});
            this.datRegisteredEffects.Location = new System.Drawing.Point(102, 7);
            this.datRegisteredEffects.Name = "datRegisteredEffects";
            this.datRegisteredEffects.ReadOnly = true;
            this.datRegisteredEffects.RowHeadersVisible = false;
            this.datRegisteredEffects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.datRegisteredEffects.Size = new System.Drawing.Size(1060, 399);
            this.datRegisteredEffects.TabIndex = 0;
            // 
            // SourceID
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.SourceID.DefaultCellStyle = dataGridViewCellStyle1;
            this.SourceID.HeaderText = "Sound FX ID";
            this.SourceID.Name = "SourceID";
            this.SourceID.ReadOnly = true;
            this.SourceID.Width = 93;
            // 
            // Filename
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Filename.DefaultCellStyle = dataGridViewCellStyle2;
            this.Filename.HeaderText = "Filename";
            this.Filename.Name = "Filename";
            this.Filename.ReadOnly = true;
            this.Filename.Width = 74;
            // 
            // Length
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "h\\:mm\\:ss\\.FFF";
            dataGridViewCellStyle3.NullValue = null;
            this.Length.DefaultCellStyle = dataGridViewCellStyle3;
            this.Length.HeaderText = "Total Time";
            this.Length.Name = "Length";
            this.Length.ReadOnly = true;
            this.Length.Width = 82;
            // 
            // CacheMode
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.CacheMode.DefaultCellStyle = dataGridViewCellStyle4;
            this.CacheMode.HeaderText = "Cached/Buffered";
            this.CacheMode.Name = "CacheMode";
            this.CacheMode.ReadOnly = true;
            this.CacheMode.Width = 114;
            // 
            // pgCues
            // 
            this.pgCues.BackColor = System.Drawing.SystemColors.Control;
            this.pgCues.Location = new System.Drawing.Point(4, 4);
            this.pgCues.Name = "pgCues";
            this.pgCues.Padding = new System.Windows.Forms.Padding(3);
            this.pgCues.Size = new System.Drawing.Size(1168, 412);
            this.pgCues.TabIndex = 1;
            this.pgCues.Text = "Sound FX Cues";
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
            this.closeToolStripMenuItem,
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
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.closeToolStripMenuItem.Text = "&Close";
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
            this.effectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.effectToolStripMenuItem.Text = "&Effect";
            // 
            // cueToolStripMenuItem
            // 
            this.cueToolStripMenuItem.Name = "cueToolStripMenuItem";
            this.cueToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            this.grpControls.Controls.Add(this.bStopAll);
            this.grpControls.Location = new System.Drawing.Point(731, 28);
            this.grpControls.Name = "grpControls";
            this.grpControls.Size = new System.Drawing.Size(455, 52);
            this.grpControls.TabIndex = 4;
            this.grpControls.TabStop = false;
            this.grpControls.Text = "Cue Controls";
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
            // dlgFolderBrowser
            // 
            this.dlgFolderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
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
            this.lblCurrentAction.Size = new System.Drawing.Size(203, 17);
            this.lblCurrentAction.Text = "1001 Arabian Nights.... and a Matinee";
            // 
            // dlgOpen
            // 
            this.dlgOpen.RestoreDirectory = true;
            this.dlgOpen.SupportMultiDottedExtensions = true;
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
            ((System.ComponentModel.ISupportInitialize)(this.datRegisteredEffects)).EndInit();
            this.mbarMainMenu.ResumeLayout(false);
            this.mbarMainMenu.PerformLayout();
            this.grpPreshow.ResumeLayout(false);
            this.grpPreshow.PerformLayout();
            this.grpInterval.ResumeLayout(false);
            this.grpControls.ResumeLayout(false);
            this.sbarMainStatus.ResumeLayout(false);
            this.sbarMainStatus.PerformLayout();
            this.grpEffectsControls.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
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
        private System.Windows.Forms.FolderBrowserDialog dlgFolderBrowser;
        private System.Windows.Forms.StatusStrip sbarMainStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblRunningTime;
        private System.Windows.Forms.ToolStripStatusLabel lblCurrentAction;
        private System.Windows.Forms.DataGridView datRegisteredEffects;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.Button bStartShow;
        private System.Windows.Forms.CheckBox chkSpecial;
        private System.Windows.Forms.DataGridViewTextBoxColumn SourceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Filename;
        private System.Windows.Forms.DataGridViewTextBoxColumn Length;
        private System.Windows.Forms.DataGridViewTextBoxColumn CacheMode;
        private System.Windows.Forms.ToolStripStatusLabel lblShowRunTime;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalTimeLabel;
        private System.Windows.Forms.Button bStopAll;
        private System.Windows.Forms.Button bEndShow;
        private System.Windows.Forms.GroupBox grpEffectsControls;
        private System.Windows.Forms.Button bAddEffect;
        private System.Windows.Forms.Button bRemoveEffect;
        private System.Windows.Forms.Button bRenumber;
        private System.Windows.Forms.Button bPlayNow;
    }
}

