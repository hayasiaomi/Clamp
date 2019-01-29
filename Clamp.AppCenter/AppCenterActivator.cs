using Clamp.AppCenter.MVC;
using Clamp.OSGI;
using Clamp.Linker.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter
{
    public class AppCenterActivator : IBundleActivator
    {
        internal static IBundleContext BundleContext;

        public void Start(IBundleContext context)
        {
            BundleContext = context;

            string cfxResHandeEmbedded = context.GetConfigMaps()[AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED];

            string url = cfxResHandeEmbedded.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || cfxResHandeEmbedded.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
                ? cfxResHandeEmbedded : $"http://{cfxResHandeEmbedded}";

            ILinkerBootstrapper linkerBootstrapper = context.GetExtensionObjects<ILinkerBootstrapper>(true).FirstOrDefault();

            if (linkerBootstrapper != null)
            {
                HTMLAnalyzer.Initialize(linkerBootstrapper, new Uri(url));
            }
        }

        public void Stop(IBundleContext context)
        {
            HTMLAnalyzer.Shutdown();
        }
    }
}
