using Clamp.Common;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework.Nodes
{
    /// <summary>
    /// 插件管理类
    /// </summary>
    public class AddInManager
    {
        private static AddInEngine addInEngine;
        private static bool initialized = false;

        /// <summary>
        /// 初始化
        /// </summary>
        internal static void Initialize()
        {
            Initialize(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AddIns"));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="addInDir"></param>
        internal static void Initialize(string addInDir)
        {
            if (!initialized)
            {
                addInEngine = new DefaultAddInEngine();

                if (!Directory.Exists(addInDir))
                    Directory.CreateDirectory(addInDir);

                addInEngine.AddAddInFile(Path.Combine(addInDir, "AddIns.Xml"));
                addInEngine.AddAddInsFromDirectory(addInDir);

                addInEngine.Initialize();

                addInEngine.Activate();

                initialized = true;
            }
        }

        /// <summary>
        /// 判断是否初始过
        /// </summary>
        public static bool IsInitialized
        {
            get { return initialized; }
        }



        /// <summary>
        /// 获得单个实例对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static object GetEntityInstance(string path, object parameter = null)
        {
            return addInEngine.BuildItem(path, parameter);
        }
        /// <summary>
        /// 获得单个实例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static T GetEntityInstance<T>(string path, object parameter = null)
        {
            object entity = addInEngine.BuildItem(path, parameter);

            if (entity != null)
                return (T)entity;

            return default(T);
        }
        /// <summary>
        /// 获得实例对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="throwOnNotFound"></param>
        /// <returns></returns>
        public static List<T> GetInstance<T>(string path, object parameter = null, bool throwOnNotFound = true)
        {
            return addInEngine.BuildItems<T>(path, parameter, throwOnNotFound);
        }
        /// <summary>
        /// 获得实例对象集合
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <param name="throwOnNotFound"></param>
        /// <returns></returns>
        public static object[] GetInstance(string path, object parameter = null, bool throwOnNotFound = true)
        {
            return addInEngine.BuildItems<object>(path, parameter, throwOnNotFound).ToArray();
        }

        public static T GetEntityService<T>(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default(T);

            return GetEntityInstance<T>("/ShanDain/Services/" + id, ObjectSingleton.ObjectProvider);
        }

        public static List<IService> GetServices()
        {
            return GetInstance<IService>("/ShanDain/Services", ObjectSingleton.ObjectProvider);
        }
    }
}
