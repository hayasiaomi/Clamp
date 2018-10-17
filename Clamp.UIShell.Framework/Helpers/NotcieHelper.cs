using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Helpers
{
    public class NotcieHelper
    {
        public static string GetFileName()
        {
            return string.Format("Advices-{0}.db", DateTime.Now.ToString("yyyyMMdd"));
        }
        public static string GetPrevFileName()
        {
            return string.Format("Advices-{0}.db", DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString("yyyyMMdd"));
        }
    }
}
