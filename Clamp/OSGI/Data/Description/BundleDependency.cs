using Clamp.OSGI.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Clamp.OSGI.Data.Description
{
    [XmlType("BundleReference")]
    public class BundleDependency : Dependency
    {
        string id;
        string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.BundleDependency"/> class.
        /// </summary>
        public BundleDependency()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.BundleDependency"/> class.
        /// </summary>
        /// <param name='fullId'>
        /// Full identifier of the add-in (includes version)
        /// </param>
        public BundleDependency(string fullId)
        {
            Bundle.GetIdParts(fullId, out id, out version);
            id = "::" + id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.BundleDependency"/> class.
        /// </summary>
        /// <param name='id'>
        /// Identifier of the add-in.
        /// </param>
        /// <param name='version'>
        /// Version of the add-in.
        /// </param>
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
        /// Gets the full addin identifier.
        /// </summary>
        /// <value>
        /// The full addin identifier.
        /// </value>
        /// <remarks>
        /// Includes namespace and version number. For example: MonoDevelop.TextEditor,1.0
        /// </remarks>
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
        /// Gets or sets the addin identifier.
        /// </summary>
        /// <value>
        /// The addin identifier.
        /// </value>
        public string BundleId
        {
            get { return id != null ? ParseString(id) : string.Empty; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version
        {
            get { return version != null ? ParseString(version) : string.Empty; }
            set { version = value; }
        }

        /// <summary>
        /// Display name of the dependency.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
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
