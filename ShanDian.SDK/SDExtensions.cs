using ShanDian.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK
{
    public static class SDExtensions
    {
        #region Service Provider Extensions
        /// <summary>
        /// Retrieves the service of type <c>T</c> from the provider.
        /// If the service cannot be found, this method returns <c>null</c>.
        /// </summary>
        public static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return (T)provider.GetService(typeof(T));
        }

        /// <summary>
        /// 获得相应的实例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static T GetRequiredInstance<T>(this IServiceProvider provider) where T : class
        {
            return (T)GetRequiredInstance(provider, typeof(T));
        }

        /// <summary>
        /// 获得相应的实例对象
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static object GetRequiredInstance(this IServiceProvider provider, Type serviceType)
        {
            return provider.GetService(serviceType); 
        }
        #endregion
    }
}
