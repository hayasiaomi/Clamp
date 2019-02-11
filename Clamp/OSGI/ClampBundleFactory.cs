using Clamp.OSG.Initial;
using Clamp.OSGI.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI
{
    public sealed class ClampBundleFactory
    {
        public static IClampBundle GetClampBundle()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            if (asm == null)
                asm = Assembly.GetCallingAssembly();

            Dictionary<string, string> configProps = LoadConfigProperties(asm);

            ClampBundle clampBundle = new ClampBundle(configProps);

            clampBundle.Initialize(asm, "bundles", null);

            return clampBundle;
        }

        private static Dictionary<string, string> LoadConfigProperties(Assembly startupAsm)
        {
            string asmFile = new Uri(startupAsm.CodeBase).LocalPath;

            string startupDir = Path.GetDirectoryName(asmFile);

            FileInfo fileInfo = new FileInfo(Path.Combine(startupDir, ClampConstants.CLAMP_CONFIG_FILE));

            Dictionary<string, string> configProps = LoadDefaultConfigProperties();

            if (fileInfo.Exists)
            {
                ExtendedProperties extendedProperties = new ExtendedProperties(fileInfo.FullName);

                if (extendedProperties.Count > 0)
                {
                    foreach (string keyName in extendedProperties.Keys)
                    {
                        if (extendedProperties.ContainsKey(keyName))
                        {
                            configProps[keyName] = extendedProperties.GetString(keyName);
                        }
                        else
                        {
                            configProps.Add(keyName, extendedProperties.GetString(keyName));
                        }
                    }
                }

                return configProps;
            }

            return configProps;
        }

        private static Dictionary<string, string> LoadDefaultConfigProperties()
        {
            return new Dictionary<string, string>()
            {

            };
        }
    }
}
