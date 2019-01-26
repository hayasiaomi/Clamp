using Clamp.OSGI.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public interface IClampBundle : IBundle, IDisposable
    {

        object[] GetExtensionObjects(Type instanceType);

        object[] GetExtensionObjectsByBundleId(string bid, Type instanceType);

        T[] GetExtensionObjects<T>();

        object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance);

        object[] GetExtensionObjects(string bid, Type instanceType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(bool reuseCachedInstance);

        object[] GetExtensionObjects(string path);

        object[] GetExtensionObjects(string path, bool reuseCachedInstance);

        object[] GetExtensionObjects(string path, Type arrayElementType);

        object[] GetExtensionObjects(string path, string bid, Type arrayElementType);

        T[] GetExtensionObjects<T>(string path);

        T[] GetExtensionObjects<T>(string path, string bid);

        object[] GetExtensionObjects(string path, string bid, Type arrayElementType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(string path, string bid, bool reuseCachedInstance);

        void WaitForStop();
    }
}
