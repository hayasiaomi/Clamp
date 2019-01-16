using Chromium;
using Chromium.WebBrowser;
using Clamp.AppCenter.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.AppCenter.CFX
{
    public class ClampResourceHandler : CfxResourceHandler
    {
        private int readResponseStreamOffset;
        private string requestUrl = null;
        private WebResource webResource;
        private ChromiumWebBrowser browser;
        private GCHandle gcHandle;
        private string schemeName;
        private IClampHandler clampHandler;
        private HttpStatusCode httpStatusCode;
        private List<string> datas;



        internal ClampResourceHandler(ChromiumWebBrowser browser, string schemeName, IClampHandler clampHandler, List<string> datas)
        {
            this.gcHandle = GCHandle.Alloc(this);
            this.datas = datas;
            this.browser = browser;
            this.schemeName = schemeName;
            this.clampHandler = clampHandler;
            this.GetResponseHeaders += ClampResourceHandler_GetResponseHeaders;
            this.ProcessRequest += ClampResourceHandler_ProcessRequest;
            this.ReadResponse += ClampResourceHandler_ReadResponse;
            this.CanGetCookie += (s, e) => e.SetReturnValue(false);
            this.CanSetCookie += (s, e) => e.SetReturnValue(false);
        }


        private void ClampResourceHandler_ProcessRequest(object sender, Chromium.Event.CfxProcessRequestEventArgs e)
        {
            this.readResponseStreamOffset = 0;
            this.requestUrl = e.Request.Url;

            if (this.clampHandler != null)
            {
                object result = this.clampHandler.Handle(new ClampHandlerContext() { ChromiumWebBrowser = this.browser, Url = this.requestUrl, Datas = this.datas });

                string rText = Convert.ToString(result);

                this.webResource = new WebResource(Encoding.UTF8.GetBytes(rText), MimeHelper.GetMimeType(".txt"));

                this.httpStatusCode = HttpStatusCode.OK;
            }
            else
            {
                this.httpStatusCode = HttpStatusCode.NotFound;
            }

            e.Callback.Continue();
            e.SetReturnValue(true);
        }

        private void ClampResourceHandler_GetResponseHeaders(object sender, Chromium.Event.CfxGetResponseHeadersEventArgs e)
        {
            if (this.httpStatusCode == HttpStatusCode.OK)
            {
                e.ResponseLength = webResource.data.Length;
                e.Response.MimeType = webResource.mimeType;
                e.Response.Status = 200;
            }
            else
            {
                e.Response.Status = (int)this.httpStatusCode;
            }
        }


        private void ClampResourceHandler_ReadResponse(object sender, Chromium.Event.CfxReadResponseEventArgs e)
        {
            int bytesToCopy = webResource.data.Length - readResponseStreamOffset;

            if (bytesToCopy > e.BytesToRead)
                bytesToCopy = e.BytesToRead;

            Marshal.Copy(webResource.data, readResponseStreamOffset, e.DataOut, bytesToCopy);

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
