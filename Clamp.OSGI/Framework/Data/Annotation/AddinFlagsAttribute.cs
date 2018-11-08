using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BundleFlagsAttribute : Attribute
    {
        /// <summary>
        /// Initializes the attribute
        /// </summary>
        /// <param name="flags">
        /// Add-in flags
        /// </param>
        public BundleFlagsAttribute(BundleFlags flags)
        {
            this.Flags = flags;
        }

        /// <summary>
        /// Add-in flags
        /// </summary>
        public BundleFlags Flags { get; set; }
    }
}
