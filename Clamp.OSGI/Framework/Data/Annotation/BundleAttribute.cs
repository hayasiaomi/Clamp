using Clamp.OSGI.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    /// Marks an assembly as being an add-in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : Attribute
    {
        private string id;
        private string version;
        private string ns;
        private string category;
        private bool enabledByDefault = true;
        private BundleFlags flags;
        private string compatVersion;
        private string url;

        /// <summary>
        /// Initializes an add-in marker attribute
        /// </summary>
        public BundleAttribute()
        {
        }

        /// <summary>
        /// Initializes an add-in marker attribute
        /// </summary>
        /// <param name="id">
        /// Identifier of the add-in
        /// </param>
        public BundleAttribute(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// Initializes an add-in marker attribute
        /// </summary>
        /// <param name="id">
        /// Identifier of the add-in
        /// </param>
        /// <param name="version">
        /// Version of the add-in
        /// </param>
        public BundleAttribute(string id, string version)
        {
            this.id = id;
            this.version = version;
        }

        /// <summary>
        /// Identifier of the add-in.
        /// </summary>
        public string Id
        {
            get { return id != null ? id : string.Empty; }
            set { id = value; }
        }

        /// <summary>
        /// Version of the add-in.
        /// </summary>
        public string Version
        {
            get { return version != null ? version : string.Empty; }
            set { version = value; }
        }

        /// <summary>
        /// Version of the add-in with which this add-in is backwards compatible.
        /// </summary>
        public string CompatVersion
        {
            get { return compatVersion != null ? compatVersion : string.Empty; }
            set { compatVersion = value; }
        }

        /// <summary>
        /// Namespace of the add-in
        /// </summary>
        public string Namespace
        {
            get { return ns != null ? ns : string.Empty; }
            set { ns = value; }
        }

        /// <summary>
        /// Category of the add-in
        /// </summary>
        public string Category
        {
            get { return category != null ? category : string.Empty; }
            set { category = value; }
        }

        /// <summary>
        /// Url to a web page with more information about the add-in
        /// </summary>
        public string Url
        {
            get { return url != null ? url : string.Empty; }
            set { url = value; }
        }

        /// <summary>
        /// When set to True, the add-in will be automatically enabled after installing.
        /// It's True by default.
        /// </summary>
        public bool EnabledByDefault
        {
            get { return this.enabledByDefault; }
            set { this.enabledByDefault = value; }
        }

        /// <summary>
        /// Add-in flags
        /// </summary>
        public BundleFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }
    }
}
