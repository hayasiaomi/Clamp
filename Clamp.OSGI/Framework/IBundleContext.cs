using Clamp.OSGI.Framework.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public interface IBundleContext
    {
        RuntimeBundle GetRuntimeBundle(string id);
        IBundle GetBundle(string id);

        IBundle[] GetBundles();

        object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(bool reuseCachedInstance);

        object[] GetExtensionObjects(string path);

        object[] GetExtensionObjects(string path, bool reuseCachedInstance);

        object[] GetExtensionObjects(string path, Type arrayElementType);

        T[] GetExtensionObjects<T>(string path);
        object[] GetExtensionObjects(string path, Type arrayElementType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance);


    }
}
