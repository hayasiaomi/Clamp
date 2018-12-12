using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// Bundle注解的共用基类
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


        public BundleAttribute()
        {
        }


        public BundleAttribute(string id)
        {
            this.id = id;
        }


        public BundleAttribute(string id, string version)
        {
            this.id = id;
            this.version = version;
        }

        /// <summary>
        /// Bundle的标识ID
        /// </summary>
        public string Id
        {
            get { return id != null ? id : string.Empty; }
            set { id = value; }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get { return version != null ? version : string.Empty; }
            set { version = value; }
        }

        /// <summary>
        /// 兼容版本号
        /// </summary>
        public string CompatVersion
        {
            get { return compatVersion != null ? compatVersion : string.Empty; }
            set { compatVersion = value; }
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace
        {
            get { return ns != null ? ns : string.Empty; }
            set { ns = value; }
        }

        /// <summary>
        /// 种类
        /// </summary>
        public string Category
        {
            get { return category != null ? category : string.Empty; }
            set { category = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Url
        {
            get { return url != null ? url : string.Empty; }
            set { url = value; }
        }

        /// <summary>
        /// 如果设置为true，安装后就可以用了。默认为true
        /// </summary>
        public bool EnabledByDefault
        {
            get { return this.enabledByDefault; }
            set { this.enabledByDefault = value; }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public BundleFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }
    }
}
