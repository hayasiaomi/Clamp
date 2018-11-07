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
    internal class ClampBundle : Bundle, IClampBundle
    {
        private Hashtable autoExtensionTypes = new Hashtable();
        private Dictionary<string, RuntimeAddin> loadedAddins = new Dictionary<string, RuntimeAddin>();
        private Dictionary<Assembly, RuntimeAddin> loadedAssemblies = new Dictionary<Assembly, RuntimeAddin>();
        private Dictionary<string, ExtensionNodeSet> nodeSets = new Dictionary<string, ExtensionNodeSet>();
        private List<Assembly> pendingRootChecks = new List<Assembly>();
        private BundleRegistry registry;
        private ExtensionContext extensionContext;
        private bool initialized;
        private string startupDirectory;
        private AddinLocalizer defaultLocalizer;
        public static event AddinErrorEventHandler AddinLoadError;

        public static event AddinEventHandler AddinLoaded;

        public bool IsInitialized
        {
            get { return initialized; }
        }

        /// <summary>
        /// 禁止的插件文件
        /// </summary>
        public string[] FilesToIgnore { private set; get; }

        /// <summary>
        /// 插件的根目录
        /// </summary>
        public string StartupDirectory { private set; get; }

        public AddinLocalizer DefaultLocalizer
        {
            get
            {
                CheckInitialized();
                var loc = defaultLocalizer;
                return loc ?? NullLocalizer.Instance;
            }
        }

        public ClampBundle()
        {

        }

        #region public method

        public void Initialize()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            if (asm == null)
                asm = Assembly.GetCallingAssembly();

            this.Initialize(asm, "Bundles", null);
        }

        public void Initialize(Assembly startupAsm, string bundlesDirectory, string[] filesToIgnore)
        {
            string asmFile = new Uri(startupAsm.CodeBase).LocalPath;

            this.StartupDirectory = Path.GetDirectoryName(asmFile);

            string customBundleDir = Environment.GetEnvironmentVariable("CLAMP_BUNDLES_DIR");

            string finalBundlesDirectory = string.Empty;

            if (!string.IsNullOrWhiteSpace(customBundleDir))
            {
                if (Path.IsPathRooted(customBundleDir))
                    finalBundlesDirectory = customBundleDir;
                else
                    finalBundlesDirectory = Path.Combine(this.StartupDirectory, customBundleDir);
            }
            else
            {
                if (!Path.IsPathRooted(bundlesDirectory))
                    finalBundlesDirectory = Path.Combine(this.StartupDirectory, bundlesDirectory);
                else
                    finalBundlesDirectory = bundlesDirectory;
            }

            this.registry = new BundleRegistry(this, this.StartupDirectory, this.StartupDirectory, finalBundlesDirectory, this.StartupDirectory);

            if (this.registry.CreateHostBundlesFile(asmFile) || this.registry.UnknownDomain)
                this.registry.Update();

            initialized = true;

            ActivateRoots();

            OnAssemblyLoaded(null, null);

            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(OnAssemblyLoaded);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }

        public bool IsAddinLoaded(string id)
        {
            CheckInitialized();
            ValidateAddinRoots();
            return loadedAddins.ContainsKey(Bundle.GetIdName(id));
        }


        #endregion

        #region 重写 Bundle有方法
        public override void Start()
        {
            //if (this.Bundles != null && this.Bundles.Count > 0)
            //{
            //    List<Bundle> addIns = this.Bundles.OrderBy(addin => addin.StartLevel).ToList();

            //    foreach (Bundle addin in addIns)
            //    {
            //        if (addin.Enabled)
            //        {
            //            if (!string.IsNullOrWhiteSpace(addin.ActivatorClassName))
            //            {
            //                IBundleActivator addInActivator = addin.CreateObject(addin.ActivatorClassName) as IBundleActivator;

            //                if (addInActivator != null)
            //                {
            //                    BundleContext context = new BundleContext(addin, this);

            //                    addInActivator.Start(context);
            //                }
            //            }
            //        }
            //    }
            //}

        }

        public override void Stop()
        {
            lock (extensionContext.LocalLock)
            {
                initialized = false;
                AppDomain.CurrentDomain.AssemblyLoad -= new AssemblyLoadEventHandler(OnAssemblyLoaded);
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainAssemblyResolve;
                loadedAddins = new Dictionary<string, RuntimeAddin>();
                loadedAssemblies = new Dictionary<Assembly, RuntimeAddin>();
                registry.Dispose();
                registry = null;
                startupDirectory = null;
                extensionContext.ClearContext();
            }
        }





        #endregion

        #region 实现接口
        public ExtensionNode GetExtensionNode(string path)
        {
            throw new NotImplementedException();
        }

        public T GetExtensionNode<T>(string path) where T : ExtensionNode
        {
            throw new NotImplementedException();
        }

        public ExtensionNodeList GetExtensionNodes(string path)
        {
            throw new NotImplementedException();
        }

        public ExtensionNodeList GetExtensionNodes(string path, Type expectedNodeType)
        {
            throw new NotImplementedException();
        }

        public ExtensionNodeList<T> GetExtensionNodes<T>(string path) where T : ExtensionNode
        {
            throw new NotImplementedException();
        }

        public ExtensionNodeList GetExtensionNodes(Type instanceType)
        {
            throw new NotImplementedException();
        }

        public ExtensionNodeList GetExtensionNodes(Type instanceType, Type expectedNodeType)
        {
            throw new NotImplementedException();
        }

        public ExtensionNodeList<T> GetExtensionNodes<T>(Type instanceType) where T : ExtensionNode
        {
            throw new NotImplementedException();
        }

        public object[] GetInstance(Type instanceType)
        {
            throw new NotImplementedException();
        }

        public T[] GetInstance<T>()
        {
            throw new NotImplementedException();
        }

        public object[] GetInstance(Type instanceType, bool reuseCachedInstance)
        {
            throw new NotImplementedException();
        }

        public T[] GetInstance<T>(bool reuseCachedInstance)
        {
            throw new NotImplementedException();
        }

        public object[] GetInstance(string path)
        {
            throw new NotImplementedException();
        }

        public object[] GetInstance(string path, bool reuseCachedInstance)
        {
            throw new NotImplementedException();
        }

        public object[] GetInstance(string path, Type arrayElementType)
        {
            throw new NotImplementedException();
        }

        public T[] GetInstance<T>(string path)
        {
            throw new NotImplementedException();
        }

        public object[] GetInstance(string path, Type arrayElementType, bool reuseCachedInstance)
        {
            throw new NotImplementedException();
        }

        public T[] GetInstance<T>(string path, bool reuseCachedInstance)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region internal method


        internal void UnregisterAddinNodeSets(string addinId)
        {
            lock (extensionContext.LocalLock)
            {
                var nodeSetsCopy = new Dictionary<string, ExtensionNodeSet>(nodeSets);
                foreach (var nset in nodeSetsCopy.Values.Where(n => n.SourceAddinId == addinId).ToArray())
                    nodeSetsCopy.Remove(nset.Id);
                nodeSets = nodeSetsCopy;
            }
        }


        internal ExtensionNodeType FindType(ExtensionNodeSet nset, string name, string callingAddinId)
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
                    ReportError("Unknown node set: " + ns, callingAddinId, null, false);
                    return null;
                }
                ExtensionNodeType nt = FindType(regSet, name, callingAddinId);
                if (nt != null)
                    return nt;
            }
            return null;
        }

        internal void RegisterAssemblies(RuntimeAddin addin)
        {
            lock (extensionContext.LocalLock)
            {
                var loadedAssembliesCopy = new Dictionary<Assembly, RuntimeAddin>(loadedAssemblies);
                foreach (Assembly asm in addin.Assemblies)
                    loadedAssembliesCopy[asm] = addin;
                loadedAssemblies = loadedAssembliesCopy;
            }
        }

        internal RuntimeAddin GetAddin(string id)
        {
            ValidateAddinRoots();
            RuntimeAddin a;
            loadedAddins.TryGetValue(Bundle.GetIdName(id), out a);
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


        internal void InsertExtensionPoint(RuntimeAddin addin, ExtensionPoint ep)
        {
            extensionContext.CreateExtensionPoint(ep);
            foreach (ExtensionNodeType nt in ep.NodeSet.NodeTypes)
            {
                if (nt.ObjectTypeName.Length > 0)
                {
                    Type ntype = addin.GetType(nt.ObjectTypeName, true);
                    RegisterAutoTypeExtensionPoint(ntype, ep.Path);
                }
            }
        }

        internal void ActivateRoots()
        {
            lock (pendingRootChecks)
                pendingRootChecks.Clear();
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                CheckHostAssembly(asm);
        }
        internal void CheckInitialized()
        {
            if (!initialized)
                throw new InvalidOperationException("Clamp框架没有初始化过");
        }

        internal void ValidateAddinRoots()
        {
            List<Assembly> copy = null;
            lock (pendingRootChecks)
            {
                if (pendingRootChecks.Count > 0)
                {
                    copy = new List<Assembly>(pendingRootChecks);
                    pendingRootChecks.Clear();
                }
            }
            if (copy != null)
            {
                foreach (Assembly asm in copy)
                    CheckHostAssembly(asm);
            }
        }


        internal void UnloadAddin(string id)
        {
            //    RemoveAddinExtensions(id);

            //    RuntimeAddin addin = GetAddin(id);
            //    if (addin != null)
            //    {
            //        addin.UnloadExtensions();
            //        lock (LocalLock)
            //        {
            //            var loadedAddinsCopy = new Dictionary<string, RuntimeAddin>(loadedAddins);
            //            loadedAddinsCopy.Remove(Addin.GetIdName(id));
            //            loadedAddins = loadedAddinsCopy;
            //            if (addin.AssembliesLoaded)
            //            {
            //                var loadedAssembliesCopy = new Dictionary<Assembly, RuntimeAddin>(loadedAssemblies);
            //                foreach (Assembly asm in addin.Assemblies)
            //                    loadedAssembliesCopy.Remove(asm);
            //                loadedAssemblies = loadedAssembliesCopy;
            //            }
            //        }
            //        ReportAddinUnload(id);
            //    }
        }

        internal void ActivateAddin(string id)
        {
            this.extensionContext.ActivateAddinExtensions(id);
        }

        internal bool LoadAddin(string id, bool throwExceptions)
        {
            try
            {
                lock (extensionContext.LocalLock)
                {
                    if (IsAddinLoaded(id))
                        return true;

                    if (!Registry.IsAddinEnabled(id))
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
                        if (IsAddinLoaded(iad.Id))
                            continue;

                        if (!InsertAddin(iad))
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
            var handler = AddinLoadError;
            if (handler != null)
                handler(null, new AddinErrorEventArgs(message, addinId, exception));
            else
            {
                Console.WriteLine(message);
                if (exception != null)
                    Console.WriteLine(exception);
            }
        }
        internal void ReportAddinLoad(string id)
        {
            var handler = AddinLoaded;
            if (handler != null)
            {
                try
                {
                    handler(null, new AddinEventArgs(id));
                }
                catch
                {
                    // Ignore subscriber exceptions
                }
            }
        }

        #endregion

        #region private method

        private bool InsertAddin(Bundle iad)
        {
            try
            {
                RuntimeAddin p = new RuntimeAddin(this);

                // Read the config file and load the add-in assemblies
                BundleDescription description = p.Load(iad);

                // Register the add-in
                var loadedAddinsCopy = new Dictionary<string, RuntimeAddin>(loadedAddins);
                loadedAddinsCopy[Bundle.GetIdName(p.Id)] = p;
                loadedAddins = loadedAddinsCopy;

                if (!BundleDatabase.RunningSetupProcess)
                {
                    // Load the extension points and other addin data

                    RegisterNodeSets(iad.Id, description.ExtensionNodeSets);

                    foreach (ConditionTypeDescription cond in description.ConditionTypes)
                    {
                        Type ctype = p.GetType(cond.TypeName, true);
                        extensionContext.RegisterCondition(cond.Id, ctype);
                    }
                }

                foreach (ExtensionPoint ep in description.ExtensionPoints)
                    InsertExtensionPoint(p, ep);

                // Fire loaded event
                extensionContext.NotifyAddinLoaded(p);
                ReportAddinLoad(p.Id);
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
            lock (extensionContext.LocalLock)
            {
                var nodeSetsCopy = new Dictionary<string, ExtensionNodeSet>(nodeSets);
                foreach (ExtensionNodeSet nset in nsets)
                {
                    nset.SourceAddinId = addinId;
                    nodeSetsCopy[nset.Id] = nset;
                }
                nodeSets = nodeSetsCopy;
            }
        }


        private bool ResolveLoadDependencies(ArrayList addins, Stack depCheck, string id, bool optional)
        {
            if (IsAddinLoaded(id))
                return true;

            if (depCheck.Contains(id))
                throw new InvalidOperationException("A cyclic addin dependency has been detected.");

            depCheck.Push(id);

            Bundle iad = Registry.GetAddin(id);

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

            foreach (Dependency dep in iad.AddinInfo.Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep != null)
                {
                    try
                    {
                        string adepid = Bundle.GetFullId(iad.AddinInfo.Namespace, adep.AddinId, adep.Version);
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

            if (iad.AddinInfo.OptionalDependencies != null)
            {
                foreach (Dependency dep in iad.AddinInfo.OptionalDependencies)
                {
                    BundleDependency adep = dep as BundleDependency;
                    if (adep != null)
                    {
                        string adepid = Bundle.GetFullId(iad.Namespace, adep.AddinId, adep.Version);
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
                lock (pendingRootChecks)
                {
                    pendingRootChecks.Add(a.LoadedAssembly);
                }
            }
        }

        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (extensionContext.LocalLock)
            {
                return loadedAddins.Values.Where(a => a.AssembliesLoaded).SelectMany(a => a.Assemblies).FirstOrDefault(a => a.FullName.ToString() == args.Name);
            }
        }

        private void CheckHostAssembly(Assembly asm)
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
            Bundle ainfo;
            try
            {
                ainfo = Registry.GetAddinForHostAssembly(asmFile);
            }
            catch (Exception ex)
            {
                Registry.Update();
                ainfo = Registry.GetAddinForHostAssembly(asmFile);
            }

            if (ainfo != null && !IsAddinLoaded(ainfo.Id))
            {
                BundleDescription adesc = null;
                try
                {
                    adesc = ainfo.Description;
                }
                catch (Exception ex)
                {
                }
                if (adesc == null || adesc.FilesChanged())
                {
                    // If the add-in has changed, update the add-in database.
                    // We do it here because once loaded, add-in roots can't be
                    // reloaded like regular add-ins.
                    Registry.Update();
                    ainfo = Registry.GetAddinForHostAssembly(asmFile);
                    if (ainfo == null)
                        return;
                }
                LoadAddin(ainfo.Id, false);
            }
        }

        #endregion

    }
}
