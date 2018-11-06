using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public sealed class ClampBundleFactory
    {

        public static IClampBundle GetClampBundle()
        {
            ClampBundle clampBundle = new ClampBundle();

            clampBundle.Initialize();

            return clampBundle;
        }
    }
}
