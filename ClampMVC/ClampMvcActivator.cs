using Clamp.OSGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker
{
    public class ClampMvcActivator : IBundleActivator
    {
        internal static IBundleContext BundleContext;

        public void Start(IBundleContext context)
        {
            BundleContext = context;

            string cfxResHandeEmbedded = context.GetConfigMaps()["clamp.mvc.pages.mode"];

            string url = cfxResHandeEmbedded.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || cfxResHandeEmbedded.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
                ? cfxResHandeEmbedded : $"http://{cfxResHandeEmbedded}";
        }

        public void Stop(IBundleContext context)
        {

        }
    }
}
