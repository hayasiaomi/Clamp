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
    public class BundleContext : IBundleContext
    {
        private ClampBundle clampBundle;

        /// <summary>
        /// 当前上下文所在的执行的Bundle
        /// </summary>
        public RuntimeBundle RuntimeBundle { private set; get; }
        /// <summary>
        /// 所在的Bundle
        /// </summary>
        public Bundle Bundle { get { return this.RuntimeBundle.Bundle; } }

        internal BundleContext(RuntimeBundle runtimeBundle, ClampBundle clampBundle)
        {
            this.clampBundle = clampBundle;
            this.RuntimeBundle = runtimeBundle;
        }
        /// <summary>
        /// 通过ID来获得执行的Bundle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RuntimeBundle GetRuntimeBundle(string id)
        {
            return this.clampBundle.GetRuntimeBundle(id);
        }

        /// <summary>
        /// 通过ID来获得Bundle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IBundle GetBundle(string id)
        {
            return this.clampBundle.Registry.GetBundle(id);
        }
        /// <summary>
        /// 获得所有的Bundle
        /// </summary>
        /// <returns></returns>
        public IBundle[] GetBundles()
        {
            return this.clampBundle.Registry.GetBundles();
        }

        /// <summary>
        /// 通过Name来对应的执行Bundle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RuntimeBundle GetRuntimeBundleByName(string name)
        {
            return this.clampBundle.GetRuntimeBundleByName(name);
        }

        #region 扩展功能
        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(Type instanceType)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(instanceType);
        }

        /// <summary>
        ///  获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>()
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>();
        }

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="instanceType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(Type instanceType, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(instanceType, reuseCachedInstance);
        }
        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>(reuseCachedInstance);
        }

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path);
        }

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path, reuseCachedInstance);
        }
        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arrayElementType"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, Type arrayElementType)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path, arrayElementType);
        }
        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(string path)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>(path);
        }

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arrayElementType"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public object[] GetExtensionObjects(string path, Type arrayElementType, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects(path, arrayElementType, reuseCachedInstance);
        }

        /// <summary>
        /// 获得扩展的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="reuseCachedInstance"></param>
        /// <returns></returns>
        public T[] GetExtensionObjects<T>(string path, bool reuseCachedInstance)
        {
            this.clampBundle.CheckInitialized();
            return this.clampBundle.GetExtensionObjects<T>(path, reuseCachedInstance);
        }

        #endregion

        #region 注册功能

        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, object instance)
        {
            return this.clampBundle.Register(registerType, instance);
        }
        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, object instance, string name)
        {
            return this.clampBundle.Register(registerType, instance, name);
        }

        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="registerImplementation"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance)
        {
            return this.clampBundle.Register(registerType, registerImplementation, instance);
        }

        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="registerType"></param>
        /// <param name="registerImplementation"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name)
        {
            return this.clampBundle.Register(registerType, registerImplementation, instance, name);
        }
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <param name="resolveType"></param>
        /// <returns></returns>
        public object Resolve(Type resolveType)
        {
            return this.clampBundle.Resolve(resolveType);
        }

        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <param name="resolveType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public object Resolve(Type resolveType, ResolveOptions options)
        {
            return this.clampBundle.Resolve(resolveType, options);
        }

        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <param name="resolveType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Resolve(Type resolveType, string name)
        {
            return this.clampBundle.Resolve(resolveType, name);
        }
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <returns></returns>
        public ResolveType Resolve<ResolveType>() where ResolveType : class
        {
            return this.clampBundle.Resolve<ResolveType>();
        }
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public ResolveType Resolve<ResolveType>(ResolveOptions options) where ResolveType : class
        {
            return this.clampBundle.Resolve<ResolveType>(options);
        }
        /// <summary>
        ///  解析服务对象
        /// </summary>
        /// <typeparam name="ResolveType"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public ResolveType Resolve<ResolveType>(string name) where ResolveType : class
        {
            return this.clampBundle.Resolve<ResolveType>(name);
        }

        #endregion

    }
}
