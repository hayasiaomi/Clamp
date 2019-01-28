using Clamp.OSGI;
using Clamp.OSGI.Data.Annotation;
using Clamp.Linker;
using Clamp.Linker.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamp.Linker.Bootstrapper;
using Clamp.Linker.ViewEngines;

namespace Clamp.AppCenter.MVC
{
    public class ClampLinkerBootstrapper : DefaultLinkerBootstrapper
    {
        public IBundleContext BundleContext { private set; get; }

        public ClampLinkerBootstrapper(IBundleContext bundleContex)
        {
            this.BundleContext = bundleContex;
            this.InternalConfiguration.ViewLocationProvider = typeof(ResourceViewLocationProvider);

            StaticConfiguration.Caching.EnableRuntimeViewDiscovery = true;
        }
    }
}
