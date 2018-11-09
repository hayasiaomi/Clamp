using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Nodes
{
    class TreeNode
    {
        ArrayList childrenList;
        TreeNodeCollection children;
        ExtensionNode extensionNode;
        bool childrenLoaded;
        string id;
        TreeNode parent;
        ExtensionNodeSet nodeTypes;
        ExtensionPoint extensionPoint;
        BaseCondition condition;
        protected ClampBundle addinEngine;

        public TreeNode(ClampBundle addinEngine, string id)
        {
            this.id = id;
            this.addinEngine = addinEngine;

            // Root node
            if (id.Length == 0)
                childrenLoaded = true;
        }

        public ClampBundle BundleEngine
        {
            get { return addinEngine; }
        }

        internal void AttachExtensionNode(ExtensionNode enode)
        {
            this.extensionNode = enode;
            if (extensionNode != null)
                extensionNode.SetTreeNode(this);
        }

        public string Id
        {
            get { return id; }
        }

        public ExtensionNode ExtensionNode
        {
            get
            {
                if (extensionNode == null && extensionPoint != null)
                {
                    extensionNode = new ExtensionNode();
                    extensionNode.SetData(addinEngine, extensionPoint.RootBundle, null, null);
                    AttachExtensionNode(extensionNode);
                }
                return extensionNode;
            }
        }

        public ExtensionPoint ExtensionPoint
        {
            get { return extensionPoint; }
            set { extensionPoint = value; }
        }

        public ExtensionNodeSet ExtensionNodeSet
        {
            get { return nodeTypes; }
            set { nodeTypes = value; }
        }

        public TreeNode Parent
        {
            get { return parent; }
        }

        public BaseCondition Condition
        {
            get { return condition; }
            set
            {
                condition = value;
            }
        }

        public virtual TreeNodeBundle Context
        {
            get
            {
                if (parent != null)
                    return parent.Context;
                else
                    return null;
            }
        }

        public bool IsEnabled
        {
            get
            {
                if (condition == null)
                    return true;
                TreeNodeBundle ctx = Context;
                if (ctx == null)
                    return true;
                else
                    return condition.Evaluate(ctx);
            }
        }

        public bool ChildrenLoaded
        {
            get { return childrenLoaded; }
        }

        public void AddChildNode(TreeNode node)
        {
            node.parent = this;
            if (childrenList == null)
                childrenList = new ArrayList();
            childrenList.Add(node);
        }

        public void InsertChildNode(int n, TreeNode node)
        {
            node.parent = this;
            if (childrenList == null)
                childrenList = new ArrayList();
            childrenList.Insert(n, node);

            // Dont call NotifyChildrenChanged here. It is called by ExtensionTree,
            // after inserting all children of the node.
        }

        internal int ChildCount
        {
            get { return childrenList == null ? 0 : childrenList.Count; }
        }

        public ExtensionNode GetExtensionNode(string path, string childId)
        {
            TreeNode node = GetNode(path, childId);
            return node != null ? node.ExtensionNode : null;
        }

        public ExtensionNode GetExtensionNode(string path)
        {
            TreeNode node = GetNode(path);
            return node != null ? node.ExtensionNode : null;
        }

        public TreeNode GetNode(string path, string childId)
        {
            if (childId == null || childId.Length == 0)
                return GetNode(path);
            else
                return GetNode(path + "/" + childId);
        }

        public TreeNode GetNode(string path)
        {
            return GetNode(path, false);
        }

        public TreeNode GetNode(string path, bool buildPath)
        {
            if (path.StartsWith("/"))
                path = path.Substring(1);

            string[] parts = path.Split('/');
            TreeNode curNode = this;

            foreach (string part in parts)
            {
                int i = curNode.Children.IndexOfNode(part);
                if (i != -1)
                {
                    curNode = curNode.Children[i];
                    continue;
                }

                if (buildPath)
                {
                    TreeNode newNode = new TreeNode(addinEngine, part);
                    curNode.AddChildNode(newNode);
                    curNode = newNode;
                }
                else
                    return null;
            }
            return curNode;
        }

        public TreeNodeCollection Children
        {
            get
            {
                if (!childrenLoaded)
                {
                    childrenLoaded = true;
                    if (extensionPoint != null)
                        Context.LoadExtensions(GetPath());
                    // We have to keep the relation info, since add-ins may be loaded/unloaded
                }
                if (childrenList == null)
                    return TreeNodeCollection.Empty;
                if (children == null)
                    children = new TreeNodeCollection(childrenList);
                return children;
            }
        }

        public string GetPath()
        {
            int num = 0;
            TreeNode node = this;
            while (node != null)
            {
                num++;
                node = node.parent;
            }

            string[] ids = new string[num];

            node = this;
            while (node != null)
            {
                ids[--num] = node.id;
                node = node.parent;
            }
            return string.Join("/", ids);
        }

        public void NotifyBundleLoaded(RuntimeBundle ad, bool recursive)
        {
            if (extensionNode != null && extensionNode.BundleId == ad.Bundle.Id)
                extensionNode.OnBundleLoaded();
            if (recursive && childrenLoaded)
            {
                foreach (TreeNode node in Children.Clone())
                    node.NotifyBundleLoaded(ad, true);
            }
        }

        public ExtensionPoint FindLoadedExtensionPoint(string path)
        {
            if (path.StartsWith("/"))
                path = path.Substring(1);

            string[] parts = path.Split('/');
            TreeNode curNode = this;

            foreach (string part in parts)
            {
                int i = curNode.Children.IndexOfNode(part);
                if (i != -1)
                {
                    curNode = curNode.Children[i];
                    if (!curNode.ChildrenLoaded)
                        return null;
                    if (curNode.ExtensionPoint != null)
                        return curNode.ExtensionPoint;
                    continue;
                }
                return null;
            }
            return null;
        }

        public void FindBundleNodes(string id, ArrayList nodes)
        {
            if (id != null && extensionPoint != null && extensionPoint.RootBundle == id)
            {
                // It is an extension point created by the add-in. All nodes below this
                // extension point will be added to the list, even if they come from other add-ins.
                id = null;
            }

            if (childrenLoaded)
            {
                // Deep-first search, to make sure children are removed before the parent.
                foreach (TreeNode node in Children)
                    node.FindBundleNodes(id, nodes);
            }

            if (id == null || (ExtensionNode != null && ExtensionNode.BundleId == id))
                nodes.Add(this);
        }

        public bool FindExtensionPathByType(Type type, string nodeName, out string path, out string pathNodeName)
        {
            if (extensionPoint != null)
            {
                foreach (ExtensionNodeType nt in extensionPoint.NodeSet.NodeTypes)
                {
                    if (nt.ObjectTypeName.Length > 0 && (nodeName.Length == 0 || nodeName == nt.Id))
                    {
                        RuntimeBundle addin = addinEngine.GetBundle(extensionPoint.RootBundle);
                        Type ot = addin.GetType(nt.ObjectTypeName);
                        if (ot != null)
                        {
                            if (ot.IsAssignableFrom(type))
                            {
                                path = extensionPoint.Path;
                                pathNodeName = nt.Id;
                                return true;
                            }
                        }

                    }
                }
            }
            else
            {
                foreach (TreeNode node in Children)
                {
                    if (node.FindExtensionPathByType(type, nodeName, out path, out pathNodeName))
                        return true;
                }
            }
            path = null;
            pathNodeName = null;
            return false;
        }

        public void Remove()
        {
            if (parent != null)
            {
                if (Condition != null)
                    Context.UnregisterNodeCondition(this, Condition);
                parent.childrenList.Remove(this);
                parent.NotifyChildrenChanged();
            }
        }

        public bool NotifyChildrenChanged()
        {
            if (extensionNode != null)
                return extensionNode.NotifyChildChanged();
            else
                return false;
        }

        public void ResetCachedData()
        {
            if (extensionPoint != null)
            {
                string aid = Bundle.GetIdName(extensionPoint.ParentBundleDescription.BundleId);
                RuntimeBundle ad = addinEngine.GetBundle(aid);
                if (ad != null)
                    extensionPoint = ad.Bundle.Description.ExtensionPoints[GetPath()];
            }
            if (childrenList != null)
            {
                foreach (TreeNode cn in childrenList)
                    cn.ResetCachedData();
            }
        }
    }
}
