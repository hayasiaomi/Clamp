using Clamp.OSGI.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Data.Description
{
    public sealed class ConditionTypeDescription : ObjectDescription
    {
        string id;
        string typeName;
        string addinId;
        string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.ConditionTypeDescription"/> class.
        /// </summary>
        public ConditionTypeDescription()
        {
        }

        internal ConditionTypeDescription(XmlElement elem) : base(elem)
        {
            id = elem.GetAttribute("id");
            typeName = elem.GetAttribute("type");
            description = ReadXmlDescription();
        }

        /// <summary>
        /// Copies data from another condition type definition
        /// </summary>
        /// <param name='cond'>
        /// Condition from which to copy
        /// </param>
        public void CopyFrom(ConditionTypeDescription cond)
        {
            id = cond.id;
            typeName = cond.typeName;
            addinId = cond.BundleId;
            description = cond.description;
        }

        internal override void Verify(string location, StringCollection errors)
        {
            VerifyNotEmpty(location + "ConditionType", errors, Id, "id");
            VerifyNotEmpty(location + "ConditionType (" + Id + ")", errors, TypeName, "type");
        }

        /// <summary>
        /// Gets or sets the identifier of the condition type
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id
        {
            get { return id != null ? id : string.Empty; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the type that implements the condition
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName
        {
            get { return typeName != null ? typeName : string.Empty; }
            set { typeName = value; }
        }

        /// <summary>
        /// Gets or sets the description of the condition.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get { return description != null ? description : string.Empty; }
            set { description = value; }
        }

        internal string BundleId
        {
            get { return addinId; }
            set { addinId = value; }
        }

        internal override void SaveXml(XmlElement parent)
        {
            CreateElement(parent, "ConditionType");
            Element.SetAttribute("id", id);
            Element.SetAttribute("type", typeName);
            SaveXmlDescription(description);
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("Id", Id);
            writer.WriteValue("TypeName", TypeName);
            writer.WriteValue("Description", Description);
            writer.WriteValue("BundleId", BundleId);
        }

        internal override void Read(BinaryXmlReader reader)
        {
            Id = reader.ReadStringValue("Id");
            TypeName = reader.ReadStringValue("TypeName");
            if (!reader.IgnoreDescriptionData)
                Description = reader.ReadStringValue("Description");
            BundleId = reader.ReadStringValue("BundleId");
        }
    }
}
