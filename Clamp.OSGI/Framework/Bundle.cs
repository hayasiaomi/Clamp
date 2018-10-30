﻿using Clamp.OSGI.Framework.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework
{
    /// <summary>
    /// 插件类
    /// </summary>
    internal class Bundle : IBundle
    {
        private volatile bool dependenciesLoaded;
        private string addInFileName;
        private string activatorClassName;
        private string mistake;
        private AddInProperties properties = new AddInProperties();
        private AddInManifest manifest = new AddInManifest();
        private List<AddInRuntime> runtimes = new List<AddInRuntime>();
        private List<string> bitmapResources = new List<string>();
        private List<string> stringResources = new List<string>();
        private Dictionary<string, AddInFeature> features = new Dictionary<string, AddInFeature>();
        private ClampFramework framework;

        /// <summary>
        /// 
        /// </summary>
        public string Copyright
        {
            get { return properties["copyright"]; }
        }
        /// <summary>
        /// 说明
        /// </summary>
        public string Description
        {
            get { return properties["description"]; }
        }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author
        {
            get { return properties["author"]; }
        }
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name
        {
            get { return properties["name"]; }
        }

        public int StartLevel
        {
            get
            {
                string vStartLevel = properties["startLevel"];

                if (string.IsNullOrWhiteSpace(vStartLevel))
                    return 0;
                return Convert.ToInt32(vStartLevel);
            }
        }

        public ClampFramework Framework
        {
            get { return this.framework; }
        }


        public bool Enabled { set; get; }

        public string Mistake
        {
            get { return this.mistake; }
            internal set
            {
                if (value != null)
                {
                    Enabled = false;
                }

                this.mistake = value;
            }
        }

        public string AddInDirectory
        {
            get { return Path.GetDirectoryName(this.FileName); }
        }

        public string FileName
        {
            get { return addInFileName; }
            set { addInFileName = value; }
        }

        public AddInManifest Manifest
        {
            get { return manifest; }
        }

        public AddInProperties Properties
        {
            set { this.properties = value; }
            get { return this.properties; }
        }

        public string ActivatorClassName
        {
            get
            {
                return this.activatorClassName;
            }
        }

        public List<string> BitmapResources
        {
            get { return bitmapResources; }
            set { bitmapResources = value; }
        }

        public List<string> StringResources
        {
            get { return stringResources; }
            set { stringResources = value; }
        }

        public ReadOnlyCollection<AddInRuntime> Runtimes
        {
            get { return runtimes.AsReadOnly(); }
        }

        public Dictionary<string, AddInFeature> Features
        {
            get { return features; }
        }

        public Guid BundleId { set; get; }

        public Version Version { private set; get; }

        internal Bundle()
        {

        }

        internal Bundle(ClampFramework clampFramework)
        {
            this.framework = clampFramework;
            this.Enabled = true;
        }

        public AddInFeature GetExtensionPath(string pathName)
        {
            if (!features.ContainsKey(pathName))
            {
                return features[pathName] = new AddInFeature(pathName, this);
            }
            return features[pathName];
        }

        public object CreateObject(string className)
        {
            Type t = FindType(className);

            if (t != null)
                return System.Activator.CreateInstance(t);
            else
                return null;
        }

        public Type FindType(string className)
        {
            foreach (AddInRuntime runtime in runtimes)
            {
                if (!runtime.IsHostApplicationAssembly)
                {
                    LoadDependencies();
                }

                Type t = runtime.FindType(className);

                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        public Stream FindResources(string resourceName)
        {
            foreach (AddInRuntime runtime in runtimes)
            {
                if (!runtime.IsHostApplicationAssembly)
                {
                    LoadDependencies();
                }

                Stream stream = runtime.FindResources(resourceName);

                if (stream != null)
                {
                    return stream;
                }
            }
            return null;
        }

        public void LoadRuntimeAssemblies()
        {
            LoadDependencies();

            foreach (AddInRuntime runtime in runtimes)
            {
                if (runtime.IsActive)
                    runtime.Load();
            }
        }

        private void LoadDependencies()
        {
            if (!dependenciesLoaded)
            {
                AssemblyLocator.Init();

                foreach (AddInReference r in manifest.Dependencies)
                {
                    if (r.RequirePreload)
                    {
                        bool found = false;
                        foreach (Bundle addIn in this.framework.AddIns)
                        {
                            if (addIn.Manifest.IsMatch(r))
                            {
                                found = true;

                                addIn.LoadRuntimeAssemblies();
                            }
                        }
                        if (!found)
                        {
                            throw new FrameworkException("Cannot load run-time dependency for " + r.ToString());
                        }
                    }
                }
                dependenciesLoaded = true;
            }
        }



        /// <summary>
        /// 通一个文件来加载插件信息
        /// </summary>
        /// <param name="addInTree"></param>
        /// <param name="fileName"></param>
        /// <param name="nameTable"></param>
        /// <returns></returns>

        public static Bundle Load(ClampFramework framework, string fileName, XmlNameTable nameTable = null)
        {
            try
            {
                using (TextReader textReader = File.OpenText(fileName))
                {
                    Bundle addIn = Load(framework, textReader, Path.GetDirectoryName(fileName), nameTable);
                    addIn.FileName = fileName;
                    return addIn;
                }
            }
            catch (FrameworkException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FrameworkException("Can't load " + fileName, e);
            }
        }

        /// <summary>
        /// 通过一个流来加载插件信息
        /// </summary>
        /// <param name="addInTree"></param>
        /// <param name="textReader"></param>
        /// <param name="hintPath"></param>
        /// <param name="nameTable"></param>
        /// <returns></returns>
        public static Bundle Load(ClampFramework framework, TextReader textReader, string hintPath = null, XmlNameTable nameTable = null)
        {
            if (nameTable == null)
                nameTable = new NameTable();
            try
            {
                Bundle addIn = new Bundle(framework);

                using (XmlTextReader reader = new XmlTextReader(textReader, nameTable))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.LocalName)
                            {
                                case "AddIn":
                                    addIn.Properties = AddInProperties.ReadFromAttributes(reader);
                                    SetupAddIn(reader, addIn, hintPath);
                                    break;
                                default:
                                    throw new FrameworkException("Unknown add-in file.");
                            }
                        }
                    }
                }

                return addIn;
            }
            catch (XmlException ex)
            {
                throw new FrameworkException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 装载插件内容
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="addIn"></param>
        /// <param name="hintPath"></param>
        static void SetupAddIn(XmlReader reader, Bundle addIn, string hintPath)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "StringResources":
                        case "BitmapResources":
                            if (reader.AttributeCount != 1)
                            {
                                throw new FrameworkException("BitmapResources requires ONE attribute.");
                            }

                            string filename = reader.GetAttribute("file");

                            if (reader.LocalName == "BitmapResources")
                            {
                                addIn.BitmapResources.Add(filename);
                            }
                            else
                            {
                                addIn.StringResources.Add(filename);
                            }
                            break;
                        case "Runtime":
                            if (!reader.IsEmptyElement)
                            {
                                addIn.runtimes.AddRange(AddInRuntime.ReadSection(reader, addIn, hintPath));
                            }
                            break;
                        case "Activator":

                            if (reader.AttributeCount != 1)
                            {
                                throw new FrameworkException("Include requires ONE attribute.");
                            }

                            if (!reader.IsEmptyElement)
                            {
                                throw new FrameworkException("Include nodes must be empty!");
                            }

                            AddInProperties addInProperties = AddInProperties.ReadFromAttributes(reader);

                            addIn.activatorClassName = addInProperties["class"];

                            break;
                        case "Feature":
                            if (reader.AttributeCount != 1)
                            {
                                throw new FrameworkException("Import node requires ONE attribute.");
                            }
                            string pathName = reader.GetAttribute(0);
                            AddInFeature addInPath = addIn.GetExtensionPath(pathName);
                            if (!reader.IsEmptyElement)
                            {
                                AddInFeature.SetUp(addInPath, reader, "Feature");
                            }
                            break;
                        case "Manifest":
                            addIn.Manifest.ReadManifestSection(reader, hintPath);
                            break;
                        default:
                            throw new FrameworkException("Unknown root path node:" + reader.LocalName);
                    }
                }
            }
        }

        public virtual void Start()
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }
    }
}