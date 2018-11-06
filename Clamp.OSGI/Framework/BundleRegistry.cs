using Clamp.OSGI.Framework.Data;
using System;
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
        private string startupDirectory;
        private string addinsDir;
        private string databaseDir;

        public string DefaultAddinsFolder
        {
            get { return addinsDir; }
        }

        public string RegistryPath
        {
            get { return basePath; }
        }

        internal string AddinCachePath
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
                return startupDirectory;
            }
        }

        internal List<string> GlobalAddinDirectories
        {
            get { return addinDirs; }
        }

        public BundleRegistry(string registryPath) : this(null, registryPath, null, null, null)
        {
        }

        public BundleRegistry(string registryPath, string startupDirectory, string addinsDir) : this(null, registryPath, startupDirectory, addinsDir, null)
        {
        }

        public BundleRegistry(string registryPath, string startupDirectory, string addinsDir, string databaseDir) : this(null, registryPath, startupDirectory, addinsDir, databaseDir)
        {
        }


        public BundleRegistry(ClampBundle clampBundle, string registryPath, string startupDirectory, string addinsDir, string databaseDir)
        {
            basePath = Path.GetFullPath(registryPath);

            if (addinsDir != null)
            {
                if (Path.IsPathRooted(addinsDir))
                    this.addinsDir = Path.GetFullPath(addinsDir);
                else
                    this.addinsDir = Path.GetFullPath(Path.Combine(basePath, addinsDir));
            }
            else
                this.addinsDir = Path.Combine(basePath, "addins");

            if (databaseDir != null)
            {
                if (Path.IsPathRooted(databaseDir))
                    this.databaseDir = Path.GetFullPath(databaseDir);
                else
                    this.databaseDir = Path.GetFullPath(Path.Combine(basePath, databaseDir));
            }
            else
                this.databaseDir = Path.GetFullPath(basePath);

            addinDirs = new List<string>();
            addinDirs.Add(DefaultAddinsFolder);

            database = new BundleDatabase(clampBundle, this);

            if (startupDirectory != null && startupDirectory.Length > 0)
            {
                currentDomain = database.GetFolderDomain(this.startupDirectory);
            }
            else
                currentDomain = BundleDatabase.GlobalDomain;
        }

        #region internal method
        internal void NotifyDatabaseUpdated()
        {
            if (startupDirectory != null)
                currentDomain = database.GetFolderDomain(startupDirectory);
        }


        /// <summary>
        /// 新建一个组件宿主
        /// </summary>
        /// <param name="hostFile"></param>
        /// <returns></returns>
        internal bool CreateHostBundlesFile(string hostFile)
        {
            hostFile = Path.GetFullPath(hostFile);

            string baseName = Path.GetFileNameWithoutExtension(hostFile);

            if (!Directory.Exists(database.HostsPath))
                Directory.CreateDirectory(database.HostsPath);

            foreach (string s in Directory.GetFiles(database.HostsPath, baseName + "*.addins"))
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

            string file = Path.Combine(database.HostsPath, baseName) + ".addins";
            int n = 1;
            while (File.Exists(file))
            {
                file = Path.Combine(database.HostsPath, baseName) + "_" + n + ".addins";
                n++;
            }

            using (StreamWriter sw = new StreamWriter(file))
            {
                XmlTextWriter tw = new XmlTextWriter(sw);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartElement("Addins");
                tw.WriteAttributeString("host-reference", hostFile);
                tw.WriteStartElement("Directory");
                tw.WriteAttributeString("shared", "false");
                tw.WriteString(Path.GetDirectoryName(hostFile));
                tw.WriteEndElement();
                tw.Close();
            }
            return true;
        }


        internal Bundle GetAddinForHostAssembly(string filePath)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return null;
            return database.GetAddinForHostAssembly(currentDomain, filePath);
        }
        #endregion

        #region  public method



        public bool IsAddinEnabled(string id)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return false;
            return database.IsAddinEnabled(currentDomain, id);
        }
        /// <summary>
        /// 更新组件注册表
        /// </summary>
        public void Update()
        {
            database.Update(currentDomain);
        }

        public Bundle[] GetAddins()
        {
            return GetModules(AddinSearchFlags.IncludeAddins);
        }

        public Bundle GetAddin(string id)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return null;
            Bundle ad = database.GetInstalledAddin(currentDomain, id);
            if (ad != null && IsRegisteredForUninstall(ad.Id))
                return null;
            return ad;
        }
        public bool IsRegisteredForUninstall(string addinId)
        {
            return database.IsRegisteredForUninstall(currentDomain, addinId);
        }

        public Bundle[] GetModules(AddinSearchFlags flags)
        {
            if (currentDomain == BundleDatabase.UnknownDomain)
                return new Bundle[0];

            AddinSearchFlagsInternal f = (AddinSearchFlagsInternal)(int)flags;

            return database.GetInstalledAddins(currentDomain, f | AddinSearchFlagsInternal.ExcludePendingUninstall).ToArray();
        }

        public void Dispose()
        {
            database.Shutdown();
        }

        #endregion



        #region internal static method

        internal static string GlobalRegistryPath
        {
            get
            {
                string customDir = Environment.GetEnvironmentVariable("MONO_ADDINS_GLOBAL_REGISTRY");
                if (customDir != null && customDir.Length > 0)
                    return Path.GetFullPath(customDir);

                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, "mono.addins");
                return Path.GetFullPath(path);
            }

        }
        #endregion

        #region private method

        #endregion


    }
}
