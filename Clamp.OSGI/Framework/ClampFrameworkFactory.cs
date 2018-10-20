using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public sealed class ClampFrameworkFactory
    {

        public IClampFramework NewClampFramework()
        {
            return new ClampFramework(null);
        }
    }
}
