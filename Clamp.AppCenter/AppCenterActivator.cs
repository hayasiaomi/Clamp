using Clamp.OSGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter
{
    public class AppCenterActivator : IBundleActivator
    {
        public void Start(IBundleContext context)
        {
            string cfxResHandeEmbedded = context.GetConfigMaps()[AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED];

            string url = cfxResHandeEmbedded.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || cfxResHandeEmbedded.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
                ? cfxResHandeEmbedded : $"http://{cfxResHandeEmbedded}";

            HTMLAnalyzer.Initialize(new Uri(url), new Uri($"http://{ context.GetConfigMaps()[AppCenterConstant.CFX_RESOURCE_HANDLER_MUI]}"));
        }

        public void Stop(IBundleContext context)
        {
            HTMLAnalyzer.Shutdown();
        }
    }
}
