using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public abstract class Dependency : ObjectDescription
    {

        internal Dependency()
        {
        }

        /// <summary>
        /// Gets the display name of the dependency.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract string Name { get; }
        internal abstract bool CheckInstalled(BundleRegistry registry);
    }
}
