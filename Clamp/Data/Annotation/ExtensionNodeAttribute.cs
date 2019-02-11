using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Description
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtensionNodeAttribute : Attribute
    {
        private string nodeName;
        private string description;
        private string customAttributeTypeName;
        private Type customAttributeType;

        /// <summary>
        /// Initializes the attribute
        /// </summary>
        public ExtensionNodeAttribute()
        {
        }

        /// <summary>
        /// Initializes the attribute
        /// </summary>
        /// <param name="nodeName">
        /// Name of the node
        /// </param>
        public ExtensionNodeAttribute(string nodeName)
        {
            this.nodeName = nodeName;
        }

        /// <summary>
        /// Initializes the attribute
        /// </summary>
        /// <param name="nodeName">
        /// Name of the node
        /// </param>
        /// <param name="description">
        /// Description of the node
        /// </param>
        public ExtensionNodeAttribute(string nodeName, string description)
        {
            this.nodeName = nodeName;
            this.description = description;
        }

        /// <summary>
        /// Default name of the extension node
        /// </summary>
        public string NodeName
        {
            get { return nodeName != null ? nodeName : string.Empty; }
            set { nodeName = value; }
        }

        /// <summary>
        /// Default description of the extension node type
        /// </summary>
        public string Description
        {
            get { return description != null ? description : string.Empty; }
            set { description = value; }
        }

        /// <summary>
        /// Type of a custom attribute which can be used to specify metadata for this extension node type
        /// </summary>
        public Type ExtensionAttributeType
        {
            get { return customAttributeType; }
            set { customAttributeType = value; customAttributeTypeName = value.FullName; }
        }

        internal string ExtensionAttributeTypeName
        {
            get { return customAttributeTypeName ?? string.Empty; }
            set { customAttributeTypeName = value; }
        }
    }
}
