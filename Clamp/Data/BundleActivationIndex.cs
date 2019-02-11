using Clamp.Data.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.Data
{
    /// <summary>
    /// Bundle住宿索引类
    /// </summary>
    class BundleActivationIndex : IBinaryXmlElement
    {
        static BinaryXmlTypeMap typeMap = new BinaryXmlTypeMap(typeof(BundleActivationIndex));

        private Hashtable index = new Hashtable();

        public ICollection Identities { get { return index.Keys; } }

        /// <summary>
        /// 注册住宿程序集相关的Bundle信息
        /// </summary>
        /// <param name="assemblyLocation"></param>
        /// <param name="bundleId"></param>
        /// <param name="bundleLocation"></param>
        /// <param name="domain"></param>
        public void RegisterAssembly(string assemblyLocation, string bundleId, string bundleLocation, string domain)
        {
            assemblyLocation = NormalizeFileName(assemblyLocation);
            index[Path.GetFullPath(assemblyLocation)] = bundleId + " " + bundleLocation + " " + domain;
        }

        /// <summary>
        /// 获得住宿程序集对应的Bundle的相关信息，成功为true，否则为false
        /// </summary>
        /// <param name="assemblyLocation"></param>
        /// <param name="bundleId"></param>
        /// <param name="bundleLocation"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool GetBundleForAssembly(string assemblyLocation, out string bundleId, out string bundleLocation, out string domain)
        {
            assemblyLocation = NormalizeFileName(assemblyLocation);

            string s = index[Path.GetFullPath(assemblyLocation)] as string;

            if (s == null)
            {
                bundleId = null;
                bundleLocation = null;
                domain = null;
                return false;
            }
            else
            {
                int i = s.IndexOf(' ');
                int j = s.LastIndexOf(' ');
                bundleId = s.Substring(0, i);
                bundleLocation = s.Substring(i + 1, j - i - 1);
                domain = s.Substring(j + 1);
                return true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleId"></param>
        /// <param name="bundleLocation"></param>
        public void RemoveHostData(string bundleId, string bundleLocation)
        {
            string loc = bundleId + " " + Path.GetFullPath(bundleLocation) + " ";

            ArrayList todelete = new ArrayList();

            foreach (DictionaryEntry e in index)
            {
                if (((string)e.Value).StartsWith(loc))
                    todelete.Add(e.Key);
            }

            foreach (string s in todelete)
                index.Remove(s);
        }
        /// <summary>
        /// 读取Bundle住宿索引类
        /// </summary>
        /// <param name="fileDatabase"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static BundleActivationIndex Read(FileDatabase fileDatabase, string file)
        {
            return (BundleActivationIndex)fileDatabase.ReadObject(file, typeMap);
        }

        public void Write(FileDatabase fileDatabase, string file)
        {
            fileDatabase.WriteObject(file, this, typeMap);
        }

        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("index", index);
        }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            reader.ReadValue("index", index);
        }

        /// <summary>
        /// 文件名正常化。即是小写
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string NormalizeFileName(string name)
        {
            return name.ToLower();
        }
    }
}
