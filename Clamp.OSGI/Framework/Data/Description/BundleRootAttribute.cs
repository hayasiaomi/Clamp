using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleRootAttribute : BundleAttribute
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public BundleRootAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="id">
        /// Identifier of the add-in root
        /// </param>
        public BundleRootAttribute(string id) : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="id">
        /// Identifier of the add-in root
        /// </param>
        /// <param name="version">
        /// Version of the add-in root
        /// </param>
        public BundleRootAttribute(string id, string version) : base(id, version)
        {
        }

    }
}
