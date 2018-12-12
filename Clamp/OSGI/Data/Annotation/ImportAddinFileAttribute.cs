using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// 引入一个文件的标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ImportBundleFileAttribute : Attribute
    {
        private string filePath;

        public ImportBundleFileAttribute(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        ///文件路径
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
    }
}
