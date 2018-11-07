using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ExtensionPoint : ObjectDescription
    {
        private ExtensionNodeSet nodeSet;
        private ConditionTypeDescriptionCollection conditions;
        private string path;
        private string name;
        private string description;

        private string defaultInsertBefore;
        private string defaultInsertAfter;

        // Information gathered from others addins:

        private List<string> addins;  // Add-ins which extend this extension point
        private string rootAddin;     // Add-in which defines this extension point

        public ExtensionNodeSet NodeSet
        {
            get
            {
                if (nodeSet == null)
                {
                    nodeSet = new ExtensionNodeSet();
                    nodeSet.SetParent(this);
                }
                return nodeSet;
            }
        }

        internal void SetExtensionsAddinId(string addinId)
        {
            //NodeSet.SetExtensionsAddinId(addinId);
            //foreach (ConditionTypeDescription cond in Conditions)
            //    cond.AddinId = addinId;
            //Addins.Add(addinId);
        }

        public string Path
        {
            get { return path != null ? path : string.Empty; }
            set { path = value; }
        }

        /// <summary>
        /// Gets or sets the display name of the extension point.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return name != null ? name : string.Empty; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the description of the extension point.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get { return description != null ? description : string.Empty; }
            set { description = value; }
        }

        /// <summary>
        /// Gets a list of add-ins that extend this extension point.
        /// </summary>
        /// <remarks>
        /// This value is only available when the add-in description is loaded from an add-in registry.
        /// </remarks>
        public string[] ExtenderAddins
        {
            get
            {
                return Addins.ToArray();
            }
        }

        internal List<string> Addins
        {
            get
            {
                if (addins == null)
                    addins = new List<string>();
                return addins;
            }
        }

        internal string RootAddin
        {
            get { return rootAddin; }
            set { rootAddin = value; }
        }

        internal void SetNodeSet(ExtensionNodeSet nset)
        {
            // Used only by the addin updater
            nodeSet = nset;
            nodeSet.SetParent(this);
        }

        /// <summary>
        /// Gets the conditions available in this node set.
        /// </summary>
        /// <value>
        /// The conditions.
        /// </value>
        public ConditionTypeDescriptionCollection Conditions
        {
            get
            {
                if (conditions == null)
                {
                    conditions = new ConditionTypeDescriptionCollection(this);
                }
                return conditions;
            }
        }

        /// <summary>
        /// Adds an extension node type.
        /// </summary>
        /// <returns>
        /// The extension node type.
        /// </returns>
        /// <param name='name'>
        /// Name of the node
        /// </param>
        /// <param name='typeName'>
        /// Name of the type that implements the extension node.
        /// </param>
        /// <remarks>
        /// This method can be used to register a new allowed node type for the extension point.
        /// </remarks>
        public ExtensionNodeType AddExtensionNode(string name, string typeName)
        {
            ExtensionNodeType ntype = new ExtensionNodeType();
            ntype.Id = name;
            ntype.TypeName = typeName;
            NodeSet.NodeTypes.Add(ntype);
            return ntype;
        }

        /// <summary>
        /// The id of the extension before which new extensions will be added, unless the extension defines its own InsertBefore value
        /// </summary>
        /// <value>The default insert before.</value>
        public string DefaultInsertBefore
        {
            get { return defaultInsertBefore ?? ""; }
            set { defaultInsertBefore = value; }
        }

        /// <summary>
        /// The id of the extension after which new extensions will be added, unless the extension defines its own InsertAfter value
        /// </summary>
        /// <value>The default insert before.</value>
        public string DefaultInsertAfter
        {
            get { return defaultInsertAfter ?? ""; }
            set { defaultInsertAfter = value; }
        }
    }
}
