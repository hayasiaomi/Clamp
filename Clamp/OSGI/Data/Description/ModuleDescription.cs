using Clamp.OSGI.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Data.Description
{
    public class ModuleDescription : ObjectDescription
    {
        private StringCollection assemblies;
        private StringCollection dataFiles;
        private StringCollection ignorePaths;
        private DependencyCollection dependencies;
        private ExtensionCollection extensions;

        // Used only at run time
        internal RuntimeBundle RuntimeBundle;

        internal ModuleDescription(XmlElement element)
        {
            this.Element = element;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.ModuleDescription"/> class.
        /// </summary>
        public ModuleDescription()
        {
        }

        internal void MergeWith(ModuleDescription module)
        {
            Dependencies.AddRange(module.Dependencies);
            Extensions.AddRange(module.Extensions);
        }

        /// <summary>
        /// Checks if this module depends on the specified add-in.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there is a dependency.
        /// </returns>
        /// <param name='addinId'>
        /// Identifier of the add-in
        /// </param>
        public bool DependsOnBundle(string addinId)
        {
            BundleDescription desc = Parent as BundleDescription;
            if (desc == null)
                throw new InvalidOperationException();

            foreach (Dependency dep in Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep == null) continue;
                if (Bundle.GetFullId(desc.Namespace, adep.BundleId, adep.Version) == addinId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the list of paths to be ignored by the add-in scanner.
        /// </summary>
        public StringCollection IgnorePaths
        {
            get
            {
                if (ignorePaths == null)
                    ignorePaths = new StringCollection();
                return ignorePaths;
            }
        }

        /// <summary>
        /// Gets all external files
        /// </summary>
        /// <value>
        /// All files.
        /// </value>
        /// <remarks>
        /// External files are data files and assemblies explicitly referenced in the Runtime section of the add-in manifest.
        /// </remarks>
        public StringCollection AllFiles
        {
            get
            {
                StringCollection col = new StringCollection();

                foreach (string s in Assemblies)
                    col.Add(s);

                foreach (string d in DataFiles)
                    col.Add(d);

                return col;
            }
        }

        /// <summary>
        /// Gets the list of external assemblies used by this module.
        /// </summary>
        public StringCollection Assemblies
        {
            get
            {
                if (assemblies == null)
                {
                    if (Element != null)
                        InitCollections();
                    else
                        assemblies = new StringCollection();
                }
                return assemblies;
            }
        }

        /// <summary>
        /// Gets the list of external data files used by this module
        /// </summary>
        public StringCollection DataFiles
        {
            get
            {
                if (dataFiles == null)
                {
                    if (Element != null)
                        InitCollections();
                    else
                        dataFiles = new StringCollection();
                }
                return dataFiles;
            }
        }

        /// <summary>
        /// Gets the dependencies of this module
        /// </summary>
        public DependencyCollection Dependencies
        {
            get
            {
                if (dependencies == null)
                {
                    dependencies = new DependencyCollection(this);
                    if (Element != null)
                    {
                        XmlNodeList elems = Element.SelectNodes("Dependencies/*");

                        foreach (XmlNode node in elems)
                        {
                            XmlElement elem = node as XmlElement;

                            if (elem == null)
                                continue;

                            if (elem.Name == "Bundle")
                            {
                                BundleDependency dep = new BundleDependency(elem);

                                dependencies.Add(dep);
                            }
                            else if (elem.Name == "Assembly")
                            {
                                AssemblyDependency dep = new AssemblyDependency(elem);

                                dependencies.Add(dep);
                            }
                        }
                    }
                }
                return dependencies;
            }
        }

        /// <summary>
        /// Gets the extensions of this module
        /// </summary>
        public ExtensionCollection Extensions
        {
            get
            {
                if (extensions == null)
                {
                    extensions = new ExtensionCollection(this);

                    if (Element != null)
                    {
                        foreach (XmlElement elem in Element.SelectNodes("Extension"))
                            extensions.Add(new Extension(elem));
                    }
                }

                return extensions;
            }
        }

        /// <summary>
        /// 增加扩展节点
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public ExtensionNodeDescription AddExtensionNode(string path, string nodeName)
        {
            ExtensionNodeDescription node = new ExtensionNodeDescription(nodeName);

            GetExtension(path).ExtensionNodes.Add(node);

            return node;
        }

        /// <summary>
        /// 获得指定路径的扩展
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Extension GetExtension(string path)
        {
            foreach (Extension e in Extensions)
            {
                if (e.Path == path)
                    return e;
            }

            Extension ex = new Extension(path);

            Extensions.Add(ex);

            return ex;
        }

        internal override void SaveXml(XmlElement parent)
        {
            CreateElement(parent, "Module");

            if (assemblies != null || dataFiles != null || ignorePaths != null)
            {
                XmlElement runtime = GetRuntimeElement();

                while (runtime.FirstChild != null)
                    runtime.RemoveChild(runtime.FirstChild);

                if (assemblies != null)
                {
                    foreach (string s in assemblies)
                    {
                        XmlElement asm = Element.OwnerDocument.CreateElement("Import");
                        asm.SetAttribute("assembly", s);
                        runtime.AppendChild(asm);
                    }
                }
                if (dataFiles != null)
                {
                    foreach (string s in dataFiles)
                    {
                        XmlElement asm = Element.OwnerDocument.CreateElement("Import");
                        asm.SetAttribute("file", s);
                        runtime.AppendChild(asm);
                    }
                }
                if (ignorePaths != null)
                {
                    foreach (string s in ignorePaths)
                    {
                        XmlElement asm = Element.OwnerDocument.CreateElement("ScanExclude");
                        asm.SetAttribute("path", s);
                        runtime.AppendChild(asm);
                    }
                }
                runtime.AppendChild(Element.OwnerDocument.CreateTextNode("\n"));
            }

            // Save dependency information

            if (dependencies != null)
            {
                XmlElement deps = GetDependenciesElement();
                dependencies.SaveXml(deps);
                deps.AppendChild(Element.OwnerDocument.CreateTextNode("\n"));

                if (extensions != null)
                    extensions.SaveXml(Element);
            }
        }

        /// <summary>
        /// Adds an add-in reference (there is a typo in the method name)
        /// </summary>
        /// <param name='id'>
        /// Identifier of the add-in.
        /// </param>
        /// <param name='version'>
        /// Version of the add-in.
        /// </param>
        public void AddAssemblyReference(string id, string version)
        {
            XmlElement deps = GetDependenciesElement();
            if (deps.SelectSingleNode("Bundle[@id='" + id + "']") != null)
                return;

            XmlElement dep = Element.OwnerDocument.CreateElement("Bundle");
            dep.SetAttribute("id", id);
            dep.SetAttribute("version", version);
            deps.AppendChild(dep);
        }

        XmlElement GetDependenciesElement()
        {
            XmlElement de = Element["Dependencies"];
            if (de != null)
                return de;

            de = Element.OwnerDocument.CreateElement("Dependencies");
            Element.AppendChild(de);
            return de;
        }

        XmlElement GetRuntimeElement()
        {
            XmlElement de = Element["Runtime"];
            if (de != null)
                return de;

            de = Element.OwnerDocument.CreateElement("Runtime");
            Element.AppendChild(de);
            return de;
        }

        void InitCollections()
        {
            dataFiles = new StringCollection();
            assemblies = new StringCollection();

            XmlNodeList elems = Element.SelectNodes("Runtime/*");

            foreach (XmlElement elem in elems)
            {
                if (elem.LocalName == "Import")
                {
                    string asm = elem.GetAttribute("assembly");

                    if (asm.Length > 0)
                    {
                        assemblies.Add(asm);
                    }
                    else
                    {
                        string file = elem.GetAttribute("file");

                        if (file.Length > 0)
                            dataFiles.Add(file);
                    }
                }
                else if (elem.LocalName == "ScanExclude")
                {
                    string path = elem.GetAttribute("path");

                    if (path.Length > 0)
                        IgnorePaths.Add(path);
                }
            }
        }

        internal override void Verify(string location, StringCollection errors)
        {
            Dependencies.Verify(location + "Module/", errors);
            Extensions.Verify(location + "Module/", errors);
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("Assemblies", Assemblies);
            writer.WriteValue("DataFiles", DataFiles);
            writer.WriteValue("Dependencies", Dependencies);
            writer.WriteValue("Extensions", Extensions);
            writer.WriteValue("IgnorePaths", ignorePaths);
        }

        internal override void Read(BinaryXmlReader reader)
        {
            assemblies = (StringCollection)reader.ReadValue("Assemblies", new StringCollection());
            dataFiles = (StringCollection)reader.ReadValue("DataFiles", new StringCollection());
            dependencies = (DependencyCollection)reader.ReadValue("Dependencies", new DependencyCollection(this));
            extensions = (ExtensionCollection)reader.ReadValue("Extensions", new ExtensionCollection(this));
            ignorePaths = (StringCollection)reader.ReadValue("IgnorePaths", new StringCollection());
        }
    }
}
