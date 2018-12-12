using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// 标识Bundle的属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundlePropertyAttribute : Attribute
    {

        public BundlePropertyAttribute(string name, string value) : this(name, null, value)
        {
        }

        public BundlePropertyAttribute(string name, string locale, string value)
        {
            Name = name;
            Locale = locale;
            Value = value;
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///本地化
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
