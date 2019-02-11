using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Clamp.Data.Description
{
    [XmlInclude(typeof(BundleDependency))]
    public abstract class Dependency : ObjectDescription
    {

        internal Dependency(XmlElement elem) : base(elem)
        {

        }

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
