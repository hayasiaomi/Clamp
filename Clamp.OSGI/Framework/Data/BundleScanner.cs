using Clamp.OSGI.Framework.Data.Annotation;
using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework.Data
{
    /// <summary>
    /// Bundle的检测者
    /// </summary>
    internal class BundleScanner
    {
        private Dictionary<IAssemblyReflector, object> coreAssemblies = new Dictionary<IAssemblyReflector, object>();
        private BundleDatabase database;
        private BundleFileSystemExtension fs;
        public BundleScanner(BundleDatabase database, BundleScanResult scanResult)
        {
            this.database = database;
            fs = database.FileSystem;
        }
        #region public mehtod

        public void UpdateDeletedBundles(BundleScanFolderInfo folderInfo, BundleScanResult scanResult)
        {
            ArrayList missing = folderInfo.GetMissingBundles(fs);

            if (missing.Count > 0)
            {
                if (fs.DirectoryExists(folderInfo.Folder))
                    scanResult.RegisterModifiedFolderInfo(folderInfo);

                scanResult.ChangesFound = true;

                if (scanResult.CheckOnly)
                    return;

                foreach (BundleFileInfo info in missing)
                {
                    database.UninstallBundle(info.Domain, info.BundleId, info.File, scanResult);
                }
            }
        }

        public void ScanFolder(string path, string domain, BundleScanResult scanResult)
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

                // First of all, look for .addin files. Bundle files must be processed before
                // assemblies, because they may add files to the ignore list (i.e., assemblies
                // included in .addin files won't be scanned twice).
                foreach (string file in files)
                {
                    if (file.EndsWith(".bundle.xml") || file.EndsWith(".bundle"))
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
                    if (Path.GetExtension(file).EndsWith(".bundles"))
                    {
                        ScanBundlesFile(file, domain, scanResult);
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

            UpdateDeletedBundles(folderInfo, scanResult);
        }

        public BundleDescription ScanSingleFile(string file, BundleScanResult scanResult)
        {
            BundleDescription config = null;

            try
            {
                string ext = Path.GetExtension(file).ToLower();
                bool scanSuccessful;

                if (ext == ".dll" || ext == ".exe")
                    scanSuccessful = ScanAssembly(file, scanResult, out config);
                else
                    scanSuccessful = ScanConfigAssemblies(file, scanResult, out config);

                if (scanSuccessful && config != null)
                {

                    config.Domain = "global";
                    if (config.Version.Length == 0)
                        config.Version = "0.0.0.0";

                    if (config.LocalId.Length == 0)
                    {
                        // Generate an internal id for this add-in
                        config.LocalId = database.GetUniqueBundleId(file, "", config.Namespace, config.Version);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return config;
        }
        public void ScanBundlesFile(string file, string domain, BundleScanResult scanResult)
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
        public void ScanFolderRec(string dir, string domain, BundleScanResult scanResult)
        {
            ScanFolder(dir, domain, scanResult);

            if (!fs.DirectoryExists(dir))
                return;

            foreach (string sd in fs.GetDirectories(dir))
                ScanFolderRec(sd, domain, scanResult);
        }
        public void ScanFile(string file, BundleScanFolderInfo folderInfo, BundleScanResult scanResult)
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

            string scannedBundleId = null;
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

                    BundleFileInfo fi = folderInfo.GetBundleFileInfo(file);

                    // If version is not specified, make up one
                    if (config.Version.Length == 0)
                    {
                        config.Version = "0.0.0.0";
                    }

                    if (config.LocalId.Length == 0)
                    {
                        // Generate an internal id for this add-in
                        config.LocalId = database.GetUniqueBundleId(file, (fi != null ? fi.BundleId : null), config.Namespace, config.Version);
                        config.HasUserId = false;
                    }

                    // Check errors in the description
                    List<string> errors = new List<string>();

                    if (database.IsGlobalRegistry && config.BundleId.IndexOf('.') == -1)
                    {
                        errors.Add("Add-ins registered in the global registry must have a namespace.");
                    }

                    if (errors.Count > 0)
                    {
                        scanSuccessful = false;
                    }

                    // Make sure all extensions sets are initialized with the correct add-in id

                    config.SetExtensionsBundleId(config.BundleId);

                    scanResult.ChangesFound = true;

                    // If the add-in already existed, try to reuse the relation data it had.
                    // Also, the dependencies of the old add-in need to be re-analyzed

                    BundleDescription existingDescription = null;

                    bool res = database.GetBundleDescription(folderInfo.Domain, config.BundleId, config.BundleFile, out existingDescription);

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
                    if (fi != null && fi.IsBundle && fi.BundleId != config.BundleId)
                    {
                        database.UninstallBundle(folderInfo.Domain, fi.BundleId, fi.File, scanResult);

                        // If the add-in version has changed, regenerate everything again since old data can't be reused
                        if (Bundle.GetIdName(fi.BundleId) == Bundle.GetIdName(config.BundleId))
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
                                scanResult.HostIndex.RegisterAssembly(asmFile, config.BundleId, config.BundleFile, config.Domain);
                            }
                        }

                        if (database.SaveDescription(config, replaceFileName))
                        {
                            // The new dependencies also have to be updated
                            Util.AddDependencies(config, scanResult);
                            scanResult.AddBundleToUpdate(config.BundleId);
                            scannedBundleId = config.BundleId;
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
                BundleFileInfo ainfo = folderInfo.SetLastScanTime(file, scannedBundleId, scannedIsRoot, fs.GetLastWriteTime(file), !scanSuccessful);

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
        #endregion

        #region internal method

        internal string GetDefaultTypeExtensionPath(BundleDescription desc, string typeFullName)
        {
            return "/" + Bundle.GetIdName(desc.BundleId) + "/TypeExtensions/" + typeFullName;
        }
        #endregion

        #region private method

        private void ReportReflectionException(Exception ex, BundleDescription config, BundleScanResult scanResult)
        {
            scanResult.AddFileToWithFailure(config.BundleFile);

            ReflectionTypeLoadException rex = ex as ReflectionTypeLoadException;
            if (rex != null)
            {
                //TODO 日志处理
            }
        }

        private bool ScanConfigAssemblies(string filePath, BundleScanResult scanResult, out BundleDescription config)
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
                config.BundleFile = filePath;

                return ScanDescription(reflector, config, null, scanResult);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool ScanAssembly(string filePath, BundleScanResult scanResult, out BundleDescription config)
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
                    // In this case, only scan the assembly if it has the Bundle attribute.
                    BundleAttribute att = (BundleAttribute)reflector.GetCustomAttribute(asm, typeof(BundleAttribute), false);
                    if (att == null)
                    {
                        config = null;
                        return true;
                    }

                    if (config == null)
                        config = new BundleDescription();
                }

                config.SetBasePath(Path.GetDirectoryName(filePath));
                config.BundleFile = filePath;

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
        private void RegisterFileToScan(string file, BundleScanResult scanResult, BundleScanFolderInfo folderInfo)
        {
            if (scanResult.LocateAssembliesOnly)
                return;

            BundleFileInfo finfo = folderInfo.GetBundleFileInfo(file);

            bool added = false;

            if (finfo != null && (!finfo.IsBundle || finfo.Domain == folderInfo.GetDomain(finfo.IsRoot)) && fs.GetLastWriteTime(file) == finfo.LastScan && !scanResult.RegenerateAllData)
            {
                if (finfo.ScanError)
                {
                    // Always schedule the file for scan if there was an error in a previous scan.
                    // However, don't set ChangesFound=true, in this way if there isn't any other
                    // change in the registry, the file won't be scanned again.
                    scanResult.AddFileToScan(file, folderInfo);
                    added = true;
                }

                if (!finfo.IsBundle)
                    return;

                if (database.BundleDescriptionExists(finfo.Domain, finfo.BundleId))
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

        private IAssemblyReflector GetReflector(BundleScanResult scanResult, string filePath)
        {
            IAssemblyReflector reflector = fs.GetReflectorForFile(scanResult, filePath);
            object coreAssembly;
            if (!coreAssemblies.TryGetValue(reflector, out coreAssembly))
            {
                coreAssemblies[reflector] = coreAssembly = reflector.LoadAssembly(GetType().Assembly.Location);
            }
            return reflector;
        }

        /// <summary>
        /// 检测Bundle的信息
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="config"></param>
        /// <param name="rootAssembly"></param>
        /// <param name="scanResult"></param>
        /// <returns></returns>
        private bool ScanDescription(IAssemblyReflector reflector, BundleDescription config, object rootAssembly, BundleScanResult scanResult)
        {
            ArrayList assemblies = new ArrayList();

            try
            {
                string rootAsmFile = null;

                if (rootAssembly != null)
                {
                    ScanAssemblyBundleHeaders(reflector, config, rootAssembly, scanResult);
                    ScanAssemblyImports(reflector, config.MainModule, rootAssembly);
                    assemblies.Add(rootAssembly);
                    rootAsmFile = Path.GetFileName(config.BundleFile);
                }

                // The assembly list may be modified while scanning the headers, so
                // we use a for loop instead of a foreach
                for (int n = 0; n < config.MainModule.Assemblies.Count; n++)
                {
                    string s = config.MainModule.Assemblies[n];
                    string asmFile = Path.GetFullPath(Path.Combine(config.BasePath, s));
                    scanResult.AddPathToIgnore(asmFile);
                    if (s == rootAsmFile || config.MainModule.IgnorePaths.Contains(s))
                        continue;
                    object asm = reflector.LoadAssembly(asmFile);
                    assemblies.Add(asm);
                    ScanAssemblyBundleHeaders(reflector, config, asm, scanResult);
                    ScanAssemblyImports(reflector, config.MainModule, asm);
                }

                // Add all data files to the ignore file list. It avoids scanning assemblies
                // which are included as 'data' in an add-in.
                foreach (string df in config.MainModule.DataFiles)
                {
                    string file = Path.Combine(config.BasePath, df);
                    scanResult.AddPathToIgnore(Path.GetFullPath(file));
                }
                foreach (string df in config.MainModule.IgnorePaths)
                {
                    string path = Path.Combine(config.BasePath, df);
                    scanResult.AddPathToIgnore(Path.GetFullPath(path));
                }

                // The add-in id and version must be already assigned at this point

                // Clean host data from the index. New data will be added.
                if (scanResult.HostIndex != null)
                    scanResult.HostIndex.RemoveHostData(config.BundleId, config.BundleFile);

                foreach (object asm in assemblies)
                    ScanAssemblyContents(reflector, config, config.MainModule, asm, scanResult);

            }
            catch (Exception ex)
            {
                ReportReflectionException(ex, config, scanResult);

                return false;
            }

            // Extension node types may have child nodes declared as attributes. Find them.

            Hashtable internalNodeSets = new Hashtable();

            ArrayList setsCopy = new ArrayList();
            setsCopy.AddRange(config.ExtensionNodeSets);
            foreach (ExtensionNodeSet eset in setsCopy)
                ScanNodeSet(reflector, config, eset, assemblies, internalNodeSets);

            foreach (ExtensionPoint ep in config.ExtensionPoints)
            {
                ScanNodeSet(reflector, config, ep.NodeSet, assemblies, internalNodeSets);
            }

            // Now scan all modules

            if (!config.IsRoot)
            {
                foreach (ModuleDescription mod in config.OptionalModules)
                {
                    try
                    {
                        var asmList = new List<Tuple<string, object>>();
                        for (int n = 0; n < mod.Assemblies.Count; n++)
                        {
                            string s = mod.Assemblies[n];
                            if (mod.IgnorePaths.Contains(s))
                                continue;
                            string asmFile = Path.Combine(config.BasePath, s);
                            object asm = reflector.LoadAssembly(asmFile);
                            asmList.Add(new Tuple<string, object>(asmFile, asm));
                            scanResult.AddPathToIgnore(Path.GetFullPath(asmFile));
                            ScanAssemblyImports(reflector, mod, asm);
                        }
                        // Add all data files to the ignore file list. It avoids scanning assemblies
                        // which are included as 'data' in an add-in.
                        foreach (string df in mod.DataFiles)
                        {
                            string file = Path.Combine(config.BasePath, df);
                            scanResult.AddPathToIgnore(Path.GetFullPath(file));
                        }
                        foreach (string df in mod.IgnorePaths)
                        {
                            string path = Path.Combine(config.BasePath, df);
                            scanResult.AddPathToIgnore(Path.GetFullPath(path));
                        }

                        foreach (var asm in asmList)
                            ScanSubmodule(mod, reflector, config, scanResult, asm.Item1, asm.Item2);

                    }
                    catch (Exception ex)
                    {
                        ReportReflectionException(ex, config, scanResult);
                    }
                }
            }


            //TODO 处理当前的DLL里面的 组件信息 
            config.StoreFileInfo();
            return true;
        }

        private bool ScanSubmodule(ModuleDescription mod, IAssemblyReflector reflector, BundleDescription config, BundleScanResult scanResult, string assemblyName, object asm)
        {
            BundleDescription mconfig;
            ScanEmbeddedDescription(assemblyName, reflector, asm, out mconfig);
            if (mconfig != null)
            {
                if (!mconfig.IsExtensionModel)
                {
                    //monitor.ReportError("Submodules can't define new add-ins: " + assemblyName, null);
                    return false;
                }
                if (mconfig.OptionalModules.Count != 0)
                {
                    //monitor.ReportError("Submodules can't define nested submodules: " + assemblyName, null);
                    return false;
                }
                if (mconfig.ConditionTypes.Count != 0)
                {
                    //monitor.ReportError("Submodules can't define condition types: " + assemblyName, null);
                    return false;
                }
                if (mconfig.ExtensionNodeSets.Count != 0)
                {
                    //monitor.ReportError("Submodules can't define extension node sets: " + assemblyName, null);
                    return false;
                }
                if (mconfig.ExtensionPoints.Count != 0)
                {
                    //monitor.ReportError("Submodules can't define extension points sets: " + assemblyName, null);
                    return false;
                }
                mod.MergeWith(mconfig.MainModule);
            }
            ScanAssemblyContents(reflector, config, mod, asm, scanResult);
            return true;
        }

        /// <summary>
        /// 检测试Bundle的元数据
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="config"></param>
        /// <param name="asm"></param>
        /// <param name="scanResult"></param>
        private void ScanAssemblyBundleHeaders(IAssemblyReflector reflector, BundleDescription config, object asm, BundleScanResult scanResult)
        {
            // Get basic add-in information
            BundleAttribute att = (BundleAttribute)reflector.GetCustomAttribute(asm, typeof(BundleAttribute), false);
            if (att != null)
            {
                if (att.Id.Length > 0)
                    config.LocalId = att.Id;
                if (att.Version.Length > 0)
                    config.Version = att.Version;
                if (att.Namespace.Length > 0)
                    config.Namespace = att.Namespace;
                if (att.Category.Length > 0)
                    config.Category = att.Category;
                if (att.CompatVersion.Length > 0)
                    config.CompatVersion = att.CompatVersion;
                if (att.Url.Length > 0)
                    config.Url = att.Url;
                config.IsRoot = att is BundleRootAttribute;
                config.EnabledByDefault = att.EnabledByDefault;
                config.Flags = att.Flags;
            }

            // Author attributes

            object[] atts = reflector.GetCustomAttributes(asm, typeof(BundleAuthorAttribute), false);
            foreach (BundleAuthorAttribute author in atts)
            {
                if (config.Author.Length == 0)
                    config.Author = author.Name;
                else
                    config.Author += ", " + author.Name;
            }

            // Name

            atts = reflector.GetCustomAttributes(asm, typeof(BundleNameAttribute), false);
            foreach (BundleNameAttribute at in atts)
            {
                if (string.IsNullOrEmpty(at.Locale))
                    config.Name = at.Name;
                else
                    config.Properties.SetPropertyValue("Name", at.Name, at.Locale);
            }

            // Description

            object catt = reflector.GetCustomAttribute(asm, typeof(AssemblyDescriptionAttribute), false);
            if (catt != null && config.Description.Length == 0)
                config.Description = ((AssemblyDescriptionAttribute)catt).Description;

            atts = reflector.GetCustomAttributes(asm, typeof(BundleDescriptionAttribute), false);
            foreach (BundleDescriptionAttribute at in atts)
            {
                if (string.IsNullOrEmpty(at.Locale))
                    config.Description = at.Description;
                else
                    config.Properties.SetPropertyValue("Description", at.Description, at.Locale);
            }

            // Copyright

            catt = reflector.GetCustomAttribute(asm, typeof(AssemblyCopyrightAttribute), false);
            if (catt != null && config.Copyright.Length == 0)
                config.Copyright = ((AssemblyCopyrightAttribute)catt).Copyright;

            // Category

            catt = reflector.GetCustomAttribute(asm, typeof(BundleCategoryAttribute), false);
            if (catt != null && config.Category.Length == 0)
                config.Category = ((BundleCategoryAttribute)catt).Category;

            // Url

            catt = reflector.GetCustomAttribute(asm, typeof(BundleUrlAttribute), false);
            if (catt != null && config.Url.Length == 0)
                config.Url = ((BundleUrlAttribute)catt).Url;

            // Flags

            catt = reflector.GetCustomAttribute(asm, typeof(BundleFlagsAttribute), false);
            if (catt != null)
                config.Flags |= ((BundleFlagsAttribute)catt).Flags;

            // Localizer

            BundleLocalizerGettextAttribute locat = (BundleLocalizerGettextAttribute)reflector.GetCustomAttribute(asm, typeof(BundleLocalizerGettextAttribute), false);
            if (locat != null)
            {
                ExtensionNodeDescription node = new ExtensionNodeDescription();
                node.SetAttribute("type", "Gettext");
                if (!string.IsNullOrEmpty(locat.Catalog))
                    node.SetAttribute("catalog", locat.Catalog);
                if (!string.IsNullOrEmpty(locat.Location))
                    node.SetAttribute("location", locat.Location);
                config.Localizer = node;
            }

            // Optional modules

            atts = reflector.GetCustomAttributes(asm, typeof(BundleModuleAttribute), false);
            foreach (BundleModuleAttribute mod in atts)
            {
                if (mod.AssemblyFile.Length > 0)
                {
                    ModuleDescription module = new ModuleDescription();
                    module.Assemblies.Add(mod.AssemblyFile);
                    config.OptionalModules.Add(module);
                }
            }
        }

        /// <summary>
        /// 检测程序集的引用
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="module"></param>
        /// <param name="asm"></param>
        private void ScanAssemblyImports(IAssemblyReflector reflector, ModuleDescription module, object asm)
        {
            object[] atts = reflector.GetCustomAttributes(asm, typeof(ImportBundleAssemblyAttribute), false);
            foreach (ImportBundleAssemblyAttribute import in atts)
            {
                if (!string.IsNullOrEmpty(import.FilePath))
                {
                    module.Assemblies.Add(import.FilePath);
                    if (!import.Scan)
                        module.IgnorePaths.Add(import.FilePath);
                }
            }
            atts = reflector.GetCustomAttributes(asm, typeof(ImportBundleFileAttribute), false);
            foreach (ImportBundleFileAttribute import in atts)
            {
                if (!string.IsNullOrEmpty(import.FilePath))
                    module.DataFiles.Add(import.FilePath);
            }
        }


        private void ScanAssemblyContents(IAssemblyReflector reflector, BundleDescription config, ModuleDescription module, object asm, BundleScanResult scanResult)
        {
            bool isMainModule = module == config.MainModule;

            // Get dependencies

            object[] deps = reflector.GetCustomAttributes(asm, typeof(BundleDependencyAttribute), false);

            foreach (BundleDependencyAttribute dep in deps)
            {
                BundleDependency adep = new BundleDependency();
                adep.BundleId = dep.Id;
                adep.Version = dep.Version;
                module.Dependencies.Add(adep);
            }

            if (isMainModule)
            {
                // Get properties

                object[] props = reflector.GetCustomAttributes(asm, typeof(BundlePropertyAttribute), false);
                foreach (BundlePropertyAttribute prop in props)
                    config.Properties.SetPropertyValue(prop.Name, prop.Value, prop.Locale);

                // Get extension points

                object[] extPoints = reflector.GetCustomAttributes(asm, typeof(ExtensionPointAttribute), false);
                foreach (ExtensionPointAttribute ext in extPoints)
                {
                    ExtensionPoint ep = config.AddExtensionPoint(ext.Path);
                    ep.Description = ext.Description;
                    ep.Name = ext.Name;
                    ep.DefaultInsertBefore = ext.DefaultInsertBefore;
                    ep.DefaultInsertAfter = ext.DefaultInsertAfter;
                    ExtensionNodeType nt = ep.AddExtensionNode(ext.NodeName, ext.NodeTypeName);
                    nt.ExtensionAttributeTypeName = ext.ExtensionAttributeTypeName;
                }
            }

            // Look for extension nodes declared using assembly attributes

            foreach (CustomAttribute att in reflector.GetRawCustomAttributes(asm, typeof(CustomExtensionAttribute), true))
                AddCustomAttributeExtension(module, att, "Type");

            // Get extensions or extension points applied to types

            foreach (object t in reflector.GetAssemblyTypes(asm))
            {

                string typeFullName = reflector.GetTypeFullName(t);

                // Look for extensions

                object[] extensionAtts = reflector.GetCustomAttributes(t, typeof(ExtensionAttribute), false);

                if (extensionAtts.Length > 0)
                {
                    Dictionary<string, ExtensionNodeDescription> nodes = new Dictionary<string, ExtensionNodeDescription>();
                    ExtensionNodeDescription uniqueNode = null;
                    foreach (ExtensionAttribute eatt in extensionAtts)
                    {
                        string path;
                        string nodeName = eatt.NodeName;

                        if (eatt.TypeName.Length > 0)
                        {
                            path = "$" + eatt.TypeName;
                        }
                        else if (eatt.Path.Length == 0)
                        {
                            path = GetBaseTypeNameList(reflector, t);
                            if (path == "$")
                            {
                                // The type does not implement any interface and has no superclass.
                                // Will be reported later as an error.
                                path = "$" + typeFullName;
                            }
                        }
                        else
                        {
                            path = eatt.Path;
                        }

                        ExtensionNodeDescription elem = module.AddExtensionNode(path, nodeName);
                        nodes[path] = elem;
                        uniqueNode = elem;

                        if (eatt.Id.Length > 0)
                        {
                            elem.SetAttribute("id", eatt.Id);
                            elem.SetAttribute("type", typeFullName);
                        }
                        else
                        {
                            elem.SetAttribute("id", typeFullName);
                        }
                        if (eatt.InsertAfter.Length > 0)
                            elem.SetAttribute("insertafter", eatt.InsertAfter);
                        if (eatt.InsertBefore.Length > 0)
                            elem.SetAttribute("insertbefore", eatt.InsertBefore);
                    }

                    // Get the node attributes

                    foreach (ExtensionAttributeAttribute eat in reflector.GetCustomAttributes(t, typeof(ExtensionAttributeAttribute), false))
                    {
                        ExtensionNodeDescription node;
                        if (!string.IsNullOrEmpty(eat.Path))
                            nodes.TryGetValue(eat.Path, out node);
                        else if (eat.TypeName.Length > 0)
                            nodes.TryGetValue("$" + eat.TypeName, out node);
                        else
                        {
                            if (nodes.Count > 1)
                                throw new Exception("Missing type or extension path value in ExtensionAttribute for type '" + typeFullName + "'.");
                            node = uniqueNode;
                        }
                        if (node == null)
                            throw new Exception("Invalid type or path value in ExtensionAttribute for type '" + typeFullName + "'.");

                        node.SetAttribute(eat.Name ?? string.Empty, eat.Value ?? string.Empty);
                    }
                }
                else
                {
                    // Look for extension points

                    extensionAtts = reflector.GetCustomAttributes(t, typeof(TypeExtensionPointAttribute), false);
                    if (extensionAtts.Length > 0 && isMainModule)
                    {
                        foreach (TypeExtensionPointAttribute epa in extensionAtts)
                        {
                            ExtensionPoint ep;

                            ExtensionNodeType nt = new ExtensionNodeType();

                            if (epa.Path.Length > 0)
                            {
                                ep = config.AddExtensionPoint(epa.Path);
                            }
                            else
                            {
                                ep = config.AddExtensionPoint(GetDefaultTypeExtensionPath(config, typeFullName));
                                nt.ObjectTypeName = typeFullName;
                            }

                            nt.Id = epa.NodeName;
                            nt.TypeName = epa.NodeTypeName;
                            nt.ExtensionAttributeTypeName = epa.ExtensionAttributeTypeName;
                            ep.NodeSet.NodeTypes.Add(nt);
                            ep.Description = epa.Description;
                            ep.Name = epa.Name;
                            ep.RootBundle = config.BundleId;
                            ep.SetExtensionsBundleId(config.BundleId);
                        }
                    }
                    else
                    {
                        // Look for custom extension attribtues
                        foreach (CustomAttribute att in reflector.GetRawCustomAttributes(t, typeof(CustomExtensionAttribute), false))
                        {
                            ExtensionNodeDescription elem = AddCustomAttributeExtension(module, att, "Type");
                            elem.SetAttribute("type", typeFullName);
                            if (string.IsNullOrEmpty(elem.GetAttribute("id")))
                                elem.SetAttribute("id", typeFullName);
                        }
                    }
                }
            }
        }

        private ExtensionNodeDescription AddCustomAttributeExtension(ModuleDescription module, CustomAttribute att, string nameName)
        {
            string path;
            if (!att.TryGetValue(CustomExtensionAttribute.PathFieldKey, out path))
                path = "%" + att.TypeName;
            ExtensionNodeDescription elem = module.AddExtensionNode(path, nameName);
            foreach (KeyValuePair<string, string> prop in att)
            {
                if (prop.Key != CustomExtensionAttribute.PathFieldKey)
                    elem.SetAttribute(prop.Key, prop.Value);
            }
            return elem;
        }

        private string GetBaseTypeNameList(IAssemblyReflector reflector, object type)
        {
            StringBuilder sb = new StringBuilder("$");
            foreach (string tn in reflector.GetBaseTypeFullNameList(type))
                sb.Append(tn).Append(',');
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private void ScanNodeSet(IAssemblyReflector reflector, BundleDescription config, ExtensionNodeSet nset, ArrayList assemblies, Hashtable internalNodeSets)
        {
            foreach (ExtensionNodeType nt in nset.NodeTypes)
                ScanNodeType(reflector, config, nt, assemblies, internalNodeSets);
        }

        private void ScanNodeType(IAssemblyReflector reflector, BundleDescription config, ExtensionNodeType nt, ArrayList assemblies, Hashtable internalNodeSets)
        {
            if (nt.TypeName.Length == 0)
                nt.TypeName = "Mono.Bundles.TypeExtensionNode";

            object ntype = FindBundleType(reflector, nt.TypeName, assemblies);

            if (ntype == null)
                return;

            // Add type information declared with attributes in the code
            ExtensionNodeAttribute nodeAtt = (ExtensionNodeAttribute)reflector.GetCustomAttribute(ntype, typeof(ExtensionNodeAttribute), true);
            if (nodeAtt != null)
            {
                if (nt.Id.Length == 0 && nodeAtt.NodeName.Length > 0)
                    nt.Id = nodeAtt.NodeName;
                if (nt.Description.Length == 0 && nodeAtt.Description.Length > 0)
                    nt.Description = nodeAtt.Description;
                if (nt.ExtensionAttributeTypeName.Length == 0 && nodeAtt.ExtensionAttributeTypeName.Length > 0)
                    nt.ExtensionAttributeTypeName = nodeAtt.ExtensionAttributeTypeName;
            }
            else
            {
                // Use the node type name as default name
                if (nt.Id.Length == 0)
                    nt.Id = reflector.GetTypeName(ntype);
            }

            // Add information about attributes
            object[] fieldAtts = reflector.GetCustomAttributes(ntype, typeof(NodeAttributeAttribute), true);
            foreach (NodeAttributeAttribute fatt in fieldAtts)
            {
                NodeTypeAttribute natt = new NodeTypeAttribute();
                natt.Name = fatt.Name;
                natt.Required = fatt.Required;
                if (fatt.TypeName != null)
                    natt.Type = fatt.TypeName;
                if (fatt.Description.Length > 0)
                    natt.Description = fatt.Description;
                nt.Attributes.Add(natt);
            }

            // Check if the type has NodeAttribute attributes applied to fields.
            foreach (object field in reflector.GetFields(ntype))
            {
                NodeAttributeAttribute fatt = (NodeAttributeAttribute)reflector.GetCustomAttribute(field, typeof(NodeAttributeAttribute), false);
                if (fatt != null)
                {
                    NodeTypeAttribute natt = new NodeTypeAttribute();
                    if (fatt.Name.Length > 0)
                        natt.Name = fatt.Name;
                    else
                        natt.Name = reflector.GetFieldName(field);
                    if (fatt.Description.Length > 0)
                        natt.Description = fatt.Description;
                    natt.Type = reflector.GetFieldTypeFullName(field);
                    natt.Required = fatt.Required;
                    nt.Attributes.Add(natt);
                }
            }

            // Check if the extension type allows children by looking for [ExtensionNodeChild] attributes.
            // First of all, look in the internalNodeSets hashtable, which is being used as cache

            string childSet = (string)internalNodeSets[nt.TypeName];

            if (childSet == null)
            {
                object[] ats = reflector.GetCustomAttributes(ntype, typeof(ExtensionNodeChildAttribute), true);
                if (ats.Length > 0)
                {
                    // Create a new node set for this type. It is necessary to create a new node set
                    // instead of just adding child ExtensionNodeType objects to the this node type
                    // because child types references can be recursive.
                    ExtensionNodeSet internalSet = new ExtensionNodeSet();
                    internalSet.Id = reflector.GetTypeName(ntype) + "_" + Guid.NewGuid().ToString();
                    foreach (ExtensionNodeChildAttribute at in ats)
                    {
                        ExtensionNodeType internalType = new ExtensionNodeType();
                        internalType.Id = at.NodeName;
                        internalType.TypeName = at.ExtensionNodeTypeName;
                        internalSet.NodeTypes.Add(internalType);
                    }
                    config.ExtensionNodeSets.Add(internalSet);
                    nt.NodeSets.Add(internalSet.Id);

                    // Register the new set in a hashtable, to allow recursive references to the
                    // same internal set.
                    internalNodeSets[nt.TypeName] = internalSet.Id;
                    internalNodeSets[reflector.GetTypeAssemblyQualifiedName(ntype)] = internalSet.Id;
                    ScanNodeSet(reflector, config, internalSet, assemblies, internalNodeSets);
                }
            }
            else
            {
                if (childSet.Length == 0)
                {
                    // The extension type does not declare children.
                    return;
                }
                // The extension type can have children. The allowed children are
                // defined in this extension set.
                nt.NodeSets.Add(childSet);
                return;
            }

            ScanNodeSet(reflector, config, nt, assemblies, internalNodeSets);
        }


        private object FindBundleType(IAssemblyReflector reflector, string typeName, ArrayList assemblies)
        {
            // Look in the current assembly
            object etype = reflector.GetType(coreAssemblies[reflector], typeName);
            if (etype != null)
                return etype;

            // Look in referenced assemblies
            foreach (object asm in assemblies)
            {
                etype = reflector.GetType(asm, typeName);
                if (etype != null)
                    return etype;
            }

            Hashtable visited = new Hashtable();

            // Look in indirectly referenced assemblies
            foreach (object asm in assemblies)
            {
                foreach (object aref in reflector.GetAssemblyReferences(asm))
                {
                    if (visited.Contains(aref))
                        continue;
                    visited.Add(aref, aref);
                    object rasm = reflector.LoadAssemblyFromReference(aref);
                    if (rasm != null)
                    {
                        etype = reflector.GetType(rasm, typeName);
                        if (etype != null)
                            return etype;
                    }
                }
            }
            return null;
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
