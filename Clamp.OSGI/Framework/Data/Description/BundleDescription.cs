using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class BundleDescription : ObjectDescription
    {
        private BundleDatabase ownerDatabase;
        private string configFile;
        private string sourceAddinFile;
        private ModuleDescription mainModule;
        private ModuleCollection optionalModules;
        private string ns;
        private Dictionary<string, string> variables;
        private AddinPropertyCollectionImpl properties;
        private ExtensionPointCollection extensionPoints;
        private ExtensionNodeSetCollection nodeSets;
        private ConditionTypeDescriptionCollection conditionTypes;
        private ExtensionNodeDescription localizer;
        private bool canWrite = true;
        private string id;
        private string name;
        private string version;
        private string compatVersion;
        private string author;
        private string url;
        private string copyright;
        private string description;
        private string category;
        private string basePath;
        private bool isroot;
        private bool hasUserId;
        private bool defaultEnabled = true;
        private BundleFlags flags = BundleFlags.None;
        private string domain;
        private object[] fileInfo;

        #region public Property
        public string BasePath
        {
            get { return basePath != null ? basePath : string.Empty; }
        }

        public string FileName
        {
            get { return configFile; }
            set { configFile = value; }
        }
        public string AddinId
        {
            get { return Bundle.GetFullId(Namespace, LocalId, Version); }
        }

        public string AddinFile
        {
            get { return sourceAddinFile; }
            set { sourceAddinFile = value; }
        }



        public string Version
        {
            get { return version != null ? ParseString(version) : string.Empty; }
            set { version = value; }
        }

        public string LocalId
        {
            get { return id != null ? ParseString(id) : string.Empty; }
            set { id = value; hasUserId = true; }
        }

        public BundleFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public string CompatVersion
        {
            get { return compatVersion != null ? ParseString(compatVersion) : string.Empty; }
            set { compatVersion = value; }
        }


        public ModuleDescription MainModule
        {
            get
            {
                if (mainModule == null)
                {
                    mainModule = new ModuleDescription();

                    mainModule.SetParent(this);
                }
                return mainModule;
            }
        }

        public ModuleCollection OptionalModules
        {
            get
            {
                if (optionalModules == null)
                {
                    optionalModules = new ModuleCollection(this);
                }
                return optionalModules;
            }
        }

        public ModuleCollection AllModules
        {
            get
            {
                ModuleCollection col = new ModuleCollection(this);

                col.Add(MainModule);

                foreach (ModuleDescription mod in OptionalModules)
                    col.Add(mod);

                return col;
            }
        }

        public StringCollection AllIgnorePaths
        {
            get
            {
                StringCollection col = new StringCollection();
                foreach (string s in MainModule.IgnorePaths)
                    col.Add(s);

                foreach (ModuleDescription mod in OptionalModules)
                {
                    foreach (string s in mod.IgnorePaths)
                        col.Add(s);
                }
                return col;
            }
        }

        public bool IsRoot
        {
            get { return isroot; }
            set { isroot = value; }
        }

        public string Namespace
        {
            get { return ns != null ? ParseString(ns) : string.Empty; }
            set { ns = value; }
        }

        public bool CanUninstall
        {
            get { return (flags & BundleFlags.CantUninstall) == 0 && !IsHidden; }
        }

        public bool EnabledByDefault
        {
            get { return defaultEnabled; }
            set { defaultEnabled = value; }
        }

        public bool IsHidden
        {
            get { return (flags & BundleFlags.Hidden) != 0; }
        }

        public ExtensionNodeDescription Localizer
        {
            get { return localizer; }
            set { localizer = value; }
        }


        public List<string> AllFiles
        {
            get
            {
                List<string> col = new List<string>();

                foreach (string s in MainModule.AllFiles)
                    col.Add(s);

                foreach (ModuleDescription mod in OptionalModules)
                {
                    foreach (string s in mod.AllFiles)
                        col.Add(s);
                }
                return col;
            }
        }

        public ExtensionPointCollection ExtensionPoints
        {
            get
            {
                if (extensionPoints == null)
                {
                    extensionPoints = new ExtensionPointCollection(this);
                }
                return extensionPoints;
            }
        }

        public ExtensionNodeSetCollection ExtensionNodeSets
        {
            get
            {
                if (nodeSets == null)
                {
                    nodeSets = new ExtensionNodeSetCollection(this);
                }
                return nodeSets;
            }
        }

        public BundlePropertyCollection Properties
        {
            get
            {
                if (properties == null)
                    properties = new AddinPropertyCollectionImpl(this);
                return properties;
            }
        }

        public string Name
        {
            get
            {
                string val = Properties.GetPropertyValue("Name");
                if (val.Length > 0)
                    return val;
                if (name != null && name.Length > 0)
                    return ParseString(name);
                if (HasUserId)
                    return AddinId;
                else if (sourceAddinFile != null)
                    return Path.GetFileNameWithoutExtension(sourceAddinFile);
                else
                    return string.Empty;
            }
            set { name = value; }
        }
        public string Url
        {
            get
            {
                string val = Properties.GetPropertyValue("Url");
                if (val.Length > 0)
                    return val;
                return ParseString(url) ?? string.Empty;
            }
            set { url = value; }
        }

        public string Description
        {
            get
            {
                string val = Properties.GetPropertyValue("Description");
                if (val.Length > 0)
                    return val;
                return ParseString(description) ?? string.Empty;
            }
            set { description = value; }
        }

        public string Author
        {
            get
            {
                string val = Properties.GetPropertyValue("Author");
                if (val.Length > 0)
                    return val;
                return ParseString(author) ?? string.Empty;
            }
            set { author = value; }
        }


        public string Copyright
        {
            get
            {
                string val = Properties.GetPropertyValue("Copyright");
                if (val.Length > 0)
                    return val;
                return ParseString(copyright) ?? string.Empty;
            }
            set { copyright = value; }
        }

        public string Category
        {
            get
            {
                string val = Properties.GetPropertyValue("Category");
                if (val.Length > 0)
                    return val;
                return ParseString(category) ?? string.Empty;
            }
            set { category = value; }
        }

        public ConditionTypeDescriptionCollection ConditionTypes
        {
            get
            {
                if (conditionTypes == null)
                {
                    conditionTypes = new ConditionTypeDescriptionCollection(this);
                }
                return conditionTypes;
            }
        }

        #endregion

        #region internal Property
        internal BundleDatabase OwnerDatabase
        {
            get { return ownerDatabase; }
            set { ownerDatabase = value; }
        }

        internal bool HasUserId
        {
            get { return hasUserId; }
            set { hasUserId = value; }
        }

        internal string Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        internal bool IsExtensionModel
        {
            get { return false; }
        }

        #endregion

        #region internal method

        internal bool FilesChanged()
        {
            // Checks if the files of the add-in have changed.
            if (fileInfo == null)
                return true;

            foreach (BundleFileInfo f in fileInfo)
            {
                string file = Path.Combine(this.BasePath, f.FileName);
                if (!File.Exists(file))
                    return true;
                if (f.Timestamp != File.GetLastWriteTime(file))
                    return true;
            }

            return false;
        }


        internal bool SupportsVersion(string ver)
        {
            return Bundle.CompareVersions(ver, Version) >= 0 &&
                   (CompatVersion.Length == 0 || Bundle.CompareVersions(ver, CompatVersion) <= 0);
        }

        internal void UnmergeExternalData(Hashtable addins)
        {
            //// Removes extension types and extension sets coming from other add-ins.
            //foreach (ExtensionPoint ep in ExtensionPoints)
            //    ep.UnmergeExternalData(AddinId, addins);

            //foreach (ExtensionNodeSet ns in ExtensionNodeSets)
            //    ns.UnmergeExternalData(AddinId, addins);
        }

        internal void SaveBinary(FileDatabase fdb, string file)
        {
            configFile = file;
            SaveBinary(fdb);
        }

        internal void SaveBinary(FileDatabase fdb)
        {
            if (!canWrite)
                throw new InvalidOperationException("不能保存不完整的组件信息");

            fdb.WriteSharedObject(AddinFile, FileName, this);
        }

        internal void MergeExternalData(BundleDescription other)
        {
            // Removes extension types and extension sets coming from other add-ins.
            //foreach (ExtensionPoint ep in other.ExtensionPoints)
            //{
            //    ExtensionPoint tep = ExtensionPoints[ep.Path];
            //    if (tep != null)
            //        tep.MergeWith(AddinId, ep);
            //}

            //foreach (ExtensionNodeSet ns in other.ExtensionNodeSets)
            //{
            //    ExtensionNodeSet tns = ExtensionNodeSets[ns.Id];
            //    if (tns != null)
            //        tns.MergeWith(AddinId, ns);
            //}
        }

        internal void SetExtensionsAddinId(string addinId)
        {
            foreach (ExtensionPoint ep in ExtensionPoints)
                ep.SetExtensionsAddinId(addinId);

            foreach (ExtensionNodeSet ns in ExtensionNodeSets)
                ns.SetExtensionsAddinId(addinId);
        }


        internal void StoreFileInfo()
        {
            ArrayList list = new ArrayList();
            foreach (string f in AllFiles)
            {
                string file = Path.Combine(this.BasePath, f);
                BundleFileInfo fi = new BundleFileInfo();
                fi.FileName = f;
                fi.Timestamp = File.GetLastWriteTime(file);
                list.Add(fi);
            }

            fileInfo = list.ToArray();
        }

        internal void SetBasePath(string path)
        {
            basePath = path;
        }

        internal string ParseString(string input)
        {
            if (input == null || input.Length < 4)
                return input;

            int i = input.IndexOf("$(");
            if (i == -1)
                return input;

            StringBuilder result = new StringBuilder(input.Length);
            result.Append(input, 0, i);

            while (i < input.Length)
            {
                if (input[i] == '$')
                {
                    i++;

                    if (i >= input.Length || input[i] != '(')
                    {
                        result.Append('$');
                        continue;
                    }

                    i++;
                    int start = i;
                    while (i < input.Length && input[i] != ')')
                        i++;

                    string tag = input.Substring(start, i - start);

                    string tagValue;
                    if (TryGetVariableValue(tag, out tagValue))
                        result.Append(tagValue);
                    else
                    {
                        result.Append('$');
                        i = start - 1;
                    }
                }
                else
                {
                    result.Append(input[i]);
                }
                i++;
            }
            return result.ToString();
        }
        #endregion

        #region public mehtod
        public void Save(string fileName)
        {
            configFile = fileName;
            Save();
        }

        public void Save()
        {
            if (configFile == null)
                throw new InvalidOperationException("File name not specified.");

            //SaveXml();

            //using (StreamWriter sw = new StreamWriter(configFile))
            //{
            //    XmlTextWriter tw = new XmlTextWriter(sw);
            //    tw.Formatting = Formatting.Indented;
            //    configDoc.Save(tw);
            //}
        }




        #endregion

        #region private method

        private bool TryGetVariableValue(string name, out string value)
        {
            if (variables != null && variables.TryGetValue(name, out value))
                return true;

            switch (name)
            {
                case "Id": value = id; return true;
                case "Namespace": value = ns; return true;
                case "Version": value = version; return true;
                case "CompatVersion": value = compatVersion; return true;
                case "DefaultEnabled": value = defaultEnabled.ToString(); return true;
                case "IsRoot": value = isroot.ToString(); return true;
                case "Flags": value = flags.ToString(); return true;
            }
            if (properties != null && properties.HasProperty(name))
            {
                value = properties.GetPropertyValue(name);
                return true;
            }
            value = null;
            return false;
        }
        #endregion

        #region internal static method
        internal void ResetXmlDoc()
        {
            //configDoc = null;
        }

        internal static BundleDescription Merge(BundleDescription desc1, BundleDescription desc2)
        {
            if (!desc2.IsExtensionModel)
            {
                BundleDescription tmp = desc1;
                desc1 = desc2; desc2 = tmp;
            }
            //((AddinPropertyCollectionImpl)desc1.Properties).AddRange(desc2.Properties);
            //        desc1.ExtensionPoints.AddRange(desc2.ExtensionPoints);
            //        desc1.ExtensionNodeSets.AddRange(desc2.ExtensionNodeSets);
            //        desc1.ConditionTypes.AddRange(desc2.ConditionTypes);
            //        desc1.OptionalModules.AddRange(desc2.OptionalModules);
            //        foreach (string s in desc2.MainModule.Assemblies)
            //            desc1.MainModule.Assemblies.Add(s);
            //        foreach (string s in desc2.MainModule.DataFiles)
            //            desc1.MainModule.DataFiles.Add(s);
            //        desc1.MainModule.MergeWith(desc2.MainModule);
            return desc1;
        }

        internal ExtensionNodeDescription FindExtensionNode(string path, bool lookInDeps)
        {
            // Look in the extensions of this add-in

            foreach (Extension ext in MainModule.Extensions)
            {
                if (path.StartsWith(ext.Path + "/"))
                {
                    string subp = path.Substring(ext.Path.Length).Trim('/');
                    ExtensionNodeDescriptionCollection nodes = ext.ExtensionNodes;
                    ExtensionNodeDescription node = null;
                    foreach (string p in subp.Split('/'))
                    {
                        if (p.Length == 0) continue;
                        node = nodes[p];
                        if (node == null)
                            break;
                        nodes = node.ChildNodes;
                    }
                    if (node != null)
                        return node;
                }
            }

            if (!lookInDeps || OwnerDatabase == null)
                return null;

            // Look in dependencies

            foreach (Dependency dep in MainModule.Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep == null) continue;
                Bundle ad = OwnerDatabase.GetInstalledAddin(Domain, adep.FullAddinId);
                if (ad != null && ad.Description != null)
                {
                    ExtensionNodeDescription node = ad.Description.FindExtensionNode(path, false);
                    if (node != null)
                        return node;
                }
            }
            return null;
        }

        internal static BundleDescription ReadBinary(FileDatabase fdb, string configFile)
        {
            BundleDescription description = (BundleDescription)fdb.ReadSharedObject(configFile);
            if (description != null)
            {
                description.FileName = configFile;
                description.canWrite = !fdb.IgnoreDescriptionData;
            }
            return description;
        }

        public static BundleDescription Read(Stream stream, string basePath)
        {
            return Read(new StreamReader(stream), basePath);
        }

        /// <summary>
        /// Load an add-in description from a text reader
        /// </summary>
        /// <param name='reader'>
        /// The text reader
        /// </param>
        /// <param name='basePath'>
        /// The path to be used to resolve relative file paths.
        /// </param>
        public static BundleDescription Read(TextReader reader, string basePath)
        {
            BundleDescription config = new BundleDescription();

            //try
            //{
            //    config.configDoc = new XmlDocument();
            //    config.configDoc.Load(reader);
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidOperationException("The add-in configuration file is invalid: " + ex.Message, ex);
            //}

            //XmlElement elem = config.configDoc.DocumentElement;
            //if (elem.LocalName == "ExtensionModel")
            //    return config;

            //XmlElement varsElem = (XmlElement)elem.SelectSingleNode("Variables");
            //if (varsElem != null)
            //{
            //    foreach (XmlNode node in varsElem.ChildNodes)
            //    {
            //        XmlElement prop = node as XmlElement;
            //        if (prop == null)
            //            continue;
            //        if (config.variables == null)
            //            config.variables = new Dictionary<string, string>();
            //        config.variables[prop.LocalName] = prop.InnerText;
            //    }
            //}

            //config.id = elem.GetAttribute("id");
            //config.ns = elem.GetAttribute("namespace");
            //config.name = elem.GetAttribute("name");
            //config.version = elem.GetAttribute("version");
            //config.compatVersion = elem.GetAttribute("compatVersion");
            //config.author = elem.GetAttribute("author");
            //config.url = elem.GetAttribute("url");
            //config.copyright = elem.GetAttribute("copyright");
            //config.description = elem.GetAttribute("description");
            //config.category = elem.GetAttribute("category");
            //config.basePath = elem.GetAttribute("basePath");
            //config.domain = "global";

            //string s = elem.GetAttribute("isRoot");
            //if (s.Length == 0) s = elem.GetAttribute("isroot");
            //config.isroot = GetBool(s, false);

            //config.defaultEnabled = GetBool(elem.GetAttribute("defaultEnabled"), true);

            //string prot = elem.GetAttribute("flags");
            //if (prot.Length == 0)
            //    config.flags = AddinFlags.None;
            //else
            //    config.flags = (AddinFlags)Enum.Parse(typeof(AddinFlags), prot);

            //XmlElement localizerElem = (XmlElement)elem.SelectSingleNode("Localizer");
            //if (localizerElem != null)
            //    config.localizer = new ExtensionNodeDescription(localizerElem);

            //XmlElement headerElem = (XmlElement)elem.SelectSingleNode("Header");
            //if (headerElem != null)
            //{
            //    foreach (XmlNode node in headerElem.ChildNodes)
            //    {
            //        XmlElement prop = node as XmlElement;
            //        if (prop == null)
            //            continue;
            //        config.Properties.SetPropertyValue(prop.LocalName, prop.InnerText, prop.GetAttribute("locale"));
            //    }
            //}

            //config.TransferCoreProperties(false);

            //if (config.id.Length > 0)
            //    config.hasUserId = true;

            return config;
        }

        #endregion

    }
}
