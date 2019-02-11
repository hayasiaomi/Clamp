using Clamp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.WF
{
    public class WFActivator : IBundleActivator
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
