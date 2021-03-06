﻿using Clamp.Data.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.Data
{
    /// <summary>
    /// Bundle检测文件夹信息
    /// </summary>
    internal class BundleScanFolderInfo : IBinaryXmlElement
    {
        private static BinaryXmlTypeMap typeMap = new BinaryXmlTypeMap(typeof(BundleScanFolderInfo), typeof(BundleFileInfo));

        private Hashtable files = new Hashtable();
        private string folder;
        private string fileName;
        private string domain;
        private bool sharedFolder = true;

        /// <summary>
        /// 文件夹的路径
        /// </summary>
        public string Folder
        {
            get { return folder; }
        }

        /// <summary>
        /// 是否为共享文件夹，默认为true
        /// </summary>
        public bool SharedFolder
        {
            get
            {
                return sharedFolder;
            }
            set
            {
                sharedFolder = value;
            }
        }

        /// <summary>
        /// 文件夹的名称
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// 文件夹的哉，如果当前的文件夹是一个共享的文件夹那么就为global域
        /// </summary>
        public string Domain
        {
            get
            {
                if (sharedFolder)
                    return BundleDatabase.GlobalDomain;
                else
                    return domain;
            }
            set
            {
                domain = value;
                sharedFolder = true;
            }
        }

        /// <summary>
        /// 文件夹的根本域
        /// </summary>
        public string RootsDomain
        {
            get
            {
                return domain;
            }
            set
            {
                domain = value;
            }
        }


        public BundleScanFolderInfo(string folder)
        {
            this.folder = folder;
        }

        internal BundleScanFolderInfo()
        {

        }
        #region public method

        /// <summary>
        /// 当前文件夹信息写入到本地的数据库
        /// </summary>
        /// <param name="filedb"></param>
        /// <param name="basePath"></param>
        public void Write(FileDatabase filedb, string basePath)
        {
            filedb.WriteSharedObject(basePath, GetDomain(folder), ".data", Path.GetFullPath(folder), fileName, typeMap, this);
        }

        /// <summary>
        /// 把指定的文件设置到当前Bundle文件夹信息类中
        /// </summary>
        /// <param name="file"></param>
        /// <param name="bundleId"></param>
        /// <param name="isBundle"></param>
        /// <param name="time"></param>
        /// <param name="scanError"></param>
        /// <returns></returns>
        public BundleFileInfo SetLastScanTime(string file, string bundleId, bool isBundle, DateTime time, bool scanError)
        {
            BundleFileInfo info = (BundleFileInfo)files[file];

            if (info == null)
            {
                info = new BundleFileInfo();
                info.File = file;
                files[file] = info;
            }

            info.LastScan = time;
            info.BundleId = bundleId;
            info.IsBundle = isBundle;
            info.ScanError = scanError;

            if (bundleId != null)
                info.Domain = GetDomain(isBundle);
            else
                info.Domain = null;

            return info;
        }

        /// <summary>
        /// 获得Bundle文件夹内部的文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public BundleFileInfo GetBundleFileInfo(string file)
        {
            return (BundleFileInfo)files[file];
        }

        /// <summary>
        /// 获得Bundle文件夹不存在文件。还有不在当前域内的文件
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public ArrayList GetMissingBundles(BundleFileSystemExtension fs)
        {
            ArrayList missing = new ArrayList();

            if (!fs.DirectoryExists(folder))
            {
                foreach (BundleFileInfo info in files.Values)
                {
                    if (info.IsNotNullBundleId)
                        missing.Add(info);
                }
                files.Clear();
                return missing;
            }

            ArrayList toDelete = new ArrayList();

            foreach (BundleFileInfo info in files.Values)
            {
                if (!fs.FileExists(info.File))
                {
                    if (info.IsNotNullBundleId)
                        missing.Add(info);

                    toDelete.Add(info.File);
                }
                else if (info.IsNotNullBundleId && info.Domain != GetDomain(info.IsBundle))
                {
                    missing.Add(info);
                }
            }

            foreach (string file in toDelete)
                files.Remove(file);

            return missing;
        }
        #endregion

        #region public static method

        /// <summary>
        /// 读取Bunlde的文件夹信息
        /// </summary>
        /// <param name="filedb"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static BundleScanFolderInfo Read(FileDatabase filedb, string file)
        {
            BundleScanFolderInfo finfo = (BundleScanFolderInfo)filedb.ReadSharedObject(file, typeMap);

            if (finfo != null)
                finfo.fileName = file;
            return finfo;
        }

        /// <summary>
        /// 根据文件夹路径来获得对应的文件夹信息
        /// </summary>
        /// <param name="filedb"></param>
        /// <param name="basePath"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static BundleScanFolderInfo Read(FileDatabase filedb, string basePath, string folderPath)
        {
            string fileName;

            BundleScanFolderInfo finfo = (BundleScanFolderInfo)filedb.ReadSharedObject(basePath, GetDomain(folderPath), ".data", Path.GetFullPath(folderPath), typeMap, out fileName);

            if (finfo != null)
                finfo.fileName = fileName;
            return finfo;
        }

        public static string GetDomain(string path)
        {
            path = Path.GetFullPath(path);

            string s = path.Replace(Path.DirectorySeparatorChar, '_');

            s = s.Replace(Path.AltDirectorySeparatorChar, '_');
            s = s.Replace(Path.VolumeSeparatorChar, '_');
            s = s.Trim('_');

            return s;
        }

        public string GetDomain(bool isRoot)
        {
            if (isRoot)
                return RootsDomain;
            else
                return Domain;
        }
        #endregion


        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            if (files.Count == 0)
            {
                domain = null;
                sharedFolder = true;
            }
            writer.WriteValue("folder", folder);
            writer.WriteValue("files", files);
            writer.WriteValue("domain", domain);
            writer.WriteValue("sharedFolder", sharedFolder);
        }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            folder = reader.ReadStringValue("folder");
            reader.ReadValue("files", files);
            domain = reader.ReadStringValue("domain");
            sharedFolder = reader.ReadBooleanValue("sharedFolder");
        }
    }
}
