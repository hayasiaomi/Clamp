using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ExtensionPointCollection : ObjectDescriptionCollection<ExtensionPoint>
    {
        public ExtensionPointCollection()
        {
        }

        internal ExtensionPointCollection(object owner) : base(owner)
        {
        }

        public ExtensionPoint this[int n]
        {
            get { return (ExtensionPoint)List[n]; }
        }

        /// <summary>
        /// Gets the <see cref="Mono.Addins.Description.ExtensionPoint"/> with the specified path.
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        public ExtensionPoint this[string path]
        {
            get
            {
                for (int n = 0; n < List.Count; n++)
                    if (((ExtensionPoint)List[n]).Path == path)
                        return (ExtensionPoint)List[n];
                return null;
            }
        }
    }
}
