using Chromium;
using Chromium.WebBrowser.Event;
using Clamp.AppCenter;
using Clamp.AppCenter.CFX;
using Clamp.OSGI.Data.Annotation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI.WF
{
    [Extension]
    public class WFAppManager : AppManager
    {
        public Thread CurrentThread { set; get; }

        public override void Initialize()
        {
            base.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Idle += Application_Idle;
            Application.ApplicationExit += Application_ApplicationExit;

        }

        private void Application_Idle(object sender, EventArgs e)
        {
            CfxRuntime.DoMessageLoopWork();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            CFXLauncher.Exit();
        }

        public override void Run(params string[] commandLines)
        {
            string assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            if (CFXLauncher.InitializeChromium(assemblyDir, BeforeChromiumInitialize))
            {
                Dictionary<string, string> configMap = this.GetClampConfiguration();

                if (configMap.ContainsKey(AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED))
                {
                    string domainValues = configMap[AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED];

                    if (!string.IsNullOrWhiteSpace(domainValues))
                    {
                        string[] domainNames = domainValues.Split(',');

                        if (domainNames != null && domainNames.Length > 0)
                        {
                            foreach (string domainName in domainNames)
                            {
                                CFXLauncher.RegisterEmbeddedScheme("http", domainName);
                            }
                        }
                    }
                }

                if (configMap.ContainsKey(AppCenterConstant.CFX_RESOURCE_HANDLER_LOCAL))
                    CFXLauncher.RegisterLocalScheme("http", configMap[AppCenterConstant.CFX_RESOURCE_HANDLER_LOCAL]);
                else
                    CFXLauncher.RegisterLocalScheme("http", "res.clamp.local");

                if (configMap.ContainsKey(AppCenterConstant.CFX_RESOURCE_HANDLER_MUI))
                    CFXLauncher.RegisterClampScheme("http", configMap[AppCenterConstant.CFX_RESOURCE_HANDLER_MUI]);
                else
                    CFXLauncher.RegisterClampScheme("http", "res.clamp.mui");

                Thread loginThread = new Thread(new ThreadStart(() =>
                {
                    Application.Run(new FrmLogin());
                }));

                loginThread.SetApartmentState(ApartmentState.STA);
                loginThread.Start();

                CurrentThread = loginThread;

                do
                {
                    CurrentThread.Join();
                }
                while (CurrentThread != null);
            }
        }

        private void BeforeChromiumInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LogSeverity = global::Chromium.CfxLogSeverity.Default;
            e.Settings.SingleProcess = false;
        }
    }
}
