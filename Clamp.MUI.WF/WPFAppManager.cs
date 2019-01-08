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
using System.Windows.Forms;

namespace Clamp.MUI.WF
{
    [Extension]
    public class WPFAppManager : AppManager
    {
        public override void Initialize()
        {
            base.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        public override void Run(params string[] commandLines)
        {
            string assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            if (CFXLauncher.InitializeChromium(assemblyDir, BeforeChromiumInitialize))
            {
                CFXLauncher.RegisterEmbeddedScheme(typeof(WPFAppManager).Assembly, "embedded");

                Application.Run(new FrmMain());
            }
        }

        private void BeforeChromiumInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.LogSeverity = global::Chromium.CfxLogSeverity.Default;
            e.Settings.SingleProcess = true;
        }
    }
}
