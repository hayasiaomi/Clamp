using Clamp.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    /// <summary>
    /// 标识当前是一个扩展点
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class TypeExtensionPointAttribute : Attribute
    {
        private string path;
        private string nodeName;
        private Type nodeType;
        private string nodeTypeName;
        private string desc;
        private string name;
        private Type customAttributeType;
        private string customAttributeTypeName;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public TypeExtensionPointAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="path">
        /// Path that identifies the extension point
        /// </param>
        public TypeExtensionPointAttribute(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// 扩展点的路径
        /// </summary>
        public string Path
        {
            get { return path != null ? path : string.Empty; }
            set { path = value; }
        }

        /// <summary>
        /// 扩展的说明
        /// </summary>
        public string Description
        {
            get { return desc != null ? desc : string.Empty; }
            set { desc = value; }
        }

        /// <summary>
        /// Element name to be used when defining an extension in an XML manifest. The default name is "Type".
        /// </summary>
        public string NodeName
        {
            get { return nodeName != null && nodeName.Length > 0 ? nodeName : "Type"; }
            set { nodeName = value; }
        }

        /// <summary>
        /// Display name of the extension point.
        /// </summary>
        public string Name
        {
            get { return name != null ? name : string.Empty; }
            set { name = value; }
        }

        /// <summary>
        /// Type of the extension node to be created for extensions
        /// </summary>
        public Type NodeType
        {
            get { return nodeType != null ? nodeType : typeof(TypeExtensionNode); }
            set { nodeType = value; nodeTypeName = value.FullName; }
        }

        internal string NodeTypeName
        {
            get { return nodeTypeName != null ? nodeTypeName : typeof(TypeExtensionNode).FullName; }
            set { nodeTypeName = value; nodeType = null; }
        }

        /// <summary>
        /// Type of the custom attribute to be used to specify metadata for the extension point
        /// </summary>
        public Type ExtensionAttributeType
        {
            get { return this.customAttributeType; }
            set { this.customAttributeType = value; customAttributeTypeName = value.FullName; }
        }

        internal string ExtensionAttributeTypeName
        {
            get { return this.customAttributeTypeName; }
            set { this.customAttributeTypeName = value; }
        }
    }
}
