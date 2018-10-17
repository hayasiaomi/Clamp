using Clamp.SDK.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AddIns
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
    }
}
