using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// 标识Bundle的说明
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleDescriptionAttribute : Attribute
    {
        public BundleDescriptionAttribute(string description)
        {
            Description = description;
        }

        public BundleDescriptionAttribute(string description, string locale)
        {
            Description = description;
            Locale = locale;
        }

        /// <summary>
        /// Bundle的说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 本地化 (列如： 'en-US', or 'en')
        /// </summary>
        public string Locale { get; set; }
    }
}
