using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class Extension : ObjectDescription, IComparable
    {
        private string path;
        private ExtensionNodeDescriptionCollection nodes;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public ObjectDescription GetExtendedObject()
        {
            BundleDescription desc = ParentAddinDescription;
            if (desc == null)
                return null;

            ExtensionPoint ep = FindExtensionPoint(desc, path);

            if (ep == null && desc.OwnerDatabase != null)
            {
                foreach (Dependency dep in desc.MainModule.Dependencies)
                {
                    BundleDependency adep = dep as BundleDependency;

                    if (adep == null) continue;

                    Bundle ad = desc.OwnerDatabase.GetInstalledAddin(ParentAddinDescription.Domain, adep.FullAddinId);
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

        public ExtensionNodeDescriptionCollection ExtensionNodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ExtensionNodeDescriptionCollection(this);
                }
                return nodes;
            }
        }

        int IComparable.CompareTo(object obj)
        {
            Extension other = (Extension)obj;
            return Path.CompareTo(other.Path);
        }
    }
}
