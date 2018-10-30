using Clamp.OSGI.DoozerImpl;
using Clamp.OSGI.Framework.Conditions;
using Clamp.OSGI.Framework.Nodes;
using Clamp.OSGI.Injection;
using Clamp.SDK.Doozer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace Clamp.OSGI.Framework
{
    internal class ClampFramework : Bundle, IClampFramework
    {
        private AddInTreeNode rootNode = new AddInTreeNode();
        private List<Bundle> addIns = new List<Bundle>();
        private ConcurrentDictionary<string, ExtensionNode> extensionNodes = new ConcurrentDictionary<string, ExtensionNode>();
        private List<string> bundleFiles = new List<string>();
        private List<string> disabledAddIns = new List<string>();
        private ObjectContainer objectContainer;

        public ReadOnlyCollection<Bundle> AddIns
        {
            get
            {
                return addIns.AsReadOnly();
            }
        }

        public ConcurrentDictionary<string, ExtensionNode> ExtensionNodes
        {
            get
            {
                return extensionNodes;
            }
        }


        /// <summary>
        /// 禁止的插件文件
        /// </summary>
        public List<string> DisableAddIns
        {
            get { return disabledAddIns; }
        }

        /// <summary>
        /// 插件文件集合
        /// </summary>
        public List<string> BundleFiles
        {
            get { return this.bundleFiles; }
        }

        internal ClampFramework()
        {
            this.objectContainer = new ObjectContainer();
        }


        public void Initialize()
        {
            this.Load(this.BundleFiles, this.DisableAddIns);
        }

        #region 重写 Bundle有方法
        public override void Start()
        {
            if (this.AddIns != null && this.AddIns.Count > 0)
            {
                List<Bundle> addIns = this.AddIns.OrderBy(addin => addin.StartLevel).ToList();

                foreach (Bundle addin in addIns)
                {
                    if (addin.Enabled)
                    {
                        if (!string.IsNullOrWhiteSpace(addin.ActivatorClassName))
                        {
                            IBundleActivator addInActivator = addin.CreateObject(addin.ActivatorClassName) as IBundleActivator;

                            if (addInActivator != null)
                            {
                                BundleContext context = new BundleContext(addin, this);

                                addInActivator.Start(context);
                            }
                        }
                    }
                }
            }

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



        public AddInTreeNode GetTreeNode(string path)
        {
            if (path == null || path.Length == 0)
            {
                return rootNode;
            }
            string[] splittedPath = path.Split('/');
            AddInTreeNode curPath = rootNode;
            for (int i = 0; i < splittedPath.Length; i++)
            {
                if (!curPath.ChildNodes.TryGetValue(splittedPath[i].ToUpper(), out curPath))
                {
                    return null;
                }
            }
            return curPath;
        }


        public void InsertAddIn(Bundle bundle)
        {
            if (bundle.Enabled)
            {
                foreach (AddInFeature addInFeature in bundle.Features.Values)
                {
                    AddExtensionPath(addInFeature);
                }

                foreach (AddInRuntime runtime in bundle.Runtimes)
                {
                    if (runtime.IsActive)
                    {
                        foreach (var pair in runtime.DefinedDoozers)
                        {
                            if (!extensionNodes.TryAdd(pair.Key, pair.Value))
                                throw new FrameworkException("Duplicate doozer: " + pair.Key);
                        }
                    }
                }

                string addInRoot = Path.GetDirectoryName(bundle.FileName);

                foreach (string bitmapResource in bundle.BitmapResources)
                {
                    string path = Path.Combine(addInRoot, bitmapResource);
                    ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
                    //ServiceSingleton.GetRequiredService<IResourceService>().RegisterNeutralImages(resourceManager);
                }

                foreach (string stringResource in bundle.StringResources)
                {
                    string path = Path.Combine(addInRoot, stringResource);
                    ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
                    //ServiceSingleton.GetRequiredService<IResourceService>().RegisterNeutralStrings(resourceManager);
                }
            }

            addIns.Add(bundle);
        }

        private void AddExtensionPath(AddInFeature path)
        {
            AddInTreeNode treePath = CreatePath(rootNode, path.Name);

            foreach (IEnumerable<Codon> innerCodons in path.GroupedCodons)
                treePath.AddCodons(innerCodons);
        }

        AddInTreeNode CreatePath(AddInTreeNode localRoot, string path)
        {
            if (path == null || path.Length == 0)
            {
                return localRoot;
            }

            string[] splittedPath = path.Split('/');

            AddInTreeNode curPath = localRoot;

            int i = 0;

            while (i < splittedPath.Length)
            {
                string keyValue = splittedPath[i]?.ToUpper();

                if (!curPath.ChildNodes.ContainsKey(keyValue))
                {
                    curPath.ChildNodes[keyValue] = new AddInTreeNode();
                }

                curPath = curPath.ChildNodes[keyValue];

                ++i;
            }

            return curPath;
        }

        private void DisableAddin(Bundle addIn, List<Bundle> enabledAddInsList, string mistake)
        {
            addIn.Enabled = false;

            addIn.Mistake = mistake;

            enabledAddInsList.Remove(addIn);
        }

        /// <summary>
        /// 加载插件树
        /// </summary>
        /// <param name="addInFiles"></param>
        /// <param name="disabledAddIns"></param>
        public void Load(List<string> addInFiles, List<string> disabledAddIns)
        {
            List<Bundle> addInslist = new List<Bundle>();
            List<Bundle> enabledAddInsList = new List<Bundle>();
            Dictionary<string, Bundle> addInDict = new Dictionary<string, Bundle>();

            var nameTable = new System.Xml.NameTable();

            foreach (string fileName in addInFiles)
            {
                Bundle bundle;

                try
                {
                    bundle = Bundle.Load(this, fileName, nameTable);
                }
                catch (FrameworkException ex)
                {
                    bundle = new Bundle(this);
                    bundle.FileName = fileName;
                    bundle.Enabled = false;
                    bundle.Mistake = ex.Message;
                }

                if (!bundle.Enabled)
                {
                    addInslist.Add(bundle);
                    continue;
                }

                bundle.Enabled = true;

                if (disabledAddIns != null && disabledAddIns.Count > 0)
                {
                    if (disabledAddIns.Contains(bundle.Name))
                    {
                        bundle.Enabled = false;
                        break;
                    }
                }

                if (bundle.Enabled)
                {
                    enabledAddInsList.Add(bundle);
                }

                addInslist.Add(bundle);
            }

        checkDependencies:

            for (int i = 0; i < addInslist.Count; i++)
            {
                Bundle addIn = addInslist[i];

                if (!addIn.Enabled)
                    continue;

                Bundle addInFound;

                foreach (AddInReference reference in addIn.Manifest.Conflicts)
                {
                    if (reference.Check(enabledAddInsList, out addInFound))
                    {
                        DisableAddin(addIn, enabledAddInsList, $"插件[{addIn.Name}]和插件[{addInFound.Name}]存在冲突");

                        //TODO 提示冲突的插件消息

                        goto checkDependencies;
                    }
                }

                foreach (AddInReference reference in addIn.Manifest.Dependencies)
                {
                    if (!reference.Check(enabledAddInsList, out addInFound))
                    {
                        DisableAddin(addIn, enabledAddInsList, $"插件[{addIn.Name}]的依赖插件[{reference.AddInId}]不存在");

                        //TODO 提示不可用的插件消息

                        goto checkDependencies;
                    }
                }
            }

            foreach (Bundle addIn in addInslist)
            {
                try
                {
                    InsertAddIn(addIn);
                }
                catch (FrameworkException ex)
                {
                }
            }

        }


        /// <summary>
        /// 生成对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="throwOnNotFound"></param>
        /// <returns></returns>
        private List<T> BuildItems<T>(string path, object parameter)
        {
            AddInTreeNode node = this.GetTreeNode(path);
            if (node == null)
                return new List<T>();
            else
                return node.BuildChildItems<T>(parameter);
        }


        public void Dispose()
        {

        }
    }
}
