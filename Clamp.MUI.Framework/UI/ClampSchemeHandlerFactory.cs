using Chromium;
using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.UI
{
    internal class ClampSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public string SchemeName { private set; get; }
        internal ClampSchemeHandlerFactory(string schemeName)
        {
            this.SchemeName = schemeName;
            this.Create += ClampSchemeHandlerFactory_Create;
        }

        private void ClampSchemeHandlerFactory_Create(object sender, Chromium.Event.CfxSchemeHandlerFactoryCreateEventArgs e)
        {
            if (e.SchemeName.Equals(this.SchemeName) && this.GetHost(e.Request.Url).EndsWith("SD-proxy.chidaoni.com", StringComparison.CurrentCultureIgnoreCase) && e.Browser != null)
            {
                ChromiumWebBrowser browser = ChromiumWebBrowser.GetBrowser(e.Browser.Identifier);
                ClampResourceHandler handler = new ClampResourceHandler(browser, this.SchemeName);
                e.SetReturnValue(handler);
            }
        }

        private string GetHost(string url)
        {
            Uri callUri;

            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out callUri))
            {
                return callUri.DnsSafeHost;
            }

            return "no uri";
        }
    }
}
