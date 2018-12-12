using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Description
{
    /// <summary>
    /// 扩展节点组的ID集合
    /// </summary>
    public class NodeSetIdCollection : IEnumerable
    {
        /// <summary>
        /// 这个集合是一个长度为2的字符串数组的集合，数组0为扩展节点的ID，数组1为扩展节点定义的Bundle
        /// </summary>
        private ArrayList list = new ArrayList();

        /// <summary>
        /// 获得指定位置的扩展节点组的ID
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public string this[int n]
        {
            get { return ((string[])list[n])[0]; }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }


        public IEnumerator GetEnumerator()
        {
            ArrayList ll = new ArrayList(list.Count);

            foreach (string[] ns in list)
                ll.Add(ns[0]);

            return ll.GetEnumerator();
        }
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="nodeSetId"></param>
        public void Add(string nodeSetId)
        {
            if (!Contains(nodeSetId))
                list.Add(new string[] { nodeSetId, null });
        }

        /// <summary>
        /// 移除指定的扩展节点组ID
        /// </summary>
        /// <param name="nodeSetId"></param>
        public void Remove(string nodeSetId)
        {
            int i = IndexOf(nodeSetId);
            if (i != -1)
                list.RemoveAt(i);
        }


        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// 是否含有指点定的扩展节点组
        /// </summary>
        /// <param name="nodeSetId"></param>
        /// <returns></returns>
        public bool Contains(string nodeSetId)
        {
            return IndexOf(nodeSetId) != -1;
        }

        /// <summary>
        /// 获得指定扩展节点组ID的位置
        /// </summary>
        /// <param name="nodeSetId"></param>
        /// <returns></returns>
        public int IndexOf(string nodeSetId)
        {
            for (int n = 0; n < list.Count; n++)
                if (((string[])list[n])[0] == nodeSetId)
                    return n;
            return -1;
        }
        /// <summary>
        /// 设置当前扩展节点组集合属于的Bundle
        /// </summary>
        /// <param name="id"></param>
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
