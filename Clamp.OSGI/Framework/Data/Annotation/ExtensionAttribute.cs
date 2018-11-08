using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExtensionAttribute : Attribute
    {
        private string path;
        private string nodeName;
        private string id;
        private string insertBefore;
        private string insertAfter;
        private string typeName;
        private Type type;

        /// <summary>
        /// Initializes a new instance of the ExtensionAttribute class.
        /// </summary>
        public ExtensionAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="path">
        /// Path of the extension point.
        /// </param>
        /// <remarks>The path is only required if there are several extension points defined for the same type.</remarks>
        public ExtensionAttribute(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="type">
        /// Type defining the extension point being extended
        /// </param>
        /// <remarks>
        /// This constructor can be used to explicitly specify the type that defines the extension point
        /// to be extended. By default, Mono.Bundles will try to find any extension point defined in any
        /// of the base classes or interfaces. The type parameter can be used when there is more than one
        /// base type providing an extension point.
        /// </remarks>
        public ExtensionAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Path of the extension point being extended
        /// </summary>
        /// <remarks>
        /// The path is only required if there are several extension points defined for the same type.
        /// </remarks>
        public string Path
        {
            get { return path ?? string.Empty; }
            set { path = value; }
        }

        /// <summary>
        /// Name of the extension node
        /// </summary>
        /// <remarks>
        /// Extension points may require extensions to use a specific node name.
        /// This is needed when an extension point may contain several different types of nodes.
        /// </remarks>
        public string NodeName
        {
            get { return !string.IsNullOrEmpty(nodeName) ? nodeName : "Type"; }
            set { nodeName = value; }
        }

        /// <summary>
        /// Identifier of the extension node.
        /// </summary>
        /// <remarks>
        /// The ExtensionAttribute.InsertAfter and ExtensionAttribute.InsertBefore
        /// properties can be used to specify the relative location of a node. The nodes
        /// referenced in those properties must be defined either in the add-in host
        /// being extended, or in any add-in on which this add-in depends.
        /// </remarks>
        public string Id
        {
            get { return id ?? string.Empty; }
            set { id = value; }
        }

        /// <summary>
        /// Identifier of the extension node before which this node has to be added in the extension point.
        /// </summary>
        /// <remarks>
        /// The ExtensionAttribute.InsertAfter and ExtensionAttribute.InsertBefore
        /// properties can be used to specify the relative location of a node. The nodes
        /// referenced in those properties must be defined either in the add-in host
        /// being extended, or in any add-in on which this add-in depends.
        /// </remarks>
        public string InsertBefore
        {
            get { return insertBefore ?? string.Empty; }
            set { insertBefore = value; }
        }

        /// <summary>
        /// Identifier of the extension node after which this node has to be added in the extension point.
        /// </summary>
        public string InsertAfter
        {
            get { return insertAfter ?? string.Empty; }
            set { insertAfter = value; }
        }

        /// <summary>
        /// Type defining the extension point being extended
        /// </summary>
        /// <remarks>
        /// This property can be used to explicitly specify the type that defines the extension point
        /// to be extended. By default, Mono.Bundles will try to find any extension point defined in any
        /// of the base classes or interfaces. This property can be used when there is more than one
        /// base type providing an extension point.
        /// </remarks>
        public Type Type
        {
            get { return type; }
            set { type = value; typeName = type.FullName; }
        }

        internal string TypeName
        {
            get { return typeName ?? string.Empty; }
            set { typeName = value; }
        }
    }
}
