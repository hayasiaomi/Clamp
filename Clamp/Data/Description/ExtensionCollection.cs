using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Description
{
    /// <summary>
    /// 扩展集合
    /// </summary>
    public class ExtensionCollection : ObjectDescriptionCollection<Extension>
    {

        public ExtensionCollection()
        {
        }

        internal ExtensionCollection(object owner) : base(owner)
        {
        }

        public Extension this[int n]
        {
            get { return (Extension)List[n]; }
        }
    }
}
