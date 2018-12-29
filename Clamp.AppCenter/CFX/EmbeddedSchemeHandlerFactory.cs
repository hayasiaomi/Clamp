using Chromium;
using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.AppCenter.CFX
{
    public class EmbeddedSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public string SchemeName { private set; get; }

        private readonly Assembly mainAssembly;

        public EmbeddedSchemeHandlerFactory(string schemeName, Assembly mainAssembly)
        {
            this.mainAssembly = mainAssembly;
            this.SchemeName = schemeName;
            this.Create += EmbeddedSchemeHandlerFactory_Create;
        }

        private void EmbeddedSchemeHandlerFactory_Create(object sender, Chromium.Event.CfxSchemeHandlerFactoryCreateEventArgs e)
        {
            if (e.SchemeName == SchemeName && e.Browser != null)
            {
                ChromiumWebBrowser browser = ChromiumWebBrowser.GetBrowser(e.Browser.Identifier);
                EmbeddedResourceHandler handler = new EmbeddedResourceHandler(mainAssembly, browser);

                e.SetReturnValue(handler);
            }

        }
    }
}
