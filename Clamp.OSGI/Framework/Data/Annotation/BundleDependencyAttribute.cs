using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    /// 标识Bundle当前的依赖
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleDependencyAttribute : Attribute
    {
        public BundleDependencyAttribute(string id, string version)
        {
            this.Id = id;
            this.Version = version;
        }
        /// <summary>
        /// Bundle的ID
        /// </summary>
        public string Id { private set; get; }
        /// <summary>
        /// Bundle的版本号
        /// </summary>
        public string Version { private set; get; }

    }
}
