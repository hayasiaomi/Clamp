using Clamp.Data.Annotation;
using Clamp.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Nodes
{
    [ExtensionNode("Type", Description = "Specifies a class that will be used to create an extension object.")]
    [NodeAttribute("class", typeof(Type), false, ContentType = ContentType.Class, Description = "Name of the class. If a value is not provided, the class name will be taken from the 'id' attribute")]
    public class TypeExtensionNode : InstanceExtensionNode
    {
        string typeName;
        Type type;

        /// <summary>
        /// Reads the extension node data
        /// </summary>
        /// <param name='elem'>
        /// The element containing the extension data
        /// </param>
        /// <remarks>
        /// This method can be overridden to provide a custom method for reading extension node data from an element.
        /// The default implementation reads the attributes if the element and assigns the values to the fields
        /// and properties of the extension node that have the corresponding [NodeAttribute] decoration.
        /// </remarks>
        internal protected override void Read(NodeElement elem)
        {
            base.Read(elem);
            typeName = elem.GetAttribute("type");
            if (typeName.Length == 0)
                typeName = elem.GetAttribute("class");
            if (typeName.Length == 0)
                typeName = elem.GetAttribute("id");
        }

        /// <summary>
        /// Creates a new extension object
        /// </summary>
        /// <returns>
        /// The extension object
        /// </returns>
        public override object CreateInstance()
        {
            return Activator.CreateInstance(Type);
        }

        /// <summary>
        /// Type of the object that this node creates
        /// </summary>
        public Type Type
        {
            get
            {
                if (type == null)
                {
                    if (typeName.Length == 0)
                        throw new InvalidOperationException("Type name not specified.");
                    type = Bundle.GetType(typeName, true);
                }
                return type;
            }
        }

        /// <summary>
        /// Name of the type of the object that this node creates
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName
        {
            get
            {
                return typeName;
            }
        }
    }

    /// <summary>
    /// An extension node which specifies a type with custom extension metadata
    /// </summary>
    /// <remarks>
    /// This is the default type for type extension nodes bound to a custom extension attribute.
    /// </remarks>
    public class TypeExtensionNode<T> : TypeExtensionNode, IAttributedExtensionNode where T : CustomExtensionAttribute
    {
        T data;

        /// <summary>
        /// The custom attribute containing the extension metadata
        /// </summary>
        [NodeAttribute]
        public T Data
        {
            get { return data; }
            internal set { data = value; }
        }

        CustomExtensionAttribute IAttributedExtensionNode.Attribute
        {
            get { return data; }
        }
    }
}
