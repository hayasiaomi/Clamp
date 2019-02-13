using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Nodes
{
    /// <summary>
    /// 树节点集合
    /// </summary>
    class ExtensionTreeNodeCollection : IEnumerable
    {
        ArrayList list;

        internal static ExtensionTreeNodeCollection Empty = new ExtensionTreeNodeCollection(null);

        public ExtensionTreeNodeCollection(ArrayList list)
        {
            this.list = list;
        }

        public IEnumerator GetEnumerator()
        {
            if (list != null)
                return list.GetEnumerator();
            else
                return Type.EmptyTypes.GetEnumerator();
        }

        public ExtensionTreeNode this[int n]
        {
            get
            {
                if (list != null)
                    return (ExtensionTreeNode)list[n];
                else
                    throw new System.IndexOutOfRangeException();
            }
        }

        public int IndexOfNode(string id)
        {
            for (int n = 0; n < Count; n++)
            {
                if (this[n].Id == id)
                    return n;
            }
            return -1;
        }

        public int Count
        {
            get { return list != null ? list.Count : 0; }
        }

        public ExtensionTreeNodeCollection Clone()
        {
            if (list != null)
                return new ExtensionTreeNodeCollection((ArrayList)list.Clone());
            else
                return Empty;
        }
    }
}
