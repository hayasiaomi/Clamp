using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    /// <summary>
    /// 引入一个程序集的标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ImportBundleAssemblyAttribute : Attribute
    {
        private string filePath;
        private bool scan = true;

        public ImportBundleAssemblyAttribute(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// 程序集路径
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// 是否要检测
        /// </summary>
        public bool Scan
        {
            get { return this.scan; }
            set { this.scan = value; }
        }
    }
}
