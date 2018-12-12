using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleModuleAttribute : Attribute
    {
        private string assemblyFile;

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        /// <param name="assemblyFile">
        /// Relative path to the assembly that implements the optional module
        /// </param>
        public BundleModuleAttribute(string assemblyFile)
        {
            this.assemblyFile = assemblyFile;
        }

        /// <summary>
        /// Relative path to the assembly that implements the optional module
        /// </summary>
        public string AssemblyFile
        {
            get { return this.assemblyFile ?? string.Empty; }
            set { this.assemblyFile = value; }
        }
    }
}
