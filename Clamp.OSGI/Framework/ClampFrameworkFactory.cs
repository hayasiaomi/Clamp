using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public sealed class ClampFrameworkFactory
    {

        public static IClampFramework NewClampFramework()
        {
            ClampFramework clampFramework = new ClampFramework();

            clampFramework.Initialize();

            return clampFramework;
        }
    }
}
