using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ExtensionNodeTypeCollection : ObjectDescriptionCollection<ExtensionNodeType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.ExtensionNodeTypeCollection"/> class.
        /// </summary>
        public ExtensionNodeTypeCollection()
        {
        }

        internal ExtensionNodeTypeCollection(object owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="Mono.Bundles.Description.ExtensionNodeType"/> at the specified index.
        /// </summary>
        /// <param name='n'>
        /// The index.
        /// </param>
        public ExtensionNodeType this[int n]
        {
            get { return (ExtensionNodeType)List[n]; }
        }

        /// <summary>
        /// Gets the <see cref="Mono.Bundles.Description.ExtensionNodeType"/> with the specified id.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        public ExtensionNodeType this[string id]
        {
            get
            {
                for (int n = 0; n < List.Count; n++)
                    if (((ExtensionNodeType)List[n]).Id == id)
                        return (ExtensionNodeType)List[n];
                return null;
            }
        }
    }
}
