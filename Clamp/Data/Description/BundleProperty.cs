using Clamp.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Clamp.Data.Description
{
    /// <summary>
    /// 用于定义Bundle的属性
    /// </summary>
    public class BundleProperty : IBinaryXmlElement
    {
        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }
        /// <summary>
        /// 本地化
        /// </summary>
        [XmlAttribute("locale")]
        public string Locale { get; set; }
        /// <summary>
        /// 值
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
