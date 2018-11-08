using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    public class ExtensionAttributeAttribute : Attribute
    {
        private Type targetType;
        private string targetTypeName;
        private string name;
        private string val;
        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.ExtensionAttributeAttribute"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the attribute
        /// </param>
        /// <param name='value'>
        /// Value of the attribute
        /// </param>
        public ExtensionAttributeAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.ExtensionAttributeAttribute"/> class.
        /// </summary>
        /// <param name='type'>
        /// Type of the extension for which the attribute value is being set
        /// </param>
        /// <param name='name'>
        /// Name of the attribute
        /// </param>
        /// <param name='value'>
        /// Value of the attribute
        /// </param>
        public ExtensionAttributeAttribute(Type type, string name, string value)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.ExtensionAttributeAttribute"/> class.
        /// </summary>
        /// <param name='path'>
        /// Path of the extension for which the attribute value is being set
        /// </param>
        /// <param name='name'>
        /// Name of the attribute
        /// </param>
        /// <param name='value'>
        /// Value of the attribute
        /// </param>
        public ExtensionAttributeAttribute(string path, string name, string value)
        {
            Name = name;
            Value = value;
            Path = path;
        }

        /// <summary>
        /// Name of the attribute
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Value of the attribute
        /// </summary>
        public string Value
        {
            get { return this.val; }
            set { this.val = value; }
        }

        /// <summary>
        /// Path of the extension for which the attribute value is being set
        /// </summary>
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        /// <summary>
        /// Type of the extension for which the attribute value is being set
        /// </summary>
        public Type Type
        {
            get { return targetType; }
            set { targetType = value; targetTypeName = targetType.FullName; }
        }

        internal string TypeName
        {
            get { return targetTypeName ?? string.Empty; }
            set { targetTypeName = value; }
        }
    }
}
