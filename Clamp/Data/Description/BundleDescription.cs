﻿using Clamp.Data.Description;
using Clamp.Data.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.Data.Description
{
    /// <summary>
    /// Bundle的详细类
    /// </summary>
    public class BundleDescription : IBinaryXmlElement
    {
        private XmlDocument configDoc;
        private string configFile;
        private BundleDatabase ownerDatabase;
        private string id;
        private string name;
        private string ns;
        private string version;
        private string compatVersion;
        private string author;
        private string url;
        private string copyright;
        private string description;
        private string category;
        private string basePath;
        private string sourceBundleFile;
        private bool isbundle;
        private bool hasUserId;
        private bool canWrite = true;
        private bool defaultEnabled = true;
        private int startLevel = 1;
        private BundleFlags flags = BundleFlags.None;
        private string domain;

        private ModuleDescription mainModule;
        private ModuleCollection optionalModules;
        private ExtensionNodeSetCollection nodeSets;
        private ConditionTypeDescriptionCollection conditionTypes;
        private ExtensionPointCollection extensionPoints;
        private ExtensionNodeDescription localizer;
        private ExtensionNodeDescription activator;
        private object[] fileInfo;

        private BundlePropertyCollectionImpl properties;
        private Dictionary<string, string> variables;

        internal static BinaryXmlTypeMap typeMap;

        static BundleDescription()
        {
            typeMap = new BinaryXmlTypeMap();
            typeMap.RegisterType(typeof(BundleDescription), "BundleDescription");
            typeMap.RegisterType(typeof(Extension), "Extension");
            typeMap.RegisterType(typeof(ExtensionNodeDescription), "Node");
            typeMap.RegisterType(typeof(ExtensionNodeSet), "NodeSet");
            typeMap.RegisterType(typeof(ExtensionNodeType), "NodeType");
            typeMap.RegisterType(typeof(ExtensionPoint), "ExtensionPoint");
            typeMap.RegisterType(typeof(ModuleDescription), "ModuleDescription");
            typeMap.RegisterType(typeof(ConditionTypeDescription), "ConditionType");
            typeMap.RegisterType(typeof(Condition), "Condition");
            typeMap.RegisterType(typeof(BundleDependency), "BundleDependency");
            typeMap.RegisterType(typeof(AssemblyDependency), "AssemblyDependency");
            typeMap.RegisterType(typeof(NodeTypeAttribute), "NodeTypeAttribute");
            typeMap.RegisterType(typeof(BundleFileInfo), "FileInfo");
            typeMap.RegisterType(typeof(BundleProperty), "Property");
        }

        internal BundleDatabase OwnerDatabase
        {
            get { return ownerDatabase; }
            set { ownerDatabase = value; }
        }

        /// <summary>
        /// 当前Bundle所在的文件
        /// </summary>
        public string BundleFile
        {
            get { return sourceBundleFile; }
            set { sourceBundleFile = value; }
        }

        /// <summary>
        /// Bundle的唯一标识
        /// </summary>
        public string BundleId
        {
            get { return Bundle.GetFullId(Namespace, LocalId, Version); }
        }

        /// <summary>
        /// 本地ID
        /// </summary>
        public string LocalId
        {
            get { return id != null ? ParseString(id) : string.Empty; }
            set { id = value; hasUserId = true; }
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace
        {
            get { return ns != null ? ParseString(ns) : string.Empty; }
            set { ns = value; }
        }

        /// <summary>
        /// Bundle的名称
        /// </summary>
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
                    return BundleId;
                else if (sourceBundleFile != null)
                    return Path.GetFileNameWithoutExtension(sourceBundleFile);
                else
                    return string.Empty;
            }
            set { name = value; }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get { return version != null ? ParseString(version) : string.Empty; }
            set { version = value; }
        }

        /// <summary>
        /// Gets or sets the version of the add-in with which this add-in is backwards compatible.
        /// </summary>
        /// <value>
        /// The compat version.
        /// </value>
        public string CompatVersion
        {
            get { return compatVersion != null ? ParseString(compatVersion) : string.Empty; }
            set { compatVersion = value; }
        }

        /// <summary>
        /// 作者
        /// </summary>
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

        /// <summary>
        /// Bundel对应的链接
        /// </summary>
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

        /// <summary>
        /// 版权
        /// </summary>
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

        /// <summary>
        /// 说明
        /// </summary>
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

        /// <summary>
        /// 分类
        /// </summary>
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

        /// <summary>
        /// 当前Bundle的根目录
        /// </summary>
        public string BasePath
        {
            get { return basePath != null ? basePath : string.Empty; }
        }

        /// <summary>
        /// 当前Bundle所在的根目录
        /// </summary>
        /// <param name="path"></param>
        internal void SetBasePath(string path)
        {
            basePath = path;
        }

        /// <summary>
        /// 指示当前是否是Bundle.如果false为fragmentBundle
        /// </summary>
        public bool IsBundle
        {
            get { return isbundle; }
            set { isbundle = value; }
        }

        /// <summary>
        /// 默认是否可以用
        /// </summary>
        public bool EnabledByDefault
        {
            get { return defaultEnabled; }
            set { defaultEnabled = value; }
        }
        /// <summary>
        /// 启动等级
        /// </summary>
        public int StartLevel
        {
            get { return startLevel; }
            set { startLevel = value; }
        }

        /// <summary>
        /// Bundle的状态
        /// </summary>
        public BundleFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        internal bool HasUserId
        {
            get { return hasUserId; }
            set { hasUserId = value; }
        }

        /// <summary>
        /// 是否可以禁止
        /// </summary>
        public bool CanDisable
        {
            get { return (flags & BundleFlags.CantDisable) == 0 && !IsHidden; }
        }

        /// <summary>
        /// 是否可以卸载
        /// </summary>
        public bool CanUninstall
        {
            get { return (flags & BundleFlags.CantUninstall) == 0 && !IsHidden; }
        }

        /// <summary>
        /// 是否是隐藏
        /// </summary>
        public bool IsHidden
        {
            get { return (flags & BundleFlags.Hidden) != 0; }
        }

        /// <summary>
        /// 指定的版本号是否支持
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        internal bool SupportsVersion(string ver)
        {
            return Bundle.CompareVersions(ver, Version) >= 0 &&
                   (CompatVersion.Length == 0 || Bundle.CompareVersions(ver, CompatVersion) <= 0);
        }

        /// <summary>
        /// 获得所有关连的文件
        /// </summary>
        public StringCollection AllFiles
        {
            get
            {
                StringCollection col = new StringCollection();

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

        /// <summary>
        /// 获得所有的怱略的路径
        /// </summary>
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

        /// <summary>
        /// 主模块信息
        /// </summary>
        public ModuleDescription MainModule
        {
            get
            {
                if (mainModule == null)
                {
                    if (RootElement == null)
                        mainModule = new ModuleDescription();
                    else
                        mainModule = new ModuleDescription(RootElement);

                    mainModule.SetParent(this);
                }

                return mainModule;
            }
        }

        /// <summary>
        /// 获得可选的Bundle模块集合
        /// </summary>
        public ModuleCollection OptionalModules
        {
            get
            {
                if (optionalModules == null)
                {
                    optionalModules = new ModuleCollection(this);
                    if (RootElement != null)
                    {
                        foreach (XmlElement mod in RootElement.SelectNodes("Module"))
                            optionalModules.Add(new ModuleDescription(mod));
                    }
                }
                return optionalModules;
            }
        }

        /// <summary>
        /// 所有模块信息
        /// </summary>
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

        /// <summary>
        /// 获得当前Bundle定义的扩展节点组集合
        /// </summary>
        public ExtensionNodeSetCollection ExtensionNodeSets
        {
            get
            {
                if (nodeSets == null)
                {
                    nodeSets = new ExtensionNodeSetCollection(this);
                    if (RootElement != null)
                    {
                        foreach (XmlElement elem in RootElement.SelectNodes("ExtensionNodeSet"))
                            nodeSets.Add(new ExtensionNodeSet(elem));
                    }
                }
                return nodeSets;
            }
        }

        /// <summary>
        /// 获得当前Bundle定义的扩展点集合
        /// </summary>
        public ExtensionPointCollection ExtensionPoints
        {
            get
            {
                if (extensionPoints == null)
                {
                    extensionPoints = new ExtensionPointCollection(this);

                    if (RootElement != null)
                    {
                        foreach (XmlElement elem in RootElement.SelectNodes("ExtensionPoint"))
                            extensionPoints.Add(new ExtensionPoint(elem));
                    }
                }
                return extensionPoints;
            }
        }

        /// <summary>
        /// Gets the condition types.
        /// </summary>
        /// <value>
        /// The condition types.
        /// </value>
        public ConditionTypeDescriptionCollection ConditionTypes
        {
            get
            {
                if (conditionTypes == null)
                {
                    conditionTypes = new ConditionTypeDescriptionCollection(this);
                    if (RootElement != null)
                    {
                        foreach (XmlElement elem in RootElement.SelectNodes("ConditionType"))
                            conditionTypes.Add(new ConditionTypeDescription(elem));
                    }
                }
                return conditionTypes;
            }
        }

        /// <summary>
        /// 本地化
        /// </summary>
        public ExtensionNodeDescription Localizer
        {
            get { return localizer; }
            set { localizer = value; }
        }

        /// <summary>
        /// 激活类
        /// </summary>
        public ExtensionNodeDescription Activator
        {
            get { return activator; }
            set { activator = value; }
        }

        /// <summary>
        /// Custom properties specified in the add-in header
        /// </summary>
        public BundlePropertyCollection Properties
        {
            get
            {
                if (properties == null)
                    properties = new BundlePropertyCollectionImpl(this);
                return properties;
            }
        }

        /// <summary>
        /// 增加一个扩展点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ExtensionPoint AddExtensionPoint(string path)
        {
            ExtensionPoint ep = new ExtensionPoint();
            ep.Path = path;
            ExtensionPoints.Add(ep);
            return ep;
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
                Bundle ad = OwnerDatabase.GetInstalledBundle(Domain, adep.FullBundleId);
                if (ad != null && ad.Description != null)
                {
                    ExtensionNodeDescription node = ad.Description.FindExtensionNode(path, false);
                    if (node != null)
                        return node;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得bundle的对应的XML的根目录
        /// </summary>
        XmlElement RootElement
        {
            get
            {
                if (configDoc != null)
                    return configDoc.DocumentElement;
                else
                    return null;
            }
        }

        internal void ResetXmlDoc()
        {
            configDoc = null;
        }

        /// <summary>
        /// Gets or sets file where this description is stored
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FileName
        {
            get { return configFile; }
            set { configFile = value; }
        }

        internal string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        internal void StoreFileInfo()
        {
            ArrayList list = new ArrayList();

            foreach (string f in AllFiles)
            {
                string file = Path.Combine(this.BasePath, f);

                BundleFileInfo fi = new BundleFileInfo();

                fi.FileName = f;
                fi.LastScan = File.GetLastWriteTime(file);

                list.Add(fi);
            }

            fileInfo = list.ToArray();
        }
        /// <summary>
        /// 判断Bundle详细对应的文件有没有发生变化
        /// </summary>
        /// <returns></returns>
        internal bool FilesChanged()
        {
            if (fileInfo == null)
                return true;

            foreach (BundleFileInfo f in fileInfo)
            {
                string file = Path.Combine(this.BasePath, f.FileName);
                if (!File.Exists(file))
                    return true;
                if (f.LastScan != File.GetLastWriteTime(file))
                    return true;
            }

            return false;
        }

        void TransferCoreProperties(bool removeProperties)
        {
            if (properties == null)
                return;

            string val = properties.ExtractCoreProperty("Id", removeProperties);
            if (val != null)
                id = val;

            val = properties.ExtractCoreProperty("Namespace", removeProperties);
            if (val != null)
                ns = val;

            val = properties.ExtractCoreProperty("Version", removeProperties);
            if (val != null)
                version = val;

            val = properties.ExtractCoreProperty("CompatVersion", removeProperties);
            if (val != null)
                compatVersion = val;

            val = properties.ExtractCoreProperty("DefaultEnabled", removeProperties);
            if (val != null)
                defaultEnabled = GetBool(val, true);

            val = properties.ExtractCoreProperty("IsRoot", removeProperties);
            if (val != null)
                isbundle = GetBool(val, true);

            val = properties.ExtractCoreProperty("Flags", removeProperties);
            if (val != null)
                flags = (BundleFlags)Enum.Parse(typeof(BundleFlags), val);
        }

        bool TryGetVariableValue(string name, out string value)
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
                case "IsBundle": value = isbundle.ToString(); return true;
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

        /// <summary>
        /// Saves the add-in description.
        /// </summary>
        /// <param name='fileName'>
        /// File name where to save this instance
        /// </param>
        /// <remarks>
        /// Saves the add-in description to the specified file and sets the FileName property.
        /// </remarks>
        public void Save(string fileName)
        {
            configFile = fileName;
            Save();
        }

        /// <summary>
        /// Saves the add-in description.
        /// </summary>
        /// <exception cref='InvalidOperationException'>
        /// It is thrown if FileName is not set
        /// </exception>
        /// <remarks>
        /// The description is saved to the file specified in the FileName property.
        /// </remarks>
        public void Save()
        {
            if (configFile == null)
                throw new InvalidOperationException("File name not specified.");

            SaveXml();

            using (StreamWriter sw = new StreamWriter(configFile))
            {
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.Formatting = Formatting.Indented;
                configDoc.Save(tw);
            }
        }

        /// <summary>
        /// Generates an XML representation of the add-in description
        /// </summary>
        /// <returns>
        /// An XML manifest.
        /// </returns>
        public XmlDocument SaveToXml()
        {
            SaveXml();
            return configDoc;
        }

        void SaveXml()
        {
            if (!canWrite)
                throw new InvalidOperationException("Can't write incomplete description.");

            XmlElement elem;

            if (configDoc == null)
            {
                configDoc = new XmlDocument();
                configDoc.AppendChild(configDoc.CreateElement("Bundle"));
            }

            elem = configDoc.DocumentElement;

            SaveCoreProperty(elem, HasUserId ? id : null, "id", "Id");
            SaveCoreProperty(elem, version, "version", "Version");
            SaveCoreProperty(elem, ns, "namespace", "Namespace");
            SaveCoreProperty(elem, isbundle ? "true" : null, "isroot", "IsRoot");

            // Name will return the file name when HasUserId=false
            if (!string.IsNullOrEmpty(name))
                elem.SetAttribute("name", name);
            else
                elem.RemoveAttribute("name");

            SaveCoreProperty(elem, compatVersion, "compatVersion", "CompatVersion");
            SaveCoreProperty(elem, defaultEnabled ? null : "false", "defaultEnabled", "DefaultEnabled");
            SaveCoreProperty(elem, flags != BundleFlags.None ? flags.ToString() : null, "flags", "Flags");

            if (author != null && author.Length > 0)
                elem.SetAttribute("author", author);
            else
                elem.RemoveAttribute("author");

            if (url != null && url.Length > 0)
                elem.SetAttribute("url", url);
            else
                elem.RemoveAttribute("url");

            if (copyright != null && copyright.Length > 0)
                elem.SetAttribute("copyright", copyright);
            else
                elem.RemoveAttribute("copyright");

            if (description != null && description.Length > 0)
                elem.SetAttribute("description", description);
            else
                elem.RemoveAttribute("description");

            if (category != null && category.Length > 0)
                elem.SetAttribute("category", category);
            else
                elem.RemoveAttribute("category");

            if (startLevel <= 0)
                elem.SetAttribute("startLevel", "1");
            else
                elem.SetAttribute("startLevel", Convert.ToString(startLevel));


            if (localizer == null || localizer.Element == null)
            {
                // Remove old element if it exists
                XmlElement oldLoc = (XmlElement)elem.SelectSingleNode("Localizer");
                if (oldLoc != null)
                    elem.RemoveChild(oldLoc);
            }
            if (localizer != null)
                localizer.SaveXml(elem);

            if (activator == null || activator.Element == null)
            {
                // Remove old element if it exists
                XmlElement oldLoc = (XmlElement)elem.SelectSingleNode("Activator");
                if (oldLoc != null)
                    elem.RemoveChild(oldLoc);
            }

            if (activator != null)
                activator.SaveXml(elem);


            if (mainModule != null)
            {
                mainModule.Element = elem;
                mainModule.SaveXml(elem);
            }

            if (optionalModules != null)
                optionalModules.SaveXml(elem);

            if (nodeSets != null)
                nodeSets.SaveXml(elem);

            if (extensionPoints != null)
                extensionPoints.SaveXml(elem);

            XmlElement oldHeader = (XmlElement)elem.SelectSingleNode("Header");
            if (properties == null || properties.Count == 0)
            {
                if (oldHeader != null)
                    elem.RemoveChild(oldHeader);
            }
            else
            {
                if (oldHeader == null)
                {
                    oldHeader = elem.OwnerDocument.CreateElement("Header");
                    if (elem.FirstChild != null)
                        elem.InsertBefore(oldHeader, elem.FirstChild);
                    else
                        elem.AppendChild(oldHeader);
                }
                else
                    oldHeader.RemoveAll();
                foreach (var prop in properties)
                {
                    XmlElement propElem = elem.OwnerDocument.CreateElement(prop.Name);
                    if (!string.IsNullOrEmpty(prop.Locale))
                        propElem.SetAttribute("locale", prop.Locale);
                    propElem.InnerText = prop.Value ?? string.Empty;
                    oldHeader.AppendChild(propElem);
                }
            }

            XmlElement oldVars = (XmlElement)elem.SelectSingleNode("Variables");
            if (variables == null || variables.Count == 0)
            {
                if (oldVars != null)
                    elem.RemoveChild(oldVars);
            }
            else
            {
                if (oldVars == null)
                {
                    oldVars = elem.OwnerDocument.CreateElement("Variables");
                    if (elem.FirstChild != null)
                        elem.InsertBefore(oldVars, elem.FirstChild);
                    else
                        elem.AppendChild(oldVars);
                }
                else
                    oldVars.RemoveAll();
                foreach (var prop in variables)
                {
                    XmlElement propElem = elem.OwnerDocument.CreateElement(prop.Key);
                    propElem.InnerText = prop.Value ?? string.Empty;
                    oldVars.AppendChild(propElem);
                }
            }
        }

        void SaveCoreProperty(XmlElement elem, string val, string attr, string prop)
        {
            if (properties != null && properties.HasProperty(prop))
            {
                elem.RemoveAttribute(attr);
                if (!string.IsNullOrEmpty(val))
                    properties.SetPropertyValue(prop, val);
                else
                    properties.RemoveProperty(prop);
            }
            else if (string.IsNullOrEmpty(val))
                elem.RemoveAttribute(attr);
            else
                elem.SetAttribute(attr, val);
        }


        /// <summary>
        /// Load an add-in description from a file
        /// </summary>
        /// <param name='configFile'>
        /// The file.
        /// </param>
        public static BundleDescription Read(string configFile)
        {
            BundleDescription config;
            using (Stream s = File.OpenRead(configFile))
            {
                config = Read(s, Path.GetDirectoryName(configFile));
            }
            config.configFile = configFile;
            return config;
        }

        /// <summary>
        /// 获得Bundle的详细对象通过文件流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static BundleDescription Read(Stream stream, string basePath)
        {
            return Read(new StreamReader(stream), basePath);
        }

        /// <summary>
        /// 通过一个文本流来读取一个Bundle说明
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static BundleDescription Read(TextReader reader, string basePath)
        {
            BundleDescription bdesc = new BundleDescription();

            try
            {
                bdesc.configDoc = new XmlDocument();
                bdesc.configDoc.Load(reader);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Bundle配置文件不是有效的文件: " + ex.Message, ex);
            }

            XmlElement elem = bdesc.configDoc.DocumentElement;

            if (elem.LocalName == "ExtensionModel")
                return bdesc;

            XmlElement varsElem = (XmlElement)elem.SelectSingleNode("Variables");

            if (varsElem != null)
            {
                foreach (XmlNode node in varsElem.ChildNodes)
                {
                    XmlElement prop = node as XmlElement;

                    if (prop == null)
                        continue;

                    if (bdesc.variables == null)
                        bdesc.variables = new Dictionary<string, string>();

                    bdesc.variables[prop.LocalName] = prop.InnerText;
                }
            }

            bdesc.id = elem.GetAttribute("id");
            bdesc.ns = elem.GetAttribute("namespace");
            bdesc.name = elem.GetAttribute("name");
            bdesc.version = elem.GetAttribute("version");
            bdesc.compatVersion = elem.GetAttribute("compatVersion");
            bdesc.author = elem.GetAttribute("author");
            bdesc.url = elem.GetAttribute("url");
            bdesc.copyright = elem.GetAttribute("copyright");
            bdesc.description = elem.GetAttribute("description");
            bdesc.category = elem.GetAttribute("category");
            bdesc.basePath = elem.GetAttribute("basePath");
            bdesc.domain = "global";

            string s = elem.GetAttribute("isBundle");

            if (s.Length == 0)
                s = elem.GetAttribute("isbundle");

            bdesc.isbundle = GetBool(s, true);

            bdesc.defaultEnabled = GetBool(elem.GetAttribute("defaultEnabled"), true);

            string prot = elem.GetAttribute("flags");

            if (prot.Length == 0)
                bdesc.flags = BundleFlags.None;
            else
                bdesc.flags = (BundleFlags)Enum.Parse(typeof(BundleFlags), prot);

            string sl = elem.GetAttribute("startLevel");

            if (sl.Length == 0)
                bdesc.startLevel = 1;
            else
                bdesc.startLevel = Convert.ToInt32(elem.GetAttribute("startLevel"));

            XmlElement localizerElem = (XmlElement)elem.SelectSingleNode("Localizer");

            if (localizerElem != null)
                bdesc.localizer = new ExtensionNodeDescription(localizerElem);

            XmlElement activatorElem = (XmlElement)elem.SelectSingleNode("Activator");

            if (activatorElem != null)
                bdesc.activator = new ExtensionNodeDescription(activatorElem);

            XmlElement headerElem = (XmlElement)elem.SelectSingleNode("Header");

            if (headerElem != null)
            {
                foreach (XmlNode node in headerElem.ChildNodes)
                {
                    XmlElement prop = node as XmlElement;

                    if (prop == null)
                        continue;

                    bdesc.Properties.SetPropertyValue(prop.LocalName, prop.InnerText, prop.GetAttribute("locale"));
                }
            }

            bdesc.TransferCoreProperties(false);

            if (bdesc.id.Length > 0)
                bdesc.hasUserId = true;

            return bdesc;
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

        /// <summary>
        /// 将定的字符串转化为BOOLEAN
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defval"></param>
        /// <returns></returns>
        static bool GetBool(string s, bool defval)
        {
            if (s.Length == 0)
                return defval;
            else
                return s == "true" || s == "yes";
        }


        internal static BundleDescription ReadBinary(FileDatabase fdb, string configFile)
        {
            BundleDescription description = (BundleDescription)fdb.ReadSharedObject(configFile, typeMap);

            if (description != null)
            {
                description.FileName = configFile;
                description.canWrite = !fdb.IgnoreDescriptionData;
            }

            return description;
        }

        internal void SaveBinary(FileDatabase fdb, string file)
        {
            configFile = file;
            SaveBinary(fdb);
        }

        internal void SaveBinary(FileDatabase fdb)
        {
            if (!canWrite)
                throw new InvalidOperationException("Can't write incomplete description.");
            fdb.WriteSharedObject(BundleFile, FileName, typeMap, this);
            //			BinaryXmlReader.DumpFile (configFile);
        }

        /// <summary>
        /// 验证有效性
        /// </summary>
        /// <returns></returns>
        public StringCollection Verify()
        {
            return Verify(new BundleFileSystemExtension());
        }

        internal StringCollection Verify(BundleFileSystemExtension fs)
        {
            StringCollection errors = new StringCollection();

            if (IsBundle)
            {
                if (OptionalModules.Count > 0)
                    errors.Add("Bundle是不可以有选择模块的");
            }

            if (BundleId.Length == 0 || Version.Length == 0)
            {
                if (ExtensionPoints.Count > 0)
                    errors.Add("Bundle必须有一个ID和版本号");
            }

            MainModule.Verify("", errors);
            OptionalModules.Verify("", errors);
            ExtensionNodeSets.Verify("", errors);
            ExtensionPoints.Verify("", errors);
            ConditionTypes.Verify("", errors);

            foreach (ExtensionNodeSet nset in ExtensionNodeSets)
            {
                if (nset.Id.Length == 0)
                    errors.Add("Attribute 'id' can't be empty for global node sets.");
            }

            string bp = null;
            if (BasePath.Length > 0)
                bp = BasePath;
            else if (sourceBundleFile != null && sourceBundleFile.Length > 0)
                bp = Path.GetDirectoryName(BundleFile);
            else if (configFile != null && configFile.Length > 0)
                bp = Path.GetDirectoryName(configFile);

            if (bp != null)
            {
                foreach (string file in AllFiles)
                {
                    string asmFile = Path.Combine(bp, file);
                    if (!fs.FileExists(asmFile))
                        errors.Add($"Bundle[{this.BundleId}]引用的文件（" + asmFile + "）找不到");
                }
            }

            if (localizer != null && localizer.GetAttribute("type").Length == 0)
            {
                errors.Add("Location节点的type属性必须要指定");
            }


            if (activator != null && activator.GetAttribute("type").Length == 0)
            {
                errors.Add("Activator节点的type属性必须要指定");
            }

            // Ensure that there are no duplicated properties

            if (properties != null)
            {
                HashSet<string> props = new HashSet<string>();
                foreach (var prop in properties)
                {
                    if (!props.Add(prop.Name + " " + prop.Locale))
                        errors.Add(string.Format("Property {0} specified more than once", prop.Name + (prop.Locale != null ? " (" + prop.Locale + ")" : "")));
                }
            }

            return errors;
        }

        /// <summary>
        /// 设置当前Bundle的扩展点集合和扩展节点组集合所属的Bundle
        /// </summary>
        /// <param name="bundleId"></param>
        internal void SetExtensionsBundleId(string bundleId)
        {
            foreach (ExtensionPoint ep in ExtensionPoints)
                ep.SetExtensionsBundleId(bundleId);

            foreach (ExtensionNodeSet ns in ExtensionNodeSets)
                ns.SetExtensionsBundleId(bundleId);
        }

        internal void UnmergeExternalData(Hashtable addins)
        {
            // Removes extension types and extension sets coming from other add-ins.
            foreach (ExtensionPoint ep in ExtensionPoints)
                ep.UnmergeExternalData(BundleId, addins);

            foreach (ExtensionNodeSet ns in ExtensionNodeSets)
                ns.UnmergeExternalData(BundleId, addins);
        }

        internal void MergeExternalData(BundleDescription other)
        {
            // Removes extension types and extension sets coming from other add-ins.
            foreach (ExtensionPoint ep in other.ExtensionPoints)
            {
                ExtensionPoint tep = ExtensionPoints[ep.Path];
                if (tep != null)
                    tep.MergeWith(BundleId, ep);
            }

            foreach (ExtensionNodeSet ns in other.ExtensionNodeSets)
            {
                ExtensionNodeSet tns = ExtensionNodeSets[ns.Id];
                if (tns != null)
                    tns.MergeWith(BundleId, ns);
            }
        }

        internal bool IsExtensionModel
        {
            get { return RootElement.LocalName == "ExtensionModel"; }
        }

        /// <summary>
        /// 将desc2的信息合并到desc1
        /// </summary>
        /// <param name="desc1"></param>
        /// <param name="desc2"></param>
        /// <returns></returns>
        internal static BundleDescription Merge(BundleDescription desc1, BundleDescription desc2)
        {
            if (!desc2.IsExtensionModel)
            {
                BundleDescription tmp = desc1;
                desc1 = desc2;
                desc2 = tmp;
            }

            ((BundlePropertyCollectionImpl)desc1.Properties).AddRange(desc2.Properties);

            desc1.ExtensionPoints.AddRange(desc2.ExtensionPoints);
            desc1.ExtensionNodeSets.AddRange(desc2.ExtensionNodeSets);
            desc1.ConditionTypes.AddRange(desc2.ConditionTypes);
            desc1.OptionalModules.AddRange(desc2.OptionalModules);

            foreach (string s in desc2.MainModule.Assemblies)
                desc1.MainModule.Assemblies.Add(s);

            foreach (string s in desc2.MainModule.DataFiles)
                desc1.MainModule.DataFiles.Add(s);

            desc1.MainModule.MergeWith(desc2.MainModule);

            return desc1;
        }

        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            TransferCoreProperties(true);
            writer.WriteValue("id", ParseString(id));
            writer.WriteValue("ns", ParseString(ns));
            writer.WriteValue("isroot", isbundle);
            writer.WriteValue("name", ParseString(name));
            writer.WriteValue("version", ParseString(version));
            writer.WriteValue("compatVersion", ParseString(compatVersion));
            writer.WriteValue("hasUserId", hasUserId);
            writer.WriteValue("author", ParseString(author));
            writer.WriteValue("url", ParseString(url));
            writer.WriteValue("copyright", ParseString(copyright));
            writer.WriteValue("description", ParseString(description));
            writer.WriteValue("category", ParseString(category));
            writer.WriteValue("basePath", basePath);
            writer.WriteValue("sourceBundleFile", sourceBundleFile);
            writer.WriteValue("defaultEnabled", defaultEnabled);
            writer.WriteValue("domain", domain);
            writer.WriteValue("startLevel", startLevel);
            writer.WriteValue("MainModule", MainModule);
            writer.WriteValue("OptionalModules", OptionalModules);
            writer.WriteValue("NodeSets", ExtensionNodeSets);
            writer.WriteValue("ExtensionPoints", ExtensionPoints);
            writer.WriteValue("ConditionTypes", ConditionTypes);
            writer.WriteValue("FilesInfo", fileInfo);
            writer.WriteValue("Localizer", localizer);
            writer.WriteValue("Activator", activator);
            writer.WriteValue("flags", (int)flags);
            writer.WriteValue("Properties", properties);

        }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            id = reader.ReadStringValue("id");
            ns = reader.ReadStringValue("ns");
            isbundle = reader.ReadBooleanValue("isroot");
            name = reader.ReadStringValue("name");
            version = reader.ReadStringValue("version");
            compatVersion = reader.ReadStringValue("compatVersion");
            hasUserId = reader.ReadBooleanValue("hasUserId");
            author = reader.ReadStringValue("author");
            url = reader.ReadStringValue("url");
            copyright = reader.ReadStringValue("copyright");
            description = reader.ReadStringValue("description");
            category = reader.ReadStringValue("category");
            basePath = reader.ReadStringValue("basePath");
            sourceBundleFile = reader.ReadStringValue("sourceBundleFile");
            defaultEnabled = reader.ReadBooleanValue("defaultEnabled");
            domain = reader.ReadStringValue("domain");
            startLevel = reader.ReadInt32Value("startLevel");
            mainModule = (ModuleDescription)reader.ReadValue("MainModule");
            optionalModules = (ModuleCollection)reader.ReadValue("OptionalModules", new ModuleCollection(this));
            nodeSets = (ExtensionNodeSetCollection)reader.ReadValue("NodeSets", new ExtensionNodeSetCollection(this));
            extensionPoints = (ExtensionPointCollection)reader.ReadValue("ExtensionPoints", new ExtensionPointCollection(this));
            conditionTypes = (ConditionTypeDescriptionCollection)reader.ReadValue("ConditionTypes", new ConditionTypeDescriptionCollection(this));
            fileInfo = (object[])reader.ReadValue("FilesInfo", null);
            localizer = (ExtensionNodeDescription)reader.ReadValue("Localizer");
            activator = (ExtensionNodeDescription)reader.ReadValue("Activator");
            flags = (BundleFlags)reader.ReadInt32Value("flags");
            properties = (BundlePropertyCollectionImpl)reader.ReadValue("Properties", new BundlePropertyCollectionImpl(this));

            if (mainModule != null)
                mainModule.SetParent(this);
        }
    }
}
