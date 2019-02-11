using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    /// <summary>
    ///  标识当前程序是一个Bundle
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class Bundle1AttributeAttribute : BundleAttribute
    {

        public Bundle1AttributeAttribute()
        {
        }

        public Bundle1AttributeAttribute(string id) : base(id)
        {
        }

        public Bundle1AttributeAttribute(string id, string version) : base(id, version)
        {
        }

    }
}
