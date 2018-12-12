using Clamp.OSGI.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public interface IClampBundle : IBundle
    {
        //ExtensionNode GetExtensionNode(string path);

        //T GetExtensionNode<T>(string path) where T : ExtensionNode;

        //ExtensionNodeList GetExtensionNodes(string path);

        //ExtensionNodeList GetExtensionNodes(string path, Type expectedNodeType);

        //ExtensionNodeList<T> GetExtensionNodes<T>(string path) where T : ExtensionNode;

        //ExtensionNodeList GetExtensionNodes(Type instanceType);

        //ExtensionNodeList GetExtensionNodes(Type instanceType, Type expectedNodeType);

        //ExtensionNodeList<T> GetExtensionNodes<T>(Type instanceType) where T : ExtensionNode;

        object[] GetExtensionObjects(Type instanceType);

        T[] GetExtensionObjects<T>();

        object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(bool reuseCachedInstance);

        object[] GetExtensionObjects(string path);

        object[] GetExtensionObjects(string path, bool reuseCachedInstance);

        object[] GetExtensionObjects(string path, Type arrayElementType);

        T[] GetExtensionObjects<T>(string path);

        object[] GetExtensionObjects(string path, Type arrayElementType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance);

        void WaitForStop();
    }
}
