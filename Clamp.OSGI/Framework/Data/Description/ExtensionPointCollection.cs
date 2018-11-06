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
    }
}
