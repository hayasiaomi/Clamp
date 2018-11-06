using Clamp.OSGI.Framework.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public interface IClampBundle : IBundle, IDisposable
    {
        ExtensionNode GetExtensionNode(string path);

        T GetExtensionNode<T>(string path) where T : ExtensionNode;

        ExtensionNodeList GetExtensionNodes(string path);

        ExtensionNodeList GetExtensionNodes(string path, Type expectedNodeType);

        ExtensionNodeList<T> GetExtensionNodes<T>(string path) where T : ExtensionNode;

        ExtensionNodeList GetExtensionNodes(Type instanceType);

        ExtensionNodeList GetExtensionNodes(Type instanceType, Type expectedNodeType);

        ExtensionNodeList<T> GetExtensionNodes<T>(Type instanceType) where T : ExtensionNode;

        object[] GetInstance(Type instanceType);

        T[] GetInstance<T>();

        object[] GetInstance(Type instanceType, bool reuseCachedInstance);

        T[] GetInstance<T>(bool reuseCachedInstance);

        object[] GetInstance(string path);

        object[] GetInstance(string path, bool reuseCachedInstance);

        object[] GetInstance(string path, Type arrayElementType);

        T[] GetInstance<T>(string path);

        object[] GetInstance(string path, Type arrayElementType, bool reuseCachedInstance);

        T[] GetInstance<T>(string path, bool reuseCachedInstance);
    }
}
