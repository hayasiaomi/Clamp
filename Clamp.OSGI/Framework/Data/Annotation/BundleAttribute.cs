using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    ///  Bundle的注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : BundleFragmentAttribute
    {
        
        public BundleAttribute()
        {
        }

        public BundleAttribute(string id) : base(id)
        {
        }

        public BundleAttribute(string id, string version) : base(id, version)
        {
        }

    }
}
