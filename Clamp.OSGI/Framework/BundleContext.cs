using Clamp.SDK.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public class BundleContext : IBundleContext
    {
        private ClampFramework framework;

        public IBundle Bundle { private set; get; }

        internal BundleContext(Bundle bundle, ClampFramework framework)
        {
            this.framework = framework;
            this.Bundle = bundle;
        }

        public IBundle GetBundle(long id)
        {
            throw new NotImplementedException();
        }

        public IBundle[] GetBundles()
        {
            return this.framework.AddIns.ToArray();
        }

        public void AddServiceListener(IServiceListener listener)
        {
        }

        public void RemoveServiceListener(IServiceListener listener)
        {

        }

        public List<T> GetInstance<T>(string path, object parameter)
        {
            return this.framework.GetInstance<T>(path, parameter);
        }

        public object[] GetInstance(string path, object parameter)
        {
            return this.framework.GetInstance(path, parameter);
        }
    }
}
