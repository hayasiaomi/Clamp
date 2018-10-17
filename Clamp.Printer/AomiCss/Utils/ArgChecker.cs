using System;
using System.IO;

namespace Hydra.AomiCss.Utils
{
    /// <summary>
    /// ���������
    /// </summary>
    internal static class ArgChecker
    {
        /// <summary>
        /// �����������������������
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void AssertIsTrue<TException>(bool condition, string message) where TException : Exception, new()
        {
            if (!condition)
            {
                throw (TException)Activator.CreateInstance(typeof(TException), message);
            }
        }

        /// <summary>
        /// ��������ΪNULL
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        public static void AssertArgNotNull(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// ��������ΪIntPtr.Zero
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        public static void AssertArgNotNull(IntPtr arg, string argName)
        {
            if (arg == IntPtr.Zero)
            {
                throw new ArgumentException("IntPtr argument cannot be Zero", argName);
            }
        }

        /// <summary>
        /// ��������ΪNULL��հ�
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        public static void AssertArgNotNullOrEmpty(string arg, string argName)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentNullException(argName);
            }
        }


        /// <summary>
        /// ��������ΪNULLͬʱ��ָ����T��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        /// <returns></returns>
        public static T AssertArgOfType<T>(object arg, string argName)
        {
            AssertArgNotNull(arg, argName);

            if (arg is T)
            {
                return (T)arg;
            }
            throw new ArgumentException(string.Format("Given argument isn't of type '{0}'.", typeof(T).Name), argName);
        }

        /// <summary>
        /// ����ļ��������
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="argName"></param>
        public static void AssertFileExist(string arg, string argName)
        {
            AssertArgNotNullOrEmpty(arg, argName);

            if (false == File.Exists(arg))
            {
                throw new FileNotFoundException(string.Format("Given file in argument '{0}' not exist.", argName), arg);
            }
        }
    }
}
