using Clamp.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.Data.Description
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

        internal override void Verify(string location, StringCollection errors)
        {
            VerifyNotEmpty(location + "Attribute", errors, Name, "name");
        }

        internal NodeTypeAttribute(XmlElement elem) : base(elem)
        {
            name = elem.GetAttribute("name");
            type = elem.GetAttribute("type");
            required = elem.GetAttribute("required").ToLower() == "true";
            localizable = elem.GetAttribute("localizable").ToLower() == "true";
            string ct = elem.GetAttribute("contentType");
            if (!string.IsNullOrEmpty(ct))
                ContentType = (ContentType)Enum.Parse(typeof(ContentType), ct);
            description = ReadXmlDescription();
        }

        internal override void SaveXml(XmlElement parent)
        {
            CreateElement(parent, "Attribute");
            Element.SetAttribute("name", name);

            if (Type.Length > 0)
                Element.SetAttribute("type", Type);
            else
                Element.RemoveAttribute("type");

            if (required)
                Element.SetAttribute("required", "True");
            else
                Element.RemoveAttribute("required");

            if (localizable)
                Element.SetAttribute("localizable", "True");
            else
                Element.RemoveAttribute("localizable");

            if (ContentType != ContentType.Text)
                Element.SetAttribute("contentType", ContentType.ToString());
            else
                Element.RemoveAttribute("contentType");

            SaveXmlDescription(description);
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("name", name);
            writer.WriteValue("type", type);
            writer.WriteValue("required", required);
            writer.WriteValue("description", description);
            writer.WriteValue("localizable", localizable);
            writer.WriteValue("contentType", ContentType.ToString());
        }

        internal override void Read(BinaryXmlReader reader)
        {
            name = reader.ReadStringValue("name");
            type = reader.ReadStringValue("type");
            required = reader.ReadBooleanValue("required");
            if (!reader.IgnoreDescriptionData)
                description = reader.ReadStringValue("description");
            localizable = reader.ReadBooleanValue("localizable");
            string ct = reader.ReadStringValue("contentType");
            try
            {
                ContentType = (ContentType)Enum.Parse(typeof(ContentType), ct);
            }
            catch
            {
                ContentType = ContentType.Text;
            }
        }
    }
}
