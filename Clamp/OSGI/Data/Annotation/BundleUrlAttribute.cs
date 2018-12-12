using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// Bundle的URL注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BundleUrlAttribute : Attribute
    {
       
        public BundleUrlAttribute(string url)
        {
            this.Url = url;
        }

        public string Url { get; set; }
    }
}
