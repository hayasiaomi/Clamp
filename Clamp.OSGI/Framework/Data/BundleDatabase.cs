using Clamp.OSGI.Framework.Data.Description;
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
    /// Bundle数据库类
    /// </summary>
    internal class BundleDatabase
    {
        public const string GlobalDomain = "global";
        public const string UnknownDomain = "unknown";
        public const string VersionTag = "001";

        private Hashtable cachedBundleSetupInfos = new Hashtable();
        private BundleFileSystemExtension fs = new BundleFileSystemExtension();
        private BundleScanResult currentScanResult;
        private bool fatalDatabseError;
        private FileDatabase fileDatabase;
        private string bundleDbDir;
        private BundleActivationIndex activationIndex;
        private BundleRegistry registry;
        private ClampBundle clampBundle;
        private int lastDomainId;
        private DatabaseConfiguration config = null;
        private List<Bundle> allSetupInfos;
        private List<Bundle> bundleSetupInfos;
        private List<Bundle> rootSetupInfos;

        private List<object> extensions = new List<object>();
        /// <summary>
        /// 表示正在执行安装Bundle的进程(AppDomain域不是系统进程)
        /// </summary>
        internal static bool RunningSetupProcess;

        #region public Property

        /// <summary>
        /// 获得Bundle数据库所在的目录
        /// </summary>
        public string BundleDbDir
        {
            get { return bundleDbDir; }
        }

        /// <summary>
        /// 获得应用程序的住宿文件路径
        /// </summary>
        public string HostsPath
        {
            get { return Path.Combine(BundleDbDir, "hosts"); }
        }

        /// <summary>
        ///  Bundle所在路径数据对的存储路径
        /// </summary>
        public string BundleFolderCachePath
        {
            get { return Path.Combine(BundleDbDir, "bundle-dir-data"); }
        }

        public string BundlePrivateDataPath
        {
            get { return Path.Combine(BundleDbDir, "bundle-priv-data"); }
        }

        /// <summary>
        /// Bundle数据对的存储路径
        /// </summary>
        public string BundleCachePath
        {
            get { return Path.Combine(BundleDbDir, "bundle-data"); }
        }

        /// <summary>
        /// 激活索引文件
        /// </summary>
        public string ActivationIndexFile
        {
            get { return Path.Combine(BundleDbDir, "activation-index"); }
        }

        public string ConfigFile
        {
            get { return Path.Combine(BundleDbDir, "config.xml"); }
        }

        public BundleFileSystemExtension FileSystem
        {
            get { return fs; }
        }
        public bool IsGlobalRegistry
        {
            get
            {
                return registry.BasePath == BundleRegistry.GlobalRegistryPath;
            }
        }

        public BundleRegistry Registry
        {
            get
            {
                return this.registry;
            }
        }

        internal DatabaseConfiguration Configuration
        {
            get
            {
                if (config == null)
                {
                    using (fileDatabase.LockRead())
                    {
                        if (fileDatabase.Exists(ConfigFile))
                            config = DatabaseConfiguration.Read(ConfigFile);
                        else
                            config = DatabaseConfiguration.ReadAppConfig();
                    }
                }
                return config;
            }
        }

        #endregion

        public BundleDatabase(ClampBundle clampBundle, BundleRegistry registry)
        {
            this.clampBundle = clampBundle;
            this.registry = registry;
            this.bundleDbDir = Path.Combine(registry.BundleCachePath, "bundle-db-" + VersionTag);
            this.fileDatabase = new FileDatabase(BundleDbDir);
        }

        #region public method


        public void CopyExtensions(BundleDatabase other)
        {
            foreach (object o in other.extensions)
                RegisterExtension(o);
        }

        public void RegisterExtension(object extension)
        {
            extensions.Add(extension);
            if (extension is BundleFileSystemExtension)
                fs = (BundleFileSystemExtension)extension;
            else
                throw new NotSupportedException();
        }

        public void ParseBundle(string domain, string file, string outFile, bool inProcess)
        {
            if (!inProcess)
            {
                ISetupHandler setup = GetSetupHandler();
                setup.GetBundleDescription(registry, Path.GetFullPath(file), outFile);
                return;
            }

            using (fileDatabase.LockRead())
            {
                // First of all, check if the file belongs to a registered add-in
                BundleScanFolderInfo finfo;

                if (GetFolderInfoForPath(Path.GetDirectoryName(file), out finfo) && finfo != null)
                {
                    BundleFileInfo afi = finfo.GetBundleFileInfo(file);
                    if (afi != null && afi.IsNotNullBundleId)
                    {
                        BundleDescription adesc;

                        GetBundleDescription(afi.Domain, afi.BundleId, file, out adesc);

                        if (adesc != null)
                            adesc.Save(outFile);
                        return;
                    }
                }

                BundleScanResult sr = new BundleScanResult();
                sr.Domain = domain;
                BundleScanner scanner = new BundleScanner(this, sr);

                SingleFileAssemblyResolver res = new SingleFileAssemblyResolver(registry, scanner);
                ResolveEventHandler resolver = new ResolveEventHandler(res.Resolve);

                EventInfo einfo = typeof(AppDomain).GetEvent("ReflectionOnlyAssemblyResolve");

                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve += resolver;
                    if (einfo != null) einfo.AddEventHandler(AppDomain.CurrentDomain, resolver);

                    BundleDescription desc = scanner.ScanSingleFile(file, sr);

                    if (desc != null)
                    {
                        // Reset the xml doc so that it is not reused when saving. We want a brand new document
                        desc.ResetXmlDoc();
                        desc.Save(outFile);
                    }
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= resolver;
                    if (einfo != null) einfo.RemoveEventHandler(AppDomain.CurrentDomain, resolver);
                }
            }
        }

        public void Clear()
        {
            if (Directory.Exists(BundleCachePath))
                Directory.Delete(BundleCachePath, true);
            if (Directory.Exists(BundleFolderCachePath))
                Directory.Delete(BundleFolderCachePath, true);
        }

        /// <summary>
        /// 根据域和Bundle的Id来判断对应的Bundle详细是否存在
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="bundleId"></param>
        /// <returns></returns>
        public bool BundleDescriptionExists(string domain, string bundleId)
        {
            string file = GetDescriptionPath(domain, bundleId);
            return fileDatabase.Exists(file);
        }

        public bool DeleteFolderInfo(BundleScanFolderInfo folderInfo)
        {
            return SafeDelete(folderInfo.FileName);
        }

        public bool IsRegisteredForUninstall(string domain, string addinId)
        {
            return Configuration.IsRegisteredForUninstall(addinId);
        }

        /// <summary>
        /// 根据域和主程序来获得主宿体的Bundle
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="assemblyLocation"></param>
        /// <returns></returns>
        public Bundle GetBundleForHostAssembly(string domain, string assemblyLocation)
        {
            InternalCheck(domain);

            Bundle bundle = null;

            object ob = cachedBundleSetupInfos[assemblyLocation];

            if (ob != null)
                return ob as Bundle; // Don't use a cast here is ob may not be an Bundle.

            BundleActivationIndex index = GetBundleActivationIndex();

            string bundleId, bundleFile, rdomain;

            if (index.GetBundleForAssembly(assemblyLocation, out bundleId, out bundleFile, out rdomain))
            {
                string sid = bundleId + " " + rdomain;

                bundle = cachedBundleSetupInfos[sid] as Bundle;

                if (bundle == null)
                    bundle = new Bundle(this.clampBundle, this, rdomain, bundleId);

                cachedBundleSetupInfos[assemblyLocation] = bundle;
                cachedBundleSetupInfos[bundleId + " " + rdomain] = bundle;
            }

            return bundle;
        }

        public IEnumerable<Bundle> GetInstalledBundles(string domain, BundleSearchFlagsInternal flags)
        {
            if (domain == null)
                domain = registry.CurrentDomain;

            // Get the cached list if the add-in list has already been loaded.
            // The domain doesn't have to be checked again, since it is always the same

            IEnumerable<Bundle> result = null;

            if ((flags & BundleSearchFlagsInternal.IncludeAll) == BundleSearchFlagsInternal.IncludeAll)
            {
                if (allSetupInfos != null)
                    result = allSetupInfos;
            }
            else if ((flags & BundleSearchFlagsInternal.IncludeBundles) == BundleSearchFlagsInternal.IncludeBundles)
            {
                if (bundleSetupInfos != null)
                    result = bundleSetupInfos;
            }
            else
            {
                if (rootSetupInfos != null)
                    result = rootSetupInfos;
            }

            if (result == null)
            {
                InternalCheck(domain);
                using (fileDatabase.LockRead())
                {
                    result = InternalGetInstalledBundles(domain, null, flags & ~BundleSearchFlagsInternal.LatestVersionsOnly);
                }
            }

            if ((flags & BundleSearchFlagsInternal.LatestVersionsOnly) == BundleSearchFlagsInternal.LatestVersionsOnly)
                result = result.Where(a => a.IsLatestVersion);

            if ((flags & BundleSearchFlagsInternal.ExcludePendingUninstall) == BundleSearchFlagsInternal.ExcludePendingUninstall)
                result = result.Where(a => !IsRegisteredForUninstall(a.Description.Domain, a.Id));

            return result;
        }

        /// <summary>
        /// 尝试是否可以读取Bundle的文件夹信息
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folderInfo"></param>
        /// <returns></returns>

        public bool ReadFolderInfo(string file, out BundleScanFolderInfo folderInfo)
        {
            try
            {
                folderInfo = BundleScanFolderInfo.Read(fileDatabase, file);
                return true;
            }
            catch (Exception ex)
            {
                folderInfo = null;
                return false;
            }
        }

        /// <summary>
        /// 获得路径的域
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetFolderDomain(string path)
        {
            BundleScanFolderInfo folderInfo;

            if (GetFolderInfoForPath(path, out folderInfo) && folderInfo == null)
            {
                if (path.Length > 0 && path[path.Length - 1] != Path.DirectorySeparatorChar)
                    GetFolderInfoForPath(path + Path.DirectorySeparatorChar, out folderInfo);
                else if (path.Length > 0 && path[path.Length - 1] == Path.DirectorySeparatorChar)
                    GetFolderInfoForPath(path.TrimEnd(Path.DirectorySeparatorChar), out folderInfo);
            }
            if (folderInfo != null && !string.IsNullOrEmpty(folderInfo.Domain))
                return folderInfo.Domain;
            else
                return UnknownDomain;
        }

        /// <summary>
        /// 根据当前的路径来获得相对应的Bundle的文件夹信息。当读取文件数据异常的时候返回false，否则只要是空的也是对的。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folderInfo"></param>
        /// <returns></returns>
        public bool GetFolderInfoForPath(string path, out BundleScanFolderInfo folderInfo)
        {
            try
            {
                folderInfo = BundleScanFolderInfo.Read(fileDatabase, BundleFolderCachePath, path);
                return true;
            }
            catch (Exception ex)
            {
                folderInfo = null;
                return false;
            }
        }
        /// <summary>
        /// 更新域下的信息
        /// </summary>
        /// <param name="domain"></param>
        public void Update(string domain)
        {
            fatalDatabseError = false;

            DateTime tim = DateTime.Now;

            RunPendingUninstalls();

            Hashtable installed = new Hashtable();

            bool changesFound = CheckFolders(domain);

            if (changesFound)
            {
                if (domain != null)
                {
                    using (fileDatabase.LockRead())
                    {
                        foreach (Bundle bundle in InternalGetInstalledBundles(domain, BundleSearchFlagsInternal.IncludeBundles))
                        {
                            installed[bundle.Id] = bundle.Id;
                        }
                    }
                }
                //开起一个检测试的进程
                RunScannerProcess();

                ResetCachedData();

                registry.NotifyDatabaseUpdated();//重新获得域
            }


            // Update the currently loaded add-ins
            if (changesFound && domain != null && clampBundle != null && clampBundle.IsInitialized)
            {
                Hashtable newInstalled = new Hashtable();

                foreach (Bundle bundle in GetInstalledBundles(domain, BundleSearchFlagsInternal.IncludeBundles))
                {
                    newInstalled[bundle.Id] = bundle.Id;
                }

                foreach (string bid in installed.Keys)
                {
                    // Always try to unload, event if the add-in was not currently loaded.
                    // Required since the add-ins has to be marked as 'disabled', to avoid
                    // extensions from this add-in to be loaded
                    if (!newInstalled.Contains(bid))
                        clampBundle.UnloadBundle(bid);
                }

                foreach (string bid in newInstalled.Keys)
                {
                    if (!installed.Contains(bid))
                    {
                        Bundle bundle = clampBundle.Registry.GetBundle(bid);

                        if (bundle != null)
                            this.clampBundle.ActivateBundle(bid);
                    }
                }
            }
        }

        /// <summary>
        /// 根据域、BundleID和Bundle文件来获得对应的Bundle详细类
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="bundleId"></param>
        /// <param name="bundleFile"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public bool GetBundleDescription(string domain, string bundleId, string bundleFile, out BundleDescription description)
        {
            // If the same add-in is installed in different folders (in the same domain) there will be several .mbundle files for it,
            // using the suffix "_X" where X is a number > 1 (for example: someBundle,1.0.mbundle, someBundle,1.0.mbundle_2, someBundle,1.0.mbundle_3, ...)
            // We need to return the .mbundle whose BundleFile matches the one being requested

            bundleFile = Path.GetFullPath(bundleFile);
            int altNum = 1;
            string baseFile = GetDescriptionPath(domain, bundleId);
            string file = baseFile;
            bool failed = false;

            do
            {
                if (!ReadBundleDescription(file, out description))
                {
                    //所以说当前这个文件是有问题，必须删除掉，这样子才可以保证调用SaveDescription不会被替掉。
                    // Avoids creating alternate versions of corrupted files when later calling SaveDescription.
                    RemoveBundleDescriptionFile(file);
                    failed = true;
                    continue;
                }

                if (description == null)
                    break;

                if (Path.GetFullPath(description.BundleFile) == bundleFile)
                    return true;

                file = baseFile + "_" + (++altNum);
            }
            while (fileDatabase.Exists(file));

            // File not found. Return false only if there has been any read error.
            description = null;

            return failed;
        }
        /// <summary>
        /// 根据文件路径读取 Bundle详细信息，如果有问题就是会返回false
        /// </summary>
        /// <param name="file"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public bool ReadBundleDescription(string file, out BundleDescription description)
        {
            try
            {
                description = BundleDescription.ReadBinary(fileDatabase, file);

                if (description != null)
                    description.OwnerDatabase = this;

                return true;
            }
            catch (Exception ex)
            {
                description = null;
                return false;
            }
        }

        /// <summary>
        /// 安全删除
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool SafeDelete(string file)
        {
            try
            {
                fileDatabase.Delete(file);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SafeDeleteDir(string dir)
        {
            try
            {
                fileDatabase.DeleteDir(dir);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 获得下一个域的值
        /// </summary>
        /// <returns></returns>
        public string GetUniqueDomainId()
        {
            if (lastDomainId != 0)
            {
                lastDomainId++;
                return lastDomainId.ToString();
            }
            lastDomainId = 1;
            foreach (string s in fileDatabase.GetDirectories(BundleCachePath))
            {
                string dn = Path.GetFileName(s);
                if (dn == GlobalDomain)
                    continue;
                try
                {
                    int n = int.Parse(dn);
                    if (n >= lastDomainId)
                        lastDomainId = n + 1;
                }
                catch
                {
                }
            }
            return lastDomainId.ToString();
        }

        public ExtensionNodeSet FindNodeSet(string domain, string bundleId, string id)
        {
            return FindNodeSet(domain, bundleId, id, new Hashtable());
        }

        /// <summary>
        /// 保存组件信息到数据库
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="replaceFileName"></param>
        /// <returns></returns>
        public bool SaveDescription(BundleDescription desc, string replaceFileName)
        {
            try
            {
                if (replaceFileName != null)
                    desc.SaveBinary(fileDatabase, replaceFileName);
                else
                {
                    string file = GetDescriptionPath(desc.Domain, desc.BundleId);
                    string dir = Path.GetDirectoryName(file);
                    if (!fileDatabase.DirExists(dir))
                        fileDatabase.CreateDir(dir);
                    if (fileDatabase.Exists(file))
                    {
                        // Another BundleDescription already exists with the same name.
                        // Create an alternate BundleDescription file
                        int altNum = 2;
                        while (fileDatabase.Exists(file + "_" + altNum))
                            altNum++;
                        file = file + "_" + altNum;
                    }
                    desc.SaveBinary(fileDatabase, file);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 保存Bundle的文件夹信息
        /// </summary>
        /// <param name="folderInfo"></param>
        /// <returns></returns>
        public bool SaveFolderInfo(BundleScanFolderInfo folderInfo)
        {
            try
            {
                folderInfo.Write(fileDatabase, BundleFolderCachePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据域和Bundle的ID来判断Bundle是否可用
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsBundleEnabled(string domain, string id)
        {
            Bundle ainfo = GetInstalledBundle(domain, id);

            if (ainfo != null)
                return ainfo.Enabled;
            else
                return false;
        }

        /// <summary>
        /// 根据域和ID来获得对应的Bundle
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bundle GetInstalledBundle(string domain, string id)
        {
            return GetInstalledBundle(domain, id, false, false);
        }

        public Bundle GetInstalledBundle(string domain, string id, bool exactVersionMatch)
        {
            return GetInstalledBundle(domain, id, exactVersionMatch, false);
        }

        public Bundle GetInstalledBundle(string domain, string id, bool exactVersionMatch, bool enabledOnly)
        {
            // Try the given domain, and if not found, try the shared domain
            Bundle ad = GetInstalledDomainBundle(domain, id, exactVersionMatch, enabledOnly, true);

            if (ad != null)
                return ad;

            if (domain != BundleDatabase.GlobalDomain)
                return GetInstalledDomainBundle(BundleDatabase.GlobalDomain, id, exactVersionMatch, enabledOnly, true);
            else
                return null;
        }

        public void DisableBundle(string domain, string id, bool exactVersionMatch = false)
        {
            Bundle ai = GetInstalledBundle(domain, id, true);
            if (ai == null)
                throw new InvalidOperationException("Add-in '" + id + "' not installed.");

            if (!IsBundleEnabled(domain, id, exactVersionMatch))
                return;

            Configuration.SetEnabled(id, false, ai.BundleInfo.EnabledByDefault, exactVersionMatch);
            SaveConfiguration();

            // Disable all add-ins which depend on it

            try
            {
                string idName = Bundle.GetIdName(id);

                foreach (Bundle ainfo in GetInstalledBundles(domain, BundleSearchFlagsInternal.IncludeBundles))
                {
                    foreach (Dependency dep in ainfo.BundleInfo.Dependencies)
                    {
                        BundleDependency adep = dep as BundleDependency;
                        if (adep == null)
                            continue;

                        string adepid = Bundle.GetFullId(ainfo.BundleInfo.Namespace, adep.BundleId, null);
                        if (adepid != idName)
                            continue;

                        // The add-in that has been disabled, might be a requirement of this one, or maybe not
                        // if there is an older version available. Check it now.

                        adepid = Bundle.GetFullId(ainfo.BundleInfo.Namespace, adep.BundleId, adep.Version);

                        Bundle adepinfo = GetInstalledBundle(domain, adepid, false, true);

                        if (adepinfo == null)
                        {
                            DisableBundle(domain, ainfo.Id);
                            break;
                        }
                    }
                }
            }
            catch
            {
                // If something goes wrong, enable the add-in again
                Configuration.SetEnabled(id, true, ai.BundleInfo.EnabledByDefault, false);
                SaveConfiguration();
                throw;
            }

            if (this.clampBundle != null && this.clampBundle.IsInitialized)
                this.clampBundle.UnloadBundle(id);
        }

        public void Shutdown()
        {
            ResetCachedData();
        }
        #endregion

        #region internal Method
        public bool BundleDependsOn(string domain, string id1, string id2)
        {
            Hashtable visited = new Hashtable();

            return BundleDependsOn(visited, domain, id1, id2);
        }
        /// <summary>
        /// 检测指定的文件夹
        /// </summary>
        /// <param name="currentDomain"></param>
        /// <param name="folderToScan"></param>
        /// <param name="filesToIgnore"></param>
        internal void ScanFolders(string currentDomain, string folderToScan, List<string> filesToIgnore)
        {
            BundleScanResult res = new BundleScanResult();

            res.Domain = currentDomain;
            res.AddPathsToIgnore(filesToIgnore);

            this.ScanFolders(res);
        }

        internal void EnableBundle(string domain, string id, bool exactVersionMatch)
        {
            Bundle ainfo = GetInstalledBundle(domain, id, exactVersionMatch, false);
            if (ainfo == null)
                // It may be an add-in root
                return;

            if (IsBundleEnabled(domain, id))
                return;

            // Enable required add-ins

            foreach (Dependency dep in ainfo.BundleInfo.Dependencies)
            {
                if (dep is BundleDependency)
                {
                    BundleDependency adep = dep as BundleDependency;
                    string adepid = Bundle.GetFullId(ainfo.BundleInfo.Namespace, adep.BundleId, adep.Version);
                    EnableBundle(domain, adepid, false);
                }
            }

            Configuration.SetEnabled(id, true, ainfo.BundleInfo.EnabledByDefault, true);
            SaveConfiguration();

            if (this.clampBundle != null && this.clampBundle.IsInitialized)
                this.clampBundle.ActivateBundle(id);
        }

        internal bool IsBundleEnabled(string domain, string id, bool exactVersionMatch)
        {
            if (!exactVersionMatch)
                return IsBundleEnabled(domain, id);
            Bundle ainfo = GetInstalledBundle(domain, id, exactVersionMatch, false);
            if (ainfo == null)
                return false;
            return Configuration.IsEnabled(id, ainfo.BundleInfo.EnabledByDefault);
        }
        /// <summary>
        /// 获得唯一的BundleID
        /// </summary>
        /// <param name="file"></param>
        /// <param name="oldId"></param>
        /// <param name="ns"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        internal string GetUniqueBundleId(string file, string oldId, string ns, string version)
        {
            string baseId = "__" + Path.GetFileNameWithoutExtension(file);

            if (Path.GetExtension(baseId) == ".bundle")
                baseId = Path.GetFileNameWithoutExtension(baseId);

            string name = baseId;
            string id = Bundle.GetFullId(ns, name, version);

            // If the old Id is already an automatically generated one, reuse it
            if (oldId != null && oldId.StartsWith(id))
                return name;

            int n = 1;

            while (BundleIdExists(id))
            {
                name = baseId + "_" + n;
                id = Bundle.GetFullId(ns, name, version);
                n++;
            }
            return name;
        }

        internal bool RemoveBundleDescriptionFile(string file)
        {
            // Removes an add-in description and shifts up alternate instances of the description file
            // (so xxx,1.0.mbundle_2 will become xxx,1.0.mbundle, xxx,1.0.mbundle_3 -> xxx,1.0.mbundle_2, etc)

            if (!SafeDelete(file))
                return false;

            int dversion;

            if (file.EndsWith(".mbundle"))
                dversion = 2;
            else
            {
                int i = file.LastIndexOf('_');
                dversion = 1 + int.Parse(file.Substring(i + 1));
                file = file.Substring(0, i);
            }

            while (fileDatabase.Exists(file + "_" + dversion))
            {
                string newFile = dversion == 2 ? file : file + "_" + (dversion - 1);
                try
                {
                    fileDatabase.Rename(file + "_" + dversion, newFile);
                }
                catch (Exception ex)
                {
                }

                dversion++;
            }
            string dir = Path.GetDirectoryName(file);

            if (fileDatabase.DirectoryIsEmpty(dir))
                SafeDeleteDir(dir);

            if (dversion == 2)
            {
                // All versions of the add-in removed.
                SafeDeleteDir(Path.Combine(BundlePrivateDataPath, Path.GetFileNameWithoutExtension(file)));
            }

            return true;
        }

        /// <summary>
        /// 根据域和ID来获得对应的mbundle的文件路径
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal string GetDescriptionPath(string domain, string id)
        {
            return Path.Combine(Path.Combine(BundleCachePath, domain), id + ".mbundle");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="bundleId"></param>
        /// <param name="addinFile"></param>
        /// <param name="scanResult"></param>
        internal void UninstallBundle(string domain, string bundleId, string addinFile, BundleScanResult scanResult)
        {
            BundleDescription desc;

            if (!GetBundleDescription(domain, bundleId, addinFile, out desc))
            {
                // If we can't get information about the old assembly, just regenerate all relation data
                scanResult.RegenerateRelationData = true;
                return;
            }

            scanResult.AddRemovedBundle(bundleId);

            // If the add-in didn't exist, there is nothing left to do

            if (desc == null)
                return;

            // If the add-in already existed, the dependencies of the old add-in need to be re-analyzed

            Util.AddDependencies(desc, scanResult);

            if (desc.IsBundle)
                scanResult.ActivationIndex.RemoveHostData(desc.BundleId, desc.BundleFile);

            RemoveBundleDescriptionFile(desc.FileName);
        }

        /// <summary>
        /// 检测文件夹是否发生变化
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        internal bool CheckFolders(string domain)
        {
            using (fileDatabase.LockRead())
            {
                BundleScanResult scanResult = new BundleScanResult();

                scanResult.CheckOnly = true;
                scanResult.Domain = domain;

                InternalScanFolders(scanResult);

                return scanResult.ChangesFound;
            }
        }

        /// <summary>
        /// 内部检测文件夹
        /// </summary>
        /// <param name="scanResult"></param>
        internal void InternalScanFolders(BundleScanResult scanResult)
        {
            try
            {
                fs.ScanStarted();

                InternalScanFolders2(scanResult);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                fs.ScanFinished();
            }
        }

        /// <summary>
        /// 内部检测文件夹
        /// </summary>
        /// <param name="scanResult"></param>
        internal void InternalScanFolders2(BundleScanResult scanResult)
        {
            DateTime tim = DateTime.Now;

            //检测文件夹结构
            DatabaseInfrastructureCheck();

            try
            {
                scanResult.ActivationIndex = GetBundleActivationIndex();
            }
            catch (Exception ex)
            {
                if (scanResult.CheckOnly)
                {
                    scanResult.ChangesFound = true;
                    return;
                }

                scanResult.RegenerateAllData = true;
            }

            BundleScanner scanner = new BundleScanner(this, scanResult);

            //检测之前检测过的文件夹是否已被删除
            foreach (string file in Directory.GetFiles(BundleFolderCachePath, "*.data"))
            {
                BundleScanFolderInfo folderInfo;

                bool res = ReadFolderInfo(file, out folderInfo);
                bool validForDomain = scanResult.Domain == null || folderInfo.Domain == GlobalDomain || folderInfo.Domain == scanResult.Domain;

                if (!res || (validForDomain && !fs.DirectoryExists(folderInfo.Folder)))
                {
                    if (res)
                    {
                        //文件夹信息的数据存在，但是文件却不存在，所以要删掉
                        scanner.UpdateDeletedBundles(folderInfo, scanResult);
                    }
                    else
                    {
                        //说明数据发生变化了。需要重新更新
                        scanResult.ChangesFound = true;
                        scanResult.RegenerateRelationData = true;
                    }

                    if (!scanResult.CheckOnly)
                        SafeDelete(file);
                    else if (scanResult.ChangesFound)
                        return;
                }
            }

            //查看对应的文件夹是否发生变化
            if (registry.BasePath != null)
                scanner.ScanFolder(registry.BasePath, null, scanResult);

            //如果只是检测就返回，不是的就并发生了变化，就继续
            if (scanResult.CheckOnly && scanResult.ChangesFound)
                return;

            if (scanResult.Domain == null)
                scanner.ScanFolder(HostsPath, GlobalDomain, scanResult);

            if (scanResult.CheckOnly && scanResult.ChangesFound)
                return;

            foreach (string dir in registry.GlobalBundleDirectories)
            {
                if (scanResult.CheckOnly && scanResult.ChangesFound)
                    return;
                scanner.ScanFolderRec(dir, GlobalDomain, scanResult);
            }

            if (scanResult.CheckOnly || !scanResult.ChangesFound)
                return;

            currentScanResult = scanResult;

            foreach (FileToScan file in scanResult.FilesToScan)
                scanner.ScanFile(file.File, file.BundleScanFolderInfo, scanResult);

            foreach (BundleScanFolderInfo finfo in scanResult.ModifiedFolderInfos)
                SaveFolderInfo(finfo);

            SaveBundleActivationIndex();

            ResetCachedData();

            if (!scanResult.ChangesFound)
                return;

            tim = DateTime.Now;

            try
            {
                if (scanResult.RegenerateRelationData)
                {
                    scanResult.BundlesToUpdate = null;
                    scanResult.BundlesToUpdateRelations = null;
                }

                GenerateBundleExtensionMapsInternal(scanResult.Domain, scanResult.BundlesToUpdate, scanResult.BundlesToUpdateRelations, scanResult.RemovedBundles);
            }
            catch (Exception ex)
            {
                fatalDatabseError = true;
            }

            SaveBundleActivationIndex();
        }


        /// <summary>
        /// 重置当前的所有缓存数据。
        /// </summary>
        internal void ResetCachedData()
        {
            ResetBasicCachedData();
            activationIndex = null;
            cachedBundleSetupInfos.Clear();

            if (clampBundle != null)
                clampBundle.ResetCachedData();
        }

        internal void ResetBasicCachedData()
        {
            allSetupInfos = null;
            bundleSetupInfos = null;
            rootSetupInfos = null;
        }

        /// <summary>
        /// 获得当前的Bundle住宿索引类
        /// </summary>
        /// <returns></returns>
        internal BundleActivationIndex GetBundleActivationIndex()
        {
            if (activationIndex != null)
                return activationIndex;

            using (fileDatabase.LockRead())
            {
                if (fileDatabase.Exists(ActivationIndexFile))
                    activationIndex = BundleActivationIndex.Read(fileDatabase, ActivationIndexFile);
                else
                    activationIndex = new BundleActivationIndex();
            }
            return activationIndex;
        }

        /// <summary>
        /// 检测试当前系统下数据库文件夹是否可以操作和结构是否合格
        /// </summary>
        /// <returns></returns>
        internal bool DatabaseInfrastructureCheck()
        {
            bool hasChanges = false;

            try
            {
                if (!Directory.Exists(BundleCachePath))
                {
                    Directory.CreateDirectory(BundleCachePath);
                    hasChanges = true;
                }

                if (!Directory.Exists(BundleFolderCachePath))
                {
                    Directory.CreateDirectory(BundleFolderCachePath);
                    hasChanges = true;
                }

                Util.CheckWrittableFloder(BundleCachePath);
                Util.CheckWrittableFloder(BundleFolderCachePath);

                fatalDatabseError = false;
            }
            catch (Exception ex)
            {
                fatalDatabseError = true;
                throw ex;
            }
            return hasChanges;
        }


        #endregion
        #region private method

        private void GenerateBundleExtensionMapsInternal(string domain, List<string> addinsToUpdate, List<string> addinsToUpdateRelations, List<string> removedBundles)
        {
            BundleUpdateData updateData = new BundleUpdateData(this);

            // Clear cached data
            cachedBundleSetupInfos.Clear();

            // Collect all information

            BundleIndex bundleHash = new BundleIndex();

            Hashtable changedBundles = null;
            ArrayList descriptionsToSave = new ArrayList();
            ArrayList files = new ArrayList();

            bool partialGeneration = addinsToUpdate != null;

            string[] domains = GetDomains().Where(d => d == domain || d == GlobalDomain).ToArray();

            // Get the files to be updated

            if (partialGeneration)
            {
                changedBundles = new Hashtable();

                // Get the files and ids of all add-ins that have to be updated
                // Include removed add-ins: if there are several instances of the same add-in, removing one of
                // them will make other instances to show up. If there is a single instance, its files are
                // already removed.
                foreach (string sa in addinsToUpdate.Union(removedBundles))
                {
                    changedBundles[sa] = sa;

                    foreach (string file in GetBundleFiles(sa, domains))
                    {
                        if (!files.Contains(file))
                        {
                            files.Add(file);
                            string an = Path.GetFileNameWithoutExtension(file);
                            changedBundles[an] = an;
                        }
                    }
                }

                // Get the files and ids of all add-ins whose relations have to be updated
                foreach (string sa in addinsToUpdateRelations)
                {
                    foreach (string file in GetBundleFiles(sa, domains))
                    {
                        if (!files.Contains(file))
                        {
                            files.Add(file);
                        }
                    }
                }
            }
            else
            {
                foreach (var dom in domains)
                    files.AddRange(fileDatabase.GetDirectoryFiles(Path.Combine(BundleCachePath, dom), "*.mbundle"));
            }

            // Load the descriptions.
            foreach (string file in files)
            {

                BundleDescription conf;

                if (!ReadBundleDescription(file, out conf))
                {
                    SafeDelete(file);
                    continue;
                }

                // If the original file does not exist, the description can be deleted
                if (!fs.FileExists(conf.BundleFile))
                {
                    SafeDelete(file);
                    continue;
                }

                // Remove old data from the description. Remove the data of the add-ins that
                // have changed. This data will be re-added later.

                conf.UnmergeExternalData(changedBundles);
                descriptionsToSave.Add(conf);

                bundleHash.Add(conf);
            }

            // Sort the add-ins, to make sure add-ins are processed before
            // all their dependencies

            var sorted = bundleHash.GetSortedBundles();

            // Register extension points and node sets
            foreach (BundleDescription conf in sorted)
                CollectExtensionPointData(conf, updateData);

            // Register extensions
            foreach (BundleDescription conf in sorted)
            {
                if (changedBundles == null || changedBundles.ContainsKey(conf.BundleId))
                {
                    CollectExtensionData(bundleHash, conf, updateData);
                }
            }

            // Save the maps
            foreach (BundleDescription conf in descriptionsToSave)
            {
                ConsolidateExtensions(conf);
                conf.SaveBinary(fileDatabase);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void RunPendingUninstalls()
        {
            bool changesDone = false;

            foreach (var adn in Configuration.GetPendingUninstalls())
            {
                HashSet<string> files = new HashSet<string>(adn.Files);

                if (ClampBundle.CheckAssembliesLoaded(files))
                    continue;

                // Make sure all files can be deleted before doing so
                bool canUninstall = true;

                foreach (string f in adn.Files)
                {
                    if (!File.Exists(f))
                        continue;

                    try
                    {
                        File.OpenWrite(f).Close();
                    }
                    catch
                    {
                        canUninstall = false;
                        break;
                    }
                }

                if (!canUninstall)
                    continue;

                foreach (string f in adn.Files)
                {
                    try
                    {
                        if (File.Exists(f))
                            File.Delete(f);
                    }
                    catch
                    {
                        canUninstall = false;
                    }
                }

                if (canUninstall)
                {
                    Configuration.UnregisterForUninstall(adn.BundleId);
                    changesDone = true;
                }
            }
            if (changesDone)
                SaveConfiguration();
        }

        private bool BundleDependsOn(Hashtable visited, string domain, string id1, string id2)
        {
            if (visited.Contains(id1))
                return false;

            visited.Add(id1, id1);

            Bundle addin1 = GetInstalledBundle(domain, id1, false);

            // We can assume that if the add-in is not returned here, it may be a root addin.
            if (addin1 == null)
                return false;

            id2 = Bundle.GetIdName(id2);

            foreach (Dependency dep in addin1.BundleInfo.Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep == null)
                    continue;

                string depid = Bundle.GetFullId(addin1.BundleInfo.Namespace, adep.BundleId, null);

                if (depid == id2)
                    return true;

                else if (BundleDependsOn(visited, domain, depid, id2))
                    return true;
            }

            return false;
        }

        private ExtensionNodeSet FindNodeSet(string domain, string addinId, string id, Hashtable visited)
        {
            if (visited.Contains(addinId))
                return null;
            visited.Add(addinId, addinId);
            Bundle addin = GetInstalledBundle(domain, addinId, true, false);
            if (addin == null)
                return null;
            BundleDescription desc = addin.Description;
            if (desc == null)
                return null;
            foreach (ExtensionNodeSet nset in desc.ExtensionNodeSets)
                if (nset.Id == id)
                    return nset;

            // Not found in the add-in. Look on add-ins on which it depends

            foreach (Dependency dep in desc.MainModule.Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep == null) continue;

                string aid = Bundle.GetFullId(desc.Namespace, adep.BundleId, adep.Version);
                ExtensionNodeSet nset = FindNodeSet(domain, aid, id, visited);
                if (nset != null)
                    return nset;
            }
            return null;
        }

        /// <summary>
        /// 检测定指定的路径
        /// </summary>
        /// <param name="scanResult"></param>
        private void ScanFolders(BundleScanResult scanResult)
        {
            IDisposable checkLock = null;

            //判断他是只是检测。如果不是就要开启一个事务用于更新
            if (scanResult.CheckOnly)
                checkLock = fileDatabase.LockRead();
            else
            {

                //开启事务，直到所有的文件都更新完成之后才可以提交
                if (!fileDatabase.BeginTransaction())
                {
                    // The database is already being updated. Can't do anything for now.
                    return;
                }
            }

            EventInfo einfo = typeof(AppDomain).GetEvent("ReflectionOnlyAssemblyResolve");
            ResolveEventHandler resolver = new ResolveEventHandler(OnResolveBundleAssembly);

            try
            {
                // Perform the add-in scan

                if (!scanResult.CheckOnly)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += resolver;

                    if (einfo != null)
                        einfo.AddEventHandler(AppDomain.CurrentDomain, resolver);
                }

                InternalScanFolders(scanResult);

                if (!scanResult.CheckOnly)
                    fileDatabase.CommitTransaction();
            }
            catch
            {
                if (!scanResult.CheckOnly)
                    fileDatabase.RollbackTransaction();
                throw;
            }
            finally
            {
                currentScanResult = null;

                if (scanResult.CheckOnly)
                    checkLock.Dispose();
                else
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= resolver;

                    if (einfo != null)
                        einfo.RemoveEventHandler(AppDomain.CurrentDomain, resolver);
                }
            }
        }
        private Assembly OnResolveBundleAssembly(object s, ResolveEventArgs args)
        {
            string file = currentScanResult != null ? currentScanResult.GetAssemblyLocation(args.Name) : null;
            if (file != null)
                return Util.LoadAssemblyForReflection(file);
            else
            {
                if (!args.Name.StartsWith("Mono.Bundles.CecilReflector"))
                    Console.WriteLine("Assembly not found: " + args.Name);
                return null;
            }
        }

        private void ConsolidateExtensions(BundleDescription conf)
        {
            // Merges extensions with the same path

            foreach (ModuleDescription module in conf.AllModules)
            {
                Dictionary<string, Extension> extensions = new Dictionary<string, Extension>();
                foreach (Extension ext in module.Extensions)
                {
                    Extension mainExt;
                    if (extensions.TryGetValue(ext.Path, out mainExt))
                    {
                        ArrayList list = new ArrayList();
                        EnsureInsertionsSorted(ext.ExtensionNodes);
                        list.AddRange(ext.ExtensionNodes);
                        int pos = -1;
                        foreach (ExtensionNodeDescription node in list)
                        {
                            ext.ExtensionNodes.Remove(node);
                            AddNodeSorted(mainExt.ExtensionNodes, node, ref pos);
                        }
                    }
                    else
                    {
                        extensions[ext.Path] = ext;
                        EnsureInsertionsSorted(ext.ExtensionNodes);
                    }
                }

                // Sort the nodes
            }
        }

        private void EnsureInsertionsSorted(ExtensionNodeDescriptionCollection list)
        {
            // Makes sure that the nodes in the collections are properly sorted wrt insertafter and insertbefore attributes
            Dictionary<string, ExtensionNodeDescription> added = new Dictionary<string, ExtensionNodeDescription>();
            List<ExtensionNodeDescription> halfSorted = new List<ExtensionNodeDescription>();
            bool orderChanged = false;

            for (int n = list.Count - 1; n >= 0; n--)
            {
                ExtensionNodeDescription node = list[n];
                if (node.Id.Length > 0)
                    added[node.Id] = node;
                if (node.InsertAfter.Length > 0)
                {
                    ExtensionNodeDescription relNode;
                    if (added.TryGetValue(node.InsertAfter, out relNode))
                    {
                        // Out of order. Move it before the referenced node
                        int i = halfSorted.IndexOf(relNode);
                        halfSorted.Insert(i, node);
                        orderChanged = true;
                    }
                    else
                    {
                        halfSorted.Add(node);
                    }
                }
                else
                    halfSorted.Add(node);
            }
            halfSorted.Reverse();
            List<ExtensionNodeDescription> fullSorted = new List<ExtensionNodeDescription>();
            added.Clear();

            foreach (ExtensionNodeDescription node in halfSorted)
            {
                if (node.Id.Length > 0)
                    added[node.Id] = node;
                if (node.InsertBefore.Length > 0)
                {
                    ExtensionNodeDescription relNode;
                    if (added.TryGetValue(node.InsertBefore, out relNode))
                    {
                        // Out of order. Move it before the referenced node
                        int i = fullSorted.IndexOf(relNode);
                        fullSorted.Insert(i, node);
                        orderChanged = true;
                    }
                    else
                    {
                        fullSorted.Add(node);
                    }
                }
                else
                    fullSorted.Add(node);
            }
            if (orderChanged)
            {
                list.Clear();
                foreach (ExtensionNodeDescription node in fullSorted)
                    list.Add(node);
            }
        }

        private void AddNodeSorted(ExtensionNodeDescriptionCollection list, ExtensionNodeDescription node, ref int curPos)
        {
            // Adds the node at the correct position, taking into account insertbefore and insertafter

            if (node.InsertAfter.Length > 0)
            {
                string afterId = node.InsertAfter;
                for (int n = 0; n < list.Count; n++)
                {
                    if (list[n].Id == afterId)
                    {
                        list.Insert(n + 1, node);
                        curPos = n + 2;
                        return;
                    }
                }
            }
            else if (node.InsertBefore.Length > 0)
            {
                string beforeId = node.InsertBefore;
                for (int n = 0; n < list.Count; n++)
                {
                    if (list[n].Id == beforeId)
                    {
                        list.Insert(n, node);
                        curPos = n + 1;
                        return;
                    }
                }
            }
            if (curPos == -1)
                list.Add(node);
            else
                list.Insert(curPos++, node);
        }

        /// <summary>
        /// 执行检测进程
        /// </summary>
        private void RunScannerProcess()
        {
            ISetupHandler setup = GetSetupHandler();

            ArrayList pparams = new ArrayList();

            bool retry = false;
            do
            {
                try
                {
                    setup.Scan(registry, null, (string[])pparams.ToArray(typeof(string)));
                    retry = false;
                }
                catch (Exception ex)
                {
                    //ProcessFailedException pex = ex as ProcessFailedException;

                    //if (pex != null)
                    //{
                    //    // Get the last logged operation.
                    //    if (pex.LastLog.StartsWith("scan:"))
                    //    {
                    //        // It crashed while scanning a file. Add the file to the ignore list and try again.
                    //        string file = pex.LastLog.Substring(5);
                    //        pparams.Add(file);
                    //        monitor.ReportWarning("Could not scan file: " + file);
                    //        retry = true;
                    //        continue;
                    //    }
                    //}

                    //fatalDatabseError = true;
                    //// If the process has crashed, try to do a new scan, this time using verbose log,
                    //// to give the user more information about the origin of the crash.
                    //if (pex != null && !retry)
                    //{
                    //    monitor.ReportError("Add-in scan operation failed. The runtime may have encountered an error while trying to load an assembly.", null);
                    //    if (monitor.LogLevel <= 1)
                    //    {
                    //        // Re-scan again using verbose log, to make it easy to find the origin of the error.
                    //        retry = true;
                    //        scanMonitor = new ConsoleProgressStatus(true);
                    //    }
                    //}
                    //else
                    //    retry = false;

                    //if (!retry)
                    //{
                    //    var pfex = ex as ProcessFailedException;
                    //    monitor.ReportError("Add-in scan operation failed", pfex != null ? pfex.InnerException : ex);
                    //    monitor.Cancel();
                    //    return;
                    //}
                }
            }
            while (retry);
        }

        /// <summary>
        /// 获得安装的处理者
        /// </summary>
        /// <returns></returns>
        private ISetupHandler GetSetupHandler()
        {
            if (fs.RequiresIsolation)
                return new SetupDomain();
            else
                return new SetupLocal();
        }

        /// <summary>
        /// 获得当前域下面所有type指定的类型Bundle集合
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<Bundle> InternalGetInstalledBundles(string domain, BundleSearchFlagsInternal type)
        {
            return InternalGetInstalledBundles(domain, null, type);
        }

        /// <summary>
        /// 内部获得域定安装过的Bundle集合，如果idFilter不为空的时候 就是返回指定的Bundle集。当idFilter为空的时候，会根据type类型的值返回对应的Bundle集合
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="idFilter"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<Bundle> InternalGetInstalledBundles(string domain, string idFilter, BundleSearchFlagsInternal type)
        {
            if ((type & BundleSearchFlagsInternal.LatestVersionsOnly) != 0)
                throw new InvalidOperationException("LatestVersionsOnly flag not supported here");

            if (allSetupInfos == null)
            {
                Dictionary<string, Bundle> bdict = new Dictionary<string, Bundle>();

                //如果不是公共的域，就从公共的域中寻找
                if (domain != BundleDatabase.GlobalDomain)
                    FindInstalledBundles(bdict, BundleDatabase.GlobalDomain, idFilter);

                FindInstalledBundles(bdict, domain, idFilter);

                List<Bundle> alist = new List<Bundle>(bdict.Values);

                UpdateLastVersionFlags(alist);

                if (idFilter != null)
                    return alist;

                allSetupInfos = alist;
            }

            //返回符合当前ID过虑的所有Bundle,其中包含特殊Bundle
            if ((type & BundleSearchFlagsInternal.IncludeAll) == BundleSearchFlagsInternal.IncludeAll)
                return FilterById(allSetupInfos, idFilter);

            //返回符合当前ID过虑的所有特殊Bundle  否则的话，就是返回所有Bundle不包含的特殊Bundle
            if ((type & BundleSearchFlagsInternal.IncludeBundles) == BundleSearchFlagsInternal.IncludeBundles)
            {
                if (bundleSetupInfos == null)
                {
                    bundleSetupInfos = new List<Bundle>();

                    foreach (Bundle bundle in allSetupInfos)
                    {
                        if (!bundle.Description.IsBundle)
                            bundleSetupInfos.Add(bundle);
                    }
                }
                return FilterById(bundleSetupInfos, idFilter);
            }
            else
            {
                if (rootSetupInfos == null)
                {
                    rootSetupInfos = new List<Bundle>();

                    foreach (Bundle adn in allSetupInfos)
                    {
                        if (adn.Description.IsBundle)
                            rootSetupInfos.Add(adn);
                    }
                }
                return FilterById(rootSetupInfos, idFilter);
            }
        }

        /// <summary>
        /// 过虑符合ID名的Bundle
        /// </summary>
        /// <param name="addins"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private IEnumerable<Bundle> FilterById(List<Bundle> addins, string id)
        {
            if (id == null)
                return addins;

            return addins.Where(a => Bundle.GetIdName(a.Id) == id);
        }

        /// <summary>
        /// 获得已经安装的Bundle,通过域和ID过虑
        /// </summary>
        /// <param name="result"></param>
        /// <param name="domain"></param>
        /// <param name="idFilter"></param>
        private void FindInstalledBundles(Dictionary<string, Bundle> result, string domain, string idFilter)
        {
            if (idFilter == null)
                idFilter = "*";

            string dir = Path.Combine(BundleCachePath, domain);//获域的所在位置

            if (Directory.Exists(dir))
            {
                foreach (string file in fileDatabase.GetDirectoryFiles(dir, idFilter + ",*.mbundle"))
                {
                    string id = Path.GetFileNameWithoutExtension(file);

                    if (!result.ContainsKey(id))
                    {
                        var adesc = GetInstalledDomainBundle(domain, id, true, false, false);

                        if (adesc != null)
                            result.Add(id, adesc);
                    }
                }
            }
        }

        /// <summary>
        /// 标记Bundle集合是否为最新版本的Bundle
        /// </summary>
        /// <param name="bundles"></param>
        private void UpdateLastVersionFlags(List<Bundle> bundles)
        {
            Dictionary<string, string> versions = new Dictionary<string, string>();

            foreach (Bundle bundle in bundles)
            {
                string last;
                string id, version;

                Bundle.GetIdParts(bundle.Id, out id, out version);

                if (!versions.TryGetValue(id, out last) || Bundle.CompareVersions(last, version) > 0)
                    versions[id] = version;
            }

            foreach (Bundle a in bundles)
            {
                string id, version;

                Bundle.GetIdParts(a.Id, out id, out version);

                a.IsLatestVersion = versions[id] == version;
            }
        }


        private void SaveConfiguration()
        {
            if (config != null)
            {
                using (fileDatabase.LockWrite())
                {
                    config.Write(ConfigFile);
                }
            }
        }
        /// <summary>
        /// 从域中根据ID获得安装过的Bundle
        /// </summary>
        /// <param name="domain">域的值</param>
        /// <param name="id">标识</param>
        /// <param name="exactVersionMatch">是否只要准确的版本号</param>
        /// <param name="enabledOnly">是否只能是可用的</param>
        /// <param name="dbLockCheck">是否重要检测域</param>
        /// <returns></returns>
        private Bundle GetInstalledDomainBundle(string domain, string id, bool exactVersionMatch, bool enabledOnly, bool dbLockCheck)
        {
            Bundle sinfo = null;

            string idd = id + " " + domain;
            object ob = cachedBundleSetupInfos[idd];

            if (ob != null)
            {
                sinfo = ob as Bundle;

                if (sinfo != null)
                {
                    if (!enabledOnly || sinfo.Enabled)
                        return sinfo;

                    if (exactVersionMatch)
                        return null;
                }
                else if (enabledOnly)
                    // Ignore the 'not installed' flag when disabled add-ins are allowed
                    return null;
            }

            if (dbLockCheck)
                InternalCheck(domain);

            using ((dbLockCheck ? fileDatabase.LockRead() : null))
            {
                string path = GetDescriptionPath(domain, id);

                if (sinfo == null && fileDatabase.Exists(path))
                {
                    sinfo = new Bundle(this.clampBundle, this, domain, id);

                    cachedBundleSetupInfos[idd] = sinfo;

                    if (!enabledOnly || sinfo.Enabled)
                        return sinfo;

                    if (exactVersionMatch)
                    {
                        // Cache lookups with negative result
                        cachedBundleSetupInfos[idd] = this;
                        return null;
                    }
                }

                // Exact version not found. Look for a compatible version
                if (!exactVersionMatch)
                {
                    sinfo = null;

                    string version, name, bestVersion = null;

                    Bundle.GetIdParts(id, out name, out version);

                    foreach (Bundle ia in InternalGetInstalledBundles(domain, name, BundleSearchFlagsInternal.IncludeAll))
                    {
                        if ((!enabledOnly || ia.Enabled) && (version.Length == 0 || ia.SupportsVersion(version)) && (bestVersion == null || Bundle.CompareVersions(bestVersion, ia.Version) > 0))
                        {
                            bestVersion = ia.Version;
                            sinfo = ia;
                        }
                    }

                    if (sinfo != null)
                    {
                        cachedBundleSetupInfos[idd] = sinfo;
                        return sinfo;
                    }
                }

                // Cache lookups with negative result
                // Ignore the 'not installed' flag when disabled add-ins are allowed
                if (enabledOnly)
                    cachedBundleSetupInfos[idd] = this;

                return null;
            }
        }

        /// <summary>
        /// 内部检测对应的域，如果Bundle的存储路径不存的时候，需要重要更新域
        /// </summary>
        /// <param name="domain"></param>
        private void InternalCheck(string domain)
        {
            if (fatalDatabseError)
                return;

            bool update = false;

            using (fileDatabase.LockRead())
            {
                if (!Directory.Exists(BundleCachePath))
                {
                    update = true;
                }
            }

            if (update)
                Update(domain);//Bundle的存储路径不存，需在更新
        }

        private bool BundleIdExists(string id)
        {
            foreach (string d in fileDatabase.GetDirectories(BundleCachePath))
            {
                if (fileDatabase.Exists(Path.Combine(d, id + ".addin")))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 保存激活的索引文件
        /// </summary>
        private void SaveBundleActivationIndex()
        {
            if (activationIndex != null)
                activationIndex.Write(fileDatabase, ActivationIndexFile);
        }

        /// <summary>
        /// 获得所有的域
        /// </summary>
        /// <returns></returns>
        private string[] GetDomains()
        {
            string[] dirs = fileDatabase.GetDirectories(BundleCachePath);
            string[] ids = new string[dirs.Length];

            for (int n = 0; n < dirs.Length; n++)
                ids[n] = Path.GetFileName(dirs[n]);

            return ids;
        }

        private IEnumerable GetBundleFiles(string fullId, string[] domains)
        {
            // Look for all versions of the add-in, because this id may be the id of a reference,
            // and the exact reference version may not be installed.
            string s = fullId;
            int i = s.LastIndexOf(',');
            if (i != -1)
                s = s.Substring(0, i);
            s += ",*";

            // Look for the add-in in any of the existing folders
            foreach (string domain in domains)
            {
                string mp = GetDescriptionPath(domain, s);
                string dir = Path.GetDirectoryName(mp);
                string pat = Path.GetFileName(mp);
                foreach (string fmp in fileDatabase.GetDirectoryFiles(dir, pat))
                    yield return fmp;
            }
        }

        private void CollectExtensionPointData(BundleDescription conf, BundleUpdateData updateData)
        {
            foreach (ExtensionNodeSet nset in conf.ExtensionNodeSets)
            {
                try
                {
                    updateData.RegisterNodeSet(conf, nset);
                    updateData.RelNodeSetTypes++;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error reading node set: " + nset.Id, ex);
                }
            }

            foreach (ExtensionPoint ep in conf.ExtensionPoints)
            {
                try
                {
                    updateData.RegisterExtensionPoint(conf, ep);
                    updateData.RelExtensionPoints++;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error reading extension point: " + ep.Path, ex);
                }
            }
        }

        private void CollectExtensionData(BundleIndex addinHash, BundleDescription conf, BundleUpdateData updateData)
        {
            IEnumerable<string> missingDeps = addinHash.GetMissingDependencies(conf, conf.MainModule);
            if (missingDeps.Any())
            {
                return;
            }

            CollectModuleExtensionData(conf, conf.MainModule, updateData, addinHash);

            foreach (ModuleDescription module in conf.OptionalModules)
            {
                missingDeps = addinHash.GetMissingDependencies(conf, module);

                if (missingDeps.Any())
                {
                    string w = "An optional module of the add-in '" + conf.BundleId + "' could not be updated because some of its dependencies are missing or not compatible:";
                    w += BuildMissingBundlesList(addinHash, conf, missingDeps);

                    //TODO 日志记录
                }
                else
                    CollectModuleExtensionData(conf, module, updateData, addinHash);
            }
        }

        private string BuildMissingBundlesList(BundleIndex addinHash, BundleDescription conf, IEnumerable<string> missingDeps)
        {
            string w = "";
            foreach (string dep in missingDeps)
            {
                var found = addinHash.GetSimilarExistingBundle(conf, dep);
                if (found == null)
                    w += "\n  missing: " + dep;
                else
                    w += "\n  required: " + dep + ", found: " + found.BundleId;
            }
            return w;
        }

        private void CollectModuleExtensionData(BundleDescription conf, ModuleDescription module, BundleUpdateData updateData, BundleIndex index)
        {
            foreach (Extension ext in module.Extensions)
            {
                updateData.RelExtensions++;
                updateData.RegisterExtension(conf, module, ext);
                AddChildExtensions(conf, module, updateData, index, ext.Path, ext.ExtensionNodes, false);
            }
        }

        private void AddChildExtensions(BundleDescription conf, ModuleDescription module, BundleUpdateData updateData, BundleIndex index, string path, ExtensionNodeDescriptionCollection nodes, bool conditionChildren)
        {
            // Don't register conditions as extension nodes.
            if (!conditionChildren)
                updateData.RegisterExtension(conf, module, path);

            foreach (ExtensionNodeDescription node in nodes)
            {
                if (node.NodeName == "ComplexCondition")
                    continue;
                updateData.RelExtensionNodes++;
                string id = node.GetAttribute("id");
                if (id.Length != 0)
                {
                    bool isCondition = node.NodeName == "Condition";
                    if (isCondition)
                    {
                        // Find the add-in that provides the implementation for this condition.
                        // Store that id in the condition. The add-in engine will ensure the add-in
                        // is loaded when it tries to evaluate this condition.
                        var condAsm = index.FindCondition(conf, module, id);
                        if (condAsm != null)
                            node.SetAttribute(Condition.SourceBundleFragmentAttribute, condAsm);
                    }
                    AddChildExtensions(conf, module, updateData, index, path + "/" + id, node.ChildNodes, isCondition);
                }
            }
        }

        #endregion


    }
}
