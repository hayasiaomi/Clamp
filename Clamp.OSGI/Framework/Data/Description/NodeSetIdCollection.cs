using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    /// <summary>
    /// A collection of node set identifiers
    /// </summary>
    public class NodeSetIdCollection : IEnumerable
    {
        // A list of string[2]. Item 0 is the node set id, item 1 is the addin that defines it.

        private ArrayList list = new ArrayList();

        /// <summary>
        /// Gets the node set identifier at the specified index.
        /// </summary>
        /// <param name='n'>
        /// An index.
        /// </param>
        public string this[int n]
        {
            get { return ((string[])list[n])[0]; }
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// Gets the collection enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            ArrayList ll = new ArrayList(list.Count);
            foreach (string[] ns in list)
                ll.Add(ns[0]);
            return ll.GetEnumerator();
        }

        /// <summary>
        /// Add the specified node set identifier.
        /// </summary>
        /// <param name='nodeSetId'>
        /// Node set identifier.
        /// </param>
        public void Add(string nodeSetId)
        {
            if (!Contains(nodeSetId))
                list.Add(new string[] { nodeSetId, null });
        }

        /// <summary>
        /// Remove a node set identifier
        /// </summary>
        /// <param name='nodeSetId'>
        /// Node set identifier.
        /// </param>
        public void Remove(string nodeSetId)
        {
            int i = IndexOf(nodeSetId);
            if (i != -1)
                list.RemoveAt(i);
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// Checks if the specified identifier is present in the collection
        /// </summary>
        /// <param name='nodeSetId'>
        /// <c>true</c> if the node set identifier is present.
        /// </param>
        public bool Contains(string nodeSetId)
        {
            return IndexOf(nodeSetId) != -1;
        }

        /// <summary>
        /// Returns the index of the specified node set identifier
        /// </summary>
        /// <returns>
        /// The index.
        /// </returns>
        /// <param name='nodeSetId'>
        /// A node set identifier.
        /// </param>
        public int IndexOf(string nodeSetId)
        {
            for (int n = 0; n < list.Count; n++)
                if (((string[])list[n])[0] == nodeSetId)
                    return n;
            return -1;
        }

        internal void SetExtensionsBundleId(string id)
        {
            foreach (string[] ns in list)
                ns[1] = id;
        }

        internal ArrayList InternalList
        {
            get { return list; }
            set { list = value; }
        }

        internal void MergeWith(string bundleId, NodeSetIdCollection other)
        {
            foreach (string[] ns in other.list)
            {
                if (ns[1] != bundleId && !list.Contains(ns))
                    list.Add(ns);
            }
        }

        internal void UnmergeExternalData(string bundleId, Hashtable addinsToUnmerge)
        {
            ArrayList newList = new ArrayList();
            foreach (string[] ns in list)
            {
                if (ns[1] == bundleId || (addinsToUnmerge != null && !addinsToUnmerge.Contains(ns[1])))
                    newList.Add(ns);
            }
            list = newList;
        }
    }
}
