using Clamp.OSGI.Framework.Annotation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework.Data
{
    internal class AddinScanner
    {
        private BundleDatabase database;
        private AddinFileSystemExtension fs;
        public AddinScanner(BundleDatabase database, AddinScanResult scanResult)
        {
            this.database = database;
            fs = database.FileSystem;
        }


        public void UpdateDeletedAddins(BundleScanFolderInfo folderInfo, AddinScanResult scanResult)
        {
            ArrayList missing = folderInfo.GetMissingAddins(fs);

            if (missing.Count > 0)
            {
                if (fs.DirectoryExists(folderInfo.Folder))
                    scanResult.RegisterModifiedFolderInfo(folderInfo);

                scanResult.ChangesFound = true;

                if (scanResult.CheckOnly)
                    return;

                foreach (AddinFileInfo info in missing)
                {
                    database.UninstallAddin(info.Domain, info.AddinId, info.File, scanResult);
                }
            }
        }

        public void ScanFolder(string path, string domain, AddinScanResult scanResult)
        {
            path = Path.GetFullPath(path);

            //判断没有访问过，并且不在忽略的列表里面
            if (!scanResult.VisitFolder(path))
                return;

            BundleScanFolderInfo folderInfo;

            if (!database.GetFolderInfoForPath(path, out folderInfo))
            {
                // folderInfo file was corrupt.
                // Just in case, we are going to regenerate all relation data.
                if (!fs.DirectoryExists(path))
                    scanResult.RegenerateRelationData = true;
            }
            else
            {
                // Directory is included but it doesn't exist. Ignore it.
                if (folderInfo == null && !fs.DirectoryExists(path))
                    return;
            }

            // if domain is null it means that a new domain has to be created.

            bool sharedFolder = domain == BundleDatabase.GlobalDomain;
            bool isNewFolder = folderInfo == null;

            if (isNewFolder)
            {
                folderInfo = new BundleScanFolderInfo(path);
            }

            if (!sharedFolder && (folderInfo.SharedFolder || folderInfo.Domain != domain))
            {
                // If the folder already has a domain, reuse it
                if (domain == null && folderInfo.RootsDomain != null && folderInfo.RootsDomain != BundleDatabase.GlobalDomain)
                    domain = folderInfo.RootsDomain;
                else if (domain == null)
                {
                    folderInfo.Domain = domain = database.GetUniqueDomainId();
                    scanResult.RegenerateRelationData = true;
                }
                else
                {
                    folderInfo.Domain = domain;
                    if (!isNewFolder)
                    {
                        // Domain has changed. Update the folder info and regenerate everything.
                        scanResult.RegenerateRelationData = true;
                        scanResult.RegisterModifiedFolderInfo(folderInfo);
                    }
                }
            }
            else if (!folderInfo.SharedFolder && sharedFolder)
            {
                scanResult.RegenerateRelationData = true;
            }

            folderInfo.SharedFolder = sharedFolder;

            // If there is no domain assigned to the host, get one now
            if (scanResult.Domain == BundleDatabase.UnknownDomain)
                scanResult.Domain = domain;

            // Discard folders not belonging to the required domain
            if (scanResult.Domain != null && domain != scanResult.Domain && domain != BundleDatabase.GlobalDomain)
            {
                return;
            }

            if (fs.DirectoryExists(path))
            {
                IEnumerable<string> files = fs.GetFiles(path);

                // First of all, look for .addin files. Addin files must be processed before
                // assemblies, because they may add files to the ignore list (i.e., assemblies
                // included in .addin files won't be scanned twice).
                foreach (string file in files)
                {
                    if (file.EndsWith(".addin.xml") || file.EndsWith(".addin"))
                    {
                        RegisterFileToScan(file, scanResult, folderInfo);
                    }
                }

                // Now scan assemblies. They can also add files to the ignore list.

                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    if (ext == ".dll" || ext == ".exe")
                    {
                        RegisterFileToScan(file, scanResult, folderInfo);
                        scanResult.AddAssemblyLocation(file);
                    }
                }

                // Finally scan .addins files

                foreach (string file in files)
                {
                    if (Path.GetExtension(file).EndsWith(".addins"))
                    {
                        ScanAddinsFile(file, domain, scanResult);
                    }
                }
            }
            else if (!scanResult.LocateAssembliesOnly)
            {
                // The folder has been deleted. All add-ins defined in that folder should also be deleted.
                scanResult.RegenerateRelationData = true;
                scanResult.ChangesFound = true;
                if (scanResult.CheckOnly)
                    return;
                database.DeleteFolderInfo(folderInfo);
            }

            if (scanResult.LocateAssembliesOnly)
                return;

            // Look for deleted add-ins.

            UpdateDeletedAddins(folderInfo, scanResult);
        }

        public void ScanAddinsFile(string file, string domain, AddinScanResult scanResult)
        {
            XmlTextReader r = null;
            ArrayList directories = new ArrayList();
            ArrayList directoriesWithSubdirs = new ArrayList();
            string basePath = Path.GetDirectoryName(file);

            try
            {
                r = new XmlTextReader(fs.OpenTextFile(file));
                r.MoveToContent();
                if (r.IsEmptyElement)
                    return;
                r.ReadStartElement();
                r.MoveToContent();
                while (r.NodeType != XmlNodeType.EndElement)
                {
                    if (r.NodeType == XmlNodeType.Element && r.LocalName == "Directory")
                    {
                        string subs = r.GetAttribute("include-subdirs");
                        string sdom;
                        string share = r.GetAttribute("shared");
                        if (share == "true")
                            sdom = BundleDatabase.GlobalDomain;
                        else if (share == "false")
                            sdom = null;
                        else
                            sdom = domain; // Inherit the domain

                        string path = r.ReadElementString().Trim();
                        if (path.Length > 0)
                        {
                            if (subs == "true")
                                directoriesWithSubdirs.Add(new string[] { path, sdom });
                            else
                                directories.Add(new string[] { path, sdom });
                        }
                    }
                    else if (r.NodeType == XmlNodeType.Element && r.LocalName == "GacAssembly")
                    {
                        string aname = r.ReadElementString().Trim();
                        if (aname.Length > 0)
                        {
                            aname = Util.GetGacPath(aname);
                            if (aname != null)
                            {
                                // Gac assemblies always use the global domain
                                directories.Add(new string[] { aname, BundleDatabase.GlobalDomain });
                            }
                        }
                    }
                    else if (r.NodeType == XmlNodeType.Element && r.LocalName == "Exclude")
                    {
                        string path = r.ReadElementString().Trim();
                        if (path.Length > 0)
                        {
                            if (!Path.IsPathRooted(path))
                                path = Path.Combine(basePath, path);
                            scanResult.AddPathToIgnore(Path.GetFullPath(path));
                        }
                    }
                    else
                        r.Skip();
                    r.MoveToContent();
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                if (r != null)
                    r.Close();
            }

            foreach (string[] d in directories)
            {
                string dir = d[0];
                if (!Path.IsPathRooted(dir))
                    dir = Path.Combine(basePath, dir);
                ScanFolder(dir, d[1], scanResult);
            }
            foreach (string[] d in directoriesWithSubdirs)
            {
                string dir = d[0];
                if (!Path.IsPathRooted(dir))
                    dir = Path.Combine(basePath, dir);
                ScanFolderRec(dir, d[1], scanResult);
            }
        }

        /// <summary>
        /// 检查当前目录下的子目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="domain"></param>
        /// <param name="scanResult"></param>
        public void ScanFolderRec(string dir, string domain, AddinScanResult scanResult)
        {
            ScanFolder(dir, domain, scanResult);

            if (!fs.DirectoryExists(dir))
                return;

            foreach (string sd in fs.GetDirectories(dir))
                ScanFolderRec(sd, domain, scanResult);
        }
        public void ScanFile(string file, BundleScanFolderInfo folderInfo, AddinScanResult scanResult)
        {
            if (scanResult.IgnorePath(file))
            {
                // The file must be ignored. Maybe it caused a crash in a previous scan, or it
                // might be included by a .addin file (in which case it will be scanned when processing
                // the .addin file).
                folderInfo.SetLastScanTime(file, null, false, fs.GetLastWriteTime(file), true);
                return;
            }

            string ext = Path.GetExtension(file).ToLower();

            if ((ext == ".dll" || ext == ".exe") && !Util.IsManagedAssembly(file))
            {
                // Ignore dlls and exes which are not managed assemblies
                folderInfo.SetLastScanTime(file, null, false, fs.GetLastWriteTime(file), true);
                return;
            }

            string scannedAddinId = null;
            bool scannedIsRoot = false;
            bool scanSuccessful = false;
            BundleDescription config = null;

            try
            {
                if (ext == ".dll" || ext == ".exe")
                    scanSuccessful = ScanAssembly(file, scanResult, out config);
                else
                    scanSuccessful = ScanConfigAssemblies(file, scanResult, out config);

                if (config != null)
                {

                    AddinFileInfo fi = folderInfo.GetAddinFileInfo(file);

                    // If version is not specified, make up one
                    if (config.Version.Length == 0)
                    {
                        config.Version = "0.0.0.0";
                    }

                    if (config.LocalId.Length == 0)
                    {
                        // Generate an internal id for this add-in
                        config.LocalId = database.GetUniqueAddinId(file, (fi != null ? fi.AddinId : null), config.Namespace, config.Version);
                        config.HasUserId = false;
                    }

                    // Check errors in the description
                    List<string> errors = new List<string>();

                    if (database.IsGlobalRegistry && config.AddinId.IndexOf('.') == -1)
                    {
                        errors.Add("Add-ins registered in the global registry must have a namespace.");
                    }

                    if (errors.Count > 0)
                    {
                        scanSuccessful = false;
                    }

                    // Make sure all extensions sets are initialized with the correct add-in id

                    config.SetExtensionsAddinId(config.AddinId);

                    scanResult.ChangesFound = true;

                    // If the add-in already existed, try to reuse the relation data it had.
                    // Also, the dependencies of the old add-in need to be re-analyzed

                    BundleDescription existingDescription = null;

                    bool res = database.GetAddinDescription(folderInfo.Domain, config.AddinId, config.AddinFile, out existingDescription);

                    // If we can't get information about the old assembly, just regenerate all relation data
                    if (!res)
                        scanResult.RegenerateRelationData = true;

                    string replaceFileName = null;

                    if (existingDescription != null)
                    {
                        // Reuse old relation data
                        config.MergeExternalData(existingDescription);
                        Util.AddDependencies(existingDescription, scanResult);
                        replaceFileName = existingDescription.FileName;
                    }

                    // If the scanned file results in an add-in version different from the one obtained from
                    // previous scans, the old add-in needs to be uninstalled.
                    if (fi != null && fi.IsAddin && fi.AddinId != config.AddinId)
                    {
                        database.UninstallAddin(folderInfo.Domain, fi.AddinId, fi.File, scanResult);

                        // If the add-in version has changed, regenerate everything again since old data can't be reused
                        if (Bundle.GetIdName(fi.AddinId) == Bundle.GetIdName(config.AddinId))
                            scanResult.RegenerateRelationData = true;
                    }

                    // If a description could be generated, save it now (if the scan was successful)
                    if (scanSuccessful)
                    {
                        // Assign the domain
                        if (config.IsRoot)
                        {
                            if (folderInfo.RootsDomain == null)
                            {
                                if (scanResult.Domain != null && scanResult.Domain != BundleDatabase.UnknownDomain && scanResult.Domain != BundleDatabase.GlobalDomain)
                                    folderInfo.RootsDomain = scanResult.Domain;
                                else
                                    folderInfo.RootsDomain = database.GetUniqueDomainId();
                            }
                            config.Domain = folderInfo.RootsDomain;
                        }
                        else
                            config.Domain = folderInfo.Domain;

                        if (config.IsRoot && scanResult.HostIndex != null)
                        {
                            foreach (string f in config.MainModule.Assemblies)
                            {
                                string asmFile = Path.Combine(config.BasePath, f);
                                scanResult.HostIndex.RegisterAssembly(asmFile, config.AddinId, config.AddinFile, config.Domain);
                            }
                        }

                        if (database.SaveDescription(config, replaceFileName))
                        {
                            // The new dependencies also have to be updated
                            Util.AddDependencies(config, scanResult);
                            scanResult.AddAddinToUpdate(config.AddinId);
                            scannedAddinId = config.AddinId;
                            scannedIsRoot = config.IsRoot;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                AddinFileInfo ainfo = folderInfo.SetLastScanTime(file, scannedAddinId, scannedIsRoot, fs.GetLastWriteTime(file), !scanSuccessful);

                if (scanSuccessful && config != null)
                {
                    // Update the ignore list in the folder info object. To be used in the next scan
                    foreach (string df in config.AllIgnorePaths)
                    {
                        string path = Path.Combine(config.BasePath, df);
                        ainfo.AddPathToIgnore(Path.GetFullPath(path));
                    }
                }

            }
        }

        #region private method
        private bool ScanConfigAssemblies(string filePath, AddinScanResult scanResult, out BundleDescription config)
        {
            config = null;

            try
            {
                IAssemblyReflector reflector = GetReflector(scanResult, filePath);

                string basePath = Path.GetDirectoryName(filePath);

                using (var s = fs.OpenFile(filePath))
                {
                    config = BundleDescription.Read(s, basePath);
                }
                config.FileName = filePath;
                config.SetBasePath(basePath);
                config.AddinFile = filePath;

                return ScanDescription(reflector, config, null, scanResult);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool ScanAssembly(string filePath, AddinScanResult scanResult, out BundleDescription config)
        {
            config = null;

            try
            {
                IAssemblyReflector reflector = GetReflector(scanResult, filePath);

                object asm = reflector.LoadAssembly(filePath);

                if (asm == null)
                    throw new Exception("Could not load assembly: " + filePath);

                // Get the config file from the resources, if there is one

                if (!ScanEmbeddedDescription(filePath, reflector, asm, out config))
                    return false;

                if (config == null || config.IsExtensionModel)
                {
                    // In this case, only scan the assembly if it has the Addin attribute.
                    AddinAttribute att = (AddinAttribute)reflector.GetCustomAttribute(asm, typeof(AddinAttribute), false);
                    if (att == null)
                    {
                        config = null;
                        return true;
                    }

                    if (config == null)
                        config = new BundleDescription();
                }

                config.SetBasePath(Path.GetDirectoryName(filePath));
                config.AddinFile = filePath;

                string rasmFile = Path.GetFileName(filePath);

                if (!config.MainModule.Assemblies.Contains(rasmFile))
                    config.MainModule.Assemblies.Add(rasmFile);

                return ScanDescription(reflector, config, asm, scanResult);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void RegisterFileToScan(string file, AddinScanResult scanResult, BundleScanFolderInfo folderInfo)
        {
            if (scanResult.LocateAssembliesOnly)
                return;

            AddinFileInfo finfo = folderInfo.GetAddinFileInfo(file);

            bool added = false;

            if (finfo != null && (!finfo.IsAddin || finfo.Domain == folderInfo.GetDomain(finfo.IsRoot)) && fs.GetLastWriteTime(file) == finfo.LastScan && !scanResult.RegenerateAllData)
            {
                if (finfo.ScanError)
                {
                    // Always schedule the file for scan if there was an error in a previous scan.
                    // However, don't set ChangesFound=true, in this way if there isn't any other
                    // change in the registry, the file won't be scanned again.
                    scanResult.AddFileToScan(file, folderInfo);
                    added = true;
                }

                if (!finfo.IsAddin)
                    return;

                if (database.AddinDescriptionExists(finfo.Domain, finfo.AddinId))
                {
                    // It is an add-in and it has not changed. Paths in the ignore list
                    // are still valid, so they can be used.
                    if (finfo.IgnorePaths != null)
                        scanResult.AddPathsToIgnore(finfo.IgnorePaths);
                    return;
                }
            }

            scanResult.ChangesFound = true;

            if (!scanResult.CheckOnly && !added)
                scanResult.AddFileToScan(file, folderInfo);
        }

        private IAssemblyReflector GetReflector(AddinScanResult scanResult, string filePath)
        {
            IAssemblyReflector reflector = fs.GetReflectorForFile(scanResult, filePath);
            object coreAssembly;
            if (!coreAssemblies.TryGetValue(reflector, out coreAssembly))
            {
                if (monitor.LogLevel > 1)
                    monitor.Log("Using assembly reflector: " + reflector.GetType());
                coreAssemblies[reflector] = coreAssembly = reflector.LoadAssembly(GetType().Assembly.Location);
            }
            return reflector;
        }

        private bool ScanDescription(IAssemblyReflector reflector, BundleDescription config, object rootAssembly, AddinScanResult scanResult)
        {
            //TODO 处理当前的DLL里面的 组件信息 
            config.StoreFileInfo();
            return true;
        }

        #endregion

        #region static  method
        static bool ScanEmbeddedDescription(string filePath, IAssemblyReflector reflector, object asm, out BundleDescription config)
        {
            config = null;

            foreach (string res in reflector.GetResourceNames(asm))
            {
                if (res.EndsWith(".bundle") || res.EndsWith(".bundle.xml"))
                {
                    using (Stream s = reflector.GetResourceStream(asm, res))
                    {
                        BundleDescription ad = BundleDescription.Read(s, Path.GetDirectoryName(filePath));
                        if (config != null)
                        {
                            if (!config.IsExtensionModel && !ad.IsExtensionModel)
                            {
                                // There is more than one add-in definition
                                return false;
                            }
                            config = BundleDescription.Merge(config, ad);
                        }
                        else
                            config = ad;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
