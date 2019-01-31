using Clamp.OSGI.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public interface IBundleContext
    {
        RuntimeBundle RuntimeBundle { get; }

        Bundle Bundle { get; }

        RuntimeBundle GetRuntimeBundle(string id);

        RuntimeBundle GetRuntimeBundleByName(string name);

        Dictionary<string, string> GetConfigMaps();

        IBundle GetBundle(string id);

        IBundle[] GetBundles();

        object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>();

        T[] GetExtensionObjects<T>(bool reuseCachedInstance);

        object[] GetExtensionObjects(string path);

        object[] GetExtensionObjects(string path, bool reuseCachedInstance);

        object[] GetExtensionObjects(string path, Type arrayElementType);

        T[] GetExtensionObjects<T>(string path);

        object[] GetExtensionObjects(string path, Type arrayElementType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance);


    }
}
