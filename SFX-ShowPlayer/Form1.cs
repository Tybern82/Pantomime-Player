using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SFX_ShowPlayer {
    public partial class Form1 : Form {

        private uDMX dmxControl;

        public Form1() {
            dmxControl = new uDMX();
            if (!dmxControl.IsOpen) {
                ExitApplication("Unable to open uDMX device - closing application.");
            }
            InitializeComponent();
        }

        private void numericUpDown1_ValueChanged(Object sender, EventArgs e) {
            byte nValue = (byte)numericUpDown1.Value;
            progressBar1.Value = nValue;
            dmxControl.SetSingleChannel(1, nValue);
            MessageBox.Show("Set channel 1: " + nValue);
        }

        private void numericUpDown2_ValueChanged(Object sender, EventArgs e) {
            byte nValue = (byte)numericUpDown2.Value;
            progressBar2.Value = nValue;
            dmxControl.SetSingleChannel(2, nValue);
            MessageBox.Show("Set channel 2: " + nValue);
        }

        public void ExitApplication(string msg) {
            MessageBox.Show(msg, "Application Terminating...");
            ExitApplication();
        }

        private void ExitApplication() {
            if (System.Windows.Forms.Application.MessageLoop) {
                // WinForms app
                System.Windows.Forms.Application.Exit();
            } else {
                // Console app
                System.Environment.Exit(1);
            }
        }
    }
}
