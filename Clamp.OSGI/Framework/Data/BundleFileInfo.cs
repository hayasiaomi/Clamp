using Clamp.OSGI.Framework.Data.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    /// <summary>
    /// Bundle文件信息
    /// </summary>
    internal class BundleFileInfo : IBinaryXmlElement
    {
        private string file;
        private string fileName;

        /// <summary>
        /// 文件
        /// </summary>
        public string File
        {
            get
            {
                return file;
            }
            set
            {
                file = value;
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.fileName))
                {
                    if (!string.IsNullOrWhiteSpace(this.file))
                    {
                        return Path.GetFileName(this.file);
                    }
                }
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }
        /// <summary>
        /// 最后访问的时间
        /// </summary>
        public DateTime LastScan { set; get; }
        /// <summary>
        /// 对应的Bundle的Id
        /// </summary>
        public string BundleId { set; get; }
        /// <summary>
        /// 是否为根文件
        /// </summary>
        public bool IsRoot { set; get; }
        public bool ScanError { set; get; }
        /// <summary>
        /// 对应的域
        /// </summary>
        public string Domain { set; get; }
        /// <summary>
        /// 需要忽略的文件
        /// </summary>
        public StringCollection IgnorePaths { set; get; }

        /// <summary>
        /// 是否为一个Bundle
        /// </summary>
        public bool IsBundle
        {
            get { return BundleId != null && BundleId.Length != 0; }
        }

        public void AddPathToIgnore(string path)
        {
            if (IgnorePaths == null)
                IgnorePaths = new StringCollection();
            IgnorePaths.Add(path);
        }

        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("File", this.File);
            writer.WriteValue("FileName", this.FileName);
            writer.WriteValue("LastScan", this.LastScan);
            writer.WriteValue("BundleId", this.BundleId);
            writer.WriteValue("IsRoot", this.IsRoot);
            writer.WriteValue("ScanError", this.ScanError);
            writer.WriteValue("Domain", this.Domain);
            if (this.IgnorePaths != null && this.IgnorePaths.Count > 0)
                writer.WriteValue("IgnorePaths", this.IgnorePaths);
        }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            this.File = reader.ReadStringValue("File");
            this.FileName = reader.ReadStringValue("FileName");
            this.LastScan = reader.ReadDateTimeValue("LastScan");
            this.BundleId = reader.ReadStringValue("BundleId");
            this.IsRoot = reader.ReadBooleanValue("IsRoot");
            this.ScanError = reader.ReadBooleanValue("ScanError");
            this.Domain = reader.ReadStringValue("Domain");
            this.IgnorePaths = (StringCollection)reader.ReadValue("IgnorePaths", new StringCollection());
        }
    }
}
