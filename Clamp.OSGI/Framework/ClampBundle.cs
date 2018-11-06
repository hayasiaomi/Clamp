using Clamp.OSGI.DoozerImpl;
using Clamp.OSGI.Framework.Description;
using Clamp.OSGI.Framework.Data;
using Clamp.OSGI.Injection;
using Clamp.SDK.Doozer;
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

namespace Clamp.OSGI.Framework
{
    internal class ClampBundle : Bundle, IClampBundle
    {
        private BundleRegistry registry;
        private ObjectContainer objectContainer;
        private bool initialized;

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


        public ClampBundle() : base(null, null, null, "clamp")
        {
            this.objectContainer = new ObjectContainer();
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

            IClampAnalyzer clampAnalyzer = new DefaultClampAnalyzer(this, filesToIgnore);

            clampAnalyzer.CheckFolder();

            clampAnalyzer.Analyze(new string[] { this.StartupDirectory, finalBundlesDirectory });


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
            ActivateAddinExtensions(id);
        }

      

        #endregion




        public void Dispose()
        {

        }
    }
}
