using Clamp.SDK.Framework.Injection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.DoozerImpl
{
    public class ServiceDoozer : IDoozer
    {
        public bool HandleConditions
        {
            get { return false; }
        }

        public object BuildItem(BuildItemArgs args)
        {
            var container = (Container)args.Parameter;
            if (container == null)
                throw new InvalidOperationException("Expected the parameter to be a service container");
            string id = args.Codon.Properties["id"];
            string typeName = args.Codon.Properties["type"];
            string className = args.Codon.Properties["class"];

            Type interfaceType = args.AddIn.FindType(typeName);

            if (!container.CanResolve(interfaceType))
            {
                bool serviceLoading = false;

                container.Register(interfaceType, (c, v) =>
                 {
                     if (serviceLoading)
                         throw new InvalidOperationException("Found cyclic dependency when initializating " + className);

                     serviceLoading = true;

                     return args.AddIn.CreateObject(className);
                 });
            }

            return container.Resolve(interfaceType);
        }
    }
}
