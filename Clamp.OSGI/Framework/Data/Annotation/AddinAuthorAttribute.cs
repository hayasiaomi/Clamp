using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleAuthorAttribute : Attribute
    {
        private string name;

        /// <summary>
        /// Initializes the attribute
        /// </summary>
        /// <param name="name">
        /// Name of the author
        /// </param>
        public BundleAuthorAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Author name
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
    }
}
