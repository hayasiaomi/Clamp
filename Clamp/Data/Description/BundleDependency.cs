using Clamp.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Clamp.Data.Description
{
    /// <summary>
    /// 表示所依赖的Bundle
    /// </summary>
    [XmlType("BundleReference")]
    public class BundleDependency : Dependency
    {
        string id;
        string version;

        public BundleDependency()
        {
        }

        public BundleDependency(string fullId)
        {
            Bundle.GetIdParts(fullId, out id, out version);
            id = "::" + id;
        }

        public BundleDependency(string id, string version)
        {
            this.id = id;
            this.version = version;
        }

        internal BundleDependency(XmlElement elem) : base(elem)
        {
            id = elem.GetAttribute("id");
            version = elem.GetAttribute("version");
        }

        internal override void Verify(string location, StringCollection errors)
        {
            VerifyNotEmpty(location + "Dependencies/Bundle/", errors, "id", BundleId);
            VerifyNotEmpty(location + "Dependencies/Bundle/", errors, "version", Version);
        }

        internal override void SaveXml(XmlElement parent)
        {
            CreateElement(parent, "Bundle");
            Element.SetAttribute("id", BundleId);
            Element.SetAttribute("version", Version);
        }

       /// <summary>
       /// Bundle的全称ID
       /// </summary>
        public string FullBundleId
        {
            get
            {
                BundleDescription desc = ParentBundleDescription;
                if (desc == null)
                    return Bundle.GetFullId(null, BundleId, Version);
                else
                    return Bundle.GetFullId(desc.Namespace, BundleId, Version);
            }
        }

       /// <summary>
       /// ID
       /// </summary>
        public string BundleId
        {
            get { return id != null ? ParseString(id) : string.Empty; }
            set { id = value; }
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
       /// 名称
       /// </summary>
        public override string Name
        {
            get { return BundleId + " v" + Version; }
        }

        internal override bool CheckInstalled(BundleRegistry registry)
        {
            Bundle[] addins = registry.GetBundles();
            foreach (Bundle addin in addins)
            {
                if (addin.Id == id && addin.SupportsVersion(version))
                {
                    return true;
                }
            }
            return false;
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            base.Write(writer);
            writer.WriteValue("id", ParseString(id));
            writer.WriteValue("version", ParseString(version));
        }

        internal override void Read(BinaryXmlReader reader)
        {
            base.Read(reader);
            id = reader.ReadStringValue("id");
            version = reader.ReadStringValue("version");
        }
    }
}
