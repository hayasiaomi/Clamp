using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Description
{
    /// <summary>
    /// 扩展节点集合
    /// </summary>
    public class ExtensionNodeTypeCollection : ObjectDescriptionCollection<ExtensionNodeType>
    {
       
        public ExtensionNodeTypeCollection()
        {
        }

        internal ExtensionNodeTypeCollection(object owner) : base(owner)
        {

        }

        public ExtensionNodeType this[int n]
        {
            get { return (ExtensionNodeType)List[n]; }
        }

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
