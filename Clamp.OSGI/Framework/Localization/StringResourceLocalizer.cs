using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Localization
{
    class StringResourceLocalizer : IBundleLocalizerFactory, IBundleLocalizer
    {
        RuntimeBundle addin;

        public IBundleLocalizer CreateLocalizer(RuntimeBundle addin, NodeElement element)
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
