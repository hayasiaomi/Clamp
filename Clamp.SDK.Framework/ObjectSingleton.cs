using Clamp.SDK.Framework.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework
{
    internal static class ObjectSingleton
    {
        private volatile static Container instance;

        /// <summary>
        /// 服务对象的供应者
        /// </summary>
        public static Container ObjectProvider
        {
            get { return instance; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                instance = value;
            }
        }

        /// <summary>
        /// 获得相应的服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRequiredInstance<T>() where T : class
        {
            if (instance != null && instance.CanResolve<T>())
            {
                T service = instance.Resolve<T>();

                if (service == null)
                    throw new ObjectNotFoundException(typeof(T));

                return (T)service;
            }

            return default(T);
        }
    }
}
