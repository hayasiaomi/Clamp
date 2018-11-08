using Clamp.OSGI.Framework.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ObjectDescription : IBinaryXmlElement
    {
        internal XmlElement Element;
        object parent;

        internal ObjectDescription(XmlElement elem)
        {
            Element = elem;
        }

        internal ObjectDescription()
        {
        }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        /// <value>
        /// The parent object.
        /// </value>
        public object Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the parent add-in description.
        /// </summary>
        /// <value>
        /// The parent add-in description.
        /// </value>
        public BundleDescription ParentBundleDescription
        {
            get
            {
                if (parent is BundleDescription)
                    return (BundleDescription)parent;
                else if (parent is ObjectDescription)
                    return ((ObjectDescription)parent).ParentBundleDescription;
                else
                    return null;
            }
        }

        internal string ParseString(string s)
        {
            var desc = ParentBundleDescription;
            if (desc != null)
                return desc.ParseString(s);
            else
                return s;
        }

        internal void SetParent(object ob)
        {
            parent = ob;
        }

        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            Write(writer);
        }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            Read(reader);
        }

        internal virtual void Write(BinaryXmlWriter writer)
        {
        }

        internal virtual void Read(BinaryXmlReader reader)
        {
        }

        internal virtual void SaveXml(XmlElement parent)
        {
        }

        internal void CreateElement(XmlElement parent, string nodeName)
        {
            if (Element == null)
            {
                Element = parent.OwnerDocument.CreateElement(nodeName);
                parent.AppendChild(Element);
            }
        }

        internal string ReadXmlDescription()
        {
            XmlElement de = Element["Description"];
            if (de != null)
                return de.InnerText;
            else
                return null;
        }

        internal void SaveXmlDescription(string desc)
        {
            XmlElement de = Element["Description"];
            if (desc != null && desc.Length > 0)
            {
                if (de == null)
                {
                    de = Element.OwnerDocument.CreateElement("Description");
                    Element.AppendChild(de);
                }
                de.InnerText = desc;
            }
            else
            {
                if (de != null)
                    Element.RemoveChild(de);
            }
        }

        internal virtual void Verify(string location, StringCollection errors)
        {
        }

        internal void VerifyNotEmpty(string location, StringCollection errors, string attr, string val)
        {
            if (val == null || val.Length == 0)
                errors.Add(location + ": attribute '" + attr + "' can't be empty.");
        }
    }
}
