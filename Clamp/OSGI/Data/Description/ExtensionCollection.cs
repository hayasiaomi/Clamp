using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Description
{
    public class ExtensionCollection : ObjectDescriptionCollection<Extension>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.ExtensionCollection"/> class.
        /// </summary>
        public ExtensionCollection()
        {
        }

        internal ExtensionCollection(object owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="Mono.Bundles.Description.Extension"/> at the specified index.
        /// </summary>
        /// <param name='n'>
        /// The index.
        /// </param>
        public Extension this[int n]
        {
            get { return (Extension)List[n]; }
        }
    }
}
