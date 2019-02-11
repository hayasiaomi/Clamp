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

namespace Clamp.MUI.WPF
{
    [Extension]
    public class WPFAppManager : AppManager
    {
        private App app;

        public override void Initialize()
        {
            base.Initialize();

            if (this.app == null)
            {
                app = new App();
                app.InitializeComponent();
                app.Exit += App_Exit;
            }
        }

        private void App_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            CFXLauncher.Exit();
        }

        public override void Run(params string[] commandLines)
        {
            string assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            if (CFXLauncher.InitializeChromium(assemblyDir, BeforeChromiumInitialize))
            {
                //CFXLauncher.RegisterEmbeddedScheme(typeof(WPFAppManager).Assembly, "embedded");

                //WindowSplash windowSplash = new WindowSplash();
                //WindowAuthority windowAuthority = new WindowAuthority();

                Dictionary<string, string> configMap = this.GetYEUXConfiguration();

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

                MainWindow mainWindow = new MainWindow();

                app.MainWindow = mainWindow;

                app.Run(mainWindow);
            }
        }

        private void BeforeChromiumInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LogSeverity = global::Chromium.CfxLogSeverity.Default;
            e.Settings.SingleProcess = false;
        }
    }
}
