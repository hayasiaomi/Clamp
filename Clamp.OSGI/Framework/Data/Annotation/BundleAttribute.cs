using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    ///  用于标识当前的程序集是一个Bundle
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : FragmentBundleAttribute
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
