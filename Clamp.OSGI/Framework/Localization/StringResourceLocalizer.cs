using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Localization
{
    class StringResourceLocalizer : IAddinLocalizerFactory, IAddinLocalizer
    {
        RuntimeAddin addin;

        public IAddinLocalizer CreateLocalizer(RuntimeAddin addin, NodeElement element)
        {
            this.addin = addin;
            return this;
        }

        public string GetString(string msgid)
        {
            string s = addin.GetResourceString(msgid, false, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (s == null)
                return msgid;
            else
                return s;
        }
    }
}
