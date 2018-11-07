using Clamp.OSGI.Framework.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public class BundleContext : IBundleContext
    {
        private ClampBundle clampBundle;

        public IBundle Bundle { private set; get; }

        internal BundleContext(Bundle bundle, ClampBundle clampBundle)
        {
            this.clampBundle = clampBundle;
            this.Bundle = bundle;
        }

        public IBundle GetBundle(long id)
        {
            throw new NotImplementedException();
        }

        public IBundle[] GetBundles()
        {
            return this.clampBundle.Registry.GetAddins();
        }

        public void AddServiceListener(IServiceListener listener)
        {
        }

        public void RemoveServiceListener(IServiceListener listener)
        {

        }

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
    }
}
