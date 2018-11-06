using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    internal class BundleDependency : Dependency
    {
        private string id;
        private string version;

        public BundleDependency()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.AddinDependency"/> class.
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
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.AddinDependency"/> class.
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

        /// <summary>
        /// Gets the full addin identifier.
        /// </summary>
        /// <value>
        /// The full addin identifier.
        /// </value>
        /// <remarks>
        /// Includes namespace and version number. For example: MonoDevelop.TextEditor,1.0
        /// </remarks>
        public string FullAddinId
        {
            get
            {
                BundleDescription desc = ParentAddinDescription;
                if (desc == null)
                    return Bundle.GetFullId(null, AddinId, Version);
                else
                    return Bundle.GetFullId(desc.Namespace, AddinId, Version);
            }
        }

        /// <summary>
        /// Gets or sets the addin identifier.
        /// </summary>
        /// <value>
        /// The addin identifier.
        /// </value>
        public string AddinId
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
            get { return AddinId + " v" + Version; }
        }

        internal override bool CheckInstalled(BundleRegistry registry)
        {
            Bundle[] bundles = registry.GetAddins();
            foreach (Bundle bundle in bundles)
            {
                if (bundle.Id == id && bundle.SupportsVersion(version))
                {
                    return true;
                }
            }
            return false;
        }
       
    }
}
