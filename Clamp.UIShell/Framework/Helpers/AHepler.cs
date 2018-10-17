using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.ClientService.Helpers
{
    public class AHepler
    {
        public static void DeleteAutoStart()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {

                    if (shanDianRegistry.GetValue("ShanDianClientService") != null)
                    {

                        shanDianRegistry.DeleteValue("ShanDianClientService");
                    }
                }
            }
            else
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (shanDianRegistry.GetValue("ShanDianClientService") != null)
                    {

                        shanDianRegistry.DeleteValue("ShanDianClientService");
                    }
                }
            }
        }

        public static void AutoStart(string launch)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (shanDianRegistry.GetValue("ShanDianClientService") == null)
                    {
                        shanDianRegistry.SetValue("ShanDianClientService", @"""" + launch + @"""");
                    }
                }
            }
            else
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (shanDianRegistry.GetValue("ShanDianClientService") == null)
                    {

                        shanDianRegistry.SetValue("ShanDianClientService", @"""" + launch + @"""");
                    }
                }
            }

        }
    }
}
