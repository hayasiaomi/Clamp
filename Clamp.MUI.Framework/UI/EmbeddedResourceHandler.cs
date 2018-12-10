﻿using Chromium;
using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.MUI.Framework.UI
{
    internal class EmbeddedResourceHandler : CfxResourceHandler
    {
        private int readResponseStreamOffset;
        private Assembly resourceAssembly;
        private string requestUrl = null;
        private WebResource webResource;
        private ChromiumWebBrowser browser;
        private GCHandle gcHandle;

        private string domain = null;

        internal EmbeddedResourceHandler(Assembly resourceAssembly, ChromiumWebBrowser browser, string domain = null)
        {
            this.gcHandle = GCHandle.Alloc(this);
            this.domain = domain;
            this.browser = browser;
            this.resourceAssembly = resourceAssembly;
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

            Uri uri = new Uri(request.Url);

            this.requestUrl = request.Url;

            var fileName = uri.AbsolutePath;

            if (fileName.StartsWith("/") && fileName.Length > 1)
            {
                fileName = fileName.Substring(1);
            }

            var ass = resourceAssembly;
            var endTrimIndex = fileName.LastIndexOf('/');

            if (endTrimIndex > -1)
            {
                var tmp = fileName.Substring(0, endTrimIndex);
                tmp = tmp.Replace("-", "_");

                fileName = string.Format("{0}{1}", tmp, fileName.Substring(endTrimIndex));
            }

            var resourcePath = string.Format("{0}.{1}", uri.Host, fileName.Replace('/', '.'));

            var resourceName = ass.GetManifestResourceNames().SingleOrDefault(p => p.Equals(resourcePath, StringComparison.CurrentCultureIgnoreCase));

            if (!string.IsNullOrEmpty(resourceName) && ass.GetManifestResourceInfo(resourceName) != null)
            {
                using (var reader = new System.IO.BinaryReader(ass.GetManifestResourceStream(resourceName)))
                {
                    var buff = reader.ReadBytes((int)reader.BaseStream.Length);

                    webResource = new WebResource(buff, MimeHelper.GetMimeType(Path.GetExtension(fileName)));

                    reader.Close();

                    if (!browser.webResources.ContainsKey(requestUrl))
                    {
                        browser.SetWebResource(requestUrl, webResource);
                    }
                }

                Console.WriteLine($"[加载]:\t{requestUrl}");

                callback.Continue();
                e.SetReturnValue(true);
            }
            else
            {
                Console.WriteLine($"[未找到]:\t{requestUrl}");

                callback.Continue();
                e.SetReturnValue(false);
            }
        }

        private void EmbeddedResourceHandler_GetResponseHeaders(object sender, Chromium.Event.CfxGetResponseHeadersEventArgs e)
        {

            if (webResource == null)
            {
                e.Response.Status = 404;
            }
            else
            {
                e.ResponseLength = webResource.data.Length;
                e.Response.MimeType = webResource.mimeType;
                e.Response.Status = 200;

                if (!browser.webResources.ContainsKey(requestUrl))
                {
                    browser.SetWebResource(requestUrl, webResource);
                }
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
