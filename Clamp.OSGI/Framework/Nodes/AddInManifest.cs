using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Clamp.OSGI.Properties;

namespace Clamp.OSGI.Framework.Nodes
{
    /// <summary>
    /// 插件清单类
    /// </summary>
    public class AddInManifest
    {
        private List<AddInReference> dependencies = new List<AddInReference>();
        private List<AddInReference> conflicts = new List<AddInReference>();
        private Dictionary<string, Version> identities = new Dictionary<string, Version>();
        /// <summary>
        /// 公司ID
        /// </summary>
        public string ArtifactId { private set; get; }

        /// <summary>
        /// 业务组ID
        /// </summary>
        public string GroupId { private set; get; }

        /// <summary>
        /// 插件ID
        /// </summary>
        public string AddInId { private set; get; }

        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version { private set; get; }

        /// <summary>
        /// 发生冲突的插件
        /// </summary>
        public ReadOnlyCollection<AddInReference> Conflicts
        {
            get { return conflicts.AsReadOnly(); }
        }

        /// <summary>
        /// 发生依懒的插件
        /// </summary>
        public ReadOnlyCollection<AddInReference> Dependencies
        {
            get { return dependencies.AsReadOnly(); }
        }

        public void ReadManifestSection(XmlReader reader, string hintPath)
        {
            if (reader.AttributeCount != 0)
            {
                throw new FrameworkException("the manifest attribute of addin is not empty.");
            }
            if (reader.IsEmptyElement)
            {
                throw new FrameworkException("the manifest content of addin is empty");
            }
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "Manifest")
                            return;
                        break;
                    case XmlNodeType.Element:
                        string nodeName = reader.LocalName;
                        switch (nodeName)
                        {
                            case "ArtifactId":
                                this.ArtifactId = this.ReadElementContent(reader, "ArtifactId");
                                break;
                            case "GroupId":
                                this.GroupId = this.ReadElementContent(reader, "GroupId");
                                break;
                            case "AddInId":
                                this.AddInId = this.ReadElementContent(reader, "AddInId");
                                break;
                            case "Version":
                                this.Version = this.ParseVersion(this.ReadElementContent(reader, "Version"), hintPath);
                                break;
                            case "Dependency":
                                dependencies.AddRange(this.ReadDependencyConflictSection(reader, hintPath, "Dependency"));
                                break;
                            case "Conflict":
                                conflicts.AddRange(this.ReadDependencyConflictSection(reader, hintPath, "Conflict"));
                                break;
                            default:
                                throw new FrameworkException("Unknown node in Manifest section:" + nodeName);
                        }
                        break;
                }
            }
        }

        public string ReadElementContent(XmlReader reader, string endElement)
        {
            if (reader.IsEmptyElement)
                return string.Empty;

            string text = string.Empty;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == endElement)
                            return text;
                        break;
                    case XmlNodeType.Text:
                        text = reader.Value;
                        break;
                }
            }

            return text;
        }


        public List<AddInReference> ReadDependencyConflictSection(XmlReader reader, string hintPath, string endElement)
        {
            if (reader.IsEmptyElement)
            {
                throw new FrameworkException(string.Format("the {0} content of addin is empty", endElement));
            }

            List<AddInReference> addInReferences = new List<AddInReference>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == endElement)
                        {
                            return addInReferences;
                        }
                        break;
                    case XmlNodeType.Element:
                        if (reader.LocalName == "AddInReference")
                        {
                            AddInReference addInReference = this.ReadAddInReferenceSection(reader, hintPath, "AddInReference");

                            if (addInReference != null)
                            {
                                addInReferences.Add(addInReference);
                            }
                        }
                        break;
                }
            }

            return addInReferences;
        }

        public AddInReference ReadAddInReferenceSection(XmlReader reader, string hintPath, string endElement)
        {
            if (reader.IsEmptyElement)
            {
                throw new FrameworkException(string.Format("the {0} content of addin is empty", endElement));
            }

            AddInReference addInReference = new AddInReference();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == endElement)
                        {
                            return addInReference;
                        }
                        break;
                    case XmlNodeType.Element:
                        string elementName = reader.LocalName;
                        switch (elementName)
                        {
                            case "ArtifactId":
                                addInReference.ArtifactId = this.ReadElementContent(reader, "ArtifactId");
                                break;
                            case "GroupId":
                                addInReference.GroupId = this.ReadElementContent(reader, "GroupId");
                                break;
                            case "AddInId":
                                addInReference.AddInId = this.ReadElementContent(reader, "AddInId");
                                break;
                            case "Version":
                                addInReference.Version = this.ParseVersion(this.ReadElementContent(reader, "Version"), hintPath);
                                break;
                            default:
                                throw new FrameworkException(string.Format("Unknown node in {0} section:{1}", endElement, elementName));
                        }
                        break;
                }

            }

            return null;
        }

        public bool IsMatch(AddInReference addInReference)
        {
            return this.IsMatch(addInReference.ArtifactId, addInReference.GroupId, addInReference.AddInId, addInReference.Version);
        }

        public bool IsMatch(string artifactId, string groupId, string addInId, Version version)
        {
            return this.ArtifactId == artifactId
                && this.GroupId == groupId
                && this.AddInId == addInId
                && this.Version == version;
        }

        public Version ParseVersion(string version, string hintPath)
        {
            if (version == null || version.Length == 0)
                return new Version(0, 0, 0, 0);

            if (version.StartsWith("@"))
            {
                if (version == "@ShanDianVersion")
                {
                    return Assembly.GetExecutingAssembly().GetName().Version;
                }

                if (hintPath != null)
                {
                    string fileName = Path.Combine(hintPath, version.Substring(1));
                    try
                    {
                        FileVersionInfo info = FileVersionInfo.GetVersionInfo(fileName);
                        return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new FrameworkException("Cannot get version '" + version + "': " + ex.Message);
                    }
                }
                return new Version(0, 0, 0, 0);
            }
            else
            {
                return new Version(version);
            }
        }

    }
}
