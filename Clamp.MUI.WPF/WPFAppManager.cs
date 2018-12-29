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
            }
        }

        public override void Run(params string[] commandLines)
        {
            string assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            if (UILauncher.InitializeChromium(assemblyDir, BeforeChromiumInitialize))
            {
                UILauncher.RegisterEmbeddedScheme(typeof(WPFAppManager).Assembly, "embedded");

                //WindowSplash windowSplash = new WindowSplash();
                WindowAuthority windowAuthority = new WindowAuthority();

                app.MainWindow = windowAuthority;

                app.Run(windowAuthority);
            }
        }

        private void BeforeChromiumInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LogSeverity = global::Chromium.CfxLogSeverity.Default;
            e.Settings.SingleProcess = true;
        }
    }
}
