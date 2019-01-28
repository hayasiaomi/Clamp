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
        private IBundleContext bundleContex;

        public ClampLinkerBootstrapper(IBundleContext bundleContex)
        {
            this.bundleContex = bundleContex;
            this.InternalConfiguration.ViewLocationProvider = typeof(ResourceViewLocationProvider);

            StaticConfiguration.Caching.EnableRuntimeViewDiscovery = true;
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, LinkerContext context)
        {
            context.BundleContext = this.bundleContex;


            base.ConfigureRequestContainer(container, context);
        }


    }
}
