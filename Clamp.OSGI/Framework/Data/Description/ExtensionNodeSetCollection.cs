using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    /// <summary>
    /// 扩展节点组集合
    /// </summary>
    public class ExtensionNodeSetCollection : ObjectDescriptionCollection<ExtensionNodeSet>
    {
        public ExtensionNodeSetCollection()
        {
        }

        internal ExtensionNodeSetCollection(object owner) : base(owner)
        {
        }

        public ExtensionNodeSet this[int n]
        {
            get { return (ExtensionNodeSet)List[n]; }
        }

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
