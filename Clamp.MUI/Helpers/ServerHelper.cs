using Clamp.MUI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Helpers
{
    public class ServerHelper
    {
        private static string ServerVersion = null;

        public static string GetServerVersion()
        {
            if (ServerVersion == null)
            {
                string[] sdsDirectoryNames = Directory.GetDirectories(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Shandian.Server", SearchOption.TopDirectoryOnly);

                if (sdsDirectoryNames == null || sdsDirectoryNames.Length <= 0)
                {
                    DebugHelper.WriteLine("找不到目录Shandian.Server");

                    return null;
                }

                Version maxVersion = null;

                if (sdsDirectoryNames.Length > 1)
                {
                    DebugHelper.WriteLine("多个目录Shandian.Server");
                }

                foreach (string sdsDirectoryName in sdsDirectoryNames)
                {
                    string hydraWorkAreaDll = System.IO.Path.Combine(sdsDirectoryName, "Hydra.WorkArea.dll");

                    string fileVersion = FileVersionInfo.GetVersionInfo(hydraWorkAreaDll).FileVersion;

                    if (maxVersion == null)
                    {
                        maxVersion = new Version(fileVersion);
                    }
                    else
                    {
                        Version tempVersion = new Version(fileVersion);

                        if (tempVersion > maxVersion)
                        {
                            maxVersion = tempVersion;
                        }
                    }

                }

                ServerVersion = maxVersion.ToString();
            }

            return ServerVersion;

        }
    }
}
