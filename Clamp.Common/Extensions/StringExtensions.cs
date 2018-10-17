using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Md5Encrypt(this string value)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(value, "MD5");
        }

        public static string StringFormat(this string value, params object[] args)
        {
            return string.Format(value, args);
        }

        public static bool EqualsIgnoreCase(this string value, string compare)
        {
            return string.Equals(value, compare, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
