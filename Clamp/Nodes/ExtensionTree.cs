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
    internal class ExtensionTree : TreeNode
    {
        internal const string AutoIdPrefix = "__nid_";

        private int internalId;
        private TreeClampBundle treeNodeBundle;

        public ExtensionTree(ClampBundle clampBundle, TreeClampBundle context) : base(clampBundle, "")
        {
            this.treeNodeBundle = context;
        }

        public override TreeClampBundle Context
        {
            get { return treeNodeBundle; }
        }


        public void LoadExtension(string addin, Extension extension, ArrayList addedNodes)
        {
            TreeNode tnode = GetNode(extension.Path);
            if (tnode == null)
            {
                clampBundle.ReportError("Can't load extensions for path '" + extension.Path + "'. Extension point not defined.", addin, null, false);
                return;
            }

            int curPos = -1;
            LoadExtensionElement(tnode, addin, extension.ExtensionNodes, (ModuleDescription)extension.Parent, ref curPos, tnode.Condition, false, addedNodes);
        }

        void LoadExtensionElement(TreeNode tnode, string addin, ExtensionNodeDescriptionCollection extension, ModuleDescription module, ref int curPos, BaseCondition parentCondition, bool inComplextCondition, ArrayList addedNodes)
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
                    LoadExtensionElement(tnode, addin, elem.ChildNodes, module, ref curPos, parentCondition, true, addedNodes);
                    continue;
                }

                if (elem.NodeName == "Condition")
                {
                    Condition cond = new Condition(InternalClampBundle, elem, parentCondition);
                    LoadExtensionElement(tnode, addin, elem.ChildNodes, module, ref curPos, cond, false, addedNodes);
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
                ExtensionNodeType ntype = clampBundle.FindType(tnode.ExtensionNodeSet, elem.NodeName, addin);

                if (ntype == null)
                {
                    clampBundle.ReportError("Node '" + elem.NodeName + "' not allowed in extension: " + tnode.GetPath(), addin, null, false);
                    continue;
                }

                string id = elem.GetAttribute("id");
                if (id.Length == 0)
                    id = AutoIdPrefix + (++internalId);

                TreeNode cnode = new TreeNode(clampBundle, id);

                ExtensionNode enode = ReadNode(cnode, addin, ntype, elem, module);
                if (enode == null)
                    continue;

                cnode.Condition = parentCondition;
                cnode.ExtensionNodeSet = ntype;
                tnode.InsertChildNode(curPos, cnode);
                addedNodes.Add(cnode);

                if (cnode.Condition != null)
                    Context.RegisterNodeCondition(cnode, cnode.Condition);

                // Load children
                if (elem.ChildNodes.Count > 0)
                {
                    int cp = 0;
                    LoadExtensionElement(cnode, addin, elem.ChildNodes, module, ref cp, parentCondition, false, addedNodes);
                }

                curPos++;
            }
            if (Context.FireEvents)
                tnode.NotifyChildrenChanged();
        }

        BaseCondition ReadComplexCondition(ExtensionNodeDescription elem, BaseCondition parentCondition)
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
                return new Condition(InternalClampBundle, elem, parentCondition);
            }

            clampBundle.ReportError("Invalid complex condition element '" + elem.NodeName + "'.", null, null, false);

            return new NullCondition();
        }

        public ExtensionNode ReadNode(TreeNode tnode, string addin, ExtensionNodeType ntype, ExtensionNodeDescription elem, ModuleDescription module)
        {
            try
            {
                if (ntype.Type == null)
                {
                    if (!InitializeNodeType(ntype))
                        return null;
                }

                ExtensionNode node;
                node = Activator.CreateInstance(ntype.Type) as ExtensionNode;

                if (node == null)
                {
                    clampBundle.ReportError("Extension node type '" + ntype.Type + "' must be a subclass of ExtensionNode", addin, null, false);
                    return null;
                }

                tnode.AttachExtensionNode(node);
                node.SetData(clampBundle, addin, ntype, module);
                node.Read(elem);

                return node;
            }
            catch (Exception ex)
            {
                clampBundle.ReportError("Could not read extension node of type '" + ntype.Type + "' from extension path '" + tnode.GetPath() + "'", addin, ex, false);
                return null;
            }
        }

        #region private mehtod
        bool InitializeNodeType(ExtensionNodeType ntype)
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

        Dictionary<string, ExtensionNodeType.FieldData> GetMembersMap(Type type, out ExtensionNodeType.FieldData boundAttributeType)
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

        ExtensionNodeType.FieldData CreateFieldData(MemberInfo member, NodeAttributeAttribute at, out string name, ref ExtensionNodeType.FieldData boundAttributeType)
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
