using Clamp.OSGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clamp.MUI.EasyUI
{
    public class EasyUIActivator : IBundleActivator
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