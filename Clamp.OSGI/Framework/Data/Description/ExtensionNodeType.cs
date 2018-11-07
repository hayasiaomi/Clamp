using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public sealed class ExtensionNodeType : ExtensionNodeSet
    {
        string typeName;
        string objectTypeName;
        string description;
        string addinId;
        NodeTypeAttributeCollection attributes;
        string customAttributeTypeName;

        // Cached clr type
        [NonSerialized]
        internal Type Type;

        // Cached serializable fields
        [NonSerialized]
        internal Dictionary<string, FieldData> Fields;

        // Cached serializable fields for the custom attribute
        [NonSerialized]
        internal Dictionary<string, FieldData> CustomAttributeFields;

        [NonSerialized]
        internal FieldData CustomAttributeMember;

        internal class FieldData
        {
            public MemberInfo Member;
            public bool Required;
            public bool Localizable;

            public void SetValue(object target, object val)
            {
                if (Member is FieldInfo)
                    ((FieldInfo)Member).SetValue(target, val);
                else
                    ((PropertyInfo)Member).SetValue(target, val, null);
            }

            public Type MemberType
            {
                get
                {
                    return (Member is FieldInfo) ? ((FieldInfo)Member).FieldType : ((PropertyInfo)Member).PropertyType;
                }
            }
        }

        // Addin where this extension type is implemented
        internal string AddinId
        {
            get { return addinId; }
            set { addinId = value; }
        }

        /// <summary>
        /// Type that implements the extension node.
        /// </summary>
        /// <value>
        /// The full name of the type.
        /// </value>
        public string TypeName
        {
            get { return typeName != null ? typeName : string.Empty; }
            set { typeName = value; }
        }

        /// <summary>
        /// Element name to be used when defining an extension in an XML manifest. The default name is "Type".
        /// </summary>
        /// <value>
        /// The name of the node.
        /// </value>
        public string NodeName
        {
            get { return Id; }
            set { Id = value; }
        }

        /// <summary>
        /// Type of the object that the extension creates (only valid for TypeNodeExtension).
        /// </summary>
        public string ObjectTypeName
        {
            get { return objectTypeName != null ? objectTypeName : string.Empty; }
            set { objectTypeName = value; }
        }

        /// <summary>
        /// Name of the custom attribute that can be used to declare nodes of this type
        /// </summary>
        public string ExtensionAttributeTypeName
        {
            get { return customAttributeTypeName ?? string.Empty; }
            set { customAttributeTypeName = value; }
        }

        /// <summary>
        /// Long description of the node type
        /// </summary>
        public string Description
        {
            get { return description != null ? description : string.Empty; }
            set { description = value; }
        }

        /// <summary>
        /// Attributes supported by the extension node type.
        /// </summary>
        public NodeTypeAttributeCollection Attributes
        {
            get
            {
                if (attributes == null)
                {
                    attributes = new NodeTypeAttributeCollection(this);
                }
                return attributes;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.ExtensionNodeType"/> class.
        /// </summary>
        public ExtensionNodeType()
        {
        }

        /// <summary>
        ///  Copies data from another node set 
        /// </summary>
        public void CopyFrom(ExtensionNodeType ntype)
        {
            base.CopyFrom(ntype);
            this.typeName = ntype.TypeName;
            this.objectTypeName = ntype.ObjectTypeName;
            this.description = ntype.Description;
            this.addinId = ntype.AddinId;
            Attributes.Clear();
            foreach (NodeTypeAttribute att in ntype.Attributes)
            {
                NodeTypeAttribute catt = new NodeTypeAttribute();
                catt.CopyFrom(att);
                Attributes.Add(catt);
            }
        }

        internal override string IdAttribute
        {
            get { return "name"; }
        }

    }
}
