﻿using Clamp.Cfg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp
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

            clampBundle.Initialize(asm, configProps[ClampConstants.CLAMP_BUNDLES_DIR], null);

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

        /// <summary>
        /// 加载默认的配置信息
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> LoadDefaultConfigProperties()
        {
            Dictionary<string, string> defaultConfigProps = new Dictionary<string, string>();

            Assembly asm = typeof(ClampBundleFactory).Assembly;

            string resourceName = asm.GetManifestResourceNames().FirstOrDefault(res => res.EndsWith(ClampConstants.CLAMP_CONFIG_FILE, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                ExtendedProperties extendedProperties = new ExtendedProperties();

                extendedProperties.Load(asm.GetManifestResourceStream(resourceName));

                if (extendedProperties.Count > 0)
                {
                    foreach (string keyName in extendedProperties.Keys)
                    {
                        if (extendedProperties.ContainsKey(keyName))
                        {
                            defaultConfigProps[keyName] = extendedProperties.GetString(keyName);
                        }
                        else
                        {
                            defaultConfigProps.Add(keyName, extendedProperties.GetString(keyName));
                        }
                    }
                }
            }

            return defaultConfigProps;
        }
    }
}
