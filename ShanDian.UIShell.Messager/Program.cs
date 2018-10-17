using CefSharp;
using ShanDian.UIShell.Assist.Brower;
using ShanDian.UIShell.Assist.Helpers;
using Newtonsoft.Json;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.InterProcess;
using ShanDian.UIShell.Framework.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShanDian.UIShell.Assist
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "142EB070-2FE9-4379-808E-8D42B44A07C9");

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.FromSeconds(2), true))
            {
                if (args != null && args.Length > 0)
                {
                    string dataString = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));

                    Log4netService.Info("dataString:" + dataString);

                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        ProcessCredential processCredential = JsonConvert.DeserializeObject<ProcessCredential>(dataString);

                        Log4netService.Info("processCredential Parameters :" + processCredential.Parameters);

                        if (processCredential != null && string.Equals(processCredential.Token, "ShanDian", StringComparison.CurrentCultureIgnoreCase))
                        {
                            SDShellHelper.GetSDShellSettings();
                            SDShellHelper.GetDemand();

                            Cef.EnableHighDPISupport();

                            CefBrower.Init(true, true);

                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);

                            FrmMain frmMain = new FrmMain();

                            frmMain.SDShellPID = processCredential.ProcessId;
                            frmMain.BindMainProcesss(frmMain.SDShellPID);

                            Application.Run(frmMain);

                            mutex.ReleaseMutex();
                        }
                    }
                }
            }
        }

    }
}
