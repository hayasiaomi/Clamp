using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ExtensionNodeSetCollection : ObjectDescriptionCollection<ExtensionNodeSet>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.ExtensionNodeSetCollection"/> class.
        /// </summary>
        public ExtensionNodeSetCollection()
        {
        }

        internal ExtensionNodeSetCollection(object owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="Mono.Addins.Description.ExtensionNodeSet"/> at the specified index.
        /// </summary>
        /// <param name='n'>
        /// The index.
        /// </param>
        public ExtensionNodeSet this[int n]
        {
            get { return (ExtensionNodeSet)List[n]; }
        }

        /// <summary>
        /// Gets the <see cref="Mono.Addins.Description.ExtensionNodeSet"/> with the specified id.
        /// </summary>
        /// <param name='id'>
        /// Identifier.
        /// </param>
        public ExtensionNodeSet this[string id]
        {
            get
            {
                for (int n = 0; n < List.Count; n++)
                    if (((ExtensionNodeSet)List[n]).Id == id)
                        return (ExtensionNodeSet)List[n];
                return null;
            }
        }
    }
}
