using Clamp.OSGI.Injection;
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

        IBundle GetBundle(string id);

        IBundle[] GetBundles();
        #region 扩展功能
        object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>();

        T[] GetExtensionObjects<T>(bool reuseCachedInstance);

        object[] GetExtensionObjects(string path);

        object[] GetExtensionObjects(string path, bool reuseCachedInstance);

        object[] GetExtensionObjects(string path, Type arrayElementType);

        T[] GetExtensionObjects<T>(string path);

        object[] GetExtensionObjects(string path, Type arrayElementType, bool reuseCachedInstance);

        T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance);
        #endregion

        #region 注册功能

        RegisterOptions Register(Type registerType, object instance);

        RegisterOptions Register(Type registerType, object instance, string name);

        RegisterOptions Register(Type registerType, Type registerImplementation, object instance);

        RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name);

        object Resolve(Type resolveType);

        object Resolve(Type resolveType, ResolveOptions options);

        object Resolve(Type resolveType, string name);

        ResolveType Resolve<ResolveType>() where ResolveType : class;

        ResolveType Resolve<ResolveType>(ResolveOptions options) where ResolveType : class;

        ResolveType Resolve<ResolveType>(string name) where ResolveType : class;

        #endregion

    }
}
