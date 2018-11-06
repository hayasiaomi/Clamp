using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class BundleDatabase
    {
        public const string GlobalDomain = "global";
        public const string UnknownDomain = "unknown";
        public const string VersionTag = "002";

        private Hashtable cachedAddinSetupInfos = new Hashtable();
        private AddinFileSystemExtension fs = new AddinFileSystemExtension();
        private AddinScanResult currentScanResult;
        private bool fatalDatabseError;
        private FileDatabase fileDatabase;
        private string addinDbDir;
        private AddinHostIndex hostIndex;
        private BundleRegistry registry;
        private ClampBundle clampBundle;
        private int lastDomainId;
        private DatabaseConfiguration config = null;
        private List<Bundle> allSetupInfos;
        private List<Bundle> addinSetupInfos;
        private List<Bundle> rootSetupInfos;


        internal static bool RunningSetupProcess;

        #region public Property

        public string AddinDbDir
        {
            get { return addinDbDir; }
        }


        public string HostsPath
        {
            get { return Path.Combine(AddinDbDir, "hosts"); }
        }

        public string AddinFolderCachePath
        {
            get { return Path.Combine(AddinDbDir, "bundle-dir-data"); }
        }

        public string AddinPrivateDataPath
        {
            get { return Path.Combine(AddinDbDir, "addin-priv-data"); }
        }

        public string AddinCachePath
        {
            get { return Path.Combine(AddinDbDir, "addin-data"); }
        }

        public string HostIndexFile
        {
            get { return Path.Combine(AddinDbDir, "host-index"); }
        }

        public string ConfigFile
        {
            get { return Path.Combine(AddinDbDir, "config.xml"); }
        }

        public AddinFileSystemExtension FileSystem
        {
            get { return fs; }
        }
        public bool IsGlobalRegistry
        {
            get
            {
                return registry.RegistryPath == BundleRegistry.GlobalRegistryPath;
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
            this.addinDbDir = Path.Combine(registry.AddinCachePath, "addin-db-" + VersionTag);
            fileDatabase = new FileDatabase(AddinDbDir);
        }

        #region public method

        public void Clear()
        {
            if (Directory.Exists(AddinCachePath))
                Directory.Delete(AddinCachePath, true);
            if (Directory.Exists(AddinFolderCachePath))
                Directory.Delete(AddinFolderCachePath, true);
        }

        public void Write(FileDatabase fileDatabase, string file)
        {
            fileDatabase.WriteObject(file, this);
        }

        public bool AddinDescriptionExists(string domain, string addinId)
        {
            string file = GetDescriptionPath(domain, addinId);
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

        public Bundle GetAddinForHostAssembly(string domain, string assemblyLocation)
        {
            InternalCheck(domain);
            Bundle ainfo = null;

            object ob = cachedAddinSetupInfos[assemblyLocation];
            if (ob != null)
                return ob as Bundle; // Don't use a cast here is ob may not be an Addin.

            AddinHostIndex index = GetAddinHostIndex();
            string addin, addinFile, rdomain;
            if (index.GetAddinForAssembly(assemblyLocation, out addin, out addinFile, out rdomain))
            {
                string sid = addin + " " + rdomain;
                ainfo = cachedAddinSetupInfos[sid] as Bundle;
                if (ainfo == null)
                    ainfo = new Bundle(this.clampBundle, this, rdomain, addin);
                cachedAddinSetupInfos[assemblyLocation] = ainfo;
                cachedAddinSetupInfos[addin + " " + rdomain] = ainfo;
            }

            return ainfo;
        }



        public IEnumerable<Bundle> GetInstalledAddins(string domain, AddinSearchFlagsInternal flags)
        {
            if (domain == null)
                domain = registry.CurrentDomain;

            // Get the cached list if the add-in list has already been loaded.
            // The domain doesn't have to be checked again, since it is always the same

            IEnumerable<Bundle> result = null;

            if ((flags & AddinSearchFlagsInternal.IncludeAll) == AddinSearchFlagsInternal.IncludeAll)
            {
                if (allSetupInfos != null)
                    result = allSetupInfos;
            }
            else if ((flags & AddinSearchFlagsInternal.IncludeAddins) == AddinSearchFlagsInternal.IncludeAddins)
            {
                if (addinSetupInfos != null)
                    result = addinSetupInfos;
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
                    result = InternalGetInstalledAddins(domain, null, flags & ~AddinSearchFlagsInternal.LatestVersionsOnly);
                }
            }

            if ((flags & AddinSearchFlagsInternal.LatestVersionsOnly) == AddinSearchFlagsInternal.LatestVersionsOnly)
                result = result.Where(a => a.IsLatestVersion);

            if ((flags & AddinSearchFlagsInternal.ExcludePendingUninstall) == AddinSearchFlagsInternal.ExcludePendingUninstall)
                result = result.Where(a => !IsRegisteredForUninstall(a.Description.Domain, a.Id));

            return result;
        }



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

        public bool GetFolderInfoForPath(string path, out BundleScanFolderInfo folderInfo)
        {
            try
            {
                folderInfo = BundleScanFolderInfo.Read(fileDatabase, AddinFolderCachePath, path);
                return true;
            }
            catch (Exception ex)
            {
                folderInfo = null;
                return false;
            }
        }

        public void Update(string domain)
        {
            fatalDatabseError = false;

            DateTime tim = DateTime.Now;

            //RunPendingUninstalls(monitor);

            Hashtable installed = new Hashtable();

            bool changesFound = CheckFolders(domain);

            if (changesFound)
            {
                // Something has changed, the add-ins need to be re-scanned, but it has
                // to be done in an external process

                if (domain != null)
                {
                    using (fileDatabase.LockRead())
                    {
                        foreach (Bundle ainfo in InternalGetInstalledAddins(domain, AddinSearchFlagsInternal.IncludeAddins))
                        {
                            installed[ainfo.Id] = ainfo.Id;
                        }
                    }
                }

                RunScannerProcess(monitor);

                ResetCachedData();

                registry.NotifyDatabaseUpdated();
            }


            // Update the currently loaded add-ins
            if (changesFound && domain != null && clampBundle != null && clampBundle.IsInitialized)
            {
                Hashtable newInstalled = new Hashtable();

                foreach (Bundle ainfo in GetInstalledAddins(domain, AddinSearchFlagsInternal.IncludeAddins))
                {
                    newInstalled[ainfo.Id] = ainfo.Id;
                }

                foreach (string aid in installed.Keys)
                {
                    // Always try to unload, event if the add-in was not currently loaded.
                    // Required since the add-ins has to be marked as 'disabled', to avoid
                    // extensions from this add-in to be loaded
                    if (!newInstalled.Contains(aid))
                        clampBundle.UnloadAddin(aid);
                }

                foreach (string aid in newInstalled.Keys)
                {
                    if (!installed.Contains(aid))
                    {
                        Bundle addin = clampBundle.Registry.GetAddin(aid);
                        if (addin != null)
                            this.clampBundle.ActivateAddin(aid);
                    }
                }
            }
        }

        public bool GetAddinDescription(string domain, string addinId, string addinFile, out BundleDescription description)
        {
            // If the same add-in is installed in different folders (in the same domain) there will be several .maddin files for it,
            // using the suffix "_X" where X is a number > 1 (for example: someAddin,1.0.maddin, someAddin,1.0.maddin_2, someAddin,1.0.maddin_3, ...)
            // We need to return the .maddin whose AddinFile matches the one being requested

            addinFile = Path.GetFullPath(addinFile);
            int altNum = 1;
            string baseFile = GetDescriptionPath(domain, addinId);
            string file = baseFile;
            bool failed = false;

            do
            {
                if (!ReadAddinDescription(file, out description))
                {
                    // Remove the AddinDescription here since it is corrupted.
                    // Avoids creating alternate versions of corrupted files when later calling SaveDescription.
                    RemoveAddinDescriptionFile(file);
                    failed = true;
                    continue;
                }

                if (description == null)
                    break;

                if (Path.GetFullPath(description.AddinFile) == addinFile)
                    return true;
                file = baseFile + "_" + (++altNum);
            }
            while (fileDatabase.Exists(file));

            // File not found. Return false only if there has been any read error.
            description = null;
            return failed;
        }

        public bool ReadAddinDescription(string file, out BundleDescription description)
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

        public string GetUniqueDomainId()
        {
            if (lastDomainId != 0)
            {
                lastDomainId++;
                return lastDomainId.ToString();
            }
            lastDomainId = 1;
            foreach (string s in fileDatabase.GetDirectories(AddinCachePath))
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
                    string file = GetDescriptionPath(desc.Domain, desc.AddinId);
                    string dir = Path.GetDirectoryName(file);
                    if (!fileDatabase.DirExists(dir))
                        fileDatabase.CreateDir(dir);
                    if (fileDatabase.Exists(file))
                    {
                        // Another AddinDescription already exists with the same name.
                        // Create an alternate AddinDescription file
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

        public bool SaveFolderInfo(BundleScanFolderInfo folderInfo)
        {
            try
            {
                folderInfo.Write(fileDatabase, AddinFolderCachePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAddinEnabled(string domain, string id)
        {
            Bundle ainfo = GetInstalledAddin(domain, id);
            if (ainfo != null)
                return ainfo.Enabled;
            else
                return false;
        }

        public Bundle GetInstalledAddin(string domain, string id)
        {
            return GetInstalledAddin(domain, id, false, false);
        }

        public Bundle GetInstalledAddin(string domain, string id, bool exactVersionMatch)
        {
            return GetInstalledAddin(domain, id, exactVersionMatch, false);
        }

        public Bundle GetInstalledAddin(string domain, string id, bool exactVersionMatch, bool enabledOnly)
        {
            // Try the given domain, and if not found, try the shared domain
            Bundle ad = GetInstalledDomainAddin(domain, id, exactVersionMatch, enabledOnly, true);
            if (ad != null)
                return ad;
            if (domain != BundleDatabase.GlobalDomain)
                return GetInstalledDomainAddin(BundleDatabase.GlobalDomain, id, exactVersionMatch, enabledOnly, true);
            else
                return null;
        }

        public void DisableAddin(string domain, string id, bool exactVersionMatch = false)
        {
            Bundle ai = GetInstalledAddin(domain, id, true);
            if (ai == null)
                throw new InvalidOperationException("Add-in '" + id + "' not installed.");

            if (!IsAddinEnabled(domain, id, exactVersionMatch))
                return;

            Configuration.SetEnabled(id, false, ai.AddinInfo.EnabledByDefault, exactVersionMatch);
            SaveConfiguration();

            // Disable all add-ins which depend on it

            try
            {
                string idName = Bundle.GetIdName(id);

                foreach (Bundle ainfo in GetInstalledAddins(domain, AddinSearchFlagsInternal.IncludeAddins))
                {
                    foreach (Dependency dep in ainfo.AddinInfo.Dependencies)
                    {
                        BundleDependency adep = dep as BundleDependency;
                        if (adep == null)
                            continue;

                        string adepid = Bundle.GetFullId(ainfo.AddinInfo.Namespace, adep.AddinId, null);
                        if (adepid != idName)
                            continue;

                        // The add-in that has been disabled, might be a requirement of this one, or maybe not
                        // if there is an older version available. Check it now.

                        adepid = Bundle.GetFullId(ainfo.AddinInfo.Namespace, adep.AddinId, adep.Version);
                        Bundle adepinfo = GetInstalledAddin(domain, adepid, false, true);

                        if (adepinfo == null)
                        {
                            DisableAddin(domain, ainfo.Id);
                            break;
                        }
                    }
                }
            }
            catch
            {
                // If something goes wrong, enable the add-in again
                Configuration.SetEnabled(id, true, ai.AddinInfo.EnabledByDefault, false);
                SaveConfiguration();
                throw;
            }

            if (this.clampBundle != null && this.clampBundle.IsInitialized)
                this.clampBundle.UnloadAddin(id);
        }

        public void Shutdown()
        {
            ResetCachedData();
        }
        #endregion

        #region internal Method

        internal void EnableAddin(string domain, string id, bool exactVersionMatch)
        {
            Bundle ainfo = GetInstalledAddin(domain, id, exactVersionMatch, false);
            if (ainfo == null)
                // It may be an add-in root
                return;

            if (IsAddinEnabled(domain, id))
                return;

            // Enable required add-ins

            foreach (Dependency dep in ainfo.AddinInfo.Dependencies)
            {
                if (dep is BundleDependency)
                {
                    BundleDependency adep = dep as BundleDependency;
                    string adepid = Bundle.GetFullId(ainfo.AddinInfo.Namespace, adep.AddinId, adep.Version);
                    EnableAddin(domain, adepid, false);
                }
            }

            Configuration.SetEnabled(id, true, ainfo.AddinInfo.EnabledByDefault, true);
            SaveConfiguration();

            if (this.clampBundle != null && this.clampBundle.IsInitialized)
                this.clampBundle.ActivateAddin(id);
        }

        internal bool IsAddinEnabled(string domain, string id, bool exactVersionMatch)
        {
            if (!exactVersionMatch)
                return IsAddinEnabled(domain, id);
            Bundle ainfo = GetInstalledAddin(domain, id, exactVersionMatch, false);
            if (ainfo == null)
                return false;
            return Configuration.IsEnabled(id, ainfo.AddinInfo.EnabledByDefault);
        }

        internal string GetUniqueAddinId(string file, string oldId, string ns, string version)
        {
            string baseId = "__" + Path.GetFileNameWithoutExtension(file);

            if (Path.GetExtension(baseId) == ".addin")
                baseId = Path.GetFileNameWithoutExtension(baseId);

            string name = baseId;
            string id = Bundle.GetFullId(ns, name, version);

            // If the old Id is already an automatically generated one, reuse it
            if (oldId != null && oldId.StartsWith(id))
                return name;

            int n = 1;

            while (AddinIdExists(id))
            {
                name = baseId + "_" + n;
                id = Bundle.GetFullId(ns, name, version);
                n++;
            }
            return name;
        }

        internal bool RemoveAddinDescriptionFile(string file)
        {
            // Removes an add-in description and shifts up alternate instances of the description file
            // (so xxx,1.0.maddin_2 will become xxx,1.0.maddin, xxx,1.0.maddin_3 -> xxx,1.0.maddin_2, etc)

            if (!SafeDelete(file))
                return false;

            int dversion;
            if (file.EndsWith(".maddin"))
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
                SafeDeleteDir(Path.Combine(AddinPrivateDataPath, Path.GetFileNameWithoutExtension(file)));
            }

            return true;
        }

        internal string GetDescriptionPath(string domain, string id)
        {
            return Path.Combine(Path.Combine(AddinCachePath, domain), id + ".maddin");
        }

        internal void UninstallAddin(string domain, string addinId, string addinFile, AddinScanResult scanResult)
        {
            BundleDescription desc;

            if (!GetAddinDescription(domain, addinId, addinFile, out desc))
            {
                // If we can't get information about the old assembly, just regenerate all relation data
                scanResult.RegenerateRelationData = true;
                return;
            }

            scanResult.AddRemovedAddin(addinId);

            // If the add-in didn't exist, there is nothing left to do

            if (desc == null)
                return;

            // If the add-in already existed, the dependencies of the old add-in need to be re-analyzed

            Util.AddDependencies(desc, scanResult);

            if (desc.IsRoot)
                scanResult.HostIndex.RemoveHostData(desc.AddinId, desc.AddinFile);

            RemoveAddinDescriptionFile(monitor, desc.FileName);
        }

        internal bool CheckFolders(string domain)
        {
            using (fileDatabase.LockRead())
            {
                AddinScanResult scanResult = new AddinScanResult();
                scanResult.CheckOnly = true;
                scanResult.Domain = domain;
                InternalScanFolders(scanResult);
                return scanResult.ChangesFound;
            }
        }


        internal void InternalScanFolders(AddinScanResult scanResult)
        {
            try
            {
                fs.ScanStarted();
                InternalScanFolders2(scanResult);
            }
            finally
            {
                fs.ScanFinished();
            }
        }

        internal void InternalScanFolders2(AddinScanResult scanResult)
        {
            DateTime tim = DateTime.Now;

            DatabaseInfrastructureCheck();

            try
            {
                scanResult.HostIndex = GetAddinHostIndex();
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

            AddinScanner scanner = new AddinScanner(this, scanResult);

            // Check if any of the previously scanned folders has been deleted

            foreach (string file in Directory.GetFiles(AddinFolderCachePath, "*.data"))
            {
                BundleScanFolderInfo folderInfo;
                bool res = ReadFolderInfo(file, out folderInfo);
                bool validForDomain = scanResult.Domain == null || folderInfo.Domain == GlobalDomain || folderInfo.Domain == scanResult.Domain;

                if (!res || (validForDomain && !fs.DirectoryExists(folderInfo.Folder)))
                {
                    if (res)
                    {
                        // Folder has been deleted. Remove the add-ins it had.
                        scanner.UpdateDeletedAddins(folderInfo, scanResult);
                    }
                    else
                    {
                        // Folder info file corrupt. Regenerate all.
                        scanResult.ChangesFound = true;
                        scanResult.RegenerateRelationData = true;
                    }

                    if (!scanResult.CheckOnly)
                        SafeDelete(file);
                    else if (scanResult.ChangesFound)
                        return;
                }
            }

            // Look for changes in the add-in folders

            if (registry.StartupDirectory != null)
                scanner.ScanFolder(registry.StartupDirectory, null, scanResult);

            if (scanResult.CheckOnly && scanResult.ChangesFound)
                return;

            if (scanResult.Domain == null)
                scanner.ScanFolder(HostsPath, GlobalDomain, scanResult);

            if (scanResult.CheckOnly && scanResult.ChangesFound)
                return;

            foreach (string dir in registry.GlobalAddinDirectories)
            {
                if (scanResult.CheckOnly && scanResult.ChangesFound)
                    return;
                scanner.ScanFolderRec(dir, GlobalDomain, scanResult);
            }

            if (scanResult.CheckOnly || !scanResult.ChangesFound)
                return;

            currentScanResult = scanResult;

            foreach (FileToScan file in scanResult.FilesToScan)
                scanner.ScanFile(file.File, file.AddinScanFolderInfo, scanResult);

            foreach (BundleScanFolderInfo finfo in scanResult.ModifiedFolderInfos)
                SaveFolderInfo(finfo);

            SaveAddinHostIndex();
            ResetCachedData();

            if (!scanResult.ChangesFound)
                return;

            tim = DateTime.Now;

            try
            {
                if (scanResult.RegenerateRelationData)
                {
                    scanResult.AddinsToUpdate = null;
                    scanResult.AddinsToUpdateRelations = null;
                }

                GenerateAddinExtensionMapsInternal(scanResult.Domain, scanResult.AddinsToUpdate, scanResult.AddinsToUpdateRelations, scanResult.RemovedAddins);
            }
            catch (Exception ex)
            {
                fatalDatabseError = true;
            }

            SaveAddinHostIndex();
        }

        void GenerateAddinExtensionMapsInternal(string domain, List<string> addinsToUpdate, List<string> addinsToUpdateRelations, List<string> removedAddins)
        {
            AddinUpdateData updateData = new AddinUpdateData(this);

            // Clear cached data
            cachedAddinSetupInfos.Clear();

            // Collect all information

            AddinIndex addinHash = new AddinIndex();

            Hashtable changedAddins = null;
            ArrayList descriptionsToSave = new ArrayList();
            ArrayList files = new ArrayList();

            bool partialGeneration = addinsToUpdate != null;
            string[] domains = GetDomains().Where(d => d == domain || d == GlobalDomain).ToArray();

            // Get the files to be updated

            if (partialGeneration)
            {
                changedAddins = new Hashtable();

                // Get the files and ids of all add-ins that have to be updated
                // Include removed add-ins: if there are several instances of the same add-in, removing one of
                // them will make other instances to show up. If there is a single instance, its files are
                // already removed.
                foreach (string sa in addinsToUpdate.Union(removedAddins))
                {
                    changedAddins[sa] = sa;

                    foreach (string file in GetAddinFiles(sa, domains))
                    {
                        if (!files.Contains(file))
                        {
                            files.Add(file);
                            string an = Path.GetFileNameWithoutExtension(file);
                            changedAddins[an] = an;
                        }
                    }
                }

                // Get the files and ids of all add-ins whose relations have to be updated
                foreach (string sa in addinsToUpdateRelations)
                {
                    foreach (string file in GetAddinFiles(sa, domains))
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
                    files.AddRange(fileDatabase.GetDirectoryFiles(Path.Combine(AddinCachePath, dom), "*.maddin"));
            }

            // Load the descriptions.
            foreach (string file in files)
            {

                BundleDescription conf;

                if (!ReadAddinDescription(file, out conf))
                {
                    SafeDelete(file);
                    continue;
                }

                // If the original file does not exist, the description can be deleted
                if (!fs.FileExists(conf.AddinFile))
                {
                    SafeDelete(file);
                    continue;
                }

                // Remove old data from the description. Remove the data of the add-ins that
                // have changed. This data will be re-added later.

                conf.UnmergeExternalData(changedAddins);
                descriptionsToSave.Add(conf);

                addinHash.Add(conf);
            }

            // Sort the add-ins, to make sure add-ins are processed before
            // all their dependencies

            var sorted = addinHash.GetSortedAddins();

            // Register extension points and node sets
            foreach (BundleDescription conf in sorted)
                CollectExtensionPointData(conf, updateData);

            // Register extensions
            foreach (BundleDescription conf in sorted)
            {
                if (changedAddins == null || changedAddins.ContainsKey(conf.AddinId))
                {
                    CollectExtensionData(monitor, addinHash, conf, updateData);
                }
            }

            // Save the maps
            foreach (BundleDescription conf in descriptionsToSave)
            {
                ConsolidateExtensions(conf);
                conf.SaveBinary(fileDatabase);
            }
        }

        internal void ResetCachedData()
        {
            ResetBasicCachedData();
            hostIndex = null;
            cachedAddinSetupInfos.Clear();

            //if (clampBundle != null)
            //    clampBundle.ResetCachedData();
        }

        internal void ResetBasicCachedData()
        {
            allSetupInfos = null;
            addinSetupInfos = null;
            rootSetupInfos = null;
        }

        internal AddinHostIndex GetAddinHostIndex()
        {
            if (hostIndex != null)
                return hostIndex;

            using (fileDatabase.LockRead())
            {
                if (fileDatabase.Exists(HostIndexFile))
                    hostIndex = AddinHostIndex.Read(fileDatabase, HostIndexFile);
                else
                    hostIndex = new AddinHostIndex();
            }
            return hostIndex;
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
                if (!Directory.Exists(AddinCachePath))
                {
                    Directory.CreateDirectory(AddinCachePath);
                    hasChanges = true;
                }

                if (!Directory.Exists(AddinFolderCachePath))
                {
                    Directory.CreateDirectory(AddinFolderCachePath);
                    hasChanges = true;
                }

                // Make sure we can write in those folders

                Util.CheckWrittableFloder(AddinCachePath);
                Util.CheckWrittableFloder(AddinFolderCachePath);

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

        private void RunScannerProcess(IProgressStatus monitor)
        {
            ISetupHandler setup = GetSetupHandler();

            IProgressStatus scanMonitor = monitor;
            ArrayList pparams = new ArrayList();

            bool retry = false;
            do
            {
                try
                {
                    if (monitor.LogLevel > 1)
                        monitor.Log("Looking for addins");
                    setup.Scan(scanMonitor, registry, null, (string[])pparams.ToArray(typeof(string)));
                    retry = false;
                }
                catch (Exception ex)
                {
                    ProcessFailedException pex = ex as ProcessFailedException;
                    if (pex != null)
                    {
                        // Get the last logged operation.
                        if (pex.LastLog.StartsWith("scan:"))
                        {
                            // It crashed while scanning a file. Add the file to the ignore list and try again.
                            string file = pex.LastLog.Substring(5);
                            pparams.Add(file);
                            monitor.ReportWarning("Could not scan file: " + file);
                            retry = true;
                            continue;
                        }
                    }
                    fatalDatabseError = true;
                    // If the process has crashed, try to do a new scan, this time using verbose log,
                    // to give the user more information about the origin of the crash.
                    if (pex != null && !retry)
                    {
                        monitor.ReportError("Add-in scan operation failed. The runtime may have encountered an error while trying to load an assembly.", null);
                        if (monitor.LogLevel <= 1)
                        {
                            // Re-scan again using verbose log, to make it easy to find the origin of the error.
                            retry = true;
                            scanMonitor = new ConsoleProgressStatus(true);
                        }
                    }
                    else
                        retry = false;

                    if (!retry)
                    {
                        var pfex = ex as ProcessFailedException;
                        monitor.ReportError("Add-in scan operation failed", pfex != null ? pfex.InnerException : ex);
                        monitor.Cancel();
                        return;
                    }
                }
            }
            while (retry);
        }

        private bool DatabaseInfrastructureCheck(IProgressStatus monitor)
        {
            // Do some sanity check, to make sure the basic database infrastructure can be created

            bool hasChanges = false;

            try
            {

                if (!Directory.Exists(AddinCachePath))
                {
                    Directory.CreateDirectory(AddinCachePath);
                    hasChanges = true;
                }

                if (!Directory.Exists(AddinFolderCachePath))
                {
                    Directory.CreateDirectory(AddinFolderCachePath);
                    hasChanges = true;
                }

                // Make sure we can write in those folders

                Util.CheckWrittableFloder(AddinCachePath);
                Util.CheckWrittableFloder(AddinFolderCachePath);

                fatalDatabseError = false;
            }
            catch (Exception ex)
            {
                monitor.ReportError("Add-in cache directory could not be created", ex);
                fatalDatabseError = true;
                monitor.Cancel();
            }
            return hasChanges;
        }

        private ISetupHandler GetSetupHandler()
        {
            if (fs.RequiresIsolation)
                return new SetupDomain();
            else
                return new SetupLocal();
        }

        private IEnumerable<Bundle> InternalGetInstalledAddins(string domain, AddinSearchFlagsInternal type)
        {
            return InternalGetInstalledAddins(domain, null, type);
        }

        private IEnumerable<Bundle> InternalGetInstalledAddins(string domain, string idFilter, AddinSearchFlagsInternal type)
        {
            if ((type & AddinSearchFlagsInternal.LatestVersionsOnly) != 0)
                throw new InvalidOperationException("LatestVersionsOnly flag not supported here");

            if (allSetupInfos == null)
            {
                Dictionary<string, Bundle> adict = new Dictionary<string, Bundle>();

                // Global add-ins are valid for any private domain
                if (domain != BundleDatabase.GlobalDomain)
                    FindInstalledAddins(adict, BundleDatabase.GlobalDomain, idFilter);

                FindInstalledAddins(adict, domain, idFilter);
                List<Bundle> alist = new List<Bundle>(adict.Values);
                UpdateLastVersionFlags(alist);
                if (idFilter != null)
                    return alist;
                allSetupInfos = alist;
            }
            if ((type & AddinSearchFlagsInternal.IncludeAll) == AddinSearchFlagsInternal.IncludeAll)
                return FilterById(allSetupInfos, idFilter);

            if ((type & AddinSearchFlagsInternal.IncludeAddins) == AddinSearchFlagsInternal.IncludeAddins)
            {
                if (addinSetupInfos == null)
                {
                    addinSetupInfos = new List<Bundle>();
                    foreach (Bundle adn in allSetupInfos)
                        if (!adn.Description.IsRoot)
                            addinSetupInfos.Add(adn);
                }
                return FilterById(addinSetupInfos, idFilter);
            }
            else
            {
                if (rootSetupInfos == null)
                {
                    rootSetupInfos = new List<Bundle>();
                    foreach (Bundle adn in allSetupInfos)
                        if (adn.Description.IsRoot)
                            rootSetupInfos.Add(adn);
                }
                return FilterById(rootSetupInfos, idFilter);
            }
        }

        private IEnumerable<Bundle> FilterById(List<Bundle> addins, string id)
        {
            if (id == null)
                return addins;
            return addins.Where(a => Bundle.GetIdName(a.Id) == id);
        }

        private void FindInstalledAddins(Dictionary<string, Bundle> result, string domain, string idFilter)
        {
            if (idFilter == null)
                idFilter = "*";
            string dir = Path.Combine(AddinCachePath, domain);
            if (Directory.Exists(dir))
            {
                foreach (string file in fileDatabase.GetDirectoryFiles(dir, idFilter + ",*.maddin"))
                {
                    string id = Path.GetFileNameWithoutExtension(file);
                    if (!result.ContainsKey(id))
                    {
                        var adesc = GetInstalledDomainAddin(domain, id, true, false, false);
                        if (adesc != null)
                            result.Add(id, adesc);
                    }
                }
            }
        }

        private void UpdateLastVersionFlags(List<Bundle> addins)
        {
            Dictionary<string, string> versions = new Dictionary<string, string>();
            foreach (Bundle a in addins)
            {
                string last;
                string id, version;
                Bundle.GetIdParts(a.Id, out id, out version);
                if (!versions.TryGetValue(id, out last) || Bundle.CompareVersions(last, version) > 0)
                    versions[id] = version;
            }
            foreach (Bundle a in addins)
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

        private Bundle GetInstalledDomainAddin(string domain, string id, bool exactVersionMatch, bool enabledOnly, bool dbLockCheck)
        {
            Bundle sinfo = null;
            string idd = id + " " + domain;
            object ob = cachedAddinSetupInfos[idd];
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
                    cachedAddinSetupInfos[idd] = sinfo;
                    if (!enabledOnly || sinfo.Enabled)
                        return sinfo;
                    if (exactVersionMatch)
                    {
                        // Cache lookups with negative result
                        cachedAddinSetupInfos[idd] = this;
                        return null;
                    }
                }

                // Exact version not found. Look for a compatible version
                if (!exactVersionMatch)
                {
                    sinfo = null;
                    string version, name, bestVersion = null;
                    Bundle.GetIdParts(id, out name, out version);

                    foreach (Bundle ia in InternalGetInstalledAddins(domain, name, AddinSearchFlagsInternal.IncludeAll))
                    {
                        if ((!enabledOnly || ia.Enabled) &&
                            (version.Length == 0 || ia.SupportsVersion(version)) &&
                            (bestVersion == null || Bundle.CompareVersions(bestVersion, ia.Version) > 0))
                        {
                            bestVersion = ia.Version;
                            sinfo = ia;
                        }
                    }
                    if (sinfo != null)
                    {
                        cachedAddinSetupInfos[idd] = sinfo;
                        return sinfo;
                    }
                }

                // Cache lookups with negative result
                // Ignore the 'not installed' flag when disabled add-ins are allowed
                if (enabledOnly)
                    cachedAddinSetupInfos[idd] = this;
                return null;
            }
        }

        private void InternalCheck(string domain)
        {
            // If the database is broken, don't try to regenerate it at every check.
            if (fatalDatabseError)
                return;

            bool update = false;
            using (fileDatabase.LockRead())
            {
                if (!Directory.Exists(AddinCachePath))
                {
                    update = true;
                }
            }
            if (update)
                Update(domain);
        }

        private bool AddinIdExists(string id)
        {
            foreach (string d in fileDatabase.GetDirectories(AddinCachePath))
            {
                if (fileDatabase.Exists(Path.Combine(d, id + ".addin")))
                    return true;
            }
            return false;
        }
        private void SaveAddinHostIndex()
        {
            if (hostIndex != null)
                hostIndex.Write(fileDatabase, HostIndexFile);
        }

        private string[] GetDomains()
        {
            string[] dirs = fileDatabase.GetDirectories(AddinCachePath);
            string[] ids = new string[dirs.Length];
            for (int n = 0; n < dirs.Length; n++)
                ids[n] = Path.GetFileName(dirs[n]);
            return ids;
        }

        private IEnumerable GetAddinFiles(string fullId, string[] domains)
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

        private void CollectExtensionPointData(BundleDescription conf, AddinUpdateData updateData)
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

        private void CollectExtensionData(AddinIndex addinHash, BundleDescription conf, AddinUpdateData updateData)
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
                    if (monitor.LogLevel > 1)
                    {
                        string w = "An optional module of the add-in '" + conf.AddinId + "' could not be updated because some of its dependencies are missing or not compatible:";
                        w += BuildMissingAddinsList(addinHash, conf, missingDeps);
                    }
                }
                else
                    CollectModuleExtensionData(conf, module, updateData, addinHash);
            }
        }

        private string BuildMissingAddinsList(AddinIndex addinHash, BundleDescription conf, IEnumerable<string> missingDeps)
        {
            string w = "";
            foreach (string dep in missingDeps)
            {
                var found = addinHash.GetSimilarExistingAddin(conf, dep);
                if (found == null)
                    w += "\n  missing: " + dep;
                else
                    w += "\n  required: " + dep + ", found: " + found.AddinId;
            }
            return w;
        }

        private void CollectModuleExtensionData(BundleDescription conf, ModuleDescription module, AddinUpdateData updateData, AddinIndex index)
        {
            foreach (Extension ext in module.Extensions)
            {
                updateData.RelExtensions++;
                updateData.RegisterExtension(conf, module, ext);
                AddChildExtensions(conf, module, updateData, index, ext.Path, ext.ExtensionNodes, false);
            }
        }
        #endregion


    }
}
