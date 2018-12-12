using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// 标识Bundle的名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleNameAttribute : Attribute
    {
        public BundleNameAttribute(string name)
        {
            Name = name;
        }

        public BundleNameAttribute(string name, string locale)
        {
            Name = name;
            Locale = locale;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 本地化 (列如, 'en-US', or 'en')
        /// </summary>
        public string Locale { get; set; }
    }
}
