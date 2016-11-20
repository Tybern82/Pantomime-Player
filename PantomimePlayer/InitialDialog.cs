using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using SFXPlayer;

namespace PantomimePlayer {
    public partial class frmInitialLoad : Form {

        public DirectoryInfo SelectedShow { get; private set; }

        public frmInitialLoad() {
            InitializeComponent();
        }

        private void bCreate_Click(Object sender, EventArgs e) {
            fDialog.ShowNewFolderButton = true;
            if (fDialog.ShowDialog() == DialogResult.OK) {
                DirectoryInfo dInfo = new DirectoryInfo(fDialog.SelectedPath);
                this.SelectedShow = dInfo;
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
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            break;
                    }
                }
            }
        }

        private void bOpen_Click(Object sender, EventArgs e) {
            fDialog.ShowNewFolderButton = false;
            if (fDialog.ShowDialog() == DialogResult.OK) {
                DirectoryInfo dInfo = new DirectoryInfo(fDialog.SelectedPath);
                if (!SFXShowFile.verifyShowFile(dInfo)) MessageBox.Show("Unable to locate a valid show file at this location.\nPlease select a different folder.", "Invalid Show");
                else {
                    SelectedShow = dInfo;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void bQuit_Click(Object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
