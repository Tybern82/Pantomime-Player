using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PantomimePlayer {
    public partial class frmLoading : Form, SFXPlayer.SFXLoadingUI {
        public frmLoading() {
            InitializeComponent();
        }

        public void close() {
            this.Invoke(new Action(() => {
                this.Close();
                this.Dispose();
                }));
        }

        public void updateProgress(Double progress) {
            this.Invoke(new Action(() => this.pLoading.Value = (int)Math.Round(progress * 100, MidpointRounding.ToEven)));
        }
    }
}
