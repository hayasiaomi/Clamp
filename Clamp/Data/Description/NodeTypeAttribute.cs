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
        private string name;
        private string type;
        private bool required;
        private bool localizable;
        private string description;

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
            this.name = att.name;
            this.type = att.type;
            this.required = att.required;
            this.localizable = att.localizable;
            this.description = att.description;
        }
        /// <summary>
        /// 属性名
        /// </summary>
        public string Name
        {
            get { return name != null ? name : string.Empty; }
            set { name = value; }
        }

        /// <summary>
        /// 是否为必需
        /// </summary>
        public bool Required
        {
            get { return required; }
            set { required = value; }
        }

        /// <summary>
        ///是否可以本地化
        /// </summary>
        public bool Localizable
        {
            get { return localizable; }
            set { localizable = value; }
        }

        /// <summary>
        /// 属性的类型
        /// </summary>
        public string Type
        {
            get { return type != null ? type : string.Empty; }
            set { type = value; }
        }

        /// <summary>
        /// 属性的说明
        /// </summary>
        public string Description
        {
            get { return description != null ? description : string.Empty; }
            set { description = value; }
        }

        /// <summary>
        /// 内容的类型
        /// </summary>
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
