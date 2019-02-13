using Clamp.Data.Annotation;
using Clamp.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.Nodes
{
    /// <summary>
    /// 扩展树
    /// </summary>
    internal class ExtensionTreeNode
    {
        internal const string AutoIdPrefix = "__nid_";

        private ArrayList childrenList;
        private TreeNodeCollection children;
        private ExtensionNode extensionNode;
        private bool childrenLoaded;
        private string id;
        private ExtensionTreeNode parent;
        private ExtensionNodeSet nodeTypes;
        private ExtensionPoint extensionPoint;
        private BaseCondition condition;
        private int internalId;

        protected ClampBundle clampBundle;

        public ExtensionTreeNode(ClampBundle clampBundle, string id)
        {
            this.id = id;
            this.clampBundle = clampBundle;

            // Root node
            if (id.Length == 0)
                childrenLoaded = true;
        }

        #region Propertise

        public string Id
        {
            get { return id; }
        }

        public ExtensionNode ExtensionNode
        {
            get
            {
                if (this.extensionNode == null && this.extensionPoint != null)
                {
                    this.extensionNode = new ExtensionNode();

                    this.extensionNode.SetData(this.clampBundle, this.extensionPoint.RootBundle, null, null);

                    AttachExtensionNode(this.extensionNode);
                }

                return this.extensionNode;
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

        public ExtensionTreeNode Parent
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
        public bool IsEnabled
        {
            get
            {
                if (condition == null)
                    return true;

                ClampBundle ctx = this.clampBundle;

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

        internal int ChildCount
        {
            get { return childrenList == null ? 0 : childrenList.Count; }
        }

        public TreeNodeCollection Children
        {
            get
            {
                if (!childrenLoaded)
                {
                    childrenLoaded = true;
                    if (extensionPoint != null)
                        this.clampBundle.LoadExtensions(GetPath());
                    // We have to keep the relation info, since add-ins may be loaded/unloaded
                }

                if (childrenList == null)
                    return TreeNodeCollection.Empty;

                if (children == null)
                    children = new TreeNodeCollection(childrenList);

                return children;
            }
        }

        #endregion

        #region internal method

        internal void AttachExtensionNode(ExtensionNode enode)
        {
            this.extensionNode = enode;

            if (this.extensionNode != null)
                this.extensionNode.SetTreeNode(this);
        }

        #endregion

        #region public method

        public ExtensionNode GetExtensionNode(string path, string childId)
        {
            ExtensionTreeNode node = GetNode(path, childId);
            return node != null ? node.ExtensionNode : null;
        }

        public ExtensionNode GetExtensionNode(string path)
        {
            ExtensionTreeNode node = GetNode(path);
            return node != null ? node.ExtensionNode : null;
        }

        public ExtensionTreeNode GetNode(string path, string childId)
        {
            if (childId == null || childId.Length == 0)
                return GetNode(path);
            else
                return GetNode(path + "/" + childId);
        }

        public ExtensionTreeNode GetNode(string path)
        {
            return GetNode(path, false);
        }

        public ExtensionTreeNode GetNode(string path, bool buildPath)
        {
            if (path.StartsWith("/"))
                path = path.Substring(1);

            string[] parts = path.Split('/');
            ExtensionTreeNode curNode = this;

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
                    ExtensionTreeNode newNode = new ExtensionTreeNode(clampBundle, part);
                    curNode.AddChildNode(newNode);
                    curNode = newNode;
                }
                else
                    return null;
            }
            return curNode;
        }



        public string GetPath()
        {
            int num = 0;
            ExtensionTreeNode node = this;
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
                foreach (ExtensionTreeNode node in Children.Clone())
                    node.NotifyBundleLoaded(ad, true);
            }
        }

        public ExtensionPoint FindLoadedExtensionPoint(string path)
        {
            if (path.StartsWith("/"))
                path = path.Substring(1);

            string[] parts = path.Split('/');
            ExtensionTreeNode curNode = this;

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
                foreach (ExtensionTreeNode node in Children)
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
                        RuntimeBundle addin = clampBundle.GetRuntimeBundle(extensionPoint.RootBundle);
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
                foreach (ExtensionTreeNode node in Children)
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
                    this.clampBundle.UnregisterNodeCondition(this, Condition);
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
                RuntimeBundle ad = clampBundle.GetRuntimeBundle(aid);
                if (ad != null)
                    extensionPoint = ad.Bundle.Description.ExtensionPoints[GetPath()];
            }
            if (childrenList != null)
            {
                foreach (ExtensionTreeNode cn in childrenList)
                    cn.ResetCachedData();
            }
        }

        public void AddChildNode(ExtensionTreeNode node)
        {
            node.parent = this;
            if (childrenList == null)
                childrenList = new ArrayList();
            childrenList.Add(node);
        }

        public void InsertChildNode(int n, ExtensionTreeNode node)
        {
            node.parent = this;
            if (childrenList == null)
                childrenList = new ArrayList();
            childrenList.Insert(n, node);

            // Dont call NotifyChildrenChanged here. It is called by ExtensionTree,
            // after inserting all children of the node.
        }

        public void LoadExtension(string bundleId, Extension extension, ArrayList addedNodes)
        {
            ExtensionTreeNode tnode = GetNode(extension.Path);

            if (tnode == null)
            {
                clampBundle.ReportError("Can't load extensions for path '" + extension.Path + "'. Extension point not defined.", bundleId, null, false);
                return;
            }

            int curPos = -1;

            LoadExtensionElement(tnode, bundleId, extension.ExtensionNodes, (ModuleDescription)extension.Parent, ref curPos, tnode.Condition, false, addedNodes);
        }


        public ExtensionNode ReadNode(ExtensionTreeNode tnode, string bundleId, ExtensionNodeType ntype, ExtensionNodeDescription elem, ModuleDescription module)
        {
            try
            {
                if (ntype.Type == null)
                {
                    if (!InitializeNodeType(ntype))
                        return null;
                }

                ExtensionNode node = Activator.CreateInstance(ntype.Type) as ExtensionNode;

                if (node == null)
                {
                    clampBundle.ReportError("Extension node type '" + ntype.Type + "' must be a subclass of ExtensionNode", bundleId, null, false);
                    return null;
                }

                tnode.AttachExtensionNode(node);

                node.SetData(clampBundle, bundleId, ntype, module);
                node.Read(elem);

                return node;
            }
            catch (Exception ex)
            {
                clampBundle.ReportError("Could not read extension node of type '" + ntype.Type + "' from extension path '" + tnode.GetPath() + "'", bundleId, ex, false);
                return null;
            }
        }

        #endregion

        #region private mehtod

        private void LoadExtensionElement(ExtensionTreeNode tnode,
            string bundleId,
            ExtensionNodeDescriptionCollection extension,
            ModuleDescription module,
            ref int curPos,
            BaseCondition parentCondition,
            bool inComplextCondition,
            ArrayList addedNodes)
        {
            foreach (ExtensionNodeDescription elem in extension)
            {

                if (inComplextCondition)
                {
                    parentCondition = ReadComplexCondition(elem, parentCondition);
                    inComplextCondition = false;
                    continue;
                }

                if (elem.NodeName == "ComplexCondition")
                {
                    LoadExtensionElement(tnode, bundleId, elem.ChildNodes, module, ref curPos, parentCondition, true, addedNodes);
                    continue;
                }

                if (elem.NodeName == "Condition")
                {
                    Condition cond = new Condition(this.clampBundle, elem, parentCondition);
                    LoadExtensionElement(tnode, bundleId, elem.ChildNodes, module, ref curPos, cond, false, addedNodes);
                    continue;
                }

                var pnode = tnode;

                ExtensionPoint extensionPoint = null;

                while (pnode != null && (extensionPoint = pnode.ExtensionPoint) == null)
                    pnode = pnode.Parent;

                string after = elem.GetAttribute("insertafter");

                if (after.Length == 0 && extensionPoint != null && curPos == -1)
                    after = extensionPoint.DefaultInsertAfter;

                if (after.Length > 0)
                {
                    int i = tnode.Children.IndexOfNode(after);
                    if (i != -1)
                        curPos = i + 1;
                }

                string before = elem.GetAttribute("insertbefore");

                if (before.Length == 0 && extensionPoint != null && curPos == -1)
                    before = extensionPoint.DefaultInsertBefore;

                if (before.Length > 0)
                {
                    int i = tnode.Children.IndexOfNode(before);
                    if (i != -1)
                        curPos = i;
                }

                // If node position is not explicitly set, add the node at the end
                if (curPos == -1)
                    curPos = tnode.Children.Count;

                // Find the type of the node in this extension
                ExtensionNodeType ntype = clampBundle.FindType(tnode.ExtensionNodeSet, elem.NodeName, bundleId);

                if (ntype == null)
                {
                    clampBundle.ReportError("Node '" + elem.NodeName + "' not allowed in extension: " + tnode.GetPath(), bundleId, null, false);
                    continue;
                }

                string id = elem.GetAttribute("id");

                if (id.Length == 0)
                    id = AutoIdPrefix + (++internalId);

                ExtensionTreeNode cnode = new ExtensionTreeNode(clampBundle, id);

                ExtensionNode enode = ReadNode(cnode, bundleId, ntype, elem, module);

                if (enode == null)
                    continue;

                cnode.Condition = parentCondition;
                cnode.ExtensionNodeSet = ntype;

                tnode.InsertChildNode(curPos, cnode);

                addedNodes.Add(cnode);

                if (cnode.Condition != null)
                    this.clampBundle.RegisterNodeCondition(cnode, cnode.Condition);

                // Load children
                if (elem.ChildNodes.Count > 0)
                {
                    int cp = 0;

                    LoadExtensionElement(cnode, bundleId, elem.ChildNodes, module, ref cp, parentCondition, false, addedNodes);
                }

                curPos++;
            }

            if (this.clampBundle.FireEvents)
                tnode.NotifyChildrenChanged();
        }

        private BaseCondition ReadComplexCondition(ExtensionNodeDescription elem, BaseCondition parentCondition)
        {
            if (elem.NodeName == "Or" || elem.NodeName == "And" || elem.NodeName == "Not")
            {
                ArrayList conds = new ArrayList();
                foreach (ExtensionNodeDescription celem in elem.ChildNodes)
                {
                    conds.Add(ReadComplexCondition(celem, null));
                }

                if (elem.NodeName == "Or")
                    return new OrCondition((BaseCondition[])conds.ToArray(typeof(BaseCondition)), parentCondition);
                else if (elem.NodeName == "And")
                    return new AndCondition((BaseCondition[])conds.ToArray(typeof(BaseCondition)), parentCondition);
                else
                {
                    if (conds.Count != 1)
                    {
                        clampBundle.ReportError("Invalid complex condition element '" + elem.NodeName + "'. 'Not' condition can only have one parameter.", null, null, false);
                        return new NullCondition();
                    }
                    return new NotCondition((BaseCondition)conds[0], parentCondition);
                }
            }

            if (elem.NodeName == "Condition")
            {
                return new Condition(this.clampBundle, elem, parentCondition);
            }

            clampBundle.ReportError("Invalid complex condition element '" + elem.NodeName + "'.", null, null, false);

            return new NullCondition();
        }

        private bool InitializeNodeType(ExtensionNodeType ntype)
        {
            RuntimeBundle p = clampBundle.GetRuntimeBundle(ntype.BundleId);

            if (p == null)
            {
                if (!clampBundle.IsBundleLoaded(ntype.BundleId))
                {
                    if (!clampBundle.LoadBundle(ntype.BundleId, false))
                        return false;

                    p = clampBundle.GetRuntimeBundle(ntype.BundleId);

                    if (p == null)
                    {
                        clampBundle.ReportError("Add-in not found", ntype.BundleId, null, false);
                        return false;
                    }
                }
            }

            // If no type name is provided, use TypeExtensionNode by default
            if (ntype.TypeName == null || ntype.TypeName.Length == 0 || ntype.TypeName == typeof(TypeExtensionNode).FullName)
            {
                // If it has a custom attribute, use the generic version of TypeExtensionNode
                if (ntype.ExtensionAttributeTypeName.Length > 0)
                {
                    Type attType = p.GetType(ntype.ExtensionAttributeTypeName, false);
                    if (attType == null)
                    {
                        clampBundle.ReportError("Custom attribute type '" + ntype.ExtensionAttributeTypeName + "' not found.", ntype.BundleId, null, false);
                        return false;
                    }
                    if (ntype.ObjectTypeName.Length > 0 || ntype.TypeName == typeof(TypeExtensionNode).FullName)
                        ntype.Type = typeof(TypeExtensionNode<>).MakeGenericType(attType);
                    else
                        ntype.Type = typeof(ExtensionNode<>).MakeGenericType(attType);
                }
                else
                {
                    ntype.Type = typeof(TypeExtensionNode);
                    return true;
                }
            }
            else
            {
                ntype.Type = p.GetType(ntype.TypeName, false);

                if (ntype.Type == null)
                {
                    clampBundle.ReportError("Extension node type '" + ntype.TypeName + "' not found.", ntype.BundleId, null, false);
                    return false;
                }
            }

            // Check if the type has NodeAttribute attributes applied to fields.
            ExtensionNodeType.FieldData boundAttributeType = null;
            Dictionary<string, ExtensionNodeType.FieldData> fields = GetMembersMap(ntype.Type, out boundAttributeType);
            ntype.CustomAttributeMember = boundAttributeType;

            if (fields.Count > 0)
                ntype.Fields = fields;

            // If the node type is bound to a custom attribute and there is a member bound to that attribute,
            // get the member map for the attribute.

            if (boundAttributeType != null)
            {
                if (ntype.ExtensionAttributeTypeName.Length == 0)
                    throw new InvalidOperationException("Extension node not bound to a custom attribute.");

                if (ntype.ExtensionAttributeTypeName != boundAttributeType.MemberType.FullName)
                    throw new InvalidOperationException("Incorrect custom attribute type declaration in " + ntype.Type + ". Expected '" + ntype.ExtensionAttributeTypeName + "' found '" + boundAttributeType.MemberType.FullName + "'");

                fields = GetMembersMap(boundAttributeType.MemberType, out boundAttributeType);
                if (fields.Count > 0)
                    ntype.CustomAttributeFields = fields;
            }

            return true;
        }

        private Dictionary<string, ExtensionNodeType.FieldData> GetMembersMap(Type type, out ExtensionNodeType.FieldData boundAttributeType)
        {
            string fname;
            Dictionary<string, ExtensionNodeType.FieldData> fields = new Dictionary<string, ExtensionNodeType.FieldData>();
            boundAttributeType = null;

            while (type != typeof(object) && type != null)
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    NodeAttributeAttribute at = (NodeAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(NodeAttributeAttribute), true);
                    if (at != null)
                    {
                        ExtensionNodeType.FieldData fd = CreateFieldData(field, at, out fname, ref boundAttributeType);
                        if (fd != null)
                            fields[fname] = fd;
                    }
                }
                foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    NodeAttributeAttribute at = (NodeAttributeAttribute)Attribute.GetCustomAttribute(prop, typeof(NodeAttributeAttribute), true);
                    if (at != null)
                    {
                        ExtensionNodeType.FieldData fd = CreateFieldData(prop, at, out fname, ref boundAttributeType);
                        if (fd != null)
                            fields[fname] = fd;
                    }
                }
                type = type.BaseType;
            }

            return fields;
        }

        private ExtensionNodeType.FieldData CreateFieldData(MemberInfo member, NodeAttributeAttribute at, out string name, ref ExtensionNodeType.FieldData boundAttributeType)
        {
            ExtensionNodeType.FieldData fdata = new ExtensionNodeType.FieldData();

            fdata.Member = member;
            fdata.Required = at.Required;
            fdata.Localizable = at.Localizable;

            if (at.Name != null && at.Name.Length > 0)
                name = at.Name;
            else
                name = member.Name;

            if (typeof(CustomExtensionAttribute).IsAssignableFrom(fdata.MemberType))
            {
                if (boundAttributeType != null)
                    throw new InvalidOperationException("Type '" + member.DeclaringType + "' has two members bound to a custom attribute. There can be only one.");

                boundAttributeType = fdata;

                return null;
            }

            return fdata;
        }

        #endregion
    }
}
