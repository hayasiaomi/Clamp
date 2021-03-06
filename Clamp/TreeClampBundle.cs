﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamp.Data.Description;
using Clamp.Nodes;

namespace Clamp
{
    internal partial class ClampBundle : Bundle, IClampBundle
    {
        private Hashtable conditionTypes = new Hashtable();
        private Hashtable conditionsToNodes = new Hashtable();
        private List<WeakReference> childContexts;
        private ExtensionTreeNode tree;
        private bool fireEvents = false;

        private ArrayList runTimeEnabledBundles;
        private ArrayList runTimeDisabledBundles;

        /// <summary>
        /// 扩展发生变化事件
        /// </summary>
        public event ExtensionEventHandler ExtensionChanged;

        internal bool FireEvents
        {
            get { return fireEvents; }
        }

        #region public mehtod


        public void RegisterCondition(string id, ConditionType type)
        {
            type.Id = id;
            ConditionInfo info = CreateConditionInfo(id);
            ConditionType ot = info.CondType as ConditionType;
            if (ot != null)
                ot.Changed -= new EventHandler(OnConditionChanged);
            info.CondType = type;
            type.Changed += new EventHandler(OnConditionChanged);
        }


        public void RegisterCondition(string id, Type type)
        {
            // Allows delayed creation of condition types
            ConditionInfo info = CreateConditionInfo(id);
            ConditionType ot = info.CondType as ConditionType;
            if (ot != null)
                ot.Changed -= new EventHandler(OnConditionChanged);
            info.CondType = type;
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ExtensionNode GetExtensionNode(string path)
        {
            ExtensionTreeNode node = GetNode(path);

            if (node == null)
                return null;

            if (node.Condition == null || node.Condition.Evaluate(this))
                return node.ExtensionNode;
            else
                return null;
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T GetExtensionNode<T>(string path) where T : ExtensionNode
        {
            return (T)GetExtensionNode(path);
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ExtensionNodeList GetExtensionNodes(string path)
        {
            return GetExtensionNodes(path, null);
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public ExtensionNodeList<T> GetExtensionNodes<T>(string path) where T : ExtensionNode
        {
            ExtensionNodeList nodes = GetExtensionNodes(path, typeof(T));
            return new ExtensionNodeList<T>(nodes.list);
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public ExtensionNodeList GetExtensionNodes(Type instanceType)
        {
            return GetExtensionNodes(instanceType, typeof(ExtensionNode));
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="expectedNodeType"></param>
        /// <returns></returns>
        public ExtensionNodeList GetExtensionNodes(Type instanceType, Type expectedNodeType)
        {
            string path = this.GetAutoTypeExtensionPoint(instanceType);
            if (path == null)
                return new ExtensionNodeList(null);
            return GetExtensionNodes(path, expectedNodeType);
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public ExtensionNodeList<T> GetExtensionNodes<T>(Type instanceType) where T : ExtensionNode
        {
            string path = this.GetAutoTypeExtensionPoint(instanceType);
            if (path == null)
                return new ExtensionNodeList<T>(null);
            return new ExtensionNodeList<T>(GetExtensionNodes(path, typeof(T)).list);
        }

        /// <summary>
        /// 获得扩展节点
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expectedNodeType"></param>
        /// <returns></returns>
        public ExtensionNodeList GetExtensionNodes(string path, Type expectedNodeType)
        {
            ExtensionTreeNode node = GetNode(path);
            if (node == null || node.ExtensionNode == null)
                return ExtensionNodeList.Empty;

            ExtensionNodeList list = node.ExtensionNode.ChildNodes;

            if (expectedNodeType != null)
            {
                bool foundError = false;
                foreach (ExtensionNode cnode in list)
                {
                    if (!expectedNodeType.IsInstanceOfType(cnode))
                    {
                        foundError = true;
                        this.ReportError("Error while getting nodes for path '" + path + "'. Expected subclass of node type '" + expectedNodeType + "'. Found '" + cnode.GetType(), null, null, false);
                    }
                }
                if (foundError)
                {
                    // Create a new list excluding the elements that failed the test
                    List<ExtensionNode> newList = new List<ExtensionNode>();
                    foreach (ExtensionNode cnode in list)
                    {
                        if (expectedNodeType.IsInstanceOfType(cnode))
                            newList.Add(cnode);
                    }
                    return new ExtensionNodeList(newList);
                }
            }
            return list;
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(Type instanceType)
        {
            return GetExtensionObjects(instanceType, true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="bid"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public object[] GetExtensionObjectsByBundleId(string bid, Type instanceType)
        {
            return GetExtensionObjects(bid, instanceType, true);
        }


        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>()
        {
            return GetExtensionObjects<T>(true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bid"></param>
        /// <returns></returns>
        public T[] GetExtensionObjectsByBundleId<T>(string bid)
        {
            return GetExtensionObjectsByBundleId<T>(bid, true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance)
        {
            string path = this.GetAutoTypeExtensionPoint(instanceType);

            if (path == null)
                return (object[])Array.CreateInstance(instanceType, 0);

            return GetExtensionObjects(path, null, instanceType, reuseCachedInstance);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="bid"></param>
        /// <param name="instanceType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string bid, Type instanceType, bool reuseCachedInstance)
        {
            string path = this.GetAutoTypeExtensionPoint(instanceType);

            if (path == null)
                return (object[])Array.CreateInstance(instanceType, 0);

            return GetExtensionObjects(path, bid, instanceType, reuseCachedInstance);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(bool reuseCachedInstance)
        {
            string path = this.GetAutoTypeExtensionPoint(typeof(T));
            if (path == null)
                return new T[0];
            return GetExtensionObjects<T>(path, reuseCachedInstance);
        }

        public T[] GetExtensionObjectsByBundleId<T>(string bid, bool reuseCachedInstance)
        {
            string path = this.GetAutoTypeExtensionPoint(typeof(T));
            if (path == null)
                return new T[0];
            return GetExtensionObjects<T>(path, bid, reuseCachedInstance);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path)
        {
            return GetExtensionObjects(path, null, typeof(object), true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bid"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, string bid)
        {
            return GetExtensionObjects(path, bid, typeof(object), true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, bool reuseCachedInstance)
        {
            return GetExtensionObjects(path, null, typeof(object), reuseCachedInstance);
        }
        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bid"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, string bid, bool reuseCachedInstance)
        {
            return GetExtensionObjects(path, bid, typeof(object), reuseCachedInstance);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arrayElementType"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, Type arrayElementType)
        {
            return GetExtensionObjects(path, null, arrayElementType, true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bid"></param>
        /// <param name="arrayElementType"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, string bid, Type arrayElementType)
        {
            return GetExtensionObjects(path, bid, arrayElementType, true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(string path)
        {
            return GetExtensionObjects<T>(path, true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="bid"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(string path, string bid)
        {
            return GetExtensionObjects<T>(path, bid, true);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance)
        {
            ExtensionNode node = GetExtensionNode(path);
            if (node == null)
                throw new InvalidOperationException("Extension node not found in path: " + path);
            return node.GetChildObjects<T>(reuseCachedInstance);
        }

        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="bid"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(string path, string bid, bool reuseCachedInstance)
        {
            ExtensionNode node = GetExtensionNode(path);
            if (node == null)
                throw new InvalidOperationException("Extension node not found in path: " + path);
            return node.GetChildObjects<T>(bid, reuseCachedInstance);
        }


        /// <summary>
        /// 获得扩展对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bid"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, string bid, Type arrayElementType, bool reuseCachedInstance)
        {
            ExtensionNode node = GetExtensionNode(path);

            if (node == null)
                throw new InvalidOperationException("没有找到扩展节点的路径： " + path);

            return node.GetChildObjects(arrayElementType, reuseCachedInstance);
        }

        /// <summary>
        /// 注册一个扩展树节点上的监听事件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="handler"></param>
        public void AddExtensionNodeHandler(string path, ExtensionNodeEventHandler handler)
        {
            ExtensionNode node = GetExtensionNode(path);

            if (node == null)
                throw new InvalidOperationException("Extension node not found in path: " + path);

            node.ExtensionNodeChanged += handler;
        }

        /// <summary>
        ///  删除一个扩展树节点上的监听事件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="handler"></param>
        public void RemoveExtensionNodeHandler(string path, ExtensionNodeEventHandler handler)
        {
            ExtensionNode node = GetExtensionNode(path);
            if (node == null)
                throw new InvalidOperationException("Extension node not found in path: " + path);
            node.ExtensionNodeChanged -= handler;
        }

        /// <summary>
        /// 注册一个扩展树节点上的监听事件
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="handler"></param>
        public void AddExtensionNodeHandler(Type instanceType, ExtensionNodeEventHandler handler)
        {
            string path = this.GetAutoTypeExtensionPoint(instanceType);
            if (path == null)
                throw new InvalidOperationException("Type '" + instanceType + "' not bound to an extension point.");
            AddExtensionNodeHandler(path, handler);
        }

        /// <summary>
        /// 删除一个扩展树节点上的监听事件
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="handler"></param>
        public void RemoveExtensionNodeHandler(Type instanceType, ExtensionNodeEventHandler handler)
        {
            string path = this.GetAutoTypeExtensionPoint(instanceType);
            if (path == null)
                throw new InvalidOperationException("Type '" + instanceType + "' not bound to an extension point.");
            RemoveExtensionNodeHandler(path, handler);
        }

        public void Dispose()
        {
            this.Stop();
        }

        #endregion


        #region internal method

        internal void ClearContext()
        {
            conditionTypes.Clear();
            conditionsToNodes.Clear();
            childContexts = null;
            tree = null;
            runTimeEnabledBundles = null;
            runTimeDisabledBundles = null;
        }

        internal virtual void ResetCachedData()
        {
            tree.ResetCachedData();

            if (childContexts != null)
            {
                foreach (WeakReference wref in childContexts)
                {
                    ClampBundle ctx = wref.Target as ClampBundle;
                    if (ctx != null)
                        ctx.ResetCachedData();
                }
            }
        }

        internal ConditionType GetCondition(string id)
        {
            ConditionType ct;
            ConditionInfo info = (ConditionInfo)conditionTypes[id];

            if (info != null)
            {
                if (info.CondType is Type)
                {
                    // The condition was registered as a type, create an instance now
                    ct = (ConditionType)Activator.CreateInstance((Type)info.CondType);
                    ct.Id = id;
                    ct.Changed += new EventHandler(OnConditionChanged);
                    info.CondType = ct;
                }
                else
                    ct = info.CondType as ConditionType;

                if (ct != null)
                    return ct;
            }

            //if (parentContext != null)
            //    return parentContext.GetCondition(id);
            //else
            //    return null;

            return this.GetCondition(id);
        }

        internal void RegisterNodeCondition(ExtensionTreeNode node, BaseCondition cond)
        {
            ArrayList list = (ArrayList)conditionsToNodes[cond];
            if (list == null)
            {
                list = new ArrayList();
                conditionsToNodes[cond] = list;
                ArrayList conditionTypeIds = new ArrayList();
                cond.GetConditionTypes(conditionTypeIds);

                foreach (string cid in conditionTypeIds)
                {

                    // Make sure the condition is properly created
                    GetCondition(cid);

                    ConditionInfo info = CreateConditionInfo(cid);
                    if (info.BoundConditions == null)
                        info.BoundConditions = new ArrayList();

                    info.BoundConditions.Add(cond);
                }
            }
            list.Add(node);
        }

        internal void UnregisterNodeCondition(ExtensionTreeNode node, BaseCondition cond)
        {
            ArrayList list = (ArrayList)conditionsToNodes[cond];
            if (list == null)
                return;

            list.Remove(node);
            if (list.Count == 0)
            {
                conditionsToNodes.Remove(cond);
                ArrayList conditionTypeIds = new ArrayList();
                cond.GetConditionTypes(conditionTypeIds);
                foreach (string cid in conditionTypes.Keys)
                {
                    ConditionInfo info = conditionTypes[cid] as ConditionInfo;
                    if (info != null && info.BoundConditions != null)
                        info.BoundConditions.Remove(cond);
                }
            }
        }

        internal void NotifyConditionChanged(ConditionType cond)
        {
            try
            {
                fireEvents = true;

                ConditionInfo info = (ConditionInfo)conditionTypes[cond.Id];
                if (info != null && info.BoundConditions != null)
                {
                    Hashtable parentsToNotify = new Hashtable();
                    foreach (BaseCondition c in info.BoundConditions)
                    {
                        ArrayList nodeList = (ArrayList)conditionsToNodes[c];
                        if (nodeList != null)
                        {
                            foreach (ExtensionTreeNode node in nodeList)
                                parentsToNotify[node.Parent] = null;
                        }
                    }
                    foreach (ExtensionTreeNode node in parentsToNotify.Keys)
                    {
                        if (node.NotifyChildrenChanged())
                            NotifyExtensionsChanged(new ExtensionEventArgs(node.GetPath()));
                    }
                }
            }
            finally
            {
                fireEvents = false;
            }

            // Notify child contexts
            lock (conditionTypes)
            {
                if (childContexts != null)
                {
                    CleanDisposedChildContexts();
                    foreach (WeakReference wref in childContexts)
                    {
                        ClampBundle ctx = wref.Target as ClampBundle;
                        if (ctx != null)
                            ctx.NotifyConditionChanged(cond);
                    }
                }
            }
        }


        internal void NotifyExtensionsChanged(ExtensionEventArgs args)
        {
            if (!fireEvents)
                return;

            if (ExtensionChanged != null)
                ExtensionChanged(this, args);
        }

        internal void NotifyBundleLoaded(RuntimeBundle ad)
        {
            tree.NotifyBundleLoaded(ad, true);

            lock (conditionTypes)
            {
                if (childContexts != null)
                {
                    CleanDisposedChildContexts();
                    foreach (WeakReference wref in childContexts)
                    {
                        ClampBundle ctx = wref.Target as ClampBundle;
                        if (ctx != null)
                            ctx.NotifyBundleLoaded(ad);
                    }
                }
            }
        }

        internal void CreateExtensionPoint(ExtensionPoint ep)
        {
            ExtensionTreeNode node = tree.GetNode(ep.Path, true);

            if (node.ExtensionPoint == null)
            {
                node.ExtensionPoint = ep;
                node.ExtensionNodeSet = ep.NodeSet;
            }
        }

        internal void ActivateBundleExtensions(string id)
        {
            // Looks for loaded extension points which are extended by the provided
            // add-in, and adds the new nodes

            try
            {
                fireEvents = true;

                Bundle bundle = this.Registry.GetBundle(id);

                if (bundle == null)
                {
                    this.ReportError("Required add-in not found", id, null, false);
                    return;
                }

                // Take note that this add-in has been enabled at run-time
                // Needed because loaded add-in descriptions may not include this add-in. 
                RegisterRuntimeEnabledBundle(id);

                // Look for loaded extension points
                Hashtable eps = new Hashtable();
                ArrayList newExtensions = new ArrayList();

                foreach (ModuleDescription mod in bundle.Description.AllModules)
                {
                    foreach (Extension ext in mod.Extensions)
                    {
                        if (!newExtensions.Contains(ext.Path))
                            newExtensions.Add(ext.Path);

                        ExtensionPoint ep = tree.FindLoadedExtensionPoint(ext.Path);

                        if (ep != null && !eps.Contains(ep))
                            eps.Add(ep, ep);
                    }
                }

                // Add the new nodes
                ArrayList loadedNodes = new ArrayList();

                foreach (ExtensionPoint ep in eps.Keys)
                {
                    ExtensionLoadData data = GetBundleExtensions(id, ep);
                    if (data != null)
                    {
                        foreach (Extension ext in data.Extensions)
                        {
                            ExtensionTreeNode node = GetNode(ext.Path);

                            if (node != null && node.ExtensionNodeSet != null)
                            {
                                if (node.ChildrenLoaded)
                                    LoadModuleExtensionNodes(ext, data.BundleId, node.ExtensionNodeSet, loadedNodes);
                            }
                            else
                                this.ReportError("Extension node not found or not extensible: " + ext.Path, id, null, false);
                        }
                    }
                }

                // Call the OnBundleLoaded method on nodes, if the add-in is already loaded
                foreach (ExtensionTreeNode nod in loadedNodes)
                    nod.ExtensionNode.OnBundleLoaded();

                // Global extension change event. Other events are fired by LoadModuleExtensionNodes.
                // The event is called for all extensions, even for those not loaded. This is for coherence,
                // although that something that it doesn't make much sense to do (subscribing the ExtensionChanged
                // event without first getting the list of nodes that may change).
                foreach (string newExt in newExtensions)
                    NotifyExtensionsChanged(new ExtensionEventArgs(newExt));
            }
            finally
            {
                fireEvents = false;
            }
            // Do the same in child contexts

            lock (conditionTypes)
            {
                if (childContexts != null)
                {
                    CleanDisposedChildContexts();
                    foreach (WeakReference wref in childContexts)
                    {
                        ClampBundle ctx = wref.Target as ClampBundle;
                        if (ctx != null)
                            ctx.ActivateBundleExtensions(id);
                    }
                }
            }
        }

        internal void RemoveBundleExtensions(string id)
        {
            try
            {
                //指定Bundle注入不可用的集合中
                RegisterRuntimeDisabledBundle(id);

                fireEvents = true;

                // This method removes all extension nodes added by the add-in
                // Get all nodes created by the addin
                ArrayList list = new ArrayList();

                tree.FindBundleNodes(id, list);

                // Remove each node and notify the change
                foreach (ExtensionTreeNode node in list)
                {
                    if (node.ExtensionNode == null)
                    {
                        // It's an extension point. Just remove it, no notifications are needed
                        node.Remove();
                    }
                    else
                    {
                        node.ExtensionNode.OnBundleUnloaded();
                        node.Remove();
                    }
                }

                // Notify global extension point changes.
                // The event is called for all extensions, even for those not loaded. This is for coherence,
                // although that something that it doesn't make much sense to do (subscribing the ExtensionChanged
                // event without first getting the list of nodes that may change).

                // We get the runtime add-in because the add-in may already have been deleted from the registry
                RuntimeBundle addin = this.GetRuntimeBundle(id);

                if (addin != null)
                {
                    ArrayList paths = new ArrayList();
                    // Using addin.Module.ParentBundleDescription here because addin.Bundle.Description may not
                    // have a valid reference (the description is lazy loaded and may already have been removed from the registry)
                    foreach (ModuleDescription mod in addin.Module.ParentBundleDescription.AllModules)
                    {
                        foreach (Extension ext in mod.Extensions)
                        {
                            if (!paths.Contains(ext.Path))
                                paths.Add(ext.Path);
                        }
                    }
                    foreach (string path in paths)
                        NotifyExtensionsChanged(new ExtensionEventArgs(path));
                }
            }
            finally
            {
                fireEvents = false;
            }
        }

        /// <summary>
        /// 获得指定路径扩展下的Bundle,过滤不可用的Bundle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        internal ICollection GetBundlesForPath(string path, List<string> col)
        {
            ArrayList newlist = null;

            if (runTimeEnabledBundles != null && runTimeEnabledBundles.Count > 0)
            {
                newlist = new ArrayList();

                newlist.AddRange(col);

                foreach (string s in runTimeEnabledBundles)
                    if (!newlist.Contains(s))
                        newlist.Add(s);
            }

            if (runTimeDisabledBundles != null && runTimeDisabledBundles.Count > 0)
            {
                if (newlist == null)
                {
                    newlist = new ArrayList();
                    newlist.AddRange(col);
                }

                foreach (string s in runTimeDisabledBundles)
                    newlist.Remove(s);
            }

            return newlist != null ? (ICollection)newlist : (ICollection)col;
        }

        /// <summary>
        /// 通过扩展路径加载扩展
        /// </summary>
        /// <param name="requestedExtensionPath"></param>
        internal void LoadExtensions(string requestedExtensionPath)
        {
            ExtensionTreeNode node = GetNode(requestedExtensionPath);

            if (node == null)
                throw new InvalidOperationException("Extension point not defined: " + requestedExtensionPath);

            ExtensionPoint ep = node.ExtensionPoint;

            if (ep != null)
            {

                //先收集相关的扩展信息

                ArrayList loadData = new ArrayList();

                foreach (string bundleId in GetBundlesForPath(ep.Path, ep.Bundles))
                {
                    ExtensionLoadData ed = GetBundleExtensions(bundleId, ep);

                    if (ed != null)
                    {
                        // Insert the addin data taking into account dependencies.
                        // An add-in must be processed after all its dependencies.
                        bool added = false;

                        for (int n = 0; n < loadData.Count; n++)
                        {
                            ExtensionLoadData other = (ExtensionLoadData)loadData[n];

                            if (this.Registry.BundleDependsOn(other.BundleId, ed.BundleId))
                            {
                                loadData.Insert(n, ed);

                                added = true;

                                break;
                            }
                        }

                        if (!added)
                            loadData.Add(ed);
                    }
                }

                // Now load the extensions

                ArrayList loadedNodes = new ArrayList();

                foreach (ExtensionLoadData data in loadData)
                {
                    foreach (Extension ext in data.Extensions)
                    {
                        ExtensionTreeNode cnode = GetNode(ext.Path);

                        if (cnode != null && cnode.ExtensionNodeSet != null)
                            LoadModuleExtensionNodes(ext, data.BundleId, cnode.ExtensionNodeSet, loadedNodes);
                        else
                            this.ReportError("Extension node not found or not extensible: " + ext.Path, data.BundleId, null, false);
                    }
                }

                // Call the OnBundleLoaded method on nodes, if the add-in is already loaded
                foreach (ExtensionTreeNode nod in loadedNodes)
                    nod.ExtensionNode.OnBundleLoaded();

                NotifyExtensionsChanged(new ExtensionEventArgs(requestedExtensionPath));
            }
        }

        internal bool FindExtensionPathByType(Type type, string nodeName, out string path, out string pathNodeName)
        {
            return tree.FindExtensionPathByType(type, nodeName, out path, out pathNodeName);
        }

        #endregion

        #region private method
        private void CleanDisposedChildContexts()
        {
            if (childContexts != null)
                childContexts.RemoveAll(w => w.Target == null);
        }

        private ConditionInfo CreateConditionInfo(string id)
        {
            ConditionInfo info = conditionTypes[id] as ConditionInfo;
            if (info == null)
            {
                info = new ConditionInfo();
                conditionTypes[id] = info;
            }
            return info;
        }


        private void OnConditionChanged(object s, EventArgs a)
        {
            ConditionType cond = (ConditionType)s;
            NotifyConditionChanged(cond);
        }

        /// <summary>
        /// 把指定的Bundle的ID注入到不可用的Bundle集合中
        /// </summary>
        /// <param name="bundleId"></param>
        private void RegisterRuntimeDisabledBundle(string bundleId)
        {
            if (runTimeDisabledBundles == null)
                runTimeDisabledBundles = new ArrayList();

            if (!runTimeDisabledBundles.Contains(bundleId))
                runTimeDisabledBundles.Add(bundleId);

            if (runTimeEnabledBundles != null)
                runTimeEnabledBundles.Remove(bundleId);
        }

        private void RegisterRuntimeEnabledBundle(string bundleId)
        {
            if (runTimeEnabledBundles == null)
                runTimeEnabledBundles = new ArrayList();
            if (!runTimeEnabledBundles.Contains(bundleId))
                runTimeEnabledBundles.Add(bundleId);

            if (runTimeDisabledBundles != null)
                runTimeDisabledBundles.Remove(bundleId);
        }

        /// <summary>
        /// 获得当前指定Bundle扩展信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ep"></param>
        /// <returns></returns>
        private ExtensionLoadData GetBundleExtensions(string id, ExtensionPoint ep)
        {
            Bundle bundle = null;

            // Root add-ins are not returned by GetInstalledBundle.
            RuntimeBundle runtimeBundle = this.GetRuntimeBundle(id);

            if (runtimeBundle != null)
                bundle = runtimeBundle.Bundle;
            else
                bundle = this.Registry.GetBundle(id);

            if (bundle == null)
            {
                this.ReportError($"找不到相关的Bundle({id})", id, null, false);
                return null;
            }

            if (!bundle.Enabled || bundle.Version != Bundle.GetIdVersion(id))
                return null;

            // Loads extensions defined in each module

            ExtensionLoadData data = null;
            BundleDescription conf = bundle.Description;

            GetBundleExtensions(conf.MainModule, id, ep, ref data);

            foreach (ModuleDescription module in conf.OptionalModules)
            {
                if (CheckOptionalBundleDependencies(conf, module))
                    GetBundleExtensions(module, id, ep, ref data);
            }

            if (data != null)
                data.Extensions.Sort();

            return data;
        }

        /// <summary>
        /// 获得Bunlde的扩展信息
        /// </summary>
        /// <param name="module"></param>
        /// <param name="budnleId"></param>
        /// <param name="ep"></param>
        /// <param name="data"></param>
        private void GetBundleExtensions(ModuleDescription module, string budnleId, ExtensionPoint ep, ref ExtensionLoadData data)
        {
            string basePath = ep.Path + "/";

            foreach (Extension extension in module.Extensions)
            {
                if (extension.Path == ep.Path || extension.Path.StartsWith(basePath))
                {
                    if (data == null)
                    {
                        data = new ExtensionLoadData();
                        data.BundleId = budnleId;
                        data.Extensions = new ArrayList();
                    }

                    data.Extensions.Add(extension);
                }
            }
        }

        private void LoadModuleExtensionNodes(Extension extension, string bundleId, ExtensionNodeSet nset, ArrayList loadedNodes)
        {
            // Now load the extensions
            ArrayList addedNodes = new ArrayList();

            tree.LoadExtension(bundleId, extension, addedNodes);

            RuntimeBundle ad = this.GetRuntimeBundle(bundleId);

            if (ad != null)
            {
                foreach (ExtensionTreeNode nod in addedNodes)
                {
                    // Don't call OnBundleLoaded here. Do it when the entire extension point has been loaded.
                    if (nod.ExtensionNode != null)
                        loadedNodes.Add(nod);
                }
            }
        }

        private bool CheckOptionalBundleDependencies(BundleDescription conf, ModuleDescription module)
        {
            foreach (Dependency dep in module.Dependencies)
            {
                BundleDependency pdep = dep as BundleDependency;

                if (pdep != null)
                {
                    Bundle pinfo = this.Registry.GetBundle(Bundle.GetFullId(conf.Namespace, pdep.BundleId, pdep.Version));

                    if (pinfo == null || !pinfo.Enabled)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 根据路径找到扩展树节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ExtensionTreeNode GetNode(string path)
        {
            ExtensionTreeNode node = this.tree.GetNode(path);

            if (node != null)
                return node;

            ExtensionTreeNode supNode = this.tree.GetNode(path);

            if (supNode == null)
                return null;

            if (path.StartsWith("/"))
                path = path.Substring(1);

            string[] parts = path.Split('/');
            ExtensionTreeNode srcNode = this.tree;
            ExtensionTreeNode dstNode = tree;

            foreach (string part in parts)
            {

                // Look for the node in the source tree

                int i = srcNode.Children.IndexOfNode(part);
                if (i != -1)
                    srcNode = srcNode.Children[i];
                else
                    return null;

                // Now get the node in the target tree

                int j = dstNode.Children.IndexOfNode(part);
                if (j != -1)
                {
                    dstNode = dstNode.Children[j];
                }
                else
                {
                    // Create if not found
                    ExtensionTreeNode newNode = new ExtensionTreeNode(this, part);
                    dstNode.AddChildNode(newNode);
                    dstNode = newNode;

                    // Copy extension data
                    dstNode.ExtensionNodeSet = srcNode.ExtensionNodeSet;
                    dstNode.ExtensionPoint = srcNode.ExtensionPoint;
                    dstNode.Condition = srcNode.Condition;

                    if (dstNode.Condition != null)
                        RegisterNodeCondition(dstNode, dstNode.Condition);
                }
            }

            return dstNode;
        }





        #endregion


    }

    class ConditionInfo
    {
        public object CondType;
        public ArrayList BoundConditions;
    }


    /// <summary>
    /// Delegate to be used in extension point subscriptions
    /// </summary>
    public delegate void ExtensionEventHandler(object sender, ExtensionEventArgs args);

    /// <summary>
    /// Delegate to be used in extension point subscriptions
    /// </summary>
    public delegate void ExtensionNodeEventHandler(object sender, ExtensionNodeEventArgs args);

    /// <summary>
    /// 扩展事件参数
    /// </summary>
    public class ExtensionEventArgs : EventArgs
    {
        string path;

        internal ExtensionEventArgs()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="path">
        /// Path of the extension node that has changed.
        /// </param>
        public ExtensionEventArgs(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Path of the extension node that has changed.
        /// </summary>
        public virtual string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Checks if a path has changed.
        /// </summary>
        /// <param name="pathToCheck">
        /// An extension path.
        /// </param>
        /// <returns>
        /// 'true' if the path is affected by the extension change event.
        /// </returns>
        /// <remarks>
        /// Checks if the specified path or any of its children paths is affected by the extension change event.
        /// </remarks>
        public bool PathChanged(string pathToCheck)
        {
            if (pathToCheck.EndsWith("/"))
                return path.StartsWith(pathToCheck);
            else
                return path.StartsWith(pathToCheck) && (pathToCheck.Length == path.Length || path[pathToCheck.Length] == '/');
        }
    }

    /// <summary>
    /// 扩展节点事件参数
    /// </summary>
    public class ExtensionNodeEventArgs : ExtensionEventArgs
    {
        ExtensionNode node;
        ExtensionChange change;

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="change">
        /// Type of change.
        /// </param>
        /// <param name="node">
        /// Node that has been added or removed.
        /// </param>
        public ExtensionNodeEventArgs(ExtensionChange change, ExtensionNode node)
        {
            this.node = node;
            this.change = change;
        }

        /// <summary>
        /// Path of the extension that changed.
        /// </summary>
        public override string Path
        {
            get { return node.Path; }
        }

        /// <summary>
        /// Type of change.
        /// </summary>
        public ExtensionChange Change
        {
            get { return change; }
        }

        /// <summary>
        /// Node that has been added or removed.
        /// </summary>
        public ExtensionNode ExtensionNode
        {
            get { return node; }
        }

        /// <summary>
        /// Extension object that has been added or removed.
        /// </summary>
        public object ExtensionObject
        {
            get
            {
                InstanceExtensionNode tnode = node as InstanceExtensionNode;
                if (tnode == null)
                    throw new InvalidOperationException("Node is not an InstanceExtensionNode");
                return tnode.GetInstance();
            }
        }
    }

    /// <summary>
    /// 事件类型
    /// </summary>
    public enum ExtensionChange
    {
        Add,
        Remove
    }


    internal class ExtensionLoadData
    {
        public string BundleId;
        public ArrayList Extensions;
    }
}
