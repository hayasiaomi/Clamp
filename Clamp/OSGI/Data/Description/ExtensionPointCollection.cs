using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Description
{
    /// <summary>
    /// 扩展点集合
    /// </summary>
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
