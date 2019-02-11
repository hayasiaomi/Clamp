using Clamp.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExtensionNodeChildAttribute : Attribute
    {
        string nodeName;
        Type extensionNodeType;
        string extensionNodeTypeName;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="nodeName">
        /// Name of the allowed child extension node.
        /// </param>
        public ExtensionNodeChildAttribute(string nodeName)
            : this(typeof(TypeExtensionNode), nodeName)
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="extensionNodeType">
        /// Type of the allowed child extension node.
        /// </param>
        public ExtensionNodeChildAttribute(Type extensionNodeType)
            : this(extensionNodeType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="extensionNodeType">
        /// Type of the allowed child extension node.
        /// </param>
        /// <param name="nodeName">
        /// Name of the allowed child extension node.
        /// </param>
        public ExtensionNodeChildAttribute(Type extensionNodeType, string nodeName)
        {
            ExtensionNodeType = extensionNodeType;
            this.nodeName = nodeName;
        }

        /// <summary>
        /// Name of the allowed child extension node.
        /// </summary>
        public string NodeName
        {
            get { return nodeName != null ? nodeName : string.Empty; }
            set { nodeName = value; }
        }

        /// <summary>
        /// Type of the allowed child extension node.
        /// </summary>
        public Type ExtensionNodeType
        {
            get { return extensionNodeType; }
            set { extensionNodeType = value; extensionNodeTypeName = value.FullName; }
        }

        internal string ExtensionNodeTypeName
        {
            get { return extensionNodeTypeName; }
            set { extensionNodeTypeName = value; extensionNodeType = null; }
        }
    }
}
