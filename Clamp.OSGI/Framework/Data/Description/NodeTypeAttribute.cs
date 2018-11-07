using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public sealed class NodeTypeAttribute : ObjectDescription
    {
        string name;
        string type;
        bool required;
        bool localizable;
        string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.NodeTypeAttribute"/> class.
        /// </summary>
        public NodeTypeAttribute()
        {
        }

        /// <summary>
        /// Copies data from another node attribute.
        /// </summary>
        /// <param name='att'>
        /// The attribute from which to copy.
        /// </param>
        public void CopyFrom(NodeTypeAttribute att)
        {
            name = att.name;
            type = att.type;
            required = att.required;
            localizable = att.localizable;
            description = att.description;
        }

        /// <summary>
        /// Gets or sets the name of the attribute.
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
        /// Gets or sets a value indicating whether this <see cref="Mono.Addins.Description.NodeTypeAttribute"/> is required.
        /// </summary>
        /// <value>
        /// <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        public bool Required
        {
            get { return required; }
            set { required = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Mono.Addins.Description.NodeTypeAttribute"/> is localizable.
        /// </summary>
        /// <value>
        /// <c>true</c> if localizable; otherwise, <c>false</c>.
        /// </value>
        public bool Localizable
        {
            get { return localizable; }
            set { localizable = value; }
        }

        /// <summary>
        /// Gets or sets the type of the attribute.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type
        {
            get { return type != null ? type : string.Empty; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the description of the attribute.
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
        /// Gets or sets the type of the content.
        /// </summary>
        /// <remarks>
        /// Allows specifying the type of the content of a string attribute.
        /// The value of this property is only informative, and it doesn't
        /// have any effect on how add-ins are packaged or loaded.
        /// </remarks>
        public ContentType ContentType { get; set; }

      
    }
}
