﻿using Clamp.Data;
using Clamp.Injection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Collections;
using Clamp.Data.Description;
using Clamp.Nodes;
using Clamp.Localization;

namespace Clamp
{
    /// <summary>
    /// 框架的Bundle
    /// </summary>
    internal partial class ClampBundle
    {
        private object LocalLock = new object();
        private Hashtable autoExtensionTypes = new Hashtable();
        private Dictionary<string, RuntimeBundle> loadedBundles = new Dictionary<string, RuntimeBundle>();
        private Dictionary<Assembly, RuntimeBundle> loadedAssemblies = new Dictionary<Assembly, RuntimeBundle>();
        private Dictionary<string, ExtensionNodeSet> nodeSets = new Dictionary<string, ExtensionNodeSet>();
        private List<Assembly> pendingAssemblyBundlesChecks = new List<Assembly>();
        private ClampObjectContainer clampObjectContainer = new ClampObjectContainer();
        private BundleRegistry registry;
        private bool initialized;
        private string startupDirectory;
        private BundleLocalizer defaultLocalizer;
        private Dictionary<string, string> configProps;

        public event BundleErrorEventHandler BundleLoadError;

        public event BundleEventHandler BundleLoaded;

        public event BundleEventHandler BundleUnloaded;

        /// <summary>
        /// 是否初始化过
        /// </summary>
        public bool IsInitialized
        {
            get { return initialized; }
        }

        /// <summary>
        /// 默认的本地化
        /// </summary>
        public BundleLocalizer DefaultLocalizer
        {
            get
            {
                CheckInitialized();
                var loc = defaultLocalizer;
                return loc ?? NullLocalizer.Instance;
            }
        }
        /// <summary>
        /// Bundle的注册者
        /// </summary>
        internal BundleRegistry Registry
        {
            get
            {
                CheckInitialized();
                return registry;
            }
        }

        internal Dictionary<string, string> ConfigProps
        {
            get
            {
                CheckInitialized();
                return configProps;
            }
        }

        internal ClampBundle(Dictionary<string, string> configProps)
        {
            this.configProps = configProps;
            this.fireEvents = false;
            this.tree = new ExtensionTreeNode(this, "");
        }

        #region public method

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="startupAsm"></param>
        /// <param name="bundlesDirectory"></param>
        /// <param name="filesToIgnore"></param>
        public void Initialize(Assembly startupAsm, string bundlesDir, string databaseDir)
        {
            lock (this.LocalLock)
            {
                if (initialized)
                    return;

                string asmFile = new Uri(startupAsm.CodeBase).LocalPath;

                this.startupDirectory = Path.GetDirectoryName(asmFile);

                this.registry = new BundleRegistry(this, this.startupDirectory, bundlesDir, databaseDir);

                if (this.registry.CreateHostBundlesFile(asmFile) || this.registry.UnknownDomain)
                    this.registry.Update();


                initialized = true;
            }
        }

        /// <summary>
        /// 是否加载了Bundle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsBundleLoaded(string id)
        {
            CheckInitialized();

            ValidateBundleRoots();

            return loadedBundles.ContainsKey(Bundle.GetIdName(id));
        }

        public void InitializeDefaultLocalizer(IBundleLocalizer localizer)
        {
            CheckInitialized();

            lock (LocalLock)
            {
                if (localizer != null)
                    defaultLocalizer = new BundleLocalizer(localizer);
                else
                    defaultLocalizer = null;
            }
        }


