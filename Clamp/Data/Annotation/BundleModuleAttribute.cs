using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    /// <summary>
    /// Bundle对应的模块标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleModuleAttribute : Attribute
    {
        private string assemblyFile;

        public BundleModuleAttribute(string assemblyFile)
        {
            this.assemblyFile = assemblyFile;
        }

        /// <summary>
        /// 模块的文件
        /// </summary>
        public string AssemblyFile
        {
            get { return this.assemblyFile ?? string.Empty; }
            set { this.assemblyFile = value; }
        }
    }
}
