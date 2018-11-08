using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ImportBundleAssemblyAttribute : Attribute
    {
        private string filePath;
        private bool scan = true;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="filePath">
        /// Path to the assembly. Must be relative to the assembly declaring this attribute.
        /// </param>
        public ImportBundleAssemblyAttribute(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Path to the assembly. Must be relative to the assembly declaring this attribute.
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// When set to true (the default), the included assembly will be scanned
        /// looking for extension point declarations.
        /// </summary>
        public bool Scan
        {
            get { return this.scan; }
            set { this.scan = value; }
        }
    }
}
