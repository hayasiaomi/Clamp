using Clamp.Data.Annotation;
using Clamp.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Nodes
{
    /// <summary>
    /// 扩展树的扩展信息节点
    /// </summary>
    public class ExtensionNode
    {
        private bool childrenLoaded;
        private ExtensionTreeNode treeNode;
        private ExtensionNodeList childNodes;
        private RuntimeBundle runtimeBundle;
        private string bundleId;
        private ExtensionNodeType nodeType;
        private ModuleDescription module;
        private ClampBundle clampBundle;
        private event ExtensionNodeEventHandler extensionNodeChanged;
        /// <summary>
        /// 树的ID
        /// </summary>
        public string Id
        {
            get { return treeNode != null ? treeNode.Id : string.Empty; }
        }

        /// <summary>
        /// 树的路径
        /// </summary>
        public string Path
        {
            get { return treeNode != null ? treeNode.GetPath() : string.Empty; }
        }

        /// <summary>
        /// 父节点
        /// </summary>
        public ExtensionNode Parent
        {
            get
            {
                if (treeNode != null && treeNode.Parent != null)
                    return treeNode.Parent.ExtensionNode;
                else
                    return null;
            }
        }


        /// <summary>
        /// 是否有ID
        /// </summary>
        public bool HasId
        {
            get { return !Id.StartsWith(ExtensionTreeNode.AutoIdPrefix); }
        }

        internal void SetTreeNode(ExtensionTreeNode node)
        {
            treeNode = node;
        }

        /// <summary>
        /// 设置扩展节点的信息
        /// </summary>
        /// <param name="clampBundle"></param>
        /// <param name="plugid"></param>
        /// <param name="nodeType"></param>
        /// <param name="module"></param>
        internal void SetData(ClampBundle clampBundle, string plugid, ExtensionNodeType nodeType, ModuleDescription module)
        {
            this.clampBundle = clampBundle;
            this.bundleId = plugid;
            this.nodeType = nodeType;
            this.module = module;
        }

        /// <summary>
        /// 对应的BUNDLE的ID
        /// </summary>
        internal string BundleId
        {
            get { return bundleId; }
        }

        internal ExtensionTreeNode TreeNode
        {
            get { return treeNode; }
        }

        /// <summary>
        /// 当前的执行的Bundle
        /// </summary>
        public RuntimeBundle RuntimeBundle
        {
            get
            {
                if (runtimeBundle == null && bundleId != null)
                {
                    if (!clampBundle.IsBundleLoaded(bundleId))
                        clampBundle.LoadBundle(bundleId, true);

                    runtimeBundle = clampBundle.GetRuntimeBundle(bundleId);

                    if (runtimeBundle != null)
                        runtimeBundle = runtimeBundle.GetModule(module);
                }

                if (runtimeBundle == null)
                    throw new InvalidOperationException($"Bundle [{bundleId}] 没有加载过");

                return runtimeBundle;
            }
        }

        /// <summary>
        /// Notifies that a child node of this node has been added or removed.
        /// </summary>
        /// <remarks>
        /// The first time the event is subscribed, the handler will be called for each existing node.
        /// </remarks>
        public event ExtensionNodeEventHandler ExtensionNodeChanged
        {
            add
            {
                extensionNodeChanged += value;
                foreach (ExtensionNode node in ChildNodes)
                {
                    try
                    {
                        value(this, new ExtensionNodeEventArgs(ExtensionChange.Add, node));
                    }
                    catch (Exception ex)
                    {
                        clampBundle.ReportError(null, node.RuntimeBundle != null ? node.RuntimeBundle.Id : null, ex, false);
                    }
                }
            }
            remove
            {
                extensionNodeChanged -= value;
            }
        }

        /// <summary>
        /// 子扩展信息节点集合
        /// </summary>
        public ExtensionNodeList ChildNodes
        {
            get
            {
                if (childrenLoaded)
                    return childNodes;

                try
                {
                    if (treeNode.Children.Count == 0)
                    {
                        childNodes = ExtensionNodeList.Empty;
                        return childNodes;
                    }
                }
                catch (Exception ex)
                {
                    clampBundle.ReportError(null, null, ex, false);
                    childNodes = ExtensionNodeList.Empty;
                    return childNodes;
                }
                finally
                {
                    childrenLoaded = true;
                }

                List<ExtensionNode> list = new List<ExtensionNode>();
                foreach (ExtensionTreeNode cn in treeNode.Children)
                {

                    // For each node check if it is visible for the current context.
                    // If something fails while evaluating the condition, just ignore the node.

                    try
                    {
                        if (cn.ExtensionNode != null && cn.IsEnabled)
                            list.Add(cn.ExtensionNode);
                    }
                    catch (Exception ex)
                    {
                        clampBundle.ReportError(null, null, ex, false);
                    }
                }
                if (list.Count > 0)
                    childNodes = new ExtensionNodeList(list);
                else
                    childNodes = ExtensionNodeList.Empty;

                return childNodes;
            }
        }

        /// <summary>
        ///  获得子节点的所有对象
        /// </summary>
        /// <returns></returns>
        public object[] GetChildObjects()
        {
            return GetChildObjects(typeof(object), true);
        }

        /// <summary>
        ///  获得子节点的所有对象
        /// </summary>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetChildObjects(bool reuseCachedInstance)
        {
            return GetChildObjects(typeof(object), reuseCachedInstance);
        }

        /// <summary>
        ///  获得子节点中指定类型的所有对象
        /// </summary>
        /// <param name="arrayElementType"></param>
        /// <returns></returns>
        public object[] GetChildObjects(Type arrayElementType)
        {
            return GetChildObjects(arrayElementType, true);
        }

        /// <summary>
        /// 获得子节点中指定类型的所有对象
        /// </summary>
        /// <param name="arrayElementType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetChildObjects(Type arrayElementType, bool reuseCachedInstance)
        {
            return (object[])GetChildObjectsInternal(arrayElementType, null, reuseCachedInstance);
        }

        /// <summary>
        /// 获得子节点中指定类型的所有对象
        /// </summary>
        /// <param name="arrayElementType"></param>
        /// <param name="bid"></param>
        /// <returns></returns>
        public object[] GetChildObjects(Type arrayElementType, string bid)
        {
            return (object[])GetChildObjectsInternal(arrayElementType, bid, false);
        }

        /// <summary>
        /// 获得子节点中指定类型的所有对象
        /// </summary>
        /// <param name="arrayElementType"></param>
        /// <param name="bid"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetChildObjects(Type arrayElementType, string bid, bool reuseCachedInstance)
        {
            return (object[])GetChildObjectsInternal(arrayElementType, bid, reuseCachedInstance);
        }


        /// <summary>
        /// 获得子节点中指定类型的所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetChildObjects<T>()
        {
            return (T[])GetChildObjectsInternal(typeof(T), null, true);
        }

        /// <summary>
        /// 获得子节点中指定Bundle的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bid"></param>
        /// <returns></returns>
        public T[] GetChildObjects<T>(string bid)
        {
            return (T[])GetChildObjectsInternal(typeof(T), bid, true);
        }

        /// <summary>
        /// 获得子节点中指定类型的所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public T[] GetChildObjects<T>(bool reuseCachedInstance)
        {
            return (T[])GetChildObjectsInternal(typeof(T), null, reuseCachedInstance);
        }

        /// <summary>
        /// 获得子节点中指定类型的所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bid"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public T[] GetChildObjects<T>(string bid, bool reuseCachedInstance)
        {
            return (T[])GetChildObjectsInternal(typeof(T), bid, reuseCachedInstance);
        }

        Array GetChildObjectsInternal(Type arrayElementType, string bid, bool reuseCachedInstance)
        {
            ArrayList list = new ArrayList(ChildNodes.Count);

            for (int n = 0; n < ChildNodes.Count; n++)
            {
                InstanceExtensionNode node = ChildNodes[n] as InstanceExtensionNode;

                if (node == null)
                {
                    clampBundle.ReportError("Error while getting object for node in path '" + Path + "'. Extension node is not a subclass of InstanceExtensionNode.", null, null, false);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(bid) && (node.RuntimeBundle == null || node.RuntimeBundle.Id != bid))
                {
                    clampBundle.ReportError($"找到对应的Path[{Path}],但是不在指定的Budnle内[{bid}]", null, null, false);
                    continue;
                }

                try
                {
                    if (reuseCachedInstance)
                        list.Add(node.GetInstance(arrayElementType));
                    else
                        list.Add(node.CreateInstance(arrayElementType));
                }
                catch (Exception ex)
                {
                    clampBundle.ReportError("Error while getting object for node in path '" + Path + "'.", node.BundleId, ex, false);
                }
            }

            return list.ToArray(arrayElementType);
        }

        /// <summary>
        /// 读取节点
        /// </summary>
        /// <param name="elem"></param>
        internal protected virtual void Read(NodeElement elem)
        {
            if (nodeType == null)
                return;

            NodeAttribute[] attributes = elem.Attributes;
            ReadObject(this, attributes, nodeType.Fields);

            if (nodeType.CustomAttributeMember != null)
            {
                var att = (CustomExtensionAttribute)Activator.CreateInstance(nodeType.CustomAttributeMember.MemberType, true);
                att.ExtensionNode = this;
                ReadObject(att, attributes, nodeType.CustomAttributeFields);
                nodeType.CustomAttributeMember.SetValue(this, att);
            }
        }

        internal void ReadObject(object ob, NodeAttribute[] attributes, Dictionary<string, ExtensionNodeType.FieldData> fields)
        {
            if (fields == null)
                return;

            // Make a copy because we are going to remove fields that have been used
            fields = new Dictionary<string, ExtensionNodeType.FieldData>(fields);

            foreach (NodeAttribute at in attributes)
            {

                ExtensionNodeType.FieldData f;
                if (!fields.TryGetValue(at.name, out f))
                    continue;

                fields.Remove(at.name);

                object val;
                Type memberType = f.MemberType;

                if (memberType == typeof(string))
                {
                    if (f.Localizable)
                        val = RuntimeBundle.Localizer.GetString(at.value);
                    else
                        val = at.value;
                }
                else if (memberType == typeof(string[]))
                {
                    string[] ss = at.value.Split(',');
                    if (ss.Length == 0 && ss[0].Length == 0)
                        val = new string[0];
                    else
                    {
                        for (int n = 0; n < ss.Length; n++)
                            ss[n] = ss[n].Trim();
                        val = ss;
                    }
                }
                else if (memberType.IsEnum)
                {
                    val = Enum.Parse(memberType, at.value);
                }
                else
                {
                    try
                    {
                        val = Convert.ChangeType(at.Value, memberType);
                    }
                    catch (InvalidCastException)
                    {
                        throw new InvalidOperationException("Property type not supported by [NodeAttribute]: " + f.Member.DeclaringType + "." + f.Member.Name);
                    }
                }

                f.SetValue(ob, val);
            }

            if (fields.Count > 0)
            {
                // Check if one of the remaining fields is mandatory
                foreach (KeyValuePair<string, ExtensionNodeType.FieldData> e in fields)
                {
                    ExtensionNodeType.FieldData f = e.Value;
                    if (f.Required)
                        throw new InvalidOperationException("Required attribute '" + e.Key + "' not found.");
                }
            }
        }

        internal bool NotifyChildChanged()
        {
            if (!childrenLoaded)
                return false;

            ExtensionNodeList oldList = childNodes;
            childrenLoaded = false;

            bool changed = false;

            foreach (ExtensionNode nod in oldList)
            {
                if (ChildNodes[nod.Id] == null)
                {
                    changed = true;
                    OnChildNodeRemoved(nod);
                }
            }
            foreach (ExtensionNode nod in ChildNodes)
            {
                if (oldList[nod.Id] == null)
                {
                    changed = true;
                    OnChildNodeAdded(nod);
                }
            }
            if (changed)
                OnChildrenChanged();
            return changed;
        }

        internal protected virtual void OnBundleLoaded()
        {
        }

   
        internal protected virtual void OnBundleUnloaded()
        {
        }

        protected virtual void OnChildrenChanged()
        {
        }

        protected virtual void OnChildNodeAdded(ExtensionNode node)
        {
            if (extensionNodeChanged != null)
                extensionNodeChanged(this, new ExtensionNodeEventArgs(ExtensionChange.Add, node));
        }

        protected virtual void OnChildNodeRemoved(ExtensionNode node)
        {
            if (extensionNodeChanged != null)
                extensionNodeChanged(this, new ExtensionNodeEventArgs(ExtensionChange.Remove, node));
        }
    }

    /// <summary>
    /// An extension node with custom metadata
    /// </summary>
    /// <remarks>
    /// This is the default type for extension nodes bound to a custom extension attribute.
    /// </remarks>
    public class ExtensionNode<T> : ExtensionNode, IAttributedExtensionNode where T : CustomExtensionAttribute
    {
        T data;

        /// <summary>
        /// The custom attribute containing the extension metadata
        /// </summary>
        [NodeAttribute]
        public T Data
        {
            get { return data; }
            internal set { data = value; }
        }

        CustomExtensionAttribute IAttributedExtensionNode.Attribute
        {
            get { return data; }
        }
    }

    /// <summary>
    /// An extension node with custom metadata provided by an attribute
    /// </summary>
    /// <remarks>
    /// This interface is implemented by ExtensionNode&lt;T&gt; to provide non-generic access to the attribute instance.
    /// </remarks>
    public interface IAttributedExtensionNode
    {
        /// <summary>
        /// The custom attribute containing the extension metadata
        /// </summary>
        CustomExtensionAttribute Attribute { get; }
    }
}
