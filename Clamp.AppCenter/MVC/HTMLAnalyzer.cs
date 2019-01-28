using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Linq;
using System.Security.Principal;
using Clamp.Linker;
using Clamp.Linker.Bootstrapper;
using Clamp.Linker.Helpers;
using Clamp.Linker.IO;
using Clamp.Linker.Extensions;
using Chromium;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.AppCenter
{
    public class HTMLAnalyzer
    {
        private const int ACCESS_DENIED = 5;
        private static IList<Uri> baseUriList;
        private static ILinkerEngine clampWebEngine;
        private static HostConfiguration hostConfiguration;
        private static ILinkerBootstrapper clampWebBootstrapper;

        public static void Initialize(params Uri[] baseUris)
        {
            Initialize(LinkerBootstrapperLocator.Bootstrapper, new HostConfiguration(), baseUris);
        }

        public static void Initialize(HostConfiguration configuration, params Uri[] baseUris)
        {
            Initialize(LinkerBootstrapperLocator.Bootstrapper, configuration, baseUris);
        }

        public static void Initialize(ILinkerBootstrapper bootstrapper, params Uri[] baseUris)
        {
            Initialize(bootstrapper, new HostConfiguration(), baseUris);
        }

       
        public static void Initialize(ILinkerBootstrapper bootstrapper, HostConfiguration configuration, params Uri[] baseUris)
        {
            clampWebBootstrapper = bootstrapper;
            hostConfiguration = configuration ?? new HostConfiguration();
            baseUriList = baseUris;

            bootstrapper.Initialise();

            clampWebEngine = bootstrapper.GetEngine();
        }


        public static void Initialize(Uri baseUri, ILinkerBootstrapper bootstrapper)
        {
            Initialize(bootstrapper, new HostConfiguration(), baseUri);
        }

        public static void Initialize(Uri baseUri, ILinkerBootstrapper bootstrapper, HostConfiguration configuration)
        {
            Initialize(bootstrapper, configuration, baseUri);
        }

        public static void Shutdown()
        {
            clampWebBootstrapper.Dispose();
        }

        private static bool TryAddUrlReservations()
        {
            var user = GetUser();

            foreach (var prefix in GetPrefixes())
            {
                if (!NetSh.AddUrlAcl(prefix, user))
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetUser()
        {
            if (!string.IsNullOrWhiteSpace(hostConfiguration.UrlReservations.User))
            {
                return hostConfiguration.UrlReservations.User;
            }

            return WindowsIdentity.GetCurrent().Name;
        }


        internal static IEnumerable<string> GetPrefixes()
        {
            foreach (var baseUri in baseUriList)
            {
                var prefix = new UriBuilder(baseUri).ToString();

                if (hostConfiguration.RewriteLocalhost && !baseUri.Host.Contains("."))
                {
                    prefix = prefix.Replace("localhost", "+");
                }

                yield return prefix;
            }
        }

        private static Uri GetBaseUri(Uri requestUrl)
        {
            var result = baseUriList.FirstOrDefault(uri => uri.IsCaseInsensitiveBaseOf(requestUrl));

            if (result != null)
            {
                return result;
            }

            if (!hostConfiguration.AllowAuthorityFallback)
            {
                return null;
            }

            return new Uri(requestUrl.GetLeftPart(UriPartial.Authority));
        }

        private static void ConvertNancyResponseToResponse(Response nancyResponse, HttpListenerResponse response)
        {
            foreach (var header in nancyResponse.Headers)
            {
                if (!IgnoredHeaders.IsIgnored(header.Key))
                {
                    response.AddHeader(header.Key, header.Value);
                }
            }

            foreach (var nancyCookie in nancyResponse.Cookies)
            {
                response.Headers.Add(HttpResponseHeader.SetCookie, nancyCookie.ToString());
            }

            if (nancyResponse.ReasonPhrase != null)
            {
                response.StatusDescription = nancyResponse.ReasonPhrase;
            }

            if (nancyResponse.ContentType != null)
            {
                response.ContentType = nancyResponse.ContentType;
            }

            response.StatusCode = (int)nancyResponse.StatusCode;

            if (hostConfiguration.AllowChunkedEncoding)
            {
                OutputWithDefaultTransferEncoding(nancyResponse, response);
            }
            else
            {
                OutputWithContentLength(nancyResponse, response);
            }
        }

        private static void OutputWithDefaultTransferEncoding(Response nancyResponse, HttpListenerResponse response)
        {
            using (var output = response.OutputStream)
            {
                nancyResponse.Contents.Invoke(output);
            }
        }

        private static void OutputWithContentLength(Response nancyResponse, HttpListenerResponse response)
        {
            byte[] buffer;
            using (var memoryStream = new MemoryStream())
            {
                nancyResponse.Contents.Invoke(memoryStream);
                buffer = memoryStream.ToArray();
            }

            var contentLength = (nancyResponse.Headers.ContainsKey("Content-Length")) ?
                Convert.ToInt64(nancyResponse.Headers["Content-Length"]) :
                buffer.Length;

            response.SendChunked = false;
            response.ContentLength64 = contentLength;

            using (var output = response.OutputStream)
            {
                using (var writer = new BinaryWriter(output))
                {
                    writer.Write(buffer);
                    writer.Flush();
                }
            }
        }

        private static long GetExpectedRequestLength(IDictionary<string, IEnumerable<string>> incomingHeaders)
        {
            if (incomingHeaders == null)
            {
                return 0;
            }

            if (!incomingHeaders.ContainsKey("Content-Length"))
            {
                return 0;
            }

            var headerValue = incomingHeaders["Content-Length"].SingleOrDefault();

            if (headerValue == null)
            {
                return 0;
            }

            long contentLength;

            return !long.TryParse(headerValue, NumberStyles.Any, CultureInfo.InvariantCulture, out contentLength) ? 0 : contentLength;
        }

        private static Dictionary<string, IEnumerable<string>> HeaderToDictionary(List<string[]> headerMaps)
        {
            Dictionary<string, IEnumerable<string>> incomingHeaders = new Dictionary<string, IEnumerable<string>>();

            foreach (string[] headerMap in headerMaps)
            {
                List<string> headers = new List<string>();

                if (string.Equals("Accept-Language", headerMap[0], StringComparison.CurrentCultureIgnoreCase))
                {
                    string[] values = headerMap[1].Split(',');

                    if (values != null && values.Length > 0)
                    {
                        foreach (string value in values)
                        {
                            headers.Add(value);
                        }
                    }
                    else
                    {
                        headers.Add(" ");
                    }
                }

                headers.Add(headerMap[1]);

                incomingHeaders.Add(headerMap[0], headers);
            }

            return incomingHeaders;
        }

        private static Stream PostDataToStream(CfxPostData postData, Dictionary<string, IEnumerable<string>> headers)
        {
            if (postData != null)
            {
                string headerValue = headers["Content-Type"].First();
                string contentType = headerValue.Split(';').First();

                if (string.Equals("application/x-www-form-urlencoded", contentType, StringComparison.CurrentCultureIgnoreCase))
                {
                    CfxPostDataElement[] cfxPostDataElements = postData.Elements;

                    foreach (CfxPostDataElement cfxPostDataElement in cfxPostDataElements)
                    {
                        if (cfxPostDataElement.Type == CfxPostdataElementType.Bytes)
                        {
                            byte[] buffer = new byte[cfxPostDataElement.BytesCount];

                            GCHandle bufferGCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                            IntPtr bufferIntPtr = bufferGCHandle.AddrOfPinnedObject();

                            var size = cfxPostDataElement.GetBytes(cfxPostDataElement.BytesCount, bufferIntPtr);

                            bufferGCHandle.Free();

                            if (buffer != null)
                            {
                                return new MemoryStream(buffer);
                            }
                        }
                        else if (cfxPostDataElement.Type == CfxPostdataElementType.File)
                        {
                            //TODO 上传文件处理
                        }
                    }
                }
                else if (string.Equals("multipart/form-data", contentType, StringComparison.CurrentCultureIgnoreCase))
                {
                    List<string> datas = new List<string>();

                    CfxPostDataElement[] cfxPostDataElements = postData.Elements;

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
                        else if (cfxPostDataElement.Type == CfxPostdataElementType.File)
                        {
                            //TODO 上传文件处理
                        }
                    }
                }
            }

            return Stream.Null;
        }

        public static LinkerContext Analyze(CfxRequest cfxRequest)
        {
            Uri requestUrl = new Uri(cfxRequest.Url);

            var baseUri = GetBaseUri(requestUrl);

            if (baseUri == null)
            {
                throw new InvalidOperationException(String.Format("Unable to locate base URI for request: {0}", cfxRequest.Url));
            }

            Dictionary<string, IEnumerable<string>> headers = HeaderToDictionary(cfxRequest.GetHeaderMap());

            Stream body = PostDataToStream(cfxRequest.PostData, headers);

            var expectedRequestLength = GetExpectedRequestLength(headers);

            var relativeUrl = baseUri.MakeAppLocalPath(requestUrl);

            Url url = new Url
            {
                Scheme = requestUrl.Scheme,
                HostName = requestUrl.Host,
                Port = requestUrl.IsDefaultPort ? null : (int?)requestUrl.Port,
                BasePath = baseUri.AbsolutePath.TrimEnd('/'),
                Path = HttpUtility.UrlDecode(relativeUrl),
                Query = requestUrl.Query,
            };

            return clampWebEngine.HandleRequest(new Request(cfxRequest.Method, url, RequestStream.FromStream(body, body.Length, false),  headers, "::1", null, "HTTP/1.1"));
        }
    }
}
