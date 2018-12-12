using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Description
{
    public class ExtensionNodeDescriptionCollection : ObjectDescriptionCollection<ExtensionNodeDescription>, NodeElementCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.ExtensionNodeDescriptionCollection"/> class.
        /// </summary>
        public ExtensionNodeDescriptionCollection()
        {
        }

        internal ExtensionNodeDescriptionCollection(object owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="Mono.Bundles.Description.ExtensionNodeDescription"/> at the specified index.
        /// </summary>
        /// <param name='n'>
        /// The index.
        /// </param>
        public ExtensionNodeDescription this[int n]
        {
            get { return (ExtensionNodeDescription)List[n]; }
        }

        /// <summary>
        /// Gets the <see cref="Mono.Bundles.Description.ExtensionNodeDescription"/> with the specified identifier.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        public ExtensionNodeDescription this[string id]
        {
            get
            {
                foreach (ExtensionNodeDescription node in List)
                    if (node.Id == id)
                        return node;
                return null;
            }
        }

        NodeElement NodeElementCollection.this[int n]
        {
            get { return (NodeElement)List[n]; }
        }
    }
}
