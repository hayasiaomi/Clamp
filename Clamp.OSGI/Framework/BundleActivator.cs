using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public abstract class BundleActivator : IBundleActivator
    {
        public void Start(IBundleContext context)
        {
            throw new NotImplementedException();
        }

        public void Stop(IBundleContext context)
        {
            throw new NotImplementedException();
        }
    }
}
