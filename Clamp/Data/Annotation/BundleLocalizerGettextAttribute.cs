using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    /// <summary>
    /// Bundle的本地化注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleLocalizerGettextAttribute : Attribute
    {
        private string catalog;
        private string location;

        public BundleLocalizerGettextAttribute()
        {
        }

        public BundleLocalizerGettextAttribute(string catalog)
        {
            this.catalog = catalog;
        }

        public BundleLocalizerGettextAttribute(string catalog, string location)
        {
            this.catalog = catalog;
            this.location = location;
        }

        /// <summary>
        /// Name of the catalog which contains the strings.
        /// </summary>
        public string Catalog
        {
            get { return this.catalog; }
            set { this.catalog = value; }
        }

        /// <summary>
        /// Relative path to the location of the catalog. This path must be relative to the add-in location.
        /// </summary>
        /// <remarks>
        /// When not specified, the default value of this property is 'locale'.
        /// The location path must contain a directory structure like this:
        /// 
        /// {language-id}/LC_MESSAGES/{Catalog}.mo
        /// 
        /// For example, the catalog for spanish strings would be located at:
        /// 
        /// locale/es/LC_MESSAGES/some-addin.mo
        /// </remarks>
        public string Location
        {
            get { return this.location; }
            set { this.location = value; }
        }
    }
}
