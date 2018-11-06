using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    class AddinUpdateData
    {
        Dictionary<string, List<ExtensionPoint>> objectTypeExtensions = new Dictionary<string, List<ExtensionPoint>>();

        internal int RelNodeSetTypes;
        internal int RelExtensionPoints;
        internal int RelExtensions;
        internal int RelExtensionNodes;
        public AddinUpdateData(BundleDatabase database)
        {
        }

        public void RegisterNodeSet(BundleDescription description, ExtensionNodeSet nset)
        {
            //List<RootExtensionPoint> extensions;
            //if (nodeSetHash.TryGetValue(nset.Id, out extensions))
            //{
            //    // Extension point already registered
            //    List<ExtensionPoint> compatExtensions = GetCompatibleExtensionPoints(nset.Id, description, description.MainModule, extensions);
            //    if (compatExtensions.Count > 0)
            //    {
            //        foreach (ExtensionPoint einfo in compatExtensions)
            //            einfo.NodeSet.MergeWith(null, nset);
            //        return;
            //    }
            //}
            //// Create a new extension set
            //RootExtensionPoint rep = new RootExtensionPoint();
            //rep.ExtensionPoint = new ExtensionPoint();
            //rep.ExtensionPoint.SetNodeSet(nset);
            //rep.ExtensionPoint.RootAddin = description.AddinId;
            //rep.ExtensionPoint.Path = nset.Id;
            //rep.Description = description;
            //if (extensions == null)
            //{
            //    extensions = new List<RootExtensionPoint>();
            //    nodeSetHash[nset.Id] = extensions;
            //}
            //extensions.Add(rep);
        }

        public void RegisterExtensionPoint(BundleDescription description, ExtensionPoint ep)
        {
            //List<RootExtensionPoint> extensions;
            //if (pathHash.TryGetValue(ep.Path, out extensions))
            //{
            //    // Extension point already registered
            //    List<ExtensionPoint> compatExtensions = GetCompatibleExtensionPoints(ep.Path, description, description.MainModule, extensions);
            //    if (compatExtensions.Count > 0)
            //    {
            //        foreach (ExtensionPoint einfo in compatExtensions)
            //            einfo.MergeWith(null, ep);
            //        RegisterObjectTypes(ep);
            //        return;
            //    }
            //}
            //// Create a new extension
            //RootExtensionPoint rep = new RootExtensionPoint();
            //rep.ExtensionPoint = ep;
            //rep.ExtensionPoint.RootAddin = description.AddinId;
            //rep.Description = description;
            //if (extensions == null)
            //{
            //    extensions = new List<RootExtensionPoint>();
            //    pathHash[ep.Path] = extensions;
            //}
            //extensions.Add(rep);
            //RegisterObjectTypes(ep);
        }

        public void RegisterExtension(BundleDescription description, ModuleDescription module, Extension extension)
        {
            //if (extension.Path.StartsWith("$"))
            //{
            //    string[] objectTypes = extension.Path.Substring(1).Split(',');
            //    bool found = false;
            //    foreach (string s in objectTypes)
            //    {
            //        List<ExtensionPoint> list;
            //        if (objectTypeExtensions.TryGetValue(s, out list))
            //        {
            //            found = true;
            //            foreach (ExtensionPoint ep in list)
            //            {
            //                if (IsAddinCompatible(ep.ParentAddinDescription, description, module))
            //                {
            //                    extension.Path = ep.Path;
            //                    RegisterExtension(description, module, ep.Path);
            //                }
            //            }
            //        }
            //    }
            //    if (!found)
            //        monitor.ReportWarning("The add-in '" + description.AddinId + "' is trying to register the class '" + extension.Path.Substring(1) + "', but there isn't any add-in defining a suitable extension point");
            //}
            //else if (extension.Path.StartsWith("%"))
            //{
            //    string[] objectTypes = extension.Path.Substring(1).Split(',');
            //    bool found = false;
            //    foreach (string s in objectTypes)
            //    {
            //        List<ExtensionNodeType> list;
            //        if (customAttributeTypeExtensions.TryGetValue(s, out list))
            //        {
            //            found = true;
            //            foreach (ExtensionNodeType nt in list)
            //            {
            //                ExtensionPoint ep = (ExtensionPoint)((ExtensionNodeSet)nt.Parent).Parent;
            //                if (IsAddinCompatible(ep.ParentAddinDescription, description, module))
            //                {
            //                    extension.Path = ep.Path;
            //                    foreach (ExtensionNodeDescription node in extension.ExtensionNodes)
            //                        node.NodeName = nt.NodeName;
            //                    RegisterExtension(description, module, ep.Path);
            //                }
            //            }
            //        }
            //    }
            //    if (!found)
            //        monitor.ReportWarning("The add-in '" + description.AddinId + "' is trying to register the class '" + extension.Path.Substring(1) + "', but there isn't any add-in defining a suitable extension point");
            //}
        }

        public void RegisterExtension(BundleDescription description, ModuleDescription module, string path)
        {
            //List<RootExtensionPoint> extensions;
            //if (!pathHash.TryGetValue(path, out extensions))
            //{
            //    // Root add-in extension points are registered before any other kind of extension,
            //    // so we should find it now.
            //    extensions = GetParentExtensionInfo(path);
            //}
            //if (extensions == null)
            //{
            //    monitor.ReportWarning("The add-in '" + description.AddinId + "' is trying to extend '" + path + "', but there isn't any add-in defining this extension point");
            //    return;
            //}

            //bool found = false;
            //foreach (RootExtensionPoint einfo in extensions)
            //{
            //    if (IsAddinCompatible(einfo.Description, description, module))
            //    {
            //        if (!einfo.ExtensionPoint.Addins.Contains(description.AddinId))
            //            einfo.ExtensionPoint.Addins.Add(description.AddinId);
            //        found = true;
            //        if (monitor.LogLevel > 2)
            //        {
            //            monitor.Log("  * " + einfo.Description.AddinId + "(" + einfo.Description.Domain + ") <- " + path);
            //        }
            //    }
            //}
            //if (!found)
            //    monitor.ReportWarning("The add-in '" + description.AddinId + "' is trying to extend '" + path + "', but there isn't any compatible add-in defining this extension point");
        }

        bool IsAddinCompatible(BundleDescription installedDescription, BundleDescription description, ModuleDescription module)
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
                if (adep != null && Bundle.GetFullId(description.Namespace, adep.AddinId, null) == addinId)
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
