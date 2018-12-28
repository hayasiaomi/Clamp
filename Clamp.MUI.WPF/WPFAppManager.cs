using Chromium.WebBrowser.Event;
using Clamp.AppCenter;
using Clamp.MUI.WPF.UI;
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
    public class WPFAppManager : IAppManager
    {
        private App app;
        public void Initialize()
        {
            if (this.app == null)
            {
                app = new App();
                app.InitializeComponent();
            }
        }

        public void Run(params string[] commandLines)
        {
            string assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            if (UILauncher.InitializeChromium(assemblyDir, BeforeChromiumInitialize))
            {
                UILauncher.RegisterEmbeddedScheme(Assembly.GetExecutingAssembly(), schemeName: "embedded", domainName: "res.clamp.local");

                WindowSplash windowSplash = new WindowSplash();

                app.MainWindow = windowSplash;

                app.Run(windowSplash);
            }
        }

        private void BeforeChromiumInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LogSeverity = global::Chromium.CfxLogSeverity.Default;
            //e.Settings.SingleProcess = true;
        }
    }
}
