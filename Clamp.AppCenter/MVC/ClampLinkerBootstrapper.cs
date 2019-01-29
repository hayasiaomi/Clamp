﻿using Clamp.OSGI;
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
    [Extension]
    public class ClampLinkerBootstrapper : DefaultLinkerBootstrapper
    {
        public ClampLinkerBootstrapper()
        {
            this.InternalConfiguration.ViewLocationProvider = typeof(ResourceViewLocationProvider);

            StaticConfiguration.Caching.EnableRuntimeViewDiscovery = true;
        }
    }
}
