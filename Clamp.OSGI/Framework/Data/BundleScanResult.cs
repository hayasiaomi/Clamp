using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    /// <summary>
    /// Bundle检测结果类
    /// </summary>
    internal class BundleScanResult : MarshalByRefObject, IAssemblyLocator
    {
        private ArrayList filesToScan = new ArrayList();
        private Hashtable visitedFolders = new Hashtable();
        private Hashtable assemblyLocations = new Hashtable();
        private Hashtable assemblyLocationsByFullName = new Hashtable();
        private Hashtable filesToIgnore;
        private bool regenerateRelationData;

        /// <summary>
        /// 是否重新执行所有数据
        /// </summary>
        public bool RegenerateAllData { set; get; }
        /// <summary>
        /// 是否只是检测，而不带更新
        /// </summary>
        public bool CheckOnly { set; get; }
        /// <summary>
        /// 当前对应的域
        /// </summary>
        public string Domain { set; get; }
        /// <summary>
        /// 是否发生变化
        /// </summary>
        public bool ChangesFound { set; get; }

        public bool LocateAssembliesOnly { set; get; }

        /// <summary>
        /// 是否重新执行相关的数据
        /// </summary>
        public bool RegenerateRelationData
        {
            get { return this.regenerateRelationData; }
            set
            {
                this.regenerateRelationData = value;
                if (value)
                    this.ChangesFound = true;
            }
        }
        /// <summary>
        /// 修改过的文件夹
        /// </summary>
        internal ArrayList ModifiedFolderInfos { set; get; }

        internal List<string> BundlesToUpdateRelations { set; get; }

        internal List<string> RemovedBundles { set; get; }

        internal List<string> BundlesToUpdate { set; get; }

        internal ArrayList FilesWithScanFailure { set; get; }

        /// <summary>
        /// 需要检测的文件集合
        /// </summary>
        internal ArrayList FilesToScan { get { return this.filesToScan; } }

        internal BundleActivationIndex ActivationIndex { set; get; }

        public BundleScanResult()
        {
            this.ModifiedFolderInfos = new ArrayList();
            this.BundlesToUpdateRelations = new List<string>();
            this.RemovedBundles = new List<string>();
            this.BundlesToUpdate = new List<string>();
            this.FilesWithScanFailure = new ArrayList();
        }


        public void RegisterModifiedFolderInfo(BundleScanFolderInfo folderInfo)
        {
            if (!this.ModifiedFolderInfos.Contains(folderInfo))
                this.ModifiedFolderInfos.Add(folderInfo);
        }

        public void AddRemovedBundle(string addinId)
        {
            if (!this.RemovedBundles.Contains(addinId))
                this.RemovedBundles.Add(addinId);
        }

        public void AddFileToWithFailure(string file)
        {
            if (!FilesWithScanFailure.Contains(file))
                FilesWithScanFailure.Add(file);
        }

        public void AddBundleToUpdateRelations(string addinId)
        {
            if (!this.BundlesToUpdateRelations.Contains(addinId))
                this.BundlesToUpdateRelations.Add(addinId);
        }

        public bool VisitFolder(string folder)
        {
            if (visitedFolders.Contains(folder) || IgnorePath(folder))
                return false;
            else
            {
                visitedFolders.Add(folder, folder);
                return true;
            }
        }

        public void AddBundleToUpdate(string bundleId)
        {
            if (!BundlesToUpdate.Contains(bundleId))
                BundlesToUpdate.Add(bundleId);
        }

        /// <summary>
        /// 是否是忽略的文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IgnorePath(string file)
        {
            if (filesToIgnore == null)
                return false;

            string root = Path.GetPathRoot(file);

            while (root != file)
            {
                if (filesToIgnore.Contains(file))
                    return true;

                file = Path.GetDirectoryName(file);
            }

            return false;
        }

        /// <summary>
        /// 增加需要检测的文件到检测文件集合里
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folderInfo"></param>
        public void AddFileToScan(string file, BundleScanFolderInfo folderInfo)
        {
            FileToScan di = new FileToScan();

            di.File = file;
            di.BundleScanFolderInfo = folderInfo;

            this.FilesToScan.Add(di);

            this.RegisterModifiedFolderInfo(folderInfo);
        }


        public void AddPathsToIgnore(IEnumerable paths)
        {
            foreach (string p in paths)
                AddPathToIgnore(p);
        }

        public void AddPathToIgnore(string path)
        {
            if (filesToIgnore == null)
                filesToIgnore = new Hashtable();

            filesToIgnore[path] = path;
        }

        public void AddAssemblyLocation(string file)
        {
            string name = Path.GetFileNameWithoutExtension(file);

            ArrayList list = assemblyLocations[name] as ArrayList;

            if (list == null)
            {
                list = new ArrayList();
                assemblyLocations[name] = list;
            }

            list.Add(file);
        }

        public string GetAssemblyLocation(string fullName)
        {
            string loc = assemblyLocationsByFullName[fullName] as String;

            if (loc != null)
                return loc;

            int i = fullName.IndexOf(',');
            string name = fullName.Substring(0, i);

            if (name == "Clamp.OSGI")
                return GetType().Assembly.Location;

            ArrayList list = assemblyLocations[name] as ArrayList;
            if (list == null)
                return null;

            string lastAsm = null;
            foreach (string file in list.ToArray())
            {
                try
                {
                    list.Remove(file);
                    AssemblyName aname = AssemblyName.GetAssemblyName(file);
                    lastAsm = file;
                    assemblyLocationsByFullName[aname.FullName] = file;
                    if (aname.FullName == fullName)
                        return file;
                }
                catch
                {
                    // Could not get the assembly name. The file either doesn't exist or it is not a valid assembly.
                    // In this case, just ignore it.
                }
            }

            if (lastAsm != null)
            {
                // If an exact version is not found, just take any of them
                return lastAsm;
            }
            return null;
        }


    }

    /// <summary>
    /// 检测文件类
    /// </summary>
    class FileToScan
    {
        public string File;
        public BundleScanFolderInfo BundleScanFolderInfo;
    }
}
