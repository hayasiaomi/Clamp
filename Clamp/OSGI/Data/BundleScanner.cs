using Clamp.OSGI.Data.Annotation;
using Clamp.OSGI.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Data
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
            this.fs = database.FileSystem;
        }

        #region public mehtod

        /// <summary>
        /// 更新删除的Bundle
        /// </summary>
        /// <param name="folderInfo"></param>
        /// <param name="scanResult"></param>
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

        /// <summary>
        /// 检测指定文件夹和域
        /// </summary>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <param name="scanResult"></param>
        public void ScanFolder(string path, string domain, BundleScanResult scanResult)
        {
            path = Path.GetFullPath(path);

            //判断没有访问过，并且不在忽略的列表里面
            if (!scanResult.VisitFolder(path))
                return;

            BundleScanFolderInfo folderInfo;

            if (!database.GetFolderInfoForPath(path, out folderInfo))
            {
                //说明文件可能存在。只是数据可能异常了。
                // 文件夹有可能动过，为了以防万一还是重新加载数据
                if (!fs.DirectoryExists(path))
                    scanResult.RegenerateRelationData = true;
            }
            else
            {
                //拿不到Bundle的文件夹信息，但是又不存文件夹，所以就是正确的。返回
                if (folderInfo == null && !fs.DirectoryExists(path))
                    return;
            }

            bool sharedFolder = domain == BundleDatabase.GlobalDomain;
            bool isNewFolder = folderInfo == null;

            if (isNewFolder)
            {
                folderInfo = new BundleScanFolderInfo(path);
            }

            //指定的域不是公共的。并且 folderInfo是公共 或是 folderInfo的域不等指定的域
            if (!sharedFolder && (folderInfo.SharedFolder || folderInfo.Domain != domain))
            {
                // 如果域为null的时候，新建一个域，如果folderInfo的域不为空的话，就重用
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

                    //域发生的变化，所以就要重新加载
                    if (!isNewFolder)
                    {
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

                foreach (string file in files)
                {
                    if (file.EndsWith(".bundle.xml") || file.EndsWith(".bundle"))
                    {
                        RegisterFileToScan(file, scanResult, folderInfo);
                    }
                }

                foreach (string file in files)
                {
                    string ext = Path.GetExtension(file).ToLower();

                    if (ext == ".dll" || ext == ".exe")
                    {
                        RegisterFileToScan(file, scanResult, folderInfo);
                        scanResult.AddAssemblyLocation(file);
                    }
                }

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
                scanResult.RegenerateRelationData = true;
                scanResult.ChangesFound = true;

                if (scanResult.CheckOnly)
                    return;

                database.DeleteFolderInfo(folderInfo);
            }

            if (scanResult.LocateAssembliesOnly)
                return;

            // 寻找删除的Bundle
            UpdateDeletedBundles(folderInfo, scanResult);
        }


        public BundleDescription ScanSingleFile(string file, BundleScanResult scanResult)
        {
            BundleDescription bdesc = null;

            try
            {
                string ext = Path.GetExtension(file).ToLower();
                bool scanSuccessful;

                if (ext == ".dll" || ext == ".exe")
                    scanSuccessful = ScanAssembly(file, scanResult, out bdesc);
                else
                    scanSuccessful = ScanConfigAssemblies(file, scanResult, out bdesc);

                if (scanSuccessful && bdesc != null)
                {

                    bdesc.Domain = "global";

                    if (bdesc.Version.Length == 0)
                        bdesc.Version = "0.0.0.0";

                    if (bdesc.LocalId.Length == 0)
                    {
                        // Generate an internal id for this add-in
                        bdesc.LocalId = database.GetUniqueBundleId(file, "", bdesc.Namespace, bdesc.Version);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return bdesc;
        }

        /// <summary>
        /// 检测当前需要指定的Bundle目录
        /// </summary>
        /// <param name="file"></param>
        /// <param name="domain"></param>
        /// <param name="scanResult"></param>
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
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
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

        /// <summary>
        /// 检查当前指定的文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folderInfo"></param>
        /// <param name="scanResult"></param>
        public void ScanFile(string file, BundleScanFolderInfo folderInfo, BundleScanResult scanResult)
        {
            if (scanResult.IgnorePath(file))
            {
                folderInfo.SetLastScanTime(file, null, false, fs.GetLastWriteTime(file), true);
                return;
            }

            string ext = Path.GetExtension(file).ToLower();

            //判断是当前文件是不是DLL和EXE，但是却没有加载进来，这个时间就可以设置一个错误的文件到Bundle的文件夹信息中
            if ((ext == ".dll" || ext == ".exe") && !Util.IsManagedAssembly(file))
            {
                folderInfo.SetLastScanTime(file, null, false, fs.GetLastWriteTime(file), true);
                return;
            }

            string scannedBundleId = null;
            bool scannedIsBundle = false;
            bool scanSuccessful = false;

            BundleDescription bdesc = null;

            try
            {
                if (ext == ".dll" || ext == ".exe")
                    scanSuccessful = ScanAssembly(file, scanResult, out bdesc);
                else
                    scanSuccessful = ScanConfigAssemblies(file, scanResult, out bdesc);

                if (bdesc != null)
                {

                    BundleFileInfo fi = folderInfo.GetBundleFileInfo(file);

                    //没有指定版本号的话，指定一个
                    if (bdesc.Version.Length == 0)
                    {
                        bdesc.Version = "0.0.0.0";
                    }

                    if (bdesc.LocalId.Length == 0)
                    {
                        // Generate an internal id for this add-in
                        bdesc.LocalId = database.GetUniqueBundleId(file, (fi != null ? fi.BundleId : null), bdesc.Namespace, bdesc.Version);
                        bdesc.HasUserId = false;
                    }

                    // Check errors in the description
                    List<string> errors = new List<string>();

                    if (database.IsGlobalRegistry && bdesc.BundleId.IndexOf('.') == -1)
                    {
                        errors.Add("插件注册到公共域的时候，必须是有一个空间命名.");
                    }

                    if (errors.Count > 0)
                    {
                        scanSuccessful = false;
                    }

                    //确保所有的扩展信息所于的Bundle是正确的

                    bdesc.SetExtensionsBundleId(bdesc.BundleId);

                    scanResult.ChangesFound = true;

                    // 如果以前的Bundle信息存在，那么就重用以前的Bundle信息。而依赖的信息需要重新分析
                    BundleDescription existingDescription = null;

                    bool res = database.GetBundleDescription(folderInfo.Domain, bdesc.BundleId, bdesc.BundleFile, out existingDescription);

                    // 如果不能加载到以前的Bundle信息就需要重新加载相关的数据
                    if (!res)
                        scanResult.RegenerateRelationData = true;

                    string replaceFileName = null;

                    if (existingDescription != null)
                    {
                        // 重新利用以前的Bundle信息
                        bdesc.MergeExternalData(existingDescription);

                        Util.AddDependencies(existingDescription, scanResult);

                        replaceFileName = existingDescription.FileName;
                    }

                    //如果获得Bundle文件信息跟Bundle的详细信息不同的时就卸载掉当前的Bundle
                    if (fi != null && fi.IsNotNullBundleId && fi.BundleId != bdesc.BundleId)
                    {
                        database.UninstallBundle(folderInfo.Domain, fi.BundleId, fi.File, scanResult);

                        // 因为Bundle的版本号发生了变，必须重新加载，不能在用以前的数据
                        if (Bundle.GetIdName(fi.BundleId) == Bundle.GetIdName(bdesc.BundleId))
                            scanResult.RegenerateRelationData = true;
                    }

                    // If a description could be generated, save it now (if the scan was successful)
                    if (scanSuccessful)
                    {
                        // Assign the domain
                        if (bdesc.IsBundle)
                        {
                            if (folderInfo.RootsDomain == null)
                            {
                                if (scanResult.Domain != null && scanResult.Domain != BundleDatabase.UnknownDomain && scanResult.Domain != BundleDatabase.GlobalDomain)
                                    folderInfo.RootsDomain = scanResult.Domain;
                                else
                                    folderInfo.RootsDomain = database.GetUniqueDomainId();
                            }

                            bdesc.Domain = folderInfo.RootsDomain;
                        }
                        else
                            bdesc.Domain = folderInfo.Domain;

                        if (bdesc.IsBundle && scanResult.ActivationIndex != null)
                        {
                            foreach (string f in bdesc.MainModule.Assemblies)
                            {
                                string asmFile = Path.Combine(bdesc.BasePath, f);

                                scanResult.ActivationIndex.RegisterAssembly(asmFile, bdesc.BundleId, bdesc.BundleFile, bdesc.Domain);
                            }
                        }

                        if (database.SaveDescription(bdesc, replaceFileName))
                        {
                            // The new dependencies also have to be updated
                            Util.AddDependencies(bdesc, scanResult);

                            scanResult.AddBundleToUpdate(bdesc.BundleId);

                            scannedBundleId = bdesc.BundleId;
                            scannedIsBundle = bdesc.IsBundle;

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
                BundleFileInfo ainfo = folderInfo.SetLastScanTime(file, scannedBundleId, scannedIsBundle, fs.GetLastWriteTime(file), !scanSuccessful);

                if (scanSuccessful && bdesc != null)
                {
                    // Update the ignore list in the folder info object. To be used in the next scan
                    foreach (string df in bdesc.AllIgnorePaths)
                    {
                        string path = Path.Combine(bdesc.BasePath, df);

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

        /// <summary>
        /// 检测指定程序集文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="scanResult"></param>
        /// <param name="bdesc"></param>
        /// <returns></returns>
        private bool ScanAssembly(string filePath, BundleScanResult scanResult, out BundleDescription bdesc)
        {
            bdesc = null;

            try
            {
                IAssemblyReflector reflector = GetReflector(scanResult, filePath);

                object asm = reflector.LoadAssembly(filePath);

                if (asm == null)
                    throw new Exception("不能加载程序集: " + filePath);

                if (!ScanEmbeddedDescription(filePath, reflector, asm, out bdesc))
                    return false;

                if (bdesc == null || bdesc.IsExtensionModel)
                {
                    // In this case, only scan the assembly if it has the Bundle attribute.
                    BundleAttribute att = (BundleAttribute)reflector.GetCustomAttribute(asm, typeof(BundleAttribute), false);

                    if (att == null)
                    {
                        bdesc = null;
                        return true;
                    }

                    if (bdesc == null)
                        bdesc = new BundleDescription();
                }

                bdesc.SetBasePath(Path.GetDirectoryName(filePath));
                bdesc.BundleFile = filePath;

                string rasmFile = Path.GetFileName(filePath);

                if (!bdesc.MainModule.Assemblies.Contains(rasmFile))
                    bdesc.MainModule.Assemblies.Add(rasmFile);

                return ScanDescription(reflector, bdesc, asm, scanResult);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 注册需要检测的文件到对应的BundleScanResult对象里面去
        /// </summary>
        /// <param name="file"></param>
        /// <param name="scanResult"></param>
        /// <param name="folderInfo"></param>
        private void RegisterFileToScan(string file, BundleScanResult scanResult, BundleScanFolderInfo folderInfo)
        {
            if (scanResult.LocateAssembliesOnly)
                return;

            BundleFileInfo finfo = folderInfo.GetBundleFileInfo(file);

            bool added = false;

            if (finfo != null && (!finfo.IsNotNullBundleId || finfo.Domain == folderInfo.GetDomain(finfo.IsBundle)) && fs.GetLastWriteTime(file) == finfo.LastScan && !scanResult.RegenerateAllData)
            {
                if (finfo.ScanError)
                {
                    scanResult.AddFileToScan(file, folderInfo);
                    added = true;
                }

                if (!finfo.IsNotNullBundleId)
                    return;

                if (database.BundleDescriptionExists(finfo.Domain, finfo.BundleId))
                {
                    //如果是Bundle，并且还有任何变化的话，他的忽略的集合是要被用的。
                    if (finfo.IgnorePaths != null)
                        scanResult.AddPathsToIgnore(finfo.IgnorePaths);

                    return;
                }
            }

            scanResult.ChangesFound = true;

            if (!scanResult.CheckOnly && !added)
                scanResult.AddFileToScan(file, folderInfo);
        }

        /// <summary>
        /// 获得程序集反射器
        /// </summary>
        /// <param name="scanResult"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
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

                //检测引入的程序集里面的信息
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

                // 所有的数据文件都在加入忽略的列表里面，这样子在检测的时候，不会被分析
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

                // 清除在Bundle激活的索引类里面对应的Bundle.后面在增加新的进来
                if (scanResult.ActivationIndex != null)
                    scanResult.ActivationIndex.RemoveHostData(config.BundleId, config.BundleFile);

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

            //检测扩展点里面的扩展节点的信息
            foreach (ExtensionPoint ep in config.ExtensionPoints)
            {
                ScanNodeSet(reflector, config, ep.NodeSet, assemblies, internalNodeSets);
            }

            // Now scan all modules

            if (!config.IsBundle)
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

        /// <summary>
        /// 检测Bundle的子模块，即是可选模块
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="reflector"></param>
        /// <param name="config"></param>
        /// <param name="scanResult"></param>
        /// <param name="assemblyName"></param>
        /// <param name="asm"></param>
        /// <returns></returns>
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
            // 获得最基本的Bundle信息
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

                config.IsBundle = !(att is BundleFragmentAttribute);
                config.EnabledByDefault = att.EnabledByDefault;
                config.StartLevel = att.StartLevel;
                config.Flags = att.Flags;
            }

            //获得作者
            object[] atts = reflector.GetCustomAttributes(asm, typeof(BundleAuthorAttribute), false);
            foreach (BundleAuthorAttribute author in atts)
            {
                if (config.Author.Length == 0)
                    config.Author = author.Name;
                else
                    config.Author += ", " + author.Name;
            }

            //获得名称
            atts = reflector.GetCustomAttributes(asm, typeof(BundleNameAttribute), false);
            foreach (BundleNameAttribute at in atts)
            {
                if (string.IsNullOrEmpty(at.Locale))
                    config.Name = at.Name;
                else
                    config.Properties.SetPropertyValue("Name", at.Name, at.Locale);
            }

            //获得说明
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

            // 分类
            catt = reflector.GetCustomAttribute(asm, typeof(BundleCategoryAttribute), false);
            if (catt != null && config.Category.Length == 0)
                config.Category = ((BundleCategoryAttribute)catt).Category;

            //路径
            catt = reflector.GetCustomAttribute(asm, typeof(BundleUrlAttribute), false);
            if (catt != null && config.Url.Length == 0)
                config.Url = ((BundleUrlAttribute)catt).Url;

            //标识
            catt = reflector.GetCustomAttribute(asm, typeof(BundleFlagsAttribute), false);
            if (catt != null)
                config.Flags |= ((BundleFlagsAttribute)catt).Flags;

            //启动等级
            catt = reflector.GetCustomAttribute(asm, typeof(BundleStartLevelAttribute), false);
            if (catt != null)
                config.StartLevel = ((BundleStartLevelAttribute)catt).StartLevel > 0 ? ((BundleStartLevelAttribute)catt).StartLevel : 1;

            //本地化

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

            //激活类

            BundleActivatorAttribute activ = (BundleActivatorAttribute)reflector.GetCustomAttribute(asm, typeof(BundleActivatorAttribute), false);

            if (activ != null)
            {
                ExtensionNodeDescription node = new ExtensionNodeDescription();

                node.SetAttribute("type", activ.TypeName);
               
                config.Activator = node;
            }


            //可以选择的Bundle模块

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

            // 获得依赖项

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
                //获得属性数据
                object[] props = reflector.GetCustomAttributes(asm, typeof(BundlePropertyAttribute), false);

                foreach (BundlePropertyAttribute prop in props)
                    config.Properties.SetPropertyValue(prop.Name, prop.Value, prop.Locale);

                // 获得扩展点

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

            // 获取应用于类型的扩展节点或扩展点

            foreach (object t in reflector.GetAssemblyTypes(asm))
            {

                string typeFullName = reflector.GetTypeFullName(t);

                //寻找扩展
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

                    //获得节点的属性值

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
                    //获得应用类型的扩展点

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

        /// <summary>
        /// 获得基类全名和接口集合的全名
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetBaseTypeNameList(IAssemblyReflector reflector, object type)
        {
            StringBuilder sb = new StringBuilder("$");

            foreach (string tn in reflector.GetBaseTypeFullNameList(type))
                sb.Append(tn).Append(',');

            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// 检测扩展节点组的信息
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="config"></param>
        /// <param name="nset"></param>
        /// <param name="assemblies"></param>
        /// <param name="internalNodeSets"></param>
        private void ScanNodeSet(IAssemblyReflector reflector, BundleDescription config, ExtensionNodeSet nset, ArrayList assemblies, Hashtable internalNodeSets)
        {
            foreach (ExtensionNodeType nt in nset.NodeTypes)
                ScanNodeType(reflector, config, nt, assemblies, internalNodeSets);
        }

        /// <summary>
        /// 检测每一个扩展节点的信息
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="config"></param>
        /// <param name="nt"></param>
        /// <param name="assemblies"></param>
        /// <param name="internalNodeSets"></param>
        private void ScanNodeType(IAssemblyReflector reflector, BundleDescription config, ExtensionNodeType nt, ArrayList assemblies, Hashtable internalNodeSets)
        {
            if (nt.TypeName.Length == 0)
                nt.TypeName = "Clamp.OSGI.Nodes.TypeExtensionNode";

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

        /// <summary>
        /// 从Bundle寻找指定的类型
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="typeName"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        private object FindBundleType(IAssemblyReflector reflector, string typeName, ArrayList assemblies)
        {
            //从当前的程序集里面找
            object etype = reflector.GetType(coreAssemblies[reflector], typeName);

            if (etype != null)
                return etype;

            //从他依赖的程序集里面找
            foreach (object asm in assemblies)
            {
                etype = reflector.GetType(asm, typeName);
                if (etype != null)
                    return etype;
            }

            Hashtable visited = new Hashtable();

            // 从他依赖的程序集的依赖程序里面找
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

        /// <summary>
        /// 检测内嵌的Bundle信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="reflector"></param>
        /// <param name="asm"></param>
        /// <param name="bdesc"></param>
        /// <returns></returns>
        static bool ScanEmbeddedDescription(string filePath, IAssemblyReflector reflector, object asm, out BundleDescription bdesc)
        {
            bdesc = null;

            foreach (string res in reflector.GetResourceNames(asm))
            {
                if (res.EndsWith(".bundle") || res.EndsWith(".bundle.xml"))
                {
                    using (Stream s = reflector.GetResourceStream(asm, res))
                    {
                        BundleDescription bundleDescription = BundleDescription.Read(s, Path.GetDirectoryName(filePath));

                        if (bdesc != null)
                        {
                            //如果俩个都不是ExtensionModel的话，那说明有一个以上的bundle文件。这是不对的。
                            if (!bdesc.IsExtensionModel && !bundleDescription.IsExtensionModel)
                            {
                                return false;
                            }

                            bdesc = BundleDescription.Merge(bdesc, bundleDescription);
                        }
                        else
                            bdesc = bundleDescription;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
