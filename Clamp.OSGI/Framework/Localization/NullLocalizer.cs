using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Localization
{
    class NullLocalizer : IAddinLocalizer
    {
        public static AddinLocalizer Instance = new AddinLocalizer(new NullLocalizer());

        public string GetString(string msgid)
        {
            return msgid;
        }
    }
}
