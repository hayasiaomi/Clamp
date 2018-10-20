using Clamp.OSGI.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    internal class ClampFramework : Bundle, IClampFramework,IDisposable
    {
        private List<string> addInFiles = new List<string>();
        private List<string> disabledAddIns = new List<string>();
        private AddInTreeImpl addInTree;
        private ObjectContainer objectContainer;
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
        public List<string> AddInFiles
        {
            get { return this.addInFiles; }
        }

        /// <summary>
        /// 插件树
        /// </summary>
        public AddInTreeImpl AddInTree
        {
            get { return addInTree; }
        }


        internal ClampFramework(IAddInTree addInTree) : base(addInTree)
        {
            this.addInTree = new AddInTreeImpl();
            this.objectContainer = new ObjectContainer();
        }


        public void Initailize()
        {
            this.AddInTree.Load(this.AddInFiles, this.DisableAddIns);

            if (this.AddInTree.AddIns != null && this.AddInTree.AddIns.Count > 0)
            {
                List<Bundle> addIns = this.AddInTree.AddIns.OrderBy(addin => addin.StartLevel).ToList();

                foreach (Bundle addin in addIns)
                {
                    if (addin.Enabled)
                    {
                        if (!string.IsNullOrWhiteSpace(addin.ActivatorClassName))
                        {
                            IBundleActivator addInActivator = addin.CreateObject(addin.ActivatorClassName) as IBundleActivator;

                            if (addInActivator != null)
                            {
                                BundleContext addInContext = new BundleContext(addin, ObjectSingleton.ObjectProvider as SDContainer);

                                addInActivator.Start(addInContext);
                            }
                        }
                    }
                }
            }
        }


        public override void Start()
        {
           
        }

        public override void Stop()
        {
        }


        public void Dispose()
        {

        }

    }
}
