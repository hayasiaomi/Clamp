using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public abstract class BundleActivator : IBundleActivator
    {
        public virtual void Start(BundleContext addInContext)
        {

        }

        public virtual void Stop(BundleContext addInContext)
        {

        }
    }
}
