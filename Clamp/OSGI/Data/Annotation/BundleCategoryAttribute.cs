using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// 标识Bundle的种类
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BundleCategoryAttribute : Attribute
    {
        public BundleCategoryAttribute(string category)
        {
            this.Category = category;
        }

        /// <summary>
        /// 种类名称
        /// </summary>
        public string Category { get; set; }
    }
}
