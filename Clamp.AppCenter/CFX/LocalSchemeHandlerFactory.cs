using Chromium;
using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.CFX
{
    public class LocalSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public string SchemeName { private set; get; }

        public string DomainName { private set; get; }

        public LocalSchemeHandlerFactory(string schemeName,string domainName)
        {
            this.SchemeName = schemeName;
            this.DomainName = domainName;
            this.Create += LocalSchemeHandlerFactory_Create;
        }

        private void LocalSchemeHandlerFactory_Create(object sender, Chromium.Event.CfxSchemeHandlerFactoryCreateEventArgs e)
        {
            Uri uri = new Uri(e.Request.Url);

            if (e.SchemeName.Equals(this.SchemeName, StringComparison.CurrentCultureIgnoreCase) && uri.Host.Equals(this.DomainName, StringComparison.CurrentCultureIgnoreCase) && e.Browser != null)
            {
                ChromiumWebBrowser browser = ChromiumWebBrowser.GetBrowser(e.Browser.Identifier);
                LocalResourceHandler handler = new LocalResourceHandler(browser);
                e.SetReturnValue(handler);
            }
        }
    }
}
