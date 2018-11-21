using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BundleCategoryAttribute : Attribute
    {
        /// <summary>
        /// Initializes the attribute
        /// </summary>
        /// <param name="category">
        /// The category to which the add-in belongs
        /// </param>
        public BundleCategoryAttribute(string category)
        {
            this.Category = category;
        }

        /// <summary>
        /// The category to which the add-in belongs
        /// </summary>
        public string Category { get; set; }
    }
}
