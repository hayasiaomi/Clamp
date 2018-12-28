using Chromium;
using Chromium.WebBrowser;
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
        private HttpStatusCode httpStatusCode;

        internal ClampResourceHandler(ChromiumWebBrowser browser, string schemeName)
        {
            this.gcHandle = GCHandle.Alloc(this);
            this.browser = browser;
            this.schemeName = schemeName;
            this.GetResponseHeaders += ClampResourceHandler_GetResponseHeaders;
            this.ProcessRequest += ClampResourceHandler_ProcessRequest;
            this.ReadResponse += ClampResourceHandler_ReadResponse;
            this.CanGetCookie += (s, e) => e.SetReturnValue(false);
            this.CanSetCookie += (s, e) => e.SetReturnValue(false);
        }


        private void ClampResourceHandler_ProcessRequest(object sender, Chromium.Event.CfxProcessRequestEventArgs e)
        {
            this.readResponseStreamOffset = 0;

            CfxRequest request = e.Request;
            CfxCallback callback = e.Callback;

            Uri uri;

            if (Uri.TryCreate(request.Url, UriKind.RelativeOrAbsolute, out uri))
            {
                this.requestUrl = request.Url;

                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create($"{this.schemeName}://127.0.0.1:31234{uri.PathAndQuery}");

                httpWebRequest.Method = request.Method;

                List<string[]> headerMaps = request.GetHeaderMap();

                if (headerMaps != null && headerMaps.Count > 0)
                {
                    foreach (string[] headers in headerMaps)
                    {
                        if (headers.Length == 1)
                        {
                            httpWebRequest.Headers.Add(headers[0]);
                        }
                        else if (headers.Length > 1)
                        {
                            string headerName = headers[0];
                            string headerValue = headers[1];

                            if (String.Compare(headerName, "Accept", true) == 0)
                            {
                                httpWebRequest.Accept = headerValue;
                            }
                            else if (String.Compare(headerName, "User-Agent", true) == 0 || String.Compare(headerName, "UserAgent", true) == 0)
                            {
                                httpWebRequest.UserAgent = headerValue;
                            }
                            else
                            {
                                httpWebRequest.Headers.Add(headers[0], headers[1]);
                            }
                        }
                    }
                }

                try
                {
                    CfxPostData cfxPostData = request.PostData;

                    if (cfxPostData != null && cfxPostData.ElementCount > 0)
                    {
                        using (Stream rStream = httpWebRequest.GetRequestStream())
                        {
                            foreach (var item in cfxPostData.Elements)
                            {
                                var size = item.GetBytes(item.BytesCount, item.NativePtr);
                                byte[] buffer = new byte[item.BytesCount];
                                Marshal.Copy(item.NativePtr, buffer, 0, (int)item.BytesCount);
                                rStream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var stream = httpWebRequest.GetResponse().GetResponseStream())
                    {
                        if (stream != null)
                        {
                            string responseText;

                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                responseText = reader.ReadToEnd();
                            }

                            byte[] buffers = Encoding.UTF8.GetBytes(responseText);

                            webResource = new WebResource(buffers.ToArray(), httpWebResponse.ContentType);

                            if (!browser.webResources.ContainsKey(requestUrl))
                            {
                                browser.SetWebResource(requestUrl, webResource);
                            }
                        }
                    }

                    this.httpStatusCode = httpWebResponse.StatusCode;

                    Console.WriteLine($"[加载]:\t{requestUrl}");

                    callback.Continue();
                    e.SetReturnValue(true);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[加载发生异常]:\t{requestUrl}，原因:{ex.Message}");

                    callback.Continue();
                    e.SetReturnValue(false);
                }
            }



        }

        private void ClampResourceHandler_GetResponseHeaders(object sender, Chromium.Event.CfxGetResponseHeadersEventArgs e)
        {
            if (this.httpStatusCode == HttpStatusCode.OK)
            {
                e.ResponseLength = webResource.data.Length;
                e.Response.MimeType = webResource.mimeType;
                e.Response.Status = 200;

                if (!browser.webResources.ContainsKey(requestUrl))
                {
                    browser.SetWebResource(requestUrl, webResource);
                }
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
