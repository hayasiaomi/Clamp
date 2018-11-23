using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    /// Bunlde的作者注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleAuthorAttribute : Attribute
    {
        private string name;

        public BundleAuthorAttribute(string name)
        {
            this.name = name;
        }
        /// <summary>
        /// 作者的名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
