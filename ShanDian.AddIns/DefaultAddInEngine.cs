using ShanDian.SDK.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns
{
    internal class DefaultAddInEngine : AddInEngine
    {
        public override void Initialize()
        {
            this.AddInTree.Load(this.AddInFiles, this.DisableAddIns);
        }

        public override void Activate()
        {
            if (this.AddInTree.AddIns != null && this.AddInTree.AddIns.Count > 0)
            {
                List<AddIn> addIns = this.AddInTree.AddIns.OrderBy(addin => addin.StartLevel).ToList();

                foreach (AddIn addin in addIns)
                {
                    if (addin.Enabled)
                    {
                        if (!string.IsNullOrWhiteSpace(addin.ActivatorClassName))
                        {
                            IAddInActivator addInActivator = addin.CreateObject(addin.ActivatorClassName) as IAddInActivator;

                            if (addInActivator != null)
                            {
                                AddInContext addInContext = new AddInContext(addin, ObjectSingleton.ObjectProvider as SDContainer);

                                addInActivator.Start(addInContext);
                            }
                        }
                    }
                }
            }
        }
    }
}
