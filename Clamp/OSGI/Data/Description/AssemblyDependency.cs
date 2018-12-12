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
    [XmlType("AssemblyDependency")]
    public class AssemblyDependency : Dependency
    {
        string fullName;
        string package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.AssemblyDependency"/> class.
        /// </summary>
        public AssemblyDependency()
        {
        }

        internal AssemblyDependency(XmlElement elem) : base(elem)
        {
            fullName = elem.GetAttribute("name");
            package = elem.GetAttribute("package");
        }

        internal override void Verify(string location, StringCollection errors)
        {
            VerifyNotEmpty(location + "Dependencies/Assembly/", errors, "name", FullName);
        }

        internal override void SaveXml(XmlElement parent)
        {
            CreateElement(parent, "Assembly");
            Element.SetAttribute("name", FullName);
            Element.SetAttribute("package", Package);
        }

        /// <summary>
        /// Gets or sets the full name of the assembly
        /// </summary>
        /// <value>
        /// The full name of the assembly.
        /// </value>
        public string FullName
        {
            get { return fullName != null ? fullName : string.Empty; }
            set { fullName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the package that provides the assembly.
        /// </summary>
        /// <value>
        /// The name of the package that provides the assembly.
        /// </value>
        public string Package
        {
            get { return package != null ? package : string.Empty; }
            set { package = value; }
        }

        /// <summary>
        /// Display name of the dependency
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get
            {
                if (Package.Length > 0)
                    return FullName + " " + GettextCatalog.GetString("(provided by {0})", Package);
                else
                    return FullName;
            }
        }

        internal override bool CheckInstalled(BundleRegistry registry)
        {
            // TODO
            return true;
        }

        internal override void Write(BinaryXmlWriter writer)
        {
            base.Write(writer);
            writer.WriteValue("fullName", fullName);
            writer.WriteValue("package", package);
        }

        internal override void Read(BinaryXmlReader reader)
        {
            base.Read(reader);
            fullName = reader.ReadStringValue("fullName");
            package = reader.ReadStringValue("package");
        }
    }
}
