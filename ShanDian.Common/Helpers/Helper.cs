using ShanDian.Common.Extensions;
using ShanDian.Common.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ShanDian.Common.Helpers
{
    public class Helper
    {
        public static List<string> GetComputerLanIP()
        {
            List<string> addrs = new List<string>();
            string strHostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            foreach (IPAddress addr in ipEntry.AddressList)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    addrs.Add(addr.ToString());
                }
            }

            return addrs;
        }

        public static int CompareVersion(string version1, string version2)
        {
            return NormalizeVersion(version1).CompareTo(NormalizeVersion(version2));
        }

     
        public static int CompareVersion(Version version1, Version version2)
        {
            return version1.Normalize().CompareTo(version2.Normalize());
        }
       
        private static Version NormalizeVersion(string version)
        {
            return Version.Parse(version).Normalize();
        }

    }
}
