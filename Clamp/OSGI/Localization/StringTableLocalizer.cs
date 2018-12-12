using Clamp.OSGI.Data.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Localization
{
    class StringTableLocalizer : IBundleLocalizerFactory, IBundleLocalizer
    {
        Hashtable locales = new Hashtable();
        static Hashtable nullLocale = new Hashtable();

        public IBundleLocalizer CreateLocalizer(RuntimeBundle addin, NodeElement element)
        {
            foreach (NodeElement nloc in element.ChildNodes)
            {
                if (nloc.NodeName != "Locale")
                    throw new InvalidOperationException("Invalid element found: '" + nloc.NodeName + "'. Expected: 'Locale'");
                string ln = nloc.GetAttribute("id");
                if (ln.Length == 0)
                    throw new InvalidOperationException("Locale id not specified");
                ln = ln.Replace('_', '-');
                Hashtable messages = new Hashtable();
                foreach (NodeElement nmsg in nloc.ChildNodes)
                {
                    if (nmsg.NodeName != "Msg")
                        throw new InvalidOperationException("Locale '" + ln + "': Invalid element found: '" + nmsg.NodeName + "'. Expected: 'Msg'");
                    string id = nmsg.GetAttribute("id");
                    if (id.Length == 0)
                        throw new InvalidOperationException("Locale '" + ln + "': Message id not specified");
                    messages[id] = nmsg.GetAttribute("str");
                }
                locales[ln] = messages;
            }
            return this;
        }

        public string GetString(string id)
        {
            string cname = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            Hashtable loc = (Hashtable)locales[cname];
            if (loc == null)
            {
                string sn = cname.Substring(0, 2);
                loc = (Hashtable)locales[sn];
                if (loc != null)
                    locales[cname] = loc;
                else
                {
                    locales[cname] = nullLocale;
                    return id;
                }
            }
            string msg = (string)loc[id];
            if (msg == null)
            {
                if (cname.Length > 2)
                {
                    // Try again without the country
                    cname = cname.Substring(0, 2);
                    loc = (Hashtable)locales[cname];
                    if (loc != null)
                    {
                        msg = (string)loc[id];
                        if (msg != null)
                            return msg;
                    }
                }
                return id;
            }
            else
                return msg;
        }
    }
}
