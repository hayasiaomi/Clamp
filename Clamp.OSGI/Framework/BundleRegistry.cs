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
    internal class BundleRegistry : IDisposable
    {
        private List<string> addinDirs;
        private BundleDatabase database;
        private string basePath;
        private string currentDomain;
        private string addinsDir;
        private string databaseDir;

        public string DefaultBundlesFolder
        {
            get { return addinsDir; }
        }

        public string BasePath
        {
            get { return basePath; }
        }

        internal string BundleCachePath
        {
            get { return databaseDir; }
        }


        internal bool UnknownDomain
        {
            get { return currentDomain == BundleDatabase.UnknownDomain; }
        }

        internal string CurrentDomain
        {
            get { return currentDomain; }
        }

        internal string StartupDirectory
        {
            get
            {
                return this.basePath;
            }
        }

        internal List<string> GlobalBundleDirectories
        {
            get { return addinDirs; }
        }

        public BundleRegistry(string basePath) : this(null, basePath, null, null)
        {
        }

        public BundleRegistry(string basePath, string addinsDir) : this(null, basePath, addinsDir, null)
        {
        }

        public BundleRegistry(string basePath, string addinsDir, string databaseDir) : this(null, basePath, addinsDir, databaseDir)
        {
        }

        public BundleRegistry(ClampBundle clampBundle, string basePath, string addinsDir, string databaseDir)
        {
            this.basePath = Path.GetFullPath(basePath);

            if (addinsDir != null)
            {
                if (Path.IsPathRooted(addinsDir))
                    this.addinsDir = Path.GetFullPath(addinsDir);
                else
                    this.addinsDir = Path.GetFullPath(Path.Combine(this.basePath, addinsDir));
            }
            else
                this.addinsDir = Path.Combine(this.basePath, "bundles");

            if (databaseDir != null)
            {
                if (Path.IsPathRooted(databaseDir))
                    this.databaseDir = Path.GetFullPath(databaseDir);
                else
                    this.databaseDir = Path.GetFullPath(Path.Combine(this.basePath, databaseDir));
            }
            else
                this.databaseDir = Path.GetFullPath(this.basePath);

            addinDirs = new List<string>();
            addinDirs.Add(DefaultBundlesFolder);

            database = new BundleDatabase(clampBundle, this);

            if (this.basePath != null && this.basePath.Length > 0)
            {
                currentDomain = database.GetFolderDomain(this.basePath);
            }
            else
                currentDomain = BundleDatabase.GlobalDomain;
        }


        #region internal method

        internal bool CreateHostBundlesFile(string hostFile)
        {
            hostFile = Path.GetFullPath(hostFile);
            string baseName = Path.GetFileNameWithoutExtension(hostFile);
            if (!Directory.Exists(database.HostsPath))
                Directory.CreateDirectory(database.HostsPath);

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
                    // Ignore this file
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

        internal void NotifyDatabaseUpdated()
        {
            currentDomain = database.GetFolderDomain(this.basePath);
        }
        internal void CopyExtensionsFrom(BundleRegistry other)
        {
            database.CopyExtensions(other.database);
        }


        internal Bundle GetBundleForHostAssembly(string filePath)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return null;
            return database.GetBundleForHostAssembly(currentDomain, filePath);
        }
        #endregion

        #region  public method


        public bool IsBundleEnabled(string id)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return false;
            return database.IsBundleEnabled(currentDomain, id);
        }
        /// <summary>
        /// 更新组件注册表
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
