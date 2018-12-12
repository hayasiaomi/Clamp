﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Nodes
{
    class TreeNodeCollection : IEnumerable
    {
        ArrayList list;

        internal static TreeNodeCollection Empty = new TreeNodeCollection(null);

        public TreeNodeCollection(ArrayList list)
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

        public TreeNode this[int n]
        {
            get
            {
                if (list != null)
                    return (TreeNode)list[n];
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

        public TreeNodeCollection Clone()
        {
            if (list != null)
                return new TreeNodeCollection((ArrayList)list.Clone());
            else
                return Empty;
        }
    }
}
