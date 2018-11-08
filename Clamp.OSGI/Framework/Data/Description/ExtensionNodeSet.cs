using Clamp.OSGI.Framework.Data.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ExtensionNodeSet : ObjectDescription
    {
        string id;
        ExtensionNodeTypeCollection nodeTypes;
        NodeSetIdCollection nodeSets;
        bool missingNodeSetId;
        ExtensionNodeTypeCollection cachedAllowedTypes;

        internal string SourceBundleId { get; set; }

        internal ExtensionNodeSet(XmlElement element)
        {
            Element = element;
            id = element.GetAttribute(IdAttribute);
        }

        /// <summary>
        /// Copies data from another node set
        /// </summary>
        /// <param name='nset'>
        /// Node set from which to copy
        /// </param>
        public void CopyFrom(ExtensionNodeSet nset)
        {
            id = nset.id;
            NodeTypes.Clear();
            foreach (ExtensionNodeType nt in nset.NodeTypes)
            {
                ExtensionNodeType cnt = new ExtensionNodeType();
                cnt.CopyFrom(nt);
                NodeTypes.Add(cnt);
            }
            NodeSets.Clear();
            foreach (string ns in nset.NodeSets)
                NodeSets.Add(ns);
            missingNodeSetId = nset.missingNodeSetId;
        }

        internal override void Verify(string location, StringCollection errors)
        {
            if (missingNodeSetId)
                errors.Add(location + "Missing id attribute in extension set reference");

            NodeTypes.Verify(location + "ExtensionNodeSet (" + Id + ")/", errors);
        }

        internal override void SaveXml(XmlElement parent)
        {
            SaveXml(parent, "ExtensionNodeSet");
        }

        internal virtual void SaveXml(XmlElement parent, string nodeName)
        {
            if (Element == null)
            {
                Element = parent.OwnerDocument.CreateElement(nodeName);
                parent.AppendChild(Element);
            }
            if (Id.Length > 0)
                Element.SetAttribute(IdAttribute, Id);
            if (nodeTypes != null)
                nodeTypes.SaveXml(Element);
            if (nodeSets != null)
            {
                foreach (string s in nodeSets)
                {
                    if (Element.SelectSingleNode("ExtensionNodeSet[@id='" + s + "']") == null)
                    {
                        XmlElement e = Element.OwnerDocument.CreateElement("ExtensionNodeSet");
                        e.SetAttribute("id", s);
                        Element.AppendChild(e);
                    }
                }
                ArrayList list = new ArrayList();
                foreach (XmlElement e in Element.SelectNodes("ExtensionNodeSet"))
                {
                    if (!nodeSets.Contains(e.GetAttribute("id")))
                        list.Add(e);
                }
                foreach (XmlElement e in list)
                    Element.RemoveChild(e);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.ExtensionNodeSet"/> class.
        /// </summary>
        public ExtensionNodeSet()
        {
        }

        /// <summary>
        /// Gets or sets the identifier of the node set.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id
        {
            get { return id != null ? id : string.Empty; }
            set { id = value; }
        }

        internal virtual string IdAttribute
        {
            get { return "id"; }
        }

        /// <summary>
        /// Gets the node types allowed in this node set.
        /// </summary>
        /// <value>
        /// The node types.
        /// </value>
        public ExtensionNodeTypeCollection NodeTypes
        {
            get
            {
                if (nodeTypes == null)
                {
                    if (Element != null)
                        InitCollections();
                    else
                        nodeTypes = new ExtensionNodeTypeCollection(this);
                }
                return nodeTypes;
            }
        }

        /// <summary>
        /// Gets a list of other node sets included in this node set.
        /// </summary>
        /// <value>
        /// The node sets.
        /// </value>
        public NodeSetIdCollection NodeSets
        {
            get
            {
                if (nodeSets == null)
                {
                    if (Element != null)
                        InitCollections();
                    else
                        nodeSets = new NodeSetIdCollection();
                }
                return nodeSets;
            }
        }

        /// <summary>
        /// Gets all the allowed node types.
        /// </summary>
        /// <returns>
        /// The allowed node types.
        /// </returns>
        /// <remarks>
        /// Gets all allowed node types, including those defined in included node sets.
        /// This method only works for descriptions loaded from a registry.
        /// </remarks>
        public ExtensionNodeTypeCollection GetAllowedNodeTypes()
        {
            if (cachedAllowedTypes == null)
            {
                cachedAllowedTypes = new ExtensionNodeTypeCollection();
                GetAllowedNodeTypes(new Hashtable(), cachedAllowedTypes);
            }
            return cachedAllowedTypes;
        }

        void GetAllowedNodeTypes(Hashtable visitedSets, ExtensionNodeTypeCollection col)
        {
            if (Id.Length > 0)
            {
                if (visitedSets.Contains(Id))
                    return;
                visitedSets[Id] = Id;
            }

            // Gets all allowed node types, including those defined in node sets
            // It only works for descriptions generated from a registry

            foreach (ExtensionNodeType nt in NodeTypes)
                col.Add(nt);

            BundleDescription desc = ParentBundleDescription;
            if (desc == null || desc.OwnerDatabase == null)
                return;

            foreach (string[] ns in NodeSets.InternalList)
            {
                string startBundle = ns[1];
                if (startBundle == null || startBundle.Length == 0)
                    startBundle = desc.BundleId;
                ExtensionNodeSet nset = desc.OwnerDatabase.FindNodeSet(ParentBundleDescription.Domain, startBundle, ns[0]);
                if (nset != null)
                    nset.GetAllowedNodeTypes(visitedSets, col);
            }
        }

        internal void Clear()
        {
            Element = null;
            nodeSets = null;
            nodeTypes = null;
        }

        internal void SetExtensionsBundleId(string addinId)
        {
            foreach (ExtensionNodeType nt in NodeTypes)
            {
                nt.BundleId = addinId;
                nt.SetExtensionsBundleId(addinId);
            }
            NodeSets.SetExtensionsBundleId(addinId);
        }

        internal void MergeWith(string thisBundleId, ExtensionNodeSet other)
        {
            foreach (ExtensionNodeType nt in other.NodeTypes)
            {
                if (nt.BundleId != thisBundleId && !NodeTypes.Contains(nt))
                    NodeTypes.Add(nt);
            }
            NodeSets.MergeWith(thisBundleId, other.NodeSets);
        }

        internal void UnmergeExternalData(string thisBundleId, Hashtable addinsToUnmerge)
        {
            // Removes extension types and extension sets coming from other add-ins.

            ArrayList todelete = new ArrayList();
            foreach (ExtensionNodeType nt in NodeTypes)
            {
                if (nt.BundleId != thisBundleId && (addinsToUnmerge == null || addinsToUnmerge.Contains(nt.BundleId)))
                    todelete.Add(nt);
            }
            foreach (ExtensionNodeType nt in todelete)
                NodeTypes.Remove(nt);

            NodeSets.UnmergeExternalData(thisBundleId, addinsToUnmerge);
        }

        void InitCollections()
        {
            nodeTypes = new ExtensionNodeTypeCollection(this);
            nodeSets = new NodeSetIdCollection();

            foreach (XmlNode n in Element.ChildNodes)
            {
                XmlElement nt = n as XmlElement;
                if (nt == null)
                    continue;
                if (nt.LocalName == "ExtensionNode")
                {
                    ExtensionNodeType etype = new ExtensionNodeType(nt);
                    nodeTypes.Add(etype);
                }
                else if (nt.LocalName == "ExtensionNodeSet")
                {
                    string id = nt.GetAttribute("id");
                    if (id.Length > 0)
                        nodeSets.Add(id);
                    else
                        missingNodeSetId = true;
                }
            }
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("Id", id);
            writer.WriteValue("NodeTypes", NodeTypes);
            writer.WriteValue("NodeSets", NodeSets.InternalList);
        }

        internal override void Read(BinaryXmlReader reader)
        {
            id = reader.ReadStringValue("Id");
            nodeTypes = (ExtensionNodeTypeCollection)reader.ReadValue("NodeTypes", new ExtensionNodeTypeCollection(this));
            reader.ReadValue("NodeSets", NodeSets.InternalList);
        }
    }

  
}
