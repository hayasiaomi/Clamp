using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Localization
{
    class NullLocalizer : IBundleLocalizer
    {
        public static BundleLocalizer Instance = new BundleLocalizer(new NullLocalizer());

        public string GetString(string msgid)
        {
            return msgid;
        }
    }
}
