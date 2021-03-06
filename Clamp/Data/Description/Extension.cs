﻿using Clamp.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.Data.Description
{
    /// <summary>
    /// 扩展
    /// </summary>
    public class Extension : ObjectDescription, IComparable
    {
        private string path;

        private ExtensionNodeDescriptionCollection nodes;

        /// <summary>
        /// 路径
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// Gets the extension nodes.
        /// </summary>
        /// <value>
        /// The extension nodes.
        /// </value>
        public ExtensionNodeDescriptionCollection ExtensionNodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ExtensionNodeDescriptionCollection(this);

                    if (Element != null)
                    {
                        foreach (XmlNode node in Element.ChildNodes)
                        {
                            XmlElement e = node as XmlElement;

                            if (e != null)
                                nodes.Add(new ExtensionNodeDescription(e));
                        }
                    }
                }
                return nodes;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.Extension"/> class.
        /// </summary>
        public Extension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.Extension"/> class.
        /// </summary>
        /// <param name='path'>
        /// Path that identifies the extension point being extended
        /// </param>
        public Extension(string path)
        {
            this.path = path;
        }

        public Extension(XmlElement element)
        {
            Element = element;
            path = element.GetAttribute("path");
        }

        /// <summary>
        /// Gets the object extended by this extension
        /// </summary>
        /// <returns>
        /// The extended object can be an <see cref="Mono.Bundles.Description.ExtensionPoint"/> or
        /// an <see cref="Mono.Bundles.Description.ExtensionNodeDescription"/>.
        /// </returns>
        /// <remarks>
        /// This method only works when the add-in description to which the extension belongs has been
        /// loaded from an add-in registry.
        /// </remarks>
        public ObjectDescription GetExtendedObject()
        {
            BundleDescription desc = ParentBundleDescription;

            if (desc == null)
                return null;

            ExtensionPoint ep = FindExtensionPoint(desc, path);

            if (ep == null && desc.OwnerDatabase != null)
            {
                foreach (Dependency dep in desc.MainModule.Dependencies)
                {
                    BundleDependency adep = dep as BundleDependency;
                    if (adep == null) continue;
                    Bundle ad = desc.OwnerDatabase.GetInstalledBundle(ParentBundleDescription.Domain, adep.FullBundleId);
                    if (ad != null && ad.Description != null)
                    {
                        ep = FindExtensionPoint(ad.Description, path);
                        if (ep != null)
                            break;
                    }
                }
            }

            if (ep != null)
            {
                string subp = path.Substring(ep.Path.Length).Trim('/');
                if (subp.Length == 0)
                    return ep; // The extension is directly extending the extension point

                // The extension is extending a node of the extension point

                return desc.FindExtensionNode(path, true);
            }

            return null;
        }

        /// <summary>
        /// Gets the node types allowed in this extension.
        /// </summary>
        /// <returns>
        /// The allowed node types.
        /// </returns>
        /// <remarks>
        /// This method only works when the add-in description to which the extension belongs has been
        /// loaded from an add-in registry.
        /// </remarks>
        public ExtensionNodeTypeCollection GetAllowedNodeTypes()
        {
            ObjectDescription ob = GetExtendedObject();
            ExtensionPoint ep = ob as ExtensionPoint;
            if (ep != null)
                return ep.NodeSet.GetAllowedNodeTypes();

            ExtensionNodeDescription node = ob as ExtensionNodeDescription;
            if (node != null)
            {
                ExtensionNodeType nt = node.GetNodeType();
                if (nt != null)
                    return nt.GetAllowedNodeTypes();
            }
            return new ExtensionNodeTypeCollection();
        }

        ExtensionPoint FindExtensionPoint(BundleDescription desc, string path)
        {
            foreach (ExtensionPoint ep in desc.ExtensionPoints)
            {
                if (ep.Path == path || path.StartsWith(ep.Path + "/"))
                    return ep;
            }
            return null;
        }

        internal override void Verify(string location, StringCollection errors)
        {
            VerifyNotEmpty(location + "Extension", errors, path, "path");
            ExtensionNodes.Verify(location + "Extension (" + path + ")/", errors);

            foreach (ExtensionNodeDescription cnode in ExtensionNodes)
                VerifyNode(location, cnode, errors);
        }

        void VerifyNode(string location, ExtensionNodeDescription node, StringCollection errors)
        {
            string id = node.GetAttribute("id");
            if (id.Length > 0)
                id = "(" + id + ")";
            if (node.NodeName == "Condition" && node.GetAttribute("id").Length == 0)
            {
                errors.Add(location + node.NodeName + id + ": Missing 'id' attribute in Condition element.");
            }
            if (node.NodeName == "ComplexCondition")
            {
                if (node.ChildNodes.Count > 0)
                {
                    VerifyConditionNode(location, node.ChildNodes[0], errors);
                    for (int n = 1; n < node.ChildNodes.Count; n++)
                        VerifyNode(location + node.NodeName + id + "/", node.ChildNodes[n], errors);
                }
                else
                    errors.Add(location + "ComplexCondition: Missing child condition in ComplexCondition element.");
            }
            foreach (ExtensionNodeDescription cnode in node.ChildNodes)
                VerifyNode(location + node.NodeName + id + "/", cnode, errors);
        }

        void VerifyConditionNode(string location, ExtensionNodeDescription node, StringCollection errors)
        {
            string nodeName = node.NodeName;
            if (nodeName != "Or" && nodeName != "And" && nodeName != "Not" && nodeName != "Condition")
            {
                errors.Add(location + "ComplexCondition: Invalid condition element: " + nodeName);
                return;
            }
            foreach (ExtensionNodeDescription cnode in node.ChildNodes)
                VerifyConditionNode(location, cnode, errors);
        }




        internal override void SaveXml(XmlElement parent)
        {
            if (Element == null)
            {
                Element = parent.OwnerDocument.CreateElement("Extension");
                parent.AppendChild(Element);
            }

            Element.SetAttribute("path", path);

            if (nodes != null)
                nodes.SaveXml(Element);

        }


        int IComparable.CompareTo(object obj)
        {
            Extension other = (Extension)obj;
            return Path.CompareTo(other.Path);
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("path", path);
            writer.WriteValue("Nodes", ExtensionNodes);
        }

        internal override void Read(BinaryXmlReader reader)
        {
            path = reader.ReadStringValue("path");
            nodes = (ExtensionNodeDescriptionCollection)reader.ReadValue("Nodes", new ExtensionNodeDescriptionCollection(this));
        }
    }
}
