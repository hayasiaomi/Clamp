using Clamp.OSGI.Framework.Data;
using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework
{
    /// <summary>
    /// Bundle的注册者
    /// </summary>
    internal class BundleRegistry : IDisposable
    {
        private List<string> bundleDirs;
        private BundleDatabase database;
        private string basePath;
        private string currentDomain;
        private string bundlesDir;
        private string databaseDir;

        /// <summary>
        /// 获得默认的Bundle的文件夹
        /// </summary>
        public string DefaultBundlesFolder
        {
            get { return bundlesDir; }
        }

        /// <summary>
        /// 根路径
        /// </summary>
        public string BasePath
        {
            get { return basePath; }
        }

        /// <summary>
        /// Bundle的存在存放路径，即是数据库的路径
        /// </summary>
        internal string BundleCachePath
        {
            get { return databaseDir; }
        }

        /// <summary>
        /// 当前是否为未知的域
        /// </summary>
        internal bool UnknownDomain
        {
            get { return currentDomain == BundleDatabase.UnknownDomain; }
        }


        /// <summary>
        /// 当前的哉
        /// </summary>
        internal string CurrentDomain
        {
            get { return currentDomain; }
        }

        internal List<string> GlobalBundleDirectories
        {
            get { return bundleDirs; }
        }

        public BundleRegistry(string basePath) : this(null, basePath, null, null)
        {
        }

        public BundleRegistry(string basePath, string bundlesDir) : this(null, basePath, bundlesDir, null)
        {
        }

        public BundleRegistry(string basePath, string bundlesDir, string databaseDir) : this(null, basePath, bundlesDir, databaseDir)
        {
        }

        public BundleRegistry(ClampBundle clampBundle, string basePath, string bundlesDir, string databaseDir)
        {
            this.basePath = Path.GetFullPath(basePath);

            if (bundlesDir != null)
            {
                if (Path.IsPathRooted(bundlesDir))
                    this.bundlesDir = Path.GetFullPath(bundlesDir);
                else
                    this.bundlesDir = Path.GetFullPath(Path.Combine(this.basePath, bundlesDir));
            }
            else
                this.bundlesDir = Path.Combine(this.basePath, "bundles");

            if (databaseDir != null)
            {
                if (Path.IsPathRooted(databaseDir))
                    this.databaseDir = Path.GetFullPath(databaseDir);
                else
                    this.databaseDir = Path.GetFullPath(Path.Combine(this.basePath, databaseDir));
            }
            else
                this.databaseDir = Path.GetFullPath(this.basePath);

            bundleDirs = new List<string>();
            bundleDirs.Add(DefaultBundlesFolder);

            database = new BundleDatabase(clampBundle, this);

            if (this.basePath != null && this.basePath.Length > 0)
            {
                currentDomain = database.GetFolderDomain(this.basePath);
            }
            else
                currentDomain = BundleDatabase.GlobalDomain;
        }


        #region internal method

        /// <summary>
        /// 创建当前住宿的数据文件，如果新建成功就返回true，否则是false
        /// </summary>
        /// <param name="hostFile"></param>
        /// <returns></returns>
        internal bool CreateHostBundlesFile(string hostFile)
        {
            hostFile = Path.GetFullPath(hostFile);

            string baseName = Path.GetFileNameWithoutExtension(hostFile);

            if (!Directory.Exists(database.HostsPath))
                Directory.CreateDirectory(database.HostsPath);

            //检测一下当前的hostFile是否存在 如果存在就不需要在创建了。如果没有存在的话，就要新建一个 并返回true
            foreach (string s in Directory.GetFiles(database.HostsPath, baseName + "*.bundles"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        XmlTextReader tr = new XmlTextReader(sr);
                        tr.MoveToContent();
                        string host = tr.GetAttribute("host-reference");
                        if (host == hostFile)
                            return false;
                    }
                }
                catch
                {
                }
            }

            string file = Path.Combine(database.HostsPath, baseName) + ".bundles";

            int n = 1;

            while (File.Exists(file))
            {
                file = Path.Combine(database.HostsPath, baseName) + "_" + n + ".bundles";
                n++;
            }

            using (StreamWriter sw = new StreamWriter(file))
            {
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartElement("Bundles");
                tw.WriteAttributeString("host-reference", hostFile);
                tw.WriteStartElement("Directory");
                tw.WriteAttributeString("shared", "false");
                tw.WriteString(Path.GetDirectoryName(hostFile));
                tw.WriteEndElement();
                tw.Close();
            }

            return true;
        }

        internal bool BundleDependsOn(string id1, string id2)
        {
            return database.BundleDependsOn(currentDomain, id1, id2);
        }

        internal void ParseBundle(string file, string outFile)
        {
            database.ParseBundle(currentDomain, file, outFile, true);
        }

        /// <summary>
        /// 通知重新获得当前最新的域
        /// </summary>
        internal void NotifyDatabaseUpdated()
        {
            currentDomain = database.GetFolderDomain(this.basePath);
        }
        internal void CopyExtensionsFrom(BundleRegistry other)
        {
            database.CopyExtensions(other.database);
        }

        /// <summary>
        /// 根据主程序集获得对应的住宿Bundle
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal Bundle GetBundleForHostAssembly(string filePath)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return null;

            return database.GetBundleForHostAssembly(currentDomain, filePath);
        }
        #endregion

        #region  public method

        /// <summary>
        /// 根据ID判断Bundle是可用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsBundleEnabled(string id)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return false;
            return database.IsBundleEnabled(currentDomain, id);
        }

        /// <summary>
        /// 更新bundle注册表
        /// </summary>
        public void Update()
        {
            database.Update(currentDomain);
        }

        public Bundle[] GetBundles()
        {
            return GetModules(BundleSearchFlags.IncludeBundles);
        }

        public Bundle GetBundle(string id)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return null;
            Bundle ad = database.GetInstalledBundle(currentDomain, id);
            if (ad != null && IsRegisteredForUninstall(ad.Id))
                return null;
            return ad;
        }

        public bool IsRegisteredForUninstall(string addinId)
        {
            return database.IsRegisteredForUninstall(currentDomain, addinId);
        }

        public Bundle[] GetModules(BundleSearchFlags flags)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return new Bundle[0];

            BundleSearchFlagsInternal f = (BundleSearchFlagsInternal)(int)flags;

            return database.GetInstalledBundles(currentDomain, f | BundleSearchFlagsInternal.ExcludePendingUninstall).ToArray();
        }

        public void Dispose()
        {
            database.Shutdown();
        }

        #endregion



        #region internal static method

        /// <summary>
        /// 检测指定的文件夹
        /// </summary>
        /// <param name="folderToScan"></param>
        /// <param name="filesToIgnore"></param>
        internal void ScanFolders(string folderToScan, List<string> filesToIgnore)
        {
            database.ScanFolders(currentDomain, folderToScan, filesToIgnore);
        }

        internal static string GlobalRegistryPath
        {
            get
            {
                string customDir = Environment.GetEnvironmentVariable("CLAMP_BUNDLES_GLOBAL_REGISTRY");
                if (customDir != null && customDir.Length > 0)
                    return Path.GetFullPath(customDir);

                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, "clamp.bundles");
                return Path.GetFullPath(path);
            }

        }

        internal static BundleRegistry GetGlobalRegistry(ClampBundle engine, string startupDirectory)
        {
            BundleRegistry reg = new BundleRegistry(engine, GlobalRegistryPath, null, null);
            string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
            reg.GlobalBundleDirectories.Add(Path.Combine(baseDir, "clamp.bundles"));
            return reg;
        }

        #endregion

        #region private method





        #endregion


    }
}
