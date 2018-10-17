using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Common.Extensions
{
    public static class CommonExtensions
    {
        private static readonly DateTime BaseTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将.NET的DateTime转换为unix timestamp时间戳，单位由参数2决定, 默认返回秒
        /// </summary>
        /// <param name="dateTime">待转换的时间</param>
        /// <param name="millisecond">返回值是毫秒还是秒</param>
        /// <returns>转换后的unix time</returns>
        public static long Unix(this DateTime dateTime, bool millisecond = false)
        {
            TimeSpan ret = (dateTime - (TimeZone.CurrentTimeZone.ToLocalTime(BaseTime)));
            if (millisecond)
                return (long)ret.TotalMilliseconds;
            else
                return (long)ret.TotalSeconds;
        }

        public static Version Normalize(this Version version)
        {
            return new Version(Math.Max(version.Major, 0), Math.Max(version.Minor, 0), Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
        }
    }
}
