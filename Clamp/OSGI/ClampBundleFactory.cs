using Clamp.OSG.Initial;
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
                InitialFile initialProperties = InitialFile.LoadFromFile(fileInfo.FullName);

                if (initialProperties != null && initialProperties.SectionCount > 0)
                {
                    foreach (InitialProperty initialProperty in initialProperties)
                    {
                        if (configProps.ContainsKey(initialProperty.Name))
                        {
                            configProps[initialProperty.Name] = initialProperty.StringValueTrimmed;
                        }
                        else
                        {
                            configProps.Add(initialProperty.Name, initialProperty.StringValueTrimmed);
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
