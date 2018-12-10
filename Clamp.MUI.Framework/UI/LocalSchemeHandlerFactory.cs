﻿using Chromium;
using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.UI
{
    internal class LocalSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        internal LocalSchemeHandlerFactory()
        {
            this.Create += LocalSchemeHandlerFactory_Create;
        }

        private void LocalSchemeHandlerFactory_Create(object sender, Chromium.Event.CfxSchemeHandlerFactoryCreateEventArgs e)
        {
            if (e.SchemeName.Equals("local") && e.Browser != null)
            {
                ChromiumWebBrowser browser = ChromiumWebBrowser.GetBrowser(e.Browser.Identifier);
                LocalResourceHandler handler = new LocalResourceHandler(browser);
                e.SetReturnValue(handler);
            }
        }
    }
}