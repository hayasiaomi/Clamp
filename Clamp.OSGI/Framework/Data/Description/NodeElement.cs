using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public interface NodeElement
    {
        /// <summary>
        /// Name of the node element.
        /// </summary>
        string NodeName { get; }

        /// <summary>
        /// Gets element attributes.
        /// </summary>
        /// <param name="key">
        /// Name of the attribute
        /// </param>
        /// <returns>
        /// The value of the attribute
        /// </returns>
        string GetAttribute(string key);

        /// <summary>
        /// Gets all attributes defined in the element.
        /// </summary>
        NodeAttribute[] Attributes { get; }

        /// <summary>
        /// Gets child nodes of this node
        /// </summary>
        NodeElementCollection ChildNodes { get; }
    }

    /// <summary>
    /// Attribute of a NodeElement.
    /// </summary>
    public class NodeAttribute
    {
        internal string name;
        internal string value;

        internal NodeAttribute()
        {
        }

        /// <summary>
        /// Name of the attribute.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Value of the attribute.
        /// </summary>
        public string Value
        {
            get { return value; }
        }
    }

    /// <summary>
    /// A collection of NodeElement objects
    /// </summary>
    public interface NodeElementCollection : IList, ICollection, IEnumerable
    {
        /// <summary>
        /// Gets the <see cref="Mono.Bundles.NodeElement"/> at the specified index
        /// </summary>
        /// <param name='n'>
        /// Index
        /// </param>
        new NodeElement this[int n] { get; }
    }
}
