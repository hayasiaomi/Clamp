using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Helpers
{
    public sealed class DotNetHelper
    {
        /// <summary>
        /// 判断环境是否安装过 .NET 3.5 而且不审 在XP SP1上。
		/// </summary>
		public static bool IsDotnet35SP1Installed()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5"))
            {
                return key != null && (key.GetValue("SP") as int?) >= 1;
            }
        }

        /// <summary>
        /// 是否已安装过 .NET Framework 4.0
        /// </summary>
        /// <returns></returns>
        public static bool IsDotnet40Installed()
        {
            return true; // required for SD to run
        }

        /// <summary>
        /// 是否已安装过 .NET Framework 4.5
        /// </summary>
        /// <returns></returns>
        public static bool IsDotnet45Installed()
        {
            return GetDotnet4Release() >= 378389;
        }

        /// <summary>
        /// 是否已安装过 .NET Framework 4.5.1
        /// </summary>
        /// <returns></returns>
        public static bool IsDotnet451Installed()
        {
            return GetDotnet4Release() >= 378675;
        }
        /// <summary>
        ///是否已安装过 .NET Framework 4.5.2
        /// </summary>
        /// <returns></returns>
        public static bool IsDotnet452Installed()
        {
            // 379893 is .NET 4.5.2 on my Win7 machine
            return GetDotnet4Release() >= 379893;
        }

        /// <summary>
        ///  获得 .NET 4.x 的发布号码
        /// </summary>
        /// <returns></returns>
        static int? GetDotnet4Release()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
            {
                if (key != null)
                    return key.GetValue("Release") as int?;
            }
            return null;
        }

        /// <summary>
        ///  是否已安装过 Microsoft Build Tools 2013 (MSBuild 12.0) .
        /// </summary>
        /// <returns></returns>
        public static bool IsBuildTools2013Installed()
        {
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\DevDiv\BuildTools\Servicing\12.0
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\BuildTools\Servicing\12.0\MSBuild"))
            {
                return key != null && key.GetValue("Install") as int? >= 1;
            }
        }
    }
}
