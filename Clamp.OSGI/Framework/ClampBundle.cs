using Clamp.OSGI.Framework.Data;
using Clamp.OSGI.Injection;
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
using Clamp.OSGI.Framework.Data.Description;
using Clamp.OSGI.Framework.Nodes;
using Clamp.OSGI.Framework.Localization;

namespace Clamp.OSGI.Framework
{
    /// <summary>
    /// 框架的Bundle
    /// </summary>
    public class ClampBundle : TreeClampBundle
    {
        private object LocalLock = new object();
        private Hashtable autoExtensionTypes = new Hashtable();
        private Dictionary<string, RuntimeBundle> loadedBundles = new Dictionary<string, RuntimeBundle>();
        private Dictionary<Assembly, RuntimeBundle> loadedAssemblies = new Dictionary<Assembly, RuntimeBundle>();
        private Dictionary<string, ExtensionNodeSet> nodeSets = new Dictionary<string, ExtensionNodeSet>();
        private List<Assembly> pendingAssemblyBundlesChecks = new List<Assembly>();
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

        internal ClampBundle(Dictionary<string, string> configProps) : base(null)
        {
            this.configProps = configProps;
        }

        #region public method

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="startupAsm"></param>
        /// <param name="bundlesDirectory"></param>
        /// <param name="filesToIgnore"></param>
        public void Initialize(Assembly startupAsm, string addinsDir, string databaseDir)
        {
            lock (this.LocalLock)
            {
                if (initialized)
                    return;

                string asmFile = new Uri(startupAsm.CodeBase).LocalPath;

                this.startupDirectory = Path.GetDirectoryName(asmFile);

                this.registry = new BundleRegistry(this, this.startupDirectory, addinsDir, databaseDir);

                if (this.registry.CreateHostBundlesFile(asmFile) || this.registry.UnknownDomain)
                    this.registry.Update();

                initialized = true;
            }
        }

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
                initialized = false;
                AppDomain.CurrentDomain.AssemblyLoad -= new AssemblyLoadEventHandler(OnAssemblyLoaded);
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainAssemblyResolve;
                loadedBundles = new Dictionary<string, RuntimeBundle>();
                loadedAssemblies = new Dictionary<Assembly, RuntimeBundle>();
                registry.Dispose();
                registry = null;
                startupDirectory = null;
                ClearContext();
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

        internal void RegisterAssemblies(RuntimeBundle addin)
        {
            lock (this.LocalLock)
            {
                var loadedAssembliesCopy = new Dictionary<Assembly, RuntimeBundle>(loadedAssemblies);
                foreach (Assembly asm in addin.Assemblies)
                    loadedAssembliesCopy[asm] = addin;
                loadedAssemblies = loadedAssembliesCopy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal RuntimeBundle GetBundle(string id)
        {
            ValidateBundleRoots();
            RuntimeBundle a;
            loadedBundles.TryGetValue(Bundle.GetIdName(id), out a);
            return a;
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

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                CheckBundleAssembly(asm);
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


        internal void UnloadBundle(string id)
        {
            RemoveBundleExtensions(id);

            RuntimeBundle addin = GetBundle(id);

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
                        string msg = GettextCatalog.GetString("Disabled add-ins can't be loaded.");
                        if (throwExceptions)
                            throw new InvalidOperationException(msg);
                        return false;
                    }

                    ArrayList addins = new ArrayList();
                    Stack depCheck = new Stack();
                    ResolveLoadDependencies(addins, depCheck, id, false);
                    addins.Reverse();

                    for (int n = 0; n < addins.Count; n++)
                    {
                        Bundle iad = (Bundle)addins[n];
                        if (IsBundleLoaded(iad.Id))
                            continue;

                        if (!InsertBundle(iad))
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

        private bool InsertBundle(Bundle iad)
        {
            try
            {
                RuntimeBundle p = new RuntimeBundle(this);

                // Read the config file and load the add-in assemblies
                BundleDescription description = p.Load(iad);

                // Register the add-in
                var loadedBundlesCopy = new Dictionary<string, RuntimeBundle>(loadedBundles);
                loadedBundlesCopy[Bundle.GetIdName(p.Id)] = p;
                loadedBundles = loadedBundlesCopy;

                if (!BundleDatabase.RunningSetupProcess)
                {
                    // Load the extension points and other addin data

                    RegisterNodeSets(iad.Id, description.ExtensionNodeSets);

                    foreach (ConditionTypeDescription cond in description.ConditionTypes)
                    {
                        Type ctype = p.GetType(cond.TypeName, true);
                        RegisterCondition(cond.Id, ctype);
                    }
                }

                foreach (ExtensionPoint ep in description.ExtensionPoints)
                    InsertExtensionPoint(p, ep);

                // Fire loaded event
                NotifyBundleLoaded(p);
                ReportBundleLoad(p.Id);
                return true;
            }
            catch (Exception ex)
            {
                ReportError("Add-in could not be loaded", iad.Id, ex, false);
                return false;
            }
        }


        private void RegisterNodeSets(string addinId, ExtensionNodeSetCollection nsets)
        {
            lock (this.LocalLock)
            {
                var nodeSetsCopy = new Dictionary<string, ExtensionNodeSet>(nodeSets);
                foreach (ExtensionNodeSet nset in nsets)
                {
                    nset.SourceBundleId = addinId;
                    nodeSetsCopy[nset.Id] = nset;
                }
                nodeSets = nodeSetsCopy;
            }
        }


        private bool ResolveLoadDependencies(ArrayList addins, Stack depCheck, string id, bool optional)
        {
            if (IsBundleLoaded(id))
                return true;

            if (depCheck.Contains(id))
                throw new InvalidOperationException("A cyclic addin dependency has been detected.");

            depCheck.Push(id);

            Bundle iad = Registry.GetBundle(id);

            if (iad == null || !iad.Enabled)
            {
                if (optional)
                    return false;
                else if (iad != null && !iad.Enabled)
                    throw new MissingDependencyException(GettextCatalog.GetString("当前请求的Bundle'{0}'是不可用的.", id));
                else
                    throw new MissingDependencyException(GettextCatalog.GetString("当前请求的Bundle'{0}'没有被安装.", id));
            }

            // If this addin has already been requested, bring it to the head
            // of the list, so it is loaded earlier than before.
            addins.Remove(iad);
            addins.Add(iad);

            foreach (Dependency dep in iad.BundleInfo.Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep != null)
                {
                    try
                    {
                        string adepid = Bundle.GetFullId(iad.BundleInfo.Namespace, adep.BundleId, adep.Version);
                        ResolveLoadDependencies(addins, depCheck, adepid, false);
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

            if (iad.BundleInfo.OptionalDependencies != null)
            {
                foreach (Dependency dep in iad.BundleInfo.OptionalDependencies)
                {
                    BundleDependency adep = dep as BundleDependency;
                    if (adep != null)
                    {
                        string adepid = Bundle.GetFullId(iad.Namespace, adep.BundleId, adep.Version);
                        if (!ResolveLoadDependencies(addins, depCheck, adepid, true))
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
