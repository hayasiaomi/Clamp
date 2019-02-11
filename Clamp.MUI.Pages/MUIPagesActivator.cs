using Clamp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clamp.MUI.Pages
{
    public class MUIPagesActivator : IBundleActivator
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