using Clamp.OSGI.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data
{
    class BundleUpdateData
    {
        // This table collects information about extensions. Each path (key)
        // has a RootExtensionPoint object with information about the addin that
        // defines the extension point and the addins which extend it
        Dictionary<string, List<RootExtensionPoint>> pathHash = new Dictionary<string, List<RootExtensionPoint>>();

        // Collects globally defined node sets. Key is node set name. Value is
        // a RootExtensionPoint
        Dictionary<string, List<RootExtensionPoint>> nodeSetHash = new Dictionary<string, List<RootExtensionPoint>>();

        Dictionary<string, List<ExtensionPoint>> objectTypeExtensions = new Dictionary<string, List<ExtensionPoint>>();

        Dictionary<string, List<ExtensionNodeType>> customAttributeTypeExtensions = new Dictionary<string, List<ExtensionNodeType>>();

        internal int RelExtensionPoints;
        internal int RelExtensions;
        internal int RelNodeSetTypes;
        internal int RelExtensionNodes;

        class RootExtensionPoint
        {
            public BundleDescription Description;
            public ExtensionPoint ExtensionPoint;
        }

        public BundleUpdateData(BundleDatabase database)
        {
        }

        public void RegisterNodeSet(BundleDescription description, ExtensionNodeSet nset)
        {
            List<RootExtensionPoint> extensions;
            if (nodeSetHash.TryGetValue(nset.Id, out extensions))
            {
                // Extension point already registered
                List<ExtensionPoint> compatExtensions = GetCompatibleExtensionPoints(nset.Id, description, description.MainModule, extensions);
                if (compatExtensions.Count > 0)
                {
                    foreach (ExtensionPoint einfo in compatExtensions)
                        einfo.NodeSet.MergeWith(null, nset);
                    return;
                }
            }
            // Create a new extension set
            RootExtensionPoint rep = new RootExtensionPoint();
            rep.ExtensionPoint = new ExtensionPoint();
            rep.ExtensionPoint.SetNodeSet(nset);
            rep.ExtensionPoint.RootBundle = description.BundleId;
            rep.ExtensionPoint.Path = nset.Id;
            rep.Description = description;
            if (extensions == null)
            {
                extensions = new List<RootExtensionPoint>();
                nodeSetHash[nset.Id] = extensions;
            }
            extensions.Add(rep);
        }

        public void RegisterExtensionPoint(BundleDescription description, ExtensionPoint ep)
        {
            List<RootExtensionPoint> extensions;
            if (pathHash.TryGetValue(ep.Path, out extensions))
            {
                // Extension point already registered
                List<ExtensionPoint> compatExtensions = GetCompatibleExtensionPoints(ep.Path, description, description.MainModule, extensions);
                if (compatExtensions.Count > 0)
                {
                    foreach (ExtensionPoint einfo in compatExtensions)
                        einfo.MergeWith(null, ep);
                    RegisterObjectTypes(ep);
                    return;
                }
            }
            // Create a new extension
            RootExtensionPoint rep = new RootExtensionPoint();
            rep.ExtensionPoint = ep;
            rep.ExtensionPoint.RootBundle = description.BundleId;
            rep.Description = description;
            if (extensions == null)
            {
                extensions = new List<RootExtensionPoint>();
                pathHash[ep.Path] = extensions;
            }
            extensions.Add(rep);
            RegisterObjectTypes(ep);
        }

        void RegisterObjectTypes(ExtensionPoint ep)
        {
            // Register extension points bound to a node type

            foreach (ExtensionNodeType nt in ep.NodeSet.NodeTypes)
            {
                if (nt.ObjectTypeName.Length > 0)
                {
                    List<ExtensionPoint> list;
                    if (!objectTypeExtensions.TryGetValue(nt.ObjectTypeName, out list))
                    {
                        list = new List<ExtensionPoint>();
                        objectTypeExtensions[nt.ObjectTypeName] = list;
                    }
                    list.Add(ep);
                }
                if (nt.ExtensionAttributeTypeName.Length > 0)
                {
                    List<ExtensionNodeType> list;
                    if (!customAttributeTypeExtensions.TryGetValue(nt.ExtensionAttributeTypeName, out list))
                    {
                        list = new List<ExtensionNodeType>();
                        customAttributeTypeExtensions[nt.ExtensionAttributeTypeName] = list;
                    }
                    list.Add(nt);
                }
            }
        }

        public void RegisterExtension(BundleDescription description, ModuleDescription module, Extension extension)
        {
            if (extension.Path.StartsWith("$"))
            {
                string[] objectTypes = extension.Path.Substring(1).Split(',');
                bool found = false;
                foreach (string s in objectTypes)
                {
                    List<ExtensionPoint> list;
                    if (objectTypeExtensions.TryGetValue(s, out list))
                    {
                        found = true;
                        foreach (ExtensionPoint ep in list)
                        {
                            if (IsBundleCompatible(ep.ParentBundleDescription, description, module))
                            {
                                extension.Path = ep.Path;
                                RegisterExtension(description, module, ep.Path);
                            }
                        }
                    }
                }
                if (!found) ;
                //TODO 记录日志

            }
            else if (extension.Path.StartsWith("%"))
            {
                string[] objectTypes = extension.Path.Substring(1).Split(',');
                bool found = false;
                foreach (string s in objectTypes)
                {
                    List<ExtensionNodeType> list;
                    if (customAttributeTypeExtensions.TryGetValue(s, out list))
                    {
                        found = true;
                        foreach (ExtensionNodeType nt in list)
                        {
                            ExtensionPoint ep = (ExtensionPoint)((ExtensionNodeSet)nt.Parent).Parent;
                            if (IsBundleCompatible(ep.ParentBundleDescription, description, module))
                            {
                                extension.Path = ep.Path;
                                foreach (ExtensionNodeDescription node in extension.ExtensionNodes)
                                    node.NodeName = nt.NodeName;
                                RegisterExtension(description, module, ep.Path);
                            }
                        }
                    }
                }
                if (!found) ;
                //TODO 记录日志
            }
        }

        public void RegisterExtension(BundleDescription description, ModuleDescription module, string path)
        {
            List<RootExtensionPoint> extensions;
            if (!pathHash.TryGetValue(path, out extensions))
            {
                // Root add-in extension points are registered before any other kind of extension,
                // so we should find it now.
                extensions = GetParentExtensionInfo(path);
            }
            if (extensions == null)
            {
                //TODO 记录日志
                return;
            }

            bool found = false;
            foreach (RootExtensionPoint einfo in extensions)
            {
                if (IsBundleCompatible(einfo.Description, description, module))
                {
                    if (!einfo.ExtensionPoint.Bundles.Contains(description.BundleId))
                        einfo.ExtensionPoint.Bundles.Add(description.BundleId);
                    found = true;
                    //TODO 记录日志
                }
            }
            if (!found) ;
            //TODO 记录日志
        }

        List<ExtensionPoint> GetCompatibleExtensionPoints(string path, BundleDescription description, ModuleDescription module, List<RootExtensionPoint> rootExtensionPoints)
        {
            List<ExtensionPoint> list = new List<ExtensionPoint>();
            foreach (RootExtensionPoint rep in rootExtensionPoints)
            {

                // Find an extension point defined in a root add-in which is compatible with the version of the extender dependency
                if (IsBundleCompatible(rep.Description, description, module))
                    list.Add(rep.ExtensionPoint);
            }
            return list;
        }

        List<RootExtensionPoint> GetParentExtensionInfo(string path)
        {
            int i = path.LastIndexOf('/');
            if (i == -1)
                return null;
            string np = path.Substring(0, i);
            List<RootExtensionPoint> ep;
            if (pathHash.TryGetValue(np, out ep))
                return ep;
            else
                return GetParentExtensionInfo(np);
        }

        bool IsBundleCompatible(BundleDescription installedDescription, BundleDescription description, ModuleDescription module)
        {
            if (installedDescription == description)
                return true;
            if (installedDescription.Domain != BundleDatabase.GlobalDomain)
            {
                if (description.Domain != BundleDatabase.GlobalDomain && description.Domain != installedDescription.Domain)
                    return false;
            }
            else if (description.Domain != BundleDatabase.GlobalDomain)
                return false;

            string addinId = Bundle.GetFullId(installedDescription.Namespace, installedDescription.LocalId, null);
            string requiredVersion = null;

            IEnumerable deps;
            if (module == description.MainModule)
                deps = module.Dependencies;
            else
            {
                ArrayList list = new ArrayList();
                list.AddRange(module.Dependencies);
                list.AddRange(description.MainModule.Dependencies);
                deps = list;
            }
            foreach (object dep in deps)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep != null && Bundle.GetFullId(description.Namespace, adep.BundleId, null) == addinId)
                {
                    requiredVersion = adep.Version;
                    break;
                }
            }
            if (requiredVersion == null)
                return false;

            // Check if the required version is between rep.Description.CompatVersion and rep.Description.Version
            if (Bundle.CompareVersions(installedDescription.Version, requiredVersion) > 0)
                return false;
            if (installedDescription.CompatVersion.Length > 0 && Bundle.CompareVersions(installedDescription.CompatVersion, requiredVersion) < 0)
                return false;

            return true;
        }
    }
}
