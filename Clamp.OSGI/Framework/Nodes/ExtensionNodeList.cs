using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Nodes
{
    /// <summary>
	/// A list of extension nodes.
	/// </summary>
	public class ExtensionNodeList : IEnumerable
    {
        internal List<ExtensionNode> list;

        internal static ExtensionNodeList Empty = new ExtensionNodeList(new List<ExtensionNode>());

        internal ExtensionNodeList(List<ExtensionNode> list)
        {
            this.list = list;
        }

        /// <summary>
        /// Returns the node in the specified index.
        /// </summary>
        /// <param name="n">
        /// The index.
        /// </param>
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

        /// <summary>
        /// Returns the node with the specified ID.
        /// </summary>
        /// <param name="id">
        /// An id.
        /// </param>
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

        /// <summary>
        /// Gets an enumerator which enumerates all nodes in the list
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            if (list == null)
                return ((IList)Type.EmptyTypes).GetEnumerator();
            return list.GetEnumerator();
        }

        /// <summary>
        /// Number of nodes of the collection.
        /// </summary>
        public int Count
        {
            get { return list == null ? 0 : list.Count; }
        }

        /// <summary>
        /// Copies all nodes to an array
        /// </summary>
        /// <param name='array'>
        /// The target array
        /// </param>
        /// <param name='index'>
        /// Initial index where to copy to
        /// </param>
        public void CopyTo(ExtensionNode[] array, int index)
        {
            if (list != null)
                list.CopyTo(array, index);
        }
    }

    /// <summary>
    /// A list of extension nodes.
    /// </summary>
    public class ExtensionNodeList<T> : IEnumerable, IEnumerable<T> where T : ExtensionNode
    {
        List<ExtensionNode> list;

        internal static ExtensionNodeList<T> Empty = new ExtensionNodeList<T>(new List<ExtensionNode>());

        internal ExtensionNodeList(List<ExtensionNode> list)
        {
            this.list = list;
        }

        /// <summary>
        /// Returns the node in the specified index.
        /// </summary>
        /// <param name="n">
        /// The index.
        /// </param>
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

        /// <summary>
        /// Returns the node with the specified ID.
        /// </summary>
        /// <param name="id">
        /// An id.
        /// </param>
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

        /// <summary>
        /// Gets an enumerator which enumerates all nodes in the list
        /// </summary>
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
        /// Number of nodes of the collection.
        /// </summary>
        public int Count
        {
            get { return list == null ? 0 : list.Count; }
        }

        /// <summary>
        /// Copies all nodes to an array
        /// </summary>
        /// <param name='array'>
        /// The target array
        /// </param>
        /// <param name='index'>
        /// Initial index where to copy to
        /// </param>
        public void CopyTo(T[] array, int index)
        {
            if (list != null)
                list.CopyTo(array, index);
        }
    }
}