        #region 注册功能

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, object instance)
        {
            return this.clampObjectContainer.Register(registerType, instance);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, object instance, string name)
        {
            return this.clampObjectContainer.Register(registerType, instance, name);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="registerImplementation"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance)
        {
            return this.clampObjectContainer.Register(registerType, registerImplementation, instance);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="registerImplementation"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name)
        {
            return this.clampObjectContainer.Register(registerType, registerImplementation, instance, name);
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <param name="resolveType"></param>
        /// <returns></returns>
        public object Resolve(Type resolveType)
        {
            return this.clampObjectContainer.Resolve(resolveType);
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <param name="resolveType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public object Resolve(Type resolveType, ResolveOptions options)
        {
            return this.clampObjectContainer.Resolve(resolveType, options);
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <param name="resolveType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Resolve(Type resolveType, string name)
        {
            return this.clampObjectContainer.Resolve(resolveType, name);
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <returns></returns>
        public ResolveType Resolve<ResolveType>() where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType));
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public ResolveType Resolve<ResolveType>(ResolveOptions options) where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), options);
        }

        /// <summary>
        /// 解析服务
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public ResolveType Resolve<ResolveType>(string name) where ResolveType : class
        {
            return (ResolveType)Resolve(typeof(ResolveType), name);
        }


        #endregion

        #endregion

        #region 重写 Bundle 的方法
        public override void Start()
        {

            ActivateBundles();

            OnAssemblyLoaded(null, null);

            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(OnAssemblyLoaded);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;

            this.registry.Update();

        }

        public override void Stop()
        {
            lock (this.LocalLock)
            {
                AppDomain.CurrentDomain.AssemblyLoad -= new AssemblyLoadEventHandler(OnAssemblyLoaded);
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainAssemblyResolve;
                loadedBundles = new Dictionary<string, RuntimeBundle>();
                loadedAssemblies = new Dictionary<Assembly, RuntimeBundle>();
                registry.Dispose();
                registry = null;
                startupDirectory = null;
                ClearContext();

                initialized = false;
            }
        }

        public void WaitForStop()
        {

        }

        #endregion
        #region internal method


        internal void UnregisterBundleNodeSets(string addinId)
        {
            lock (this.LocalLock)
            {
                var nodeSetsCopy = new Dictionary<string, ExtensionNodeSet>(nodeSets);
                foreach (var nset in nodeSetsCopy.Values.Where(n => n.SourceBundleId == addinId).ToArray())
                    nodeSetsCopy.Remove(nset.Id);
                nodeSets = nodeSetsCopy;
            }
        }


        internal ExtensionNodeType FindType(ExtensionNodeSet nset, string name, string callingBundleId)
        {
            if (nset == null)
                return null;

            foreach (ExtensionNodeType nt in nset.NodeTypes)
            {
                if (nt.Id == name)
                    return nt;
            }

            foreach (string ns in nset.NodeSets)
            {
                ExtensionNodeSet regSet;
                if (!nodeSets.TryGetValue(ns, out regSet))
                {
                    ReportError("Unknown node set: " + ns, callingBundleId, null, false);
                    return null;
                }
                ExtensionNodeType nt = FindType(regSet, name, callingBundleId);
                if (nt != null)
                    return nt;
            }
            return null;
        }

        internal void RegisterAssemblies(RuntimeBundle runtimeBundle)
        {
            lock (this.LocalLock)
            {
                var loadedAssembliesCopy = new Dictionary<Assembly, RuntimeBundle>(loadedAssemblies);

                foreach (Assembly asm in runtimeBundle.Assemblies)
                    loadedAssembliesCopy[asm] = runtimeBundle;

                loadedAssemblies = loadedAssembliesCopy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal RuntimeBundle GetRuntimeBundle(string id)
        {
            ValidateBundleRoots();

            RuntimeBundle a;

            loadedBundles.TryGetValue(Bundle.GetIdName(id), out a);

            return a;
        }

        internal RuntimeBundle GetRuntimeBundleByName(string name)
        {
            ValidateBundleRoots();

            return this.loadedBundles.Values.FirstOrDefault(rb => string.Equals(rb.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        internal void RegisterAutoTypeExtensionPoint(Type type, string path)
        {
            autoExtensionTypes[type] = path;
        }

        internal void UnregisterAutoTypeExtensionPoint(Type type, string path)
        {
            autoExtensionTypes.Remove(type);
        }

        internal string GetAutoTypeExtensionPoint(Type type)
        {
            return autoExtensionTypes[type] as string;
        }


        internal void InsertExtensionPoint(RuntimeBundle addin, ExtensionPoint ep)
        {
            CreateExtensionPoint(ep);

            foreach (ExtensionNodeType nt in ep.NodeSet.NodeTypes)
            {
                if (nt.ObjectTypeName.Length > 0)
                {
                    Type ntype = addin.GetType(nt.ObjectTypeName, true);
                    RegisterAutoTypeExtensionPoint(ntype, ep.Path);
                }
            }
        }

        /// <summary>
        /// 激活Bundle
        /// </summary>
        internal void ActivateBundles()
        {
            lock (pendingAssemblyBundlesChecks)
                pendingAssemblyBundlesChecks.Clear();

            List<Bundle> pendingActivateBundles = Registry.GetPendingActivateBundles();

            foreach (Bundle bundle in pendingActivateBundles)
            {
                if (bundle != null && !IsBundleLoaded(bundle.Id))
                {
                    BundleDescription bdesc = null;

                    try
                    {
                        bdesc = bundle.Description;
                    }
                    catch (Exception ex)
                    {

                    }

                    if (bdesc == null || bdesc.FilesChanged())
                    {
                        // If the add-in has changed, update the add-in database.
                        // We do it here because once loaded, add-in roots can't be
                        // reloaded like regular add-ins.
                        Registry.Update();

                        this.ActivateBundles();

                        break;
                    }

                    LoadBundle(bundle.Id, false);
                }
            }

            //启动激活类
            foreach (RuntimeBundle runtimeBundle in this.loadedBundles.Values)
            {
                if (runtimeBundle.BundleActivator != null)
                {
                    runtimeBundle.BundleActivator.Start(new BundleContext(runtimeBundle, this));
                }
            }
        }

        /// <summary>
        /// 检测框架是否初始化过
        /// </summary>
        internal void CheckInitialized()
        {
            if (!initialized)
                throw new InvalidOperationException("Clamp框架没有初始化过");
        }

        /// <summary>
        /// 验证Bundle的
        /// </summary>
        internal void ValidateBundleRoots()
        {
            List<Assembly> copy = null;

            lock (pendingAssemblyBundlesChecks)
            {
                if (pendingAssemblyBundlesChecks.Count > 0)
                {
                    copy = new List<Assembly>(pendingAssemblyBundlesChecks);
                    pendingAssemblyBundlesChecks.Clear();
                }
            }

            if (copy != null)
            {
                foreach (Assembly asm in copy)
                    CheckBundleAssembly(asm);
            }
        }

        /// <summary>
        /// 根据指定ID来卸载Bundle
        /// </summary>
        /// <param name="id"></param>
        internal void UnloadBundle(string id)
        {
            RemoveBundleExtensions(id);

            RuntimeBundle addin = GetRuntimeBundle(id);

            if (addin != null)
            {
                addin.UnloadExtensions();

                lock (LocalLock)
                {
                    var loadedBundlesCopy = new Dictionary<string, RuntimeBundle>(loadedBundles);

                    loadedBundlesCopy.Remove(Bundle.GetIdName(id));

                    loadedBundles = loadedBundlesCopy;

                    if (addin.AssembliesLoaded)
                    {
                        var loadedAssembliesCopy = new Dictionary<Assembly, RuntimeBundle>(loadedAssemblies);
                        foreach (Assembly asm in addin.Assemblies)
                            loadedAssembliesCopy.Remove(asm);
                        loadedAssemblies = loadedAssembliesCopy;
                    }
                }

                ReportBundleUnload(id);
            }
        }

        internal void ActivateBundle(string id)
        {
            ActivateBundleExtensions(id);
        }
        /// <summary>
        /// 加载Bundle
        /// </summary>
        /// <param name="id"></param>
        /// <param name="throwExceptions"></param>
        /// <returns></returns>
        internal bool LoadBundle(string id, bool throwExceptions)
        {
            try
            {
                lock (this.LocalLock)
                {
                    if (IsBundleLoaded(id))
                        return true;

                    if (!Registry.IsBundleEnabled(id))
                    {
                        string msg = GettextCatalog.GetString("加载不了不能使用的Bundle");

                        if (throwExceptions)
                            throw new InvalidOperationException(msg);
                        return false;
                    }

                    List<Bundle> bundles = new List<Bundle>();

                    Stack depCheck = new Stack();

                    ResolveLoadDependencies(bundles, depCheck, id, false);

                    Bundle currentBundle = bundles.FirstOrDefault(b => b.Id == id);

                    bundles.Remove(currentBundle);

                    List<Bundle> sortedBundles = bundles.OrderByDescending(b => b.StartLevel).ToList();

                    sortedBundles.Add(currentBundle);

                    for (int n = 0; n < sortedBundles.Count; n++)
                    {
                        Bundle bundle = sortedBundles[n];

                        if (IsBundleLoaded(bundle.Id))
                            continue;

                        if (!InsertBundle(bundle))
                            return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                ReportError("Add-in could not be loaded: " + ex.Message, id, ex, false);
                if (throwExceptions)
                    throw;
                return false;
            }
        }
        internal void ReportError(string message, string addinId, Exception exception, bool fatal)
        {
            var handler = BundleLoadError;
            if (handler != null)
                handler(null, new BundleErrorEventArgs(message, addinId, exception));
            else
            {
                Console.WriteLine(message);
                if (exception != null)
                    Console.WriteLine(exception);
            }
        }
        internal void ReportBundleLoad(string id)
        {
            var handler = BundleLoaded;
            if (handler != null)
            {
                try
                {
                    handler(null, new BundleEventArgs(id));
                }
                catch
                {
                    // Ignore subscriber exceptions
                }
            }
        }

        internal void ReportBundleUnload(string id)
        {
            var handler = BundleUnloaded;
            if (handler != null)
            {
                try
                {
                    handler(null, new BundleEventArgs(id));
                }
                catch
                {
                    // Ignore subscriber exceptions
                }
            }
        }

        #endregion

        #region private method

        private bool InsertBundle(Bundle bundle)
        {
            try
            {
                RuntimeBundle runtimeBundle = new RuntimeBundle(this);

                BundleDescription description = runtimeBundle.Load(bundle);

                //增加已经加载的Bundle
                var loadedBundlesCopy = new Dictionary<string, RuntimeBundle>(this.loadedBundles);

                loadedBundlesCopy[Bundle.GetIdName(runtimeBundle.Id)] = runtimeBundle;

                this.loadedBundles = loadedBundlesCopy;

                if (!BundleDatabase.RunningSetupProcess)
                {
                    // Load the extension points and other addin data

                    RegisterNodeSets(bundle.Id, description.ExtensionNodeSets);

                    foreach (ConditionTypeDescription cond in description.ConditionTypes)
                    {
                        Type ctype = runtimeBundle.GetType(cond.TypeName, true);

                        RegisterCondition(cond.Id, ctype);
                    }
                }

                foreach (ExtensionPoint ep in description.ExtensionPoints)
                    InsertExtensionPoint(runtimeBundle, ep);

                // Fire loaded event
                NotifyBundleLoaded(runtimeBundle);
                ReportBundleLoad(runtimeBundle.Id);

                return true;
            }
            catch (Exception ex)
            {
                ReportError("Add-in could not be loaded", bundle.Id, ex, false);
                return false;
            }
        }


        private void RegisterNodeSets(string bundleId, ExtensionNodeSetCollection nsets)
        {
            lock (this.LocalLock)
            {
                var nodeSetsCopy = new Dictionary<string, ExtensionNodeSet>(nodeSets);

                foreach (ExtensionNodeSet nset in nsets)
                {
                    nset.SourceBundleId = bundleId;
                    nodeSetsCopy[nset.Id] = nset;
                }

                nodeSets = nodeSetsCopy;
            }
        }


        private bool ResolveLoadDependencies(List<Bundle> bundles, Stack depCheck, string id, bool optional)
        {
            if (IsBundleLoaded(id))
                return true;

            if (depCheck.Contains(id))
                throw new InvalidOperationException("A cyclic addin dependency has been detected.");

            depCheck.Push(id);

            Bundle bundle = Registry.GetBundle(id);

            if (bundle == null || !bundle.Enabled)
            {
                if (optional)
                    return false;
                else if (bundle != null && !bundle.Enabled)
                    throw new MissingDependencyException(GettextCatalog.GetString("当前请求的Bundle'{0}'是不可用的.", id));
                else
                    throw new MissingDependencyException(GettextCatalog.GetString("当前请求的Bundle'{0}'没有被安装.", id));
            }

            // If this addin has already been requested, bring it to the head
            // of the list, so it is loaded earlier than before.
            bundles.Remove(bundle);
            bundles.Add(bundle);

            foreach (Dependency dep in bundle.BundleInfo.Dependencies)
            {
                BundleDependency bdep = dep as BundleDependency;

                if (bdep != null)
                {
                    try
                    {
                        string adepid = Bundle.GetFullId(bundle.BundleInfo.Namespace, bdep.BundleId, bdep.Version);

                        ResolveLoadDependencies(bundles, depCheck, adepid, false);
                    }
                    catch (MissingDependencyException)
                    {
                        if (optional)
                            return false;
                        else
                            throw;
                    }
                }
            }

            if (bundle.BundleInfo.OptionalDependencies != null)
            {
                foreach (Dependency dep in bundle.BundleInfo.OptionalDependencies)
                {
                    BundleDependency adep = dep as BundleDependency;

                    if (adep != null)
                    {
                        string adepid = Bundle.GetFullId(bundle.Namespace, adep.BundleId, adep.Version);

                        if (!ResolveLoadDependencies(bundles, depCheck, adepid, true))
                            return false;
                    }
                }
            }

            depCheck.Pop();
            return true;
        }
        private void OnAssemblyLoaded(object s, AssemblyLoadEventArgs a)
        {
            if (a != null)
            {
                lock (pendingAssemblyBundlesChecks)
                {
                    pendingAssemblyBundlesChecks.Add(a.LoadedAssembly);
                }
            }
        }

        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (this.LocalLock)
            {
                return loadedBundles.Values.Where(a => a.AssembliesLoaded).SelectMany(a => a.Assemblies).FirstOrDefault(a => a.FullName.ToString() == args.Name);
            }
        }

        /// <summary>
        /// 检测当前的程序集是否为Bundle，如果是就加载
        /// </summary>
        /// <param name="asm"></param>
        private void CheckBundleAssembly(Assembly asm)
        {
            if (BundleDatabase.RunningSetupProcess || asm is System.Reflection.Emit.AssemblyBuilder || asm.IsDynamic)
                return;

            string codeBase;

            try
            {
                codeBase = asm.CodeBase;
            }
            catch
            {
                return;
            }

            Uri u;

            if (!Uri.TryCreate(codeBase, UriKind.Absolute, out u))
                return;

            string asmFile = u.LocalPath;

            Bundle bundle;

            try
            {
                bundle = Registry.GetBundleForHostAssembly(asmFile);
            }
            catch (Exception ex)
            {
                Registry.Update();

                bundle = Registry.GetBundleForHostAssembly(asmFile);
            }

            if (bundle != null && !IsBundleLoaded(bundle.Id))
            {
                BundleDescription bdesc = null;

                try
                {
                    bdesc = bundle.Description;
                }
                catch (Exception ex)
                {

                }

                if (bdesc == null || bdesc.FilesChanged())
                {
                    // If the add-in has changed, update the add-in database.
                    // We do it here because once loaded, add-in roots can't be
                    // reloaded like regular add-ins.
                    Registry.Update();

                    bundle = Registry.GetBundleForHostAssembly(asmFile);
                    if (bundle == null)
                        return;
                }

                LoadBundle(bundle.Id, false);
            }
        }

        #endregion

        #region internal static
        /// <summary>
        /// 检测指点定的文件集合里面是否存在已经加载的文件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        internal static bool CheckAssembliesLoaded(HashSet<string> files)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm is System.Reflection.Emit.AssemblyBuilder)
                    continue;

                try
                {
                    Uri u;

                    if (!Uri.TryCreate(asm.CodeBase, UriKind.Absolute, out u))
                        continue;

                    string asmFile = u.LocalPath;

                    if (files.Contains(Path.GetFullPath(asmFile)))
                        return true;
                }
                catch
                {
                    // Ignore
                }
            }

            return false;
        }

        #endregion

    }
}
