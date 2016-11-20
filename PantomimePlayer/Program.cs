using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PantomimePlayer {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmPantomime mainWindow = new frmPantomime();
            // Configure logging for the system.
            log4net.Config.BasicConfigurator.Configure();
            if (mainWindow.DialogResult != DialogResult.Abort) {
                Application.Run(mainWindow);
            } else {
                log4net.LogManager.Shutdown();
            }
        }
    }
}
