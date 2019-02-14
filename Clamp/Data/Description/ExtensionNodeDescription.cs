using Clamp.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.Data.Description
{
    /// <summary>
    /// 扩展节点说明
    /// </summary>
    public class ExtensionNodeDescription : ObjectDescription, NodeElement
    {
        private ExtensionNodeDescriptionCollection childNodes;
        private string[] attributes;
        private string nodeName;

        public ExtensionNodeDescription(string nodeName)
        {
            this.nodeName = nodeName;
        }

        internal ExtensionNodeDescription(XmlElement elem)
        {
            Element = elem;
            nodeName = elem.LocalName;
        }

        internal ExtensionNodeDescription()
        {
        }

        /// <summary>
        /// 获得当前节点的节点类型
        /// </summary>
        /// <returns></returns>
        public ExtensionNodeType GetNodeType()
        {
            if (Parent is Extension)
            {
                Extension ext = (Extension)Parent;

                object ob = ext.GetExtendedObject();

                if (ob is ExtensionPoint)
                {
                    ExtensionPoint ep = (ExtensionPoint)ob;
                    return ep.NodeSet.GetAllowedNodeTypes()[NodeName];
                }
                else if (ob is ExtensionNodeDescription)
                {
                    ExtensionNodeDescription pn = (ExtensionNodeDescription)ob;

                    ExtensionNodeType pt = ((ExtensionNodeDescription)pn).GetNodeType();

                    if (pt != null)
                        return pt.GetAllowedNodeTypes()[NodeName];
                }

            }
            else if (Parent is ExtensionNodeDescription)
            {
                ExtensionNodeType pt = ((ExtensionNodeDescription)Parent).GetNodeType();

                if (pt != null)
                    return pt.GetAllowedNodeTypes()[NodeName];
            }

            return null;
        }

        /// <summary>
        /// 获得父母的路径
        /// </summary>
        /// <returns></returns>
        public string GetParentPath()
        {
            if (Parent is Extension)
                return ((Extension)Parent).Path;
            else if (Parent is ExtensionNodeDescription)
            {
                ExtensionNodeDescription pn = (ExtensionNodeDescription)Parent;
                return pn.GetParentPath() + "/" + pn.Id;
            }
            else
                return string.Empty;
        }

        internal override void Verify(string location, StringCollection errors)
        {
            if (nodeName == null || nodeName.Length == 0)
                errors.Add(location + "Node: NodeName can't be empty.");
            ChildNodes.Verify(location + NodeName + "/", errors);
        }

        /// <summary>
        /// 节点名
        /// </summary>
        public string NodeName
        {
            get { return nodeName; }
            internal set
            {
                if (Element != null)
                    throw new InvalidOperationException("Can't change node name of xml element");
                nodeName = value;
            }
        }


        public string Id
        {
            get { return GetAttribute("id"); }
            set { SetAttribute("id", value); }
        }

        public string InsertAfter
        {
            get { return GetAttribute("insertafter"); }
            set
            {
                if (value == null || value.Length == 0)
                    RemoveAttribute("insertafter");
                else
                    SetAttribute("insertafter", value);
            }
        }


        public string InsertBefore
        {
            get { return GetAttribute("insertbefore"); }
            set
            {
                if (value == null || value.Length == 0)
                    RemoveAttribute("insertbefore");
                else
                    SetAttribute("insertbefore", value);
            }
        }

        /// <summary>
        /// 是否有条件
        /// </summary>
        public bool IsCondition
        {
            get { return nodeName == "Condition" || nodeName == "ComplexCondition"; }
        }

        internal override void SaveXml(XmlElement parent)
        {
            if (Element == null)
            {
                Element = parent.OwnerDocument.CreateElement(nodeName);
                parent.AppendChild(Element);
                if (attributes != null)
                {
                    for (int n = 0; n < attributes.Length; n += 2)
                        Element.SetAttribute(attributes[n], attributes[n + 1]);
                }
                ChildNodes.SaveXml(Element);
            }
        }

        /// <summary>
        /// 获得对应的属性值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetAttribute(string key)
        {
            if (Element != null)
                return Element.GetAttribute(key);

            if (attributes == null)
                return string.Empty;
            for (int n = 0; n < attributes.Length; n += 2)
            {
                if (attributes[n] == key)
                    return attributes[n + 1];
            }
            return string.Empty;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetAttribute(string key, string value)
        {
            if (Element != null)
            {
                Element.SetAttribute(key, value);
                return;
            }

            if (value == null)
                value = string.Empty;

            if (attributes == null)
            {
                attributes = new string[2];
                attributes[0] = key;
                attributes[1] = value;
                return;
            }

            for (int n = 0; n < attributes.Length; n += 2)
            {
                if (attributes[n] == key)
                {
                    attributes[n + 1] = value;
                    return;
                }
            }
            string[] newList = new string[attributes.Length + 2];

            attributes.CopyTo(newList, 0);
            attributes = newList;
            attributes[attributes.Length - 2] = key;
            attributes[attributes.Length - 1] = value;
        }

        /// <summary>
        /// 移除指定的属性
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAttribute(string name)
        {
            if (Element != null)
            {
                Element.RemoveAttribute(name);
                return;
            }

            if (attributes == null)
                return;

            for (int n = 0; n < attributes.Length; n += 2)
            {
                if (attributes[n] == name)
                {
                    string[] newar = new string[attributes.Length - 2];
                    Array.Copy(attributes, 0, newar, 0, n);
                    Array.Copy(attributes, n + 2, newar, n, attributes.Length - n - 2);
                    attributes = newar;
                    break;
                }
            }
        }

        /// <summary>
        /// 节点的属性集合
        /// </summary>
        public NodeAttribute[] Attributes
        {
            get
            {
                if (Element != null)
                    SaveXmlAttributes();

                if (attributes == null)
                    return new NodeAttribute[0];

                NodeAttribute[] ats = new NodeAttribute[attributes.Length / 2];

                for (int n = 0; n < ats.Length; n++)
                {
                    NodeAttribute at = new NodeAttribute();

                    at.name = attributes[n * 2];
                    at.value = attributes[n * 2 + 1];
                    ats[n] = at;

                }
                return ats;
            }
        }

        /// <summary>
        /// 获得子节点
        /// </summary>
        public ExtensionNodeDescriptionCollection ChildNodes
        {
            get
            {
                if (childNodes == null)
                {
                    childNodes = new ExtensionNodeDescriptionCollection(this);
                    if (Element != null)
                    {
                        foreach (XmlNode nod in Element.ChildNodes)
                        {
                            if (nod is XmlElement)
                                childNodes.Add(new ExtensionNodeDescription((XmlElement)nod));
                        }
                    }
                }
                return childNodes;
            }
        }

        NodeElementCollection NodeElement.ChildNodes
        {
            get { return ChildNodes; }
        }

        void SaveXmlAttributes()
        {
            attributes = new string[Element.Attributes.Count * 2];
            for (int n = 0; n < attributes.Length; n += 2)
            {
                XmlAttribute at = Element.Attributes[n / 2];
                attributes[n] = at.LocalName;
                attributes[n + 1] = at.Value;
            }
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            if (Element != null)
                SaveXmlAttributes();

            writer.WriteValue("nodeName", nodeName);
            writer.WriteValue("attributes", attributes);
            writer.WriteValue("ChildNodes", ChildNodes);
        }

        internal override void Read(BinaryXmlReader reader)
        {
            nodeName = reader.ReadStringValue("nodeName");
            attributes = (string[])reader.ReadValue("attributes");
            childNodes = (ExtensionNodeDescriptionCollection)reader.ReadValue("ChildNodes", new ExtensionNodeDescriptionCollection(this));
        }
    }
}
