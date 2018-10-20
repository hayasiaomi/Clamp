using Clamp.SDK.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public class BundleContext
    {
        private IClampFramework clampFramework;

        public IBundle Bundle { private set; get; }

        internal BundleContext(Bundle bundle, IClampFramework clampFramework)
        {
            this.clampFramework = clampFramework;
            this.Bundle = bundle;
        }

        public object GetService(Type sType, string name)
        {
            return this.container.GetService(sType, name);
        }

        public object GetService(Type sType)
        {
            return this.container.GetService(sType);
        }

        public void AddService(Type sType)
        {
            this.container.AddService(sType);
        }

        public void AddService(Type sType, string name)
        {
            this.container.AddService(sType, name);
        }

        public void AddService(Type sType, object sInstance)
        {
            this.container.AddService(sType, sInstance);
        }

        public void AddService(Type sType, object sInstance, string name)
        {
            this.container.Register(sType, sInstance, name);
        }

        public void RemoveService(Type sType)
        {
            this.container.RemoveService(sType);
        }

        public void RemoveService(Type sType, string name)
        {
            this.container.RemoveService(sType, name);
        }
    }
}
