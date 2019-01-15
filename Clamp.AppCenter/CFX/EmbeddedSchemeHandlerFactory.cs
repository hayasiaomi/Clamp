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

        private string[] extAssemblies;

        public string DomainName { private set; get; }

        public EmbeddedSchemeHandlerFactory(string schemeName, string domainName)
        {
            this.DomainName = domainName;
            this.SchemeName = schemeName;
            this.Create += EmbeddedSchemeHandlerFactory_Create;
        }

        private void EmbeddedSchemeHandlerFactory_Create(object sender, Chromium.Event.CfxSchemeHandlerFactoryCreateEventArgs e)
        {
            Uri uri = new Uri(e.Request.Url);

            if (e.SchemeName.Equals(this.SchemeName, StringComparison.CurrentCultureIgnoreCase) && uri.Host.Equals(this.DomainName, StringComparison.CurrentCultureIgnoreCase) && e.Browser != null)
            {
                ChromiumWebBrowser browser = ChromiumWebBrowser.GetBrowser(e.Browser.Identifier);
                EmbeddedResourceHandler handler = new EmbeddedResourceHandler(browser, this.DomainName);

                e.SetReturnValue(handler);
            }

        }
    }
}
