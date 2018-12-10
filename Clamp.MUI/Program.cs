using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Clamp.MUI.Framework;
using Clamp.MUI.Framework.UI;
using Clamp.MUI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI
{
    static class Program
    {
        ///// <summary>
        ///// 应用于唯一的实列应用
        ///// </summary>
        private static Mutex mutex = new Mutex(true, "739F31D3-1B85-4DA9-8543-5C0ED4C8B063");

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                ChromiumSettings.InitializeClampSettings();

                string assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                if (UILauncher.InitializeChromium(assemblyDir, BeforeChromiumInitialize))
                {
                    UILauncher.RegisterEmbeddedScheme(Assembly.GetExecutingAssembly(), schemeName: "embedded", domainName: "res.welcome.local");

                    ChromiumSettings.SplashUIThread = new Thread(new ThreadStart(() =>
                    {
                        Application.Run(new Splash.FrmSplash());
                    }));

                    ChromiumSettings.SplashUIThread.SetApartmentState(ApartmentState.STA);
                    ChromiumSettings.SplashUIThread.Start();

                    ChromiumSettings.FrmMainChromium = new FrmMainChromium();
                    ChromiumSettings.FrmMainChromium.FrmOpacity = 0;
                    ChromiumSettings.FrmMainChromium.OnChromiumLoadCompleted += FrmMain_OnChromiumLoadCompleted;

                    //启动主窗体
                    Application.Run(ChromiumSettings.FrmMainChromium);

                    if (ChromiumSettings.SplashResult != null && !ChromiumSettings.SplashResult.Completed)
                    {
                        Application.Run(new FrmError(ChromiumSettings.SplashResult.ErrorMessage));
                    }
                }

                mutex.ReleaseMutex();
            }

        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

        }

        private static void FrmMain_OnChromiumLoadCompleted(ChromiumWinForm chromiumForm, ChromiumWebBrowser chromiumWebBrowser)
        {
            ChromiumSettings.SplashUIThread.Join();

            if (!ChromiumSettings.SplashResult.Completed)
            {
                ChromiumSettings.FrmMainChromium.Invoke(new Action(() =>
                {
                    ChromiumSettings.FrmMainChromium.Close();
                }));

                Application.DoEvents();
            }
            else
            {
                if (!ChromiumSettings.IsChromiumInitialized)
                {
                    ChromiumSettings.FrmMainChromium.Invoke(new Action(() =>
                    {
                        ChromiumSettings.FrmMainChromium.FrmOpacity = 1;
                        ChromiumSettings.FrmMainChromium.NotifyIcon.Visible = true;
                    }));

                    ChromiumSettings.IsChromiumInitialized = true;

                    if (ChromiumSettings.ChildProcess == null)
                    {
                        object floatSwitchValue = DBHelper.AcquireValue("FloatSwitch");

                        if (floatSwitchValue != null && Convert.ToBoolean(floatSwitchValue))
                            ChromiumSettings.FrmMainChromium.SwitchFloatWindow(true, false);
                        else
                            ChromiumSettings.FrmMainChromium.SwitchFloatWindow(false, false);
                    }

                }
            }
        }

        private static void BeforeChromiumInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LogSeverity = global::Chromium.CfxLogSeverity.Default;
            //e.Settings.SingleProcess = true;
        }

    }
}
