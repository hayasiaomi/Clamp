using Clamp.Injection;
using Clamp.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp
{
    /// <summary>
    /// Bundle的上下文
    /// </summary>
    public interface IBundleContext
    {

        /// <summary>
        /// 当前上下文所在的执行的Bundle
        /// </summary>
        RuntimeBundle RuntimeBundle { get; }

        /// <summary>
        /// 所在的Bundle
        /// </summary>
        Bundle Bundle { get; }

        /// <summary>
        /// 通过ID来获得执行的Bundle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RuntimeBundle GetRuntimeBundle(string id);
        /// <summary>
        /// 通过Name来对应的执行Bundle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        RuntimeBundle GetRuntimeBundleByName(string name);

        /// <summary>
        /// 通过ID来获得Bundle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IBundle GetBundle(string id);

        /// <summary>
        /// 获得所有的Bundle
        /// </summary>
        /// <returns></returns>
        IBundle[] GetBundles();

        #region 扩展功能
        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance);
        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] GetExtensionObjects<T>();
        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        T[] GetExtensionObjects<T>(bool reuseCachedInstance);

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        object[] GetExtensionObjects(string path);

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        object[] GetExtensionObjects(string path, bool reuseCachedInstance);

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arrayElementType"></param>
        /// <returns></returns>
        object[] GetExtensionObjects(string path, Type arrayElementType);

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        T[] GetExtensionObjects<T>(string path);

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        object[] GetExtensionObjects(string path, Type arrayElementType, bool reuseCachedInstance);

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance);
        #endregion

        #region 注册功能
        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        RegisterOptions Register(Type registerType, object instance);
        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        RegisterOptions Register(Type registerType, object instance, string name);
        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="registerImplementation"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        RegisterOptions Register(Type registerType, Type registerImplementation, object instance);
        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="registerImplementation"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name);
        /// <summary>
        /// 解析服务对象
        /// </summary>
        /// <param name="resolveType"></param>
        /// <returns></returns>
        object Resolve(Type resolveType);
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <param name="resolveType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        object Resolve(Type resolveType, ResolveOptions options);
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <param name="resolveType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object Resolve(Type resolveType, string name);
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <returns></returns>
        ResolveType Resolve<ResolveType>() where ResolveType : class;
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        ResolveType Resolve<ResolveType>(ResolveOptions options) where ResolveType : class;
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        ResolveType Resolve<ResolveType>(string name) where ResolveType : class;

        #endregion

    }
}
