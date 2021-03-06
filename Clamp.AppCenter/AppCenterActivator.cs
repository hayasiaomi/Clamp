﻿using Clamp.AppCenter.MVC;
using Clamp;
using Clamp.Linker.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Clamp.Cfg;
using Clamp.AppCenter.CFX;
using Chromium.WebBrowser.Event;

namespace Clamp.AppCenter
{
    public class AppCenterActivator : IBundleActivator
    {
        internal static IBundleContext BundleContext;

        public void Start(IBundleContext context)
        {
            BundleContext = context;


            Dictionary<string, string> appCenterConfigMaps = this.GetClampAppCenterConfiguration();

            string cfxResHandeEmbedded = appCenterConfigMaps[AppCenterConstant.CFX_RESOURCE_HANDLER_EMBEDDED];

            string url = cfxResHandeEmbedded.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase)
                || cfxResHandeEmbedded.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
                ? cfxResHandeEmbedded : $"http://{cfxResHandeEmbedded}";

            HTMLAnalyzer.Initialize(new ClampLinkerBootstrapper(), new Uri(url));

            context.Register(typeof(Dictionary<string, string>), appCenterConfigMaps, AppCenterConstant.CFG_APPCENTER);

        }

        public void Stop(IBundleContext context)
        {
            HTMLAnalyzer.Shutdown();
          
        }
     

        private Dictionary<string, string> GetClampAppCenterConfiguration()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            string clampConfFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppCenterConstant.APPCENTER_CONFIG_FILE);

            if (File.Exists(clampConfFile))
            {
                ExtendedProperties initialProperties = new ExtendedProperties(clampConfFile);

                if (initialProperties.Count > 0)
                {
                    foreach (string keyName in initialProperties.Keys)
                    {
                        keyValuePairs.Add(keyName, initialProperties.GetString(keyName));
                    }
                }
            }

            return keyValuePairs;
        }
    }
}
