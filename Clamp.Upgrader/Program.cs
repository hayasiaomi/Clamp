using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.Upgrader
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "ED444D3C-3743-4890-9301-1A6B374FD83E");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.FromSeconds(1), true))
            {
                string dataString = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));

                if (!string.IsNullOrWhiteSpace(dataString))
                {
                    UpgradeInfo upgradeInfo = JsonConvert.DeserializeObject<UpgradeInfo>(dataString);

                    if (upgradeInfo != null)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        FrmMain frmMain = new FrmMain();

                        Application.Run(frmMain);

                        if (frmMain.DialogResult == DialogResult.OK)
                        {
                            Application.Run(new FrmInstall(upgradeInfo));
                        }
                    }
                }

                mutex.ReleaseMutex();
            }
        }
    }
}
