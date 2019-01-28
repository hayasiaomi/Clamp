using Chromium;
using Chromium.WebBrowser;
using Clamp.AppCenter.MVC;
using Clamp.Linker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.AppCenter.CFX
{
    public class EmbeddedResourceHandler : CfxResourceHandler
    {
        private int readResponseStreamOffset;
        private string requestUrl = null;
        private WebResource webResource;
        private ChromiumWebBrowser browser;
        private GCHandle gcHandle;
        private Assembly assembly;
        private string domainName;

        public EmbeddedResourceHandler(ChromiumWebBrowser browser, string domainName)
        {
            this.domainName = domainName;
            this.gcHandle = GCHandle.Alloc(this);
            this.browser = browser;
            this.GetResponseHeaders += EmbeddedResourceHandler_GetResponseHeaders;
            this.ProcessRequest += EmbeddedResourceHandler_ProcessRequest;
            this.ReadResponse += EmbeddedResourceHandler_ReadResponse;
            this.CanGetCookie += (s, e) => e.SetReturnValue(false);
            this.CanSetCookie += (s, e) => e.SetReturnValue(false);
        }


        private void EmbeddedResourceHandler_ProcessRequest(object sender, Chromium.Event.CfxProcessRequestEventArgs e)
        {
            readResponseStreamOffset = 0;

            CfxRequest request = e.Request;
            CfxCallback callback = e.Callback;

            using (LinkerContext context = HTMLAnalyzer.Analyze(request))
            {
                if (context.Response != null)
                {
                    using (MemoryStream reader = new MemoryStream())
                    {
                        context.Response.Contents.Invoke(reader);

                        if (reader.Length > 0)
                        {
                            webResource = new WebResource(reader.ToArray(), context.Response.ContentType);
                        }
                    }
                }
            }

            callback.Continue();
            e.SetReturnValue(true);
        }

        private void EmbeddedResourceHandler_GetResponseHeaders(object sender, Chromium.Event.CfxGetResponseHeadersEventArgs e)
        {
            if (webResource == null)
            {
                e.Response.Status = 404;
                e.Response.StatusText = "Not Found";
            }
            else
            {
                e.ResponseLength = webResource.data.Length;
                e.Response.MimeType = webResource.mimeType;
                e.Response.Status = 200;
                e.Response.StatusText = "OK";
            }
        }


        private void EmbeddedResourceHandler_ReadResponse(object sender, Chromium.Event.CfxReadResponseEventArgs e)
        {
            int bytesToCopy = webResource.data.Length - readResponseStreamOffset;

            if (bytesToCopy > e.BytesToRead)
                bytesToCopy = e.BytesToRead;

            System.Runtime.InteropServices.Marshal.Copy(webResource.data, readResponseStreamOffset, e.DataOut, bytesToCopy);
            e.BytesRead = bytesToCopy;
            readResponseStreamOffset += bytesToCopy;

            e.SetReturnValue(true);

            if (readResponseStreamOffset == webResource.data.Length)
            {
                gcHandle.Free();

                Console.WriteLine($"[完成]:\t{requestUrl}");
            }
        }
    }
}
