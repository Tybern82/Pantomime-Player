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
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            break;
                    }
                } else {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
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
