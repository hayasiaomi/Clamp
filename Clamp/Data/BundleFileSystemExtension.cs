using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.Data
{
    internal class BundleFileSystemExtension
    {
        private IAssemblyReflector reflector;

        /// <summary>
        /// Bundle开始检测
        /// </summary>
        public virtual void ScanStarted()
        {
        }

        /// <summary>
        /// Bundle结束检测
        /// </summary>
        public virtual void ScanFinished()
        {
        }
        /// <summary>
        /// 判断路径是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// 获得目录下的所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }


        /// <summary>
        /// 获得目录下的所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> GetFiles(string path, string searchOption)
        {
            return Directory.GetFiles(path, searchOption, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// 获得上一次写入的时间
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public virtual DateTime GetLastWriteTime(string filePath)
        {
            return File.GetLastWriteTime(filePath);
        }

        public virtual System.IO.StreamReader OpenTextFile(string path)
        {
            return new StreamReader(path);
        }

        public virtual System.Collections.Generic.IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public virtual System.IO.Stream OpenFile(string path)
        {
            return File.OpenRead(path);
        }

        public virtual bool RequiresIsolation
        {
            get { return true; }
        }

        public virtual IAssemblyReflector GetReflectorForFile(IAssemblyLocator locator, string path)
        {
            if (reflector != null)
                return reflector;

            reflector = new DefaultAssemblyReflector();

            reflector.Initialize(locator);

            return reflector;
        }

    }
}
