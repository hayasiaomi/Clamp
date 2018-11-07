using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public abstract class BundleActivator : IBundleActivator
    {
        public virtual void Start(IBundleContext context)
        {
            throw new NotImplementedException();
        }

        public virtual void Stop(IBundleContext context)
        {
            throw new NotImplementedException();
        }
    }
}
