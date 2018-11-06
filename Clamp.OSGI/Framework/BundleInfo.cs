using Clamp.OSGI.Framework.Data;
using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
   /// <summary>
   /// Bundle的信息
   /// </summary>
    internal class BundleInfo
    {
       private string id = "";
       private string namspace = "";
       private  string name = "";
       private string version = "";
       private string baseVersion = "";
       private string author = "";
       private string copyright = "";
       private string url = "";
       private string description = "";
       private string category = "";
       private bool defaultEnabled = true;
       private bool isroot;
       private DependencyCollection dependencies;
       private DependencyCollection optionalDependencies;
       private AddinPropertyCollection properties;

        private BundleInfo()
        {
            dependencies = new DependencyCollection();
            optionalDependencies = new DependencyCollection();
        }

        public string Id
        {
            get { return Bundle.GetFullId(namspace, id, version); }
        }

        public string LocalId
        {
            get { return id; }
            set { id = value; }
        }

        public string Namespace
        {
            get { return namspace; }
            set { namspace = value; }
        }

        public bool IsRoot
        {
            get { return isroot; }
            set { isroot = value; }
        }

        public string Name
        {
            get
            {
                string s = Properties.GetPropertyValue("Name");
                if (s.Length > 0)
                    return s;
                if (name != null && name.Length > 0)
                    return name;
                string sid = id;
                if (sid.StartsWith("__"))
                    sid = sid.Substring(2);
                return Bundle.GetFullId(namspace, sid, null);
            }
            set { name = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public string BaseVersion
        {
            get { return baseVersion; }
            set { baseVersion = value; }
        }

        public string Author
        {
            get
            {
                string s = Properties.GetPropertyValue("Author");
                if (s.Length > 0)
                    return s;
                return author;
            }
            set { author = value; }
        }

        public string Copyright
        {
            get
            {
                string s = Properties.GetPropertyValue("Copyright");
                if (s.Length > 0)
                    return s;
                return copyright;
            }
            set { copyright = value; }
        }

        public string Url
        {
            get
            {
                string s = Properties.GetPropertyValue("Url");
                if (s.Length > 0)
                    return s;
                return url;
            }
            set { url = value; }
        }

        public string Description
        {
            get
            {
                string s = Properties.GetPropertyValue("Description");
                if (s.Length > 0)
                    return s;
                return description;
            }
            set { description = value; }
        }

        public string Category
        {
            get
            {
                string s = Properties.GetPropertyValue("Category");
                if (s.Length > 0)
                    return s;
                return category;
            }
            set { category = value; }
        }

        public bool EnabledByDefault
        {
            get { return defaultEnabled; }
            set { defaultEnabled = value; }
        }

        public DependencyCollection Dependencies
        {
            get { return dependencies; }
        }

        public DependencyCollection OptionalDependencies
        {
            get { return optionalDependencies; }
        }

        public AddinPropertyCollection Properties
        {
            get { return properties; }
        }

        internal static BundleInfo ReadFromDescription(BundleDescription description)
        {
            BundleInfo info = new BundleInfo();
            info.id = description.LocalId;
            info.namspace = description.Namespace;
            info.name = description.Name;
            info.version = description.Version;
            info.author = description.Author;
            info.copyright = description.Copyright;
            info.url = description.Url;
            info.description = description.Description;
            info.category = description.Category;
            info.baseVersion = description.CompatVersion;
            info.isroot = description.IsRoot;
            info.defaultEnabled = description.EnabledByDefault;

            foreach (Dependency dep in description.MainModule.Dependencies)
                info.Dependencies.Add(dep);

            foreach (ModuleDescription mod in description.OptionalModules)
            {
                foreach (Dependency dep in mod.Dependencies)
                    info.OptionalDependencies.Add(dep);
            }
            info.properties = description.Properties;

            return info;
        }

        public bool SupportsVersion(string version)
        {
            if (Bundle.CompareVersions(Version, version) > 0)
                return false;
            if (baseVersion == "")
                return true;
            return Bundle.CompareVersions(BaseVersion, version) >= 0;
        }

        public int CompareVersionTo(BundleInfo other)
        {
            return Bundle.CompareVersions(this.version, other.Version);
        }
    }
}
