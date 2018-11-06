using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    internal class GettextCatalog
    {
        public static string GetString(string str)
        {
            return str;
        }

        public static string GetString(string str, params object[] arguments)
        {
            return string.Format(GetString(str), arguments);
        }
    }
}
