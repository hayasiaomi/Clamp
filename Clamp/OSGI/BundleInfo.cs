using Clamp.OSGI.Data;
using Clamp.OSGI.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    /// <summary>
    /// Bundle的信息
    /// </summary>
    internal class BundleInfo
    {
        private string id = "";
        private string namspace = "";
        private string name = "";
        private string version = "";
        private string baseVersion = "";
        private string author = "";
        private string copyright = "";
        private string url = "";
        private string description = "";
        private string category = "";
        private bool defaultEnabled = true;
        private bool isBundle;
        private int startLevel = 1;
        private DependencyCollection dependencies;
        private DependencyCollection optionalDependencies;
        private BundlePropertyCollection properties;

        private BundleInfo()
        {
            dependencies = new DependencyCollection();
            optionalDependencies = new DependencyCollection();
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id
        {
            get { return Bundle.GetFullId(namspace, id, version); }
        }

        public string LocalId
        {
            get { return id; }
            set { id = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Namespace
        {
            get { return namspace; }
            set { namspace = value; }
        }
        /// <summary>
        /// 是否是一个Bundle
        /// </summary>
        public bool IsBundle
        {
            get { return isBundle; }
            set { isBundle = value; }
        }

        /// <summary>
        /// 名称
        /// </summary>
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
        /// <summary>
        /// 当前版本号
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// 最基本的版本号
        /// </summary>
        public string BaseVersion
        {
            get { return baseVersion; }
            set { baseVersion = value; }
        }

        /// <summary>
        /// 作者
        /// </summary>
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
        /// <summary>
        /// 版权
        /// </summary>
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

        /// <summary>
        /// 说明
        /// </summary>
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



        /// <summary>
        /// 种类
        /// </summary>
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

        /// <summary>
        /// 默认是否可用
        /// </summary>
        public bool EnabledByDefault
        {
            get { return defaultEnabled; }
            set { defaultEnabled = value; }
        }

        /// <summary>
        /// 启动等级
        /// </summary>
        public int StartLevel
        {
            get { return startLevel; }
            set { startLevel = value; }
        }


        /// <summary>
        /// 当前依赖项
        /// </summary>
        public DependencyCollection Dependencies
        {
            get { return dependencies; }
        }

        public DependencyCollection OptionalDependencies
        {
            get { return optionalDependencies; }
        }

        public BundlePropertyCollection Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// 从Bundle的详细类中获得Bundle的信息
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
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
            info.isBundle = description.IsBundle;
            info.defaultEnabled = description.EnabledByDefault;
            info.startLevel = description.StartLevel;

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

        /// <summary>
        /// 支持的版本号必须是在Version和BaseVersion之间，如果BaseVersion为空的时候，只要小于Version;
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool SupportsVersion(string version)
        {
            if (Bundle.CompareVersions(Version, version) > 0)
                return false;

            if (baseVersion == "")
                return true;

            return Bundle.CompareVersions(BaseVersion, version) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareVersionTo(BundleInfo other)
        {
            return Bundle.CompareVersions(this.version, other.Version);
        }
    }
}
