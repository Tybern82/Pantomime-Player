namespace PantomimePlayer {
    partial class frmInitialLoad {
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
            this.bCreate = new System.Windows.Forms.Button();
            this.bQuit = new System.Windows.Forms.Button();
            this.bOpen = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.fDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // bCreate
            // 
            this.bCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bCreate.Location = new System.Drawing.Point(13, 143);
            this.bCreate.Name = "bCreate";
            this.bCreate.Size = new System.Drawing.Size(104, 23);
            this.bCreate.TabIndex = 0;
            this.bCreate.Text = "Create New Show";
            this.bCreate.UseVisualStyleBackColor = true;
            this.bCreate.Click += new System.EventHandler(this.bCreate_Click);
            // 
            // bQuit
            // 
            this.bQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bQuit.Location = new System.Drawing.Point(245, 143);
            this.bQuit.Name = "bQuit";
            this.bQuit.Size = new System.Drawing.Size(75, 23);
            this.bQuit.TabIndex = 1;
            this.bQuit.Text = "Quit";
            this.bQuit.UseVisualStyleBackColor = true;
            this.bQuit.Click += new System.EventHandler(this.bQuit_Click);
            // 
            // bOpen
            // 
            this.bOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bOpen.Location = new System.Drawing.Point(123, 143);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(115, 23);
            this.bOpen.TabIndex = 2;
            this.bOpen.Text = "Open Existing Show";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(10, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(310, 131);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "Welcome to SFX Show Player\r\n\r\nTo Begin, just select whether you would like to ope" +
    "n an existing show file, or create a new one.";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fDialog
            // 
            this.fDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // frmInitialLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 178);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.bOpen);
            this.Controls.Add(this.bQuit);
            this.Controls.Add(this.bCreate);
            this.Name = "frmInitialLoad";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SFX Show Player";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bCreate;
        private System.Windows.Forms.Button bQuit;
        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.FolderBrowserDialog fDialog;
    }
}