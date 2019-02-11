using Clamp.AppCenter.MVC;
using Clamp.OSGI;
using Clamp.Linker.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Clamp.OSGI.Collections;

namespace Clamp.AppCenter
{
    public class AppCenterActivator : IBundleActivator
    {
        internal static IBundleContext BundleContext;

        public void Start(IBundleContext context)
        {
            BundleContext = context;

            Dictionary<string, string> appCenterConfigMaps = this.GetClampAppCenterConfiguration();

            string cfxResHandeEmbedded = appCenterConfigMaps[AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED];

            string url = cfxResHandeEmbedded.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || cfxResHandeEmbedded.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
                ? cfxResHandeEmbedded : $"http://{cfxResHandeEmbedded}";

            ILinkerBootstrapper linkerBootstrapper = context.GetExtensionObjects<ILinkerBootstrapper>(true).FirstOrDefault();

            if (linkerBootstrapper != null)
            {
                HTMLAnalyzer.Initialize(linkerBootstrapper, new Uri(url));
            }

            context.Register(typeof(Dictionary<string, string>), appCenterConfigMaps,"cfg.appcenter");
        }

        public void Stop(IBundleContext context)
        {
            HTMLAnalyzer.Shutdown();
        }

        private Dictionary<string, string> GetClampAppCenterConfiguration()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            string clampConfFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppCenter.cfg");

            if (File.Exists(clampConfFile))
            {
                ExtendedProperties initialProperties = new ExtendedProperties(clampConfFile);

                if (initialProperties.Count > 0)
                {
                    foreach (string keyName in initialProperties.Keys)
                    {
                        keyValuePairs.Add(keyName, initialProperties.GetString(keyName));
                    }
                }
            }

            return keyValuePairs;
        }
    }
}
