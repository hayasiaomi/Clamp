using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BundleUrlAttribute : Attribute
    {
        /// <summary>
        /// Initializes the attribute
        /// </summary>
        /// <param name="url">
        /// Url of the add-in
        /// </param>
        public BundleUrlAttribute(string url)
        {
            this.Url = url;
        }

        /// <summary>
        /// Url of the add-in
        /// </summary>
        public string Url { get; set; }
    }
}
