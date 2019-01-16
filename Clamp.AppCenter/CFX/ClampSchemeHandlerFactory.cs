using Chromium;
using Chromium.WebBrowser;
using Clamp.AppCenter.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Clamp.AppCenter.CFX
{
    public class ClampSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public string SchemeName { private set; get; }

        public string DomainName { private set; get; }

        public ClampSchemeHandlerFactory(string schemeName, string domainName)
        {
            this.SchemeName = schemeName;
            this.DomainName = domainName;
            this.Create += ClampSchemeHandlerFactory_Create;
        }

        private void ClampSchemeHandlerFactory_Create(object sender, Chromium.Event.CfxSchemeHandlerFactoryCreateEventArgs e)
        {
            Uri uri = new Uri(e.Request.Url);

            if (e.SchemeName.Equals(this.SchemeName, StringComparison.CurrentCultureIgnoreCase) && uri.Host.Equals(this.DomainName, StringComparison.CurrentCultureIgnoreCase))
            {
                ChromiumWebBrowser browser = ChromiumWebBrowser.GetBrowser(e.Browser.Identifier);

                IClampHandlerFactory clampHandlerFactory = null;

                Control control = browser.Parent;

                while (control != null && control.Parent != null)
                {
                    if (control is IClampHandlerFactory)
                        break;

                    control = control.Parent;
                }

                if (control != null)
                    clampHandlerFactory = control as IClampHandlerFactory;

                if (clampHandlerFactory != null)
                {
                    IClampHandler clampHandler = clampHandlerFactory.GetClampHandler();

                    if (clampHandler != null)
                    {
                        List<string> datas = new List<string>();

                        if (e.Request.PostData != null)
                        {
                            CfxPostDataElement[] cfxPostDataElements = e.Request.PostData.Elements;

                            foreach (CfxPostDataElement cfxPostDataElement in cfxPostDataElements)
                            {
                                if (cfxPostDataElement.Type == CfxPostdataElementType.Bytes)
                                {
                                    byte[] buffer = new byte[cfxPostDataElement.BytesCount];

                                    GCHandle bufferGCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                                    IntPtr bufferIntPtr = bufferGCHandle.AddrOfPinnedObject();

                                    var size = cfxPostDataElement.GetBytes(cfxPostDataElement.BytesCount, bufferIntPtr);

                                    string data = Encoding.UTF8.GetString(buffer);

                                    bufferGCHandle.Free();


                                    if (!string.IsNullOrWhiteSpace(data))
                                    {
                                        datas.Add(data);
                                    }
                                }
                            }
                        }

                        e.SetReturnValue(new ClampResourceHandler(browser, this.SchemeName, clampHandler, datas));
                    }
                }
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
