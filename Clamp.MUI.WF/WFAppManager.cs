using Chromium;
using Chromium.WebBrowser.Event;
using Clamp.AppCenter;
using Clamp.AppCenter.CFX;
using Clamp.Cfg;
using Clamp.Data.Annotation;
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
        private static WFAppManager appManager;

        public static WFAppManager Current
        {
            get
            {
                return appManager;
            }
        }

        public Thread CurrentThread { set; get; }


        public Dictionary<string, string> ConfigYEUXMaps { private set; get; }

        public override void Initialize()
        {
            base.Initialize();

            if (appManager == null)
                appManager = this;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Idle += Application_Idle;

        }

        private void Application_Idle(object sender, EventArgs e)
        {
            CfxRuntime.DoMessageLoopWork();
        }


        public override void Run(params string[] commandLines)
        {
            if (CFXLauncher.InitializeChromium(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FX"), BeforeChromiumInitialize))
            {
                this.ConfigYEUXMaps = this.GetYEUXConfiguration();

                Dictionary<string, string> configAppCenterMaps = WFActivator.BundleContext.Resolve<Dictionary<string, string>>(AppCenterConstant.CFG_APPCENTER);

                if (configAppCenterMaps.ContainsKey(AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED))
                {
                    string domainValues = configAppCenterMaps[AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED];

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

                if (configAppCenterMaps.ContainsKey(AppCenterConstant.CFX_RESOURCE_HANDLER_LOCAL))
                    CFXLauncher.RegisterLocalScheme("http", configAppCenterMaps[AppCenterConstant.CFX_RESOURCE_HANDLER_LOCAL]);
                else
                    CFXLauncher.RegisterLocalScheme("http", "res.clamp.local");

                if (configAppCenterMaps.ContainsKey(AppCenterConstant.CFX_RESOURCE_HANDLER_MUI))
                    CFXLauncher.RegisterClampScheme("http", configAppCenterMaps[AppCenterConstant.CFX_RESOURCE_HANDLER_MUI]);
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

                CFXLauncher.Exit();
            }
        }

        private void BeforeChromiumInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LogSeverity = global::Chromium.CfxLogSeverity.Default;
            e.Settings.SingleProcess = false;
        }


        private Dictionary<string, string> GetYEUXConfiguration()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            string clampConfFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YEUX.cfg");

            if (File.Exists(clampConfFile))
            {
                ExtendedProperties extendedProperties = new ExtendedProperties(clampConfFile);

                if (extendedProperties.Count > 0)
                {
                    foreach (string keyName in extendedProperties.Keys)
                    {
                        keyValuePairs.Add(keyName, extendedProperties.GetString(keyName));
                    }
                }
            }

            return keyValuePairs;
        }
    }
}
