using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ImportBundleFileAttribute : Attribute
    {
        private string filePath;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="filePath">
        /// Path to the file. Must be relative to the assembly declaring this attribute.
        /// </param>
        public ImportBundleFileAttribute(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Path to the file. Must be relative to the assembly declaring this attribute.
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
    }
}
