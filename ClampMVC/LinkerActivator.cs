using Clamp.OSGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker
{
    public class LinkerActivator : IBundleActivator
    {
        internal static IBundleContext BundleContext;

        public void Start(IBundleContext context)
        {
            BundleContext = context;

        }

        public void Stop(IBundleContext context)
        {

        }
    }
}
