using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    /// Bundle的状态
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BundleFlagsAttribute : Attribute
    {

        public BundleFlagsAttribute(BundleFlags flags)
        {
            this.Flags = flags;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public BundleFlags Flags { get; set; }
    }
}
