using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Description
{
    public class NodeTypeAttributeCollection : ObjectDescriptionCollection<NodeTypeAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.NodeTypeAttributeCollection"/> class.
        /// </summary>
        public NodeTypeAttributeCollection()
        {
        }

        internal NodeTypeAttributeCollection(object owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="Mono.Bundles.Description.NodeTypeAttribute"/> at the specified index.
        /// </summary>
        /// <param name='n'>
        /// The index.
        /// </param>
        public NodeTypeAttribute this[int n]
        {
            get { return (NodeTypeAttribute)List[n]; }
        }
    }
}
