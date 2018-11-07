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
            Assembly asm = Assembly.GetEntryAssembly();

            if (asm == null)
                asm = Assembly.GetCallingAssembly();

            ClampBundle clampBundle = new ClampBundle();

            clampBundle.Initialize(asm, "bundles", null);

            return clampBundle;
        }
    }
}
