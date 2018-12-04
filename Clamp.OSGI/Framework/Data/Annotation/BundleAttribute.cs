using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    ///  标识当前程序是一个Bundle
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : BundleBaseAttribute
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
