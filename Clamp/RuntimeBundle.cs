using Clamp.Data;
using Clamp.Data.Description;
using Clamp.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Clamp
{
    /// <summary>
    /// 运行时候的Bundle
    /// </summary>
    public class RuntimeBundle
    {
        private string id;
        private string baseDirectory;
        private string privatePath;
        private Bundle bundle;
        private RuntimeBundle parentBundle;
        private Assembly[] assemblies;
        private RuntimeBundle[] depBundles;
        private ResourceManager[] resourceManagers;
        private BundleLocalizer localizer;
        private IBundleActivator bundleActivator;
        private ModuleDescription module;
        private ClampBundle clampBundle;

        internal RuntimeBundle(ClampBundle clampBundle)
        {
            this.clampBundle = clampBundle;
        }

        internal RuntimeBundle(ClampBundle clampBundle, RuntimeBundle parentBundle, ModuleDescription module)
        {
            this.clampBundle = clampBundle;
            this.parentBundle = parentBundle;
            this.module = module;
            this.id = parentBundle.id;
            this.baseDirectory = parentBundle.baseDirectory;
            this.privatePath = parentBundle.privatePath;
            this.bundle = parentBundle.bundle;
            this.localizer = parentBundle.localizer;
            this.module.RuntimeBundle = this;
        }

        /// <summary>
        /// Bundle的Id
        /// </summary>
        public string Id
        {
            get { return Bundle.GetIdName(id); }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get { return Bundle.GetIdVersion(id); }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return this.bundle.Name; }
        }


        /// <summary>
        /// Path to a directory where add-ins can store private configuration or status data
        /// </summary>
        public string PrivateDataPath
        {
            get
            {
                if (privatePath == null)
                {
                    privatePath = bundle.PrivateDataPath;
                    if (!Directory.Exists(privatePath))
                        Directory.CreateDirectory(privatePath);
                }
                return privatePath;
            }
        }


        /// <summary>
        /// 本地化
        /// </summary>
        public BundleLocalizer Localizer
        {
            get
            {
                if (localizer != null)
                    return localizer;
                else
                    return clampBundle.DefaultLocalizer;
            }
        }

        /// <summary>
        /// 激活类
        /// </summary>
        internal IBundleActivator BundleActivator
        {
            get
            {
                return this.bundleActivator;
            }
        }

        /// <summary>
        /// 对应的Bundle
        /// </summary>
        internal Bundle Bundle
        {
            get { return bundle; }
        }

        /// <summary>
        /// 是否加载过程序集
        /// </summary>
        internal bool AssembliesLoaded
        {
            get { return assemblies != null; }
        }

        internal ModuleDescription Module
        {
            get { return module; }
        }

        internal Assembly[] Assemblies
        {
            get
            {
                EnsureAssembliesLoaded();
                return assemblies;
            }
        }


        #region public method

        public override string ToString()
        {
            return bundle.ToString();
        }


        /// <summary>
        /// 获得Bundle的内部资源文件的内容
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetResourceString(string name)
        {
            return (string)GetResourceObject(name, true, null);
        }

        /// <summary>
        /// 获得Bundle的内部资源文件的内容
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public string GetResourceString(string name, bool throwIfNotFound)
        {
            return (string)GetResourceObject(name, throwIfNotFound, null);
        }

        /// <summary>
        /// 获得Bundle的内部资源文件的内容
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwIfNotFound"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetResourceString(string name, bool throwIfNotFound, CultureInfo culture)
        {
            return (string)GetResourceObject(name, throwIfNotFound, culture);
        }

        /// <summary>
        /// 获得Bundle的内部资源文件的内容
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetResourceObject(string name)
        {
            return GetResourceObject(name, true, null);
        }

        /// <summary>
        /// 获得Bundle的内部资源文件的内容
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public object GetResourceObject(string name, bool throwIfNotFound)
        {
            return GetResourceObject(name, throwIfNotFound, null);
        }

        /// <summary>
        /// 获得Bundle的内部资源文件的内容
        /// </summary>
        /// <param name="name"></param>
        /// <param name="throwIfNotFound"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object GetResourceObject(string name, bool throwIfNotFound, CultureInfo culture)
        {
            // Look in resources of this add-in
            foreach (ResourceManager manager in GetAllResourceManagers())
            {
                object t = manager.GetObject(name, culture);
                if (t != null)
                    return t;
            }

            // Look in resources of dependent add-ins
            foreach (RuntimeBundle addin in GetAllDependencies())
            {
                object t = addin.GetResourceObject(name, false, culture);
                if (t != null)
                    return t;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException("Resource object '" + name + "' not found in add-in '" + id + "'");

            return null;
        }

        /// <summary>
        /// 获得Bundle的类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Type GetType(string typeName)
        {
            return GetType(typeName, true);
        }

        /// <summary>
        /// 获得Bundle的类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public Type GetType(string typeName, bool throwIfNotFound)
        {
            EnsureAssembliesLoaded();

            // Look in the addin assemblies

            Type at = Type.GetType(typeName, false);
            if (at != null)
                return at;

            foreach (Assembly asm in GetAllAssemblies())
            {
                Type t = asm.GetType(typeName, false);
                if (t != null)
                    return t;
            }

            // Look in the dependent add-ins
            foreach (RuntimeBundle rb in GetAllDependencies())
            {
                Type t = rb.GetType(typeName, false);
                if (t != null)
                    return t;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException("Type '" + typeName + "' not found in add-in '" + id + "'");
            return null;
        }


        /// <summary>
        /// 创建一个指定的类型的对象
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public object CreateInstance(string typeName)
        {
            return CreateInstance(typeName, true);
        }

        /// <summary>
        /// 创建一个指定的类型的对象
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public object CreateInstance(string typeName, bool throwIfNotFound)
        {
            Type type = GetType(typeName, throwIfNotFound);
            if (type == null)
                return null;
            else
                return Activator.CreateInstance(type, true);
        }

        /// <summary>
        /// 获得文件名来获得Bundle的绝对路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFilePath(string fileName)
        {
            return Path.Combine(baseDirectory, fileName);
        }

        /// <summary>
        /// 获得文件名来获得Bundle的绝对路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string GetFilePath(params string[] filePath)
        {
            return Path.Combine(baseDirectory, string.Join("" + Path.DirectorySeparatorChar, filePath));
        }

        /// <summary>
        /// 获得当前Bundle的内部所有资源文件名
        /// </summary>
        /// <returns></returns>
        public List<string> GetResourceNames()
        {
            EnsureAssembliesLoaded();

            List<string> resourceNames = new List<string>();

            foreach (Assembly asm in GetAllAssemblies())
            {
                resourceNames.AddRange(asm.GetManifestResourceNames());
            }

            return resourceNames;
        }

        /// <summary>
        /// 通过指定的资源文件名来获得流
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public Stream GetResource(string resourceName)
        {
            return GetResource(resourceName, false);
        }

        /// <summary>
        /// 通过指定的资源文件名来获得流
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        public Stream GetResource(string resourceName, bool throwIfNotFound)
        {
            EnsureAssembliesLoaded();

            foreach (Assembly asm in GetAllAssemblies())
            {
                Stream res = asm.GetManifestResourceStream(resourceName);
                if (res != null)
                    return res;
            }

            foreach (RuntimeBundle addin in GetAllDependencies())
            {
                Stream res = addin.GetResource(resourceName);
                if (res != null)
                    return res;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException("Resource '" + resourceName + "' not found in add-in '" + id + "'");

            return null;
        }


        public ManifestResourceInfo GetResourceInfo(string resourceName)
        {
            EnsureAssembliesLoaded();

            foreach (Assembly asm in GetAllAssemblies())
            {
                var res = asm.GetManifestResourceInfo(resourceName);

                if (res != null)
                {
                    if (res.ReferencedAssembly == null)
                        return new ManifestResourceInfo(asm, res.FileName, res.ResourceLocation);
                    return res;
                }
            }

            foreach (RuntimeBundle addin in GetAllDependencies())
            {
                var res = addin.GetResourceInfo(resourceName);
                if (res != null)
                    return res;
            }

            return null;
        }

        /// <summary>
        /// 获得当前Bundle的扩展对象
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(Type instanceType)
        {
            return this.clampBundle.GetExtensionObjects(this.Id, instanceType);
        }

        /// <summary>
        /// 获得当前Bundle的扩展对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>()
        {
            return this.clampBundle.GetExtensionObjectsByBundleId<T>(this.Id);
        }

        #endregion

        #region internal Method

        /// <summary>
        /// 通过模块详细来获得执行Bundle
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        internal RuntimeBundle GetModule(ModuleDescription module)
        {
            // If requesting the root module, return this
            if (module == module.ParentBundleDescription.MainModule)
                return this;

            if (module.RuntimeBundle != null)
                return module.RuntimeBundle;

            return new RuntimeBundle(clampBundle, this, module);
        }

        internal BundleDescription Load(Bundle bundle)
        {
            this.bundle = bundle;

            BundleDescription description = bundle.Description;

            this.id = description.BundleId;
            this.baseDirectory = description.BasePath;
            this.module = description.MainModule;
            this.module.RuntimeBundle = this;

            if (description.Localizer != null)
            {
                string cls = description.Localizer.GetAttribute("type");

                // First try getting one of the stock localizers. If none of found try getting the type.
                object fob = null;
                Type t = Type.GetType("Mono.Bundles.Localization." + cls + "Localizer, " + GetType().Assembly.FullName, false);
                if (t != null)
                    fob = Activator.CreateInstance(t);

                if (fob == null)
                    fob = CreateInstance(cls, true);

                IBundleLocalizerFactory factory = fob as IBundleLocalizerFactory;

                if (factory == null)
                    throw new InvalidOperationException("Localizer factory type '" + cls + "' must implement IBundleLocalizerFactory");

                this.localizer = new BundleLocalizer(factory.CreateLocalizer(this, description.Localizer));
            }

            if (description.Activator != null)
            {
                string cls = description.Activator.GetAttribute("type");

                this.bundleActivator = this.CreateInstance(cls, true) as IBundleActivator;
            }

            this.EnsureAssembliesLoaded();

            return description;
        }

        internal void UnloadExtensions()
        {
            clampBundle.UnregisterBundleNodeSets(id);
        }

        /// <summary>
        /// 确保当前Bundle需要的程序集都加载过了
        /// </summary>
        internal void EnsureAssembliesLoaded()
        {
            if (assemblies != null)
                return;

            ArrayList asmList = new ArrayList();

            CheckBundleDependencies(module, true);

            LoadModule(module, asmList);

            assemblies = (Assembly[])asmList.ToArray(typeof(Assembly));

            clampBundle.RegisterAssemblies(this);
        }

        #endregion

        #region private method


        private IEnumerable<ResourceManager> GetAllResourceManagers()
        {
            foreach (ResourceManager rm in GetResourceManagers())
                yield return rm;

            if (parentBundle != null)
            {
                foreach (ResourceManager rm in parentBundle.GetResourceManagers())
                    yield return rm;
            }
        }

        private IEnumerable<Assembly> GetAllAssemblies()
        {
            foreach (Assembly asm in Assemblies)
                yield return asm;

            // Look in the parent addin assemblies

            if (parentBundle != null)
            {
                foreach (Assembly asm in parentBundle.Assemblies)
                    yield return asm;
            }
        }

        private IEnumerable<RuntimeBundle> GetAllDependencies()
        {
            // Look in the dependent add-ins
            foreach (RuntimeBundle addin in GetDepBundles())
                yield return addin;

            if (parentBundle != null)
            {
                // Look in the parent dependent add-ins
                foreach (RuntimeBundle addin in parentBundle.GetDepBundles())
                    yield return addin;
            }
        }

        private ResourceManager[] GetResourceManagers()
        {
            if (resourceManagers != null)
                return resourceManagers;

            EnsureAssembliesLoaded();
            ArrayList managersList = new ArrayList();

            // Search for embedded resource files
            foreach (Assembly asm in assemblies)
            {
                foreach (string res in asm.GetManifestResourceNames())
                {
                    if (res.EndsWith(".resources"))
                        managersList.Add(new ResourceManager(res.Substring(0, res.Length - ".resources".Length), asm));
                }
            }

            return resourceManagers = (ResourceManager[])managersList.ToArray(typeof(ResourceManager));
        }

        private RuntimeBundle[] GetDepBundles()
        {
            if (depBundles != null)
                return depBundles;

            ArrayList plugList = new ArrayList();
            string ns = bundle.Description.Namespace;

            // Collect dependent ids
            foreach (Dependency dep in module.Dependencies)
            {
                BundleDependency pdep = dep as BundleDependency;
                if (pdep != null)
                {
                    RuntimeBundle adn = clampBundle.GetRuntimeBundle(Bundle.GetFullId(ns, pdep.BundleId, pdep.Version));
                    if (adn != null)
                        plugList.Add(adn);
                    else
                        clampBundle.ReportError("Add-in dependency not loaded: " + pdep.FullBundleId, module.ParentBundleDescription.BundleId, null, false);
                }
            }

            return depBundles = (RuntimeBundle[])plugList.ToArray(typeof(RuntimeBundle));
        }
        /// <summary>
        /// 加载当前Bundle所有程序集
        /// </summary>
        /// <param name="module"></param>
        /// <param name="asmList"></param>
        private void LoadModule(ModuleDescription module, ArrayList asmList)
        {
            foreach (string s in module.Assemblies)
            {
                Assembly asm = null;

                string asmPath = Path.Combine(baseDirectory, s);

                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a is System.Reflection.Emit.AssemblyBuilder || a.IsDynamic)
                    {
                        continue;
                    }

                    try
                    {
                        if (a.Location == asmPath)
                        {
                            asm = a;
                            break;
                        }
                    }
                    catch (NotSupportedException)
                    {
                       
                    }
                }

                if (asm == null)
                {
                    asm = Assembly.LoadFrom(asmPath);
                }

                asmList.Add(asm);
            }
        }

        /// <summary>
        /// 检测依赖的Bundle是否加载了。forceLoadAssemblies表示如果没有是否要加载
        /// </summary>
        /// <param name="module"></param>
        /// <param name="forceLoadAssemblies"></param>
        /// <returns></returns>
        private bool CheckBundleDependencies(ModuleDescription module, bool forceLoadAssemblies)
        {
            foreach (Dependency dep in module.Dependencies)
            {
                BundleDependency pdep = dep as BundleDependency;

                if (pdep == null)
                    continue;

                if (!clampBundle.IsBundleLoaded(pdep.FullBundleId))
                    return false;

                if (forceLoadAssemblies)
                    clampBundle.GetRuntimeBundle(pdep.FullBundleId).EnsureAssembliesLoaded();
            }

            return true;
        }

        #endregion

    }
}
