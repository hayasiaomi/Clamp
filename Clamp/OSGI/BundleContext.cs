using Clamp.OSGI.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public class BundleContext : IBundleContext
    {
        private ClampBundle clampBundle;

        public RuntimeBundle RuntimeBundle { private set; get; }

        public Bundle Bundle { get { return this.RuntimeBundle.Bundle; } }

        internal BundleContext(RuntimeBundle runtimeBundle, ClampBundle clampBundle)
        {
            this.clampBundle = clampBundle;
            this.RuntimeBundle = runtimeBundle;
        }

        public RuntimeBundle GetRuntimeBundle(string id)
        {
            return this.clampBundle.GetBundle(id);
        }

        public IBundle GetBundle(string id)
        {
            return this.clampBundle.Registry.GetBundle(id);
        }

        public IBundle[] GetBundles()
        {
            return this.clampBundle.Registry.GetBundles();
        }

        public object[] GetExtensionObjects(Type instanceType)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(instanceType);
        }


        public T[] GetExtensionObjects<T>()
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>();
        }


        public object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(instanceType, reuseCachedInstance);
        }

        public T[] GetExtensionObjects<T>(bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>(reuseCachedInstance);
        }


        public object[] GetExtensionObjects(string path)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path);
        }


        public object[] GetExtensionObjects(string path, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path, reuseCachedInstance);
        }

        public object[] GetExtensionObjects(string path, Type arrayElementType)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path, arrayElementType);
        }

        public T[] GetExtensionObjects<T>(string path)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>(path);
        }

        public object[] GetExtensionObjects(string path, Type arrayElementType, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path, arrayElementType, reuseCachedInstance);
        }

        public T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>(path, reuseCachedInstance);
        }


    }
}
