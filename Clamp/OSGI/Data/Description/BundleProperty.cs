using Clamp.OSGI.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Clamp.OSGI.Data.Description
{
    public class BundleProperty : IBinaryXmlElement
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Locale of the property. It is null if the property is not localized.
        /// </summary>
        [XmlAttribute("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Value of the property.
        /// </summary>
        [XmlText]
        public string Value { get; set; }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            Name = reader.ReadStringValue("name");
            Locale = reader.ReadStringValue("locale");
            Value = reader.ReadStringValue("value");
        }

        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("name", Name);
            writer.WriteValue("locale", Locale);
            writer.WriteValue("value", Value);
        }
    }
}
