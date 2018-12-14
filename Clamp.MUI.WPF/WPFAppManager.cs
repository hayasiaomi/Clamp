using Clamp.AppCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.WPF
{
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
            WindowSplash windowSplash = new WindowSplash();

            app.MainWindow = windowSplash;

            app.Run(windowSplash);
        }
    }
}
