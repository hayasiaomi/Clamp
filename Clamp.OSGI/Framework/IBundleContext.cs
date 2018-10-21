using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public interface IBundleContext
    {
        IBundle GetBundle(long id);

        IBundle[] GetBundles();

        void AddServiceListener(IServiceListener listener);

        void RemoveServiceListener(IServiceListener listener);

        /// <summary>
        /// 获得实例对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="throwOnNotFound"></param>
        /// <returns></returns>
        List<T> GetInstance<T>(string path, object parameter, bool throwOnNotFound = true);
        /// <summary>
        /// 获得实例对象集合
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="throwOnNotFound"></param>
        /// <returns></returns>
        object[] GetInstance(string path, object parameter, bool throwOnNotFound = true);
    }
}
