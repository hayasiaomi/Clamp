using Clamp.OSGI.Framework.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    /// 标识一个扩展点
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ExtensionPointAttribute : Attribute
    {
        private string path;
        private Type nodeType;
        private string nodeName;
        private string desc;
        private string name;
        private Type objectType;
        private string nodeTypeName;
        private string objectTypeName;
        private Type customAttributeType;
        private string customAttributeTypeName;
        private string defaultInsertBefore;
        private string defaultInsertAfter;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public ExtensionPointAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="path">
        /// Extension path that identifies the extension point
        /// </param>
        public ExtensionPointAttribute(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="path">
        /// Extension path that identifies the extension point
        /// </param>
        /// <param name="nodeType">
        /// Type of the extension node to be created for extensions
        /// </param>
        public ExtensionPointAttribute(string path, Type nodeType)
        {
            this.path = path;
            this.nodeType = nodeType;
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="path">
        /// Extension path that identifies the extension point
        /// </param>
        /// <param name="nodeName">
        /// Element name to be used when defining an extension in an XML manifest.
        /// </param>
        /// <param name="nodeType">
        /// Type of the extension node to be created for extensions
        /// </param>
        public ExtensionPointAttribute(string path, string nodeName, Type nodeType)
        {
            this.path = path;
            this.nodeType = nodeType;
            this.nodeName = nodeName;
        }

        /// <summary>
        /// Extension path that identifies the extension point
        /// </summary>
        public string Path
        {
            get { return path != null ? path : string.Empty; }
            set { path = value; }
        }

        /// <summary>
        /// Long description of the extension point.
        /// </summary>
        public string Description
        {
            get { return desc != null ? desc : string.Empty; }
            set { desc = value; }
        }

        /// <summary>
        /// Type of the extension node to be created for extensions
        /// </summary>
        public Type NodeType
        {
            get { return nodeType != null ? nodeType : typeof(TypeExtensionNode); }
            set { nodeType = value; nodeTypeName = value.FullName; }
        }

        /// <summary>
        /// Expected extension object type (when nodes are of type TypeExtensionNode)
        /// </summary>
        public Type ObjectType
        {
            get { return objectType; }
            set { objectType = value; objectTypeName = value.FullName; }
        }

        internal string NodeTypeName
        {
            get { return nodeTypeName != null ? nodeTypeName : typeof(TypeExtensionNode).FullName; }
            set { nodeTypeName = value; }
        }

        internal string ObjectTypeName
        {
            get { return objectTypeName; }
            set { objectTypeName = value; }
        }

        /// <summary>
        /// Element name to be used when defining an extension in an XML manifest. The default name is "Type".
        /// </summary>
        public string NodeName
        {
            get { return nodeName != null && nodeName.Length > 0 ? nodeName : string.Empty; }
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
