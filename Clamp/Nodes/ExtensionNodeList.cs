using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Nodes
{
    /// <summary>
    /// 树的扩展节点集合
    /// </summary>
    public class ExtensionNodeList : IEnumerable
    {
        internal List<ExtensionNode> list;

        internal static ExtensionNodeList Empty = new ExtensionNodeList(new List<ExtensionNode>());

        internal ExtensionNodeList(List<ExtensionNode> list)
        {
            this.list = list;
        }

        public ExtensionNode this[int n]
        {
            get
            {
                if (list == null)
                    throw new System.IndexOutOfRangeException();
                else
                    return (ExtensionNode)list[n];
            }
        }

        public ExtensionNode this[string id]
        {
            get
            {
                if (list == null)
                    return null;
                else
                {
                    for (int n = list.Count - 1; n >= 0; n--)
                        if (((ExtensionNode)list[n]).Id == id)
                            return (ExtensionNode)list[n];
                    return null;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            if (list == null)
                return ((IList)Type.EmptyTypes).GetEnumerator();
            return list.GetEnumerator();
        }

        /// <summary>
        /// 节点数量
        /// </summary>
        public int Count
        {
            get { return list == null ? 0 : list.Count; }
        }

        /// <summary>
        /// 将整个扩展节点集合复制到兼容的一个扩展节点集合中，从目标扩展节点集合的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(ExtensionNode[] array, int index)
        {
            if (list != null)
                list.CopyTo(array, index);
        }
    }

    /// <summary>
    /// 扩展节点集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtensionNodeList<T> : IEnumerable, IEnumerable<T> where T : ExtensionNode
    {
        private List<ExtensionNode> list;

        internal static ExtensionNodeList<T> Empty = new ExtensionNodeList<T>(new List<ExtensionNode>());

        internal ExtensionNodeList(List<ExtensionNode> list)
        {
            this.list = list;
        }

        public T this[int n]
        {
            get
            {
                if (list == null)
                    throw new System.IndexOutOfRangeException();
                else
                    return (T)list[n];
            }
        }

        public T this[string id]
        {
            get
            {
                if (list == null)
                    return null;
                else
                {
                    for (int n = list.Count - 1; n >= 0; n--)
                        if (list[n].Id == id)
                            return (T)list[n];
                    return null;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (list == null)
                yield break;
            foreach (ExtensionNode n in list)
                yield return (T)n;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (list == null)
                return ((IList)Type.EmptyTypes).GetEnumerator();
            return list.GetEnumerator();
        }

        /// <summary>
        /// 集合数量
        /// </summary>
        public int Count
        {
            get { return list == null ? 0 : list.Count; }
        }


        /// <summary>
        /// 将整个扩展节点集合复制到兼容的一个扩展节点集合中，从目标扩展节点集合的指定索引位置开始放置。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index)
        {
            if (list != null)
                list.CopyTo(array, index);
        }
    }
}
