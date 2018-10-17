using ShanDian.SDK.Framework;
using ShanDian.SDK.Listeners;
using ShanDian.Webwork;
using ShanDian.Webwork.Extensions;
using ShanDian.Webwork.Helpers;
using ShanDian.Webwork.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace ShanDian.SDK.Listeners
{
    class SDHttpListener : ISDListener<Request, WebworkContext>
    {
        private HttpListener listener;

        public MediumConfiguration MediumConfiguration { private set; get; }

        public bool AllowAuthorityFallback { get; set; }
        public bool RewriteLocalhost { get; set; }

        public bool EnableClientCertificates { get; set; }

        public bool AllowChunkedEncoding { get; set; }

        public Uri[] BaseUriList { private set; get; }

        public bool IsListening { get { return this.listener.IsListening; } }

        public Func<Request, WebworkContext> HandleMessage { set; get; }

        public SDHttpListener(MediumConfiguration mediumConfiguration)
        {
            this.RewriteLocalhost = true;
            this.AllowChunkedEncoding = true;
            this.EnableClientCertificates = false;
            this.MediumConfiguration = mediumConfiguration;
        }

        public void StartListen()
        {
            try
            {
                this.BaseUriList = this.MediumConfiguration.GetUris();

                this.listener = new HttpListener();

                foreach (var prefix in this.GetPrefixes())
                {
                    this.listener.Prefixes.Add(prefix);

                    SD.Log.Info($"监听端口:{prefix}");
                }

                this.listener.Start();

                this.listener.BeginGetContext(this.GotCallback, null);
            }
            catch (HttpListenerException ex)
            {
                SD.Log.Error("HTTP监听失败", ex);
            }
        }

        public void Shutdown()
        {

        }

        internal IEnumerable<string> GetPrefixes()
        {
            foreach (Uri baseUri in this.BaseUriList)
            {
                var prefix = new UriBuilder(baseUri).ToString();

                if (this.RewriteLocalhost && !baseUri.Host.Contains("."))
                {
                    prefix = prefix.Replace("localhost", "+");
                }

                yield return prefix;
            }
        }

        private Request ConvertRequestToWebworkRequest(HttpListenerRequest request)
        {
            var baseUri = this.GetBaseUri(request);

            if (baseUri == null)
            {
                throw new InvalidOperationException(String.Format("Unable to locate base URI for request: {0}", request.Url));
            }

            var expectedRequestLength = GetExpectedRequestLength(request.Headers.ToDictionary());

            var relativeUrl = baseUri.MakeAppLocalPath(request.Url);

            relativeUrl = this.CheckOldRouting(relativeUrl);

            var webworkUrl = new Url
            {
                Scheme = request.Url.Scheme,
                HostName = request.Url.Host,
                Port = request.Url.IsDefaultPort ? null : (int?)request.Url.Port,
                BasePath = baseUri.AbsolutePath.TrimEnd('/'),
                Path = HttpUtility.UrlDecode(relativeUrl),
                Query = request.Url.Query,
            };

            byte[] certificate = null;

            if (this.EnableClientCertificates)
            {
                var x509Certificate = request.GetClientCertificate();

                if (x509Certificate != null)
                {
                    certificate = x509Certificate.RawData;
                }
            }

            var fieldCount = request.ProtocolVersion.Major == 2 ? 1 : 2;

            var protocolVersion = string.Format("HTTP/{0}", request.ProtocolVersion.ToString(fieldCount));

            return new Request(
                request.HttpMethod,
                webworkUrl,
                RequestStream.FromStream(request.InputStream, expectedRequestLength, StaticConfiguration.DisableRequestStreamSwitching ?? false),
                request.Headers.ToDictionary(),
                (request.RemoteEndPoint != null) ? request.RemoteEndPoint.Address.ToString() : null,
                certificate,
                protocolVersion);
        }

        private string CheckOldRouting(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string[] segments = url.Split('/');

                if (segments != null && segments.Length >= 3)
                {
                    List<string> nSegments = new List<string>();

                    for (int i = 0; i < segments.Length; i++)
                    {
                        if (string.Equals(segments[i], "api", StringComparison.CurrentCultureIgnoreCase) && string.Equals(segments[i + 1], "1.0.0.0", StringComparison.CurrentCultureIgnoreCase))
                        {
                            i = i + 1;
                            continue;
                        }
                        nSegments.Add(segments[i]);
                    }

                    return string.Join("/", nSegments);
                }
            }

            return url;
        }

        private void ConvertWebworkResponseToResponse(Response webworkResponse, HttpListenerResponse response)
        {
            foreach (var header in webworkResponse.Headers)
            {
                if (!IgnoredHeaders.IsIgnored(header.Key))
                {
                    response.AddHeader(header.Key, header.Value);
                }
            }

            foreach (var cookie in webworkResponse.Cookies)
            {
                response.Headers.Add(HttpResponseHeader.SetCookie, cookie.ToString());
            }

            if (webworkResponse.ReasonPhrase != null)
            {
                response.StatusDescription = webworkResponse.ReasonPhrase;
            }

            if (webworkResponse.ContentType != null)
            {
                response.ContentType = webworkResponse.ContentType;
            }

            response.StatusCode = (int)webworkResponse.StatusCode;

            if (this.AllowChunkedEncoding)
            {
                OutputWithDefaultTransferEncoding(webworkResponse, response);
            }
            else
            {
                OutputWithContentLength(webworkResponse, response);
            }
        }


        private Uri GetBaseUri(HttpListenerRequest request)
        {
            var result = this.BaseUriList.FirstOrDefault(uri => uri.IsCaseInsensitiveBaseOf(request.Url));

            if (result != null)
            {
                return result;
            }

            if (!this.AllowAuthorityFallback)
            {
                return null;
            }

            return new Uri(request.Url.GetLeftPart(UriPartial.Authority));
        }

        private static void OutputWithDefaultTransferEncoding(Response webworkResponse, HttpListenerResponse response)
        {
            using (var output = response.OutputStream)
            {
                webworkResponse.Contents.Invoke(output);
            }
        }

        private static void OutputWithContentLength(Response webworkResponse, HttpListenerResponse response)
        {
            byte[] buffer;
            using (var memoryStream = new MemoryStream())
            {
                webworkResponse.Contents.Invoke(memoryStream);
                buffer = memoryStream.ToArray();
            }

            var contentLength = (webworkResponse.Headers.ContainsKey("Content-Length")) ?
                Convert.ToInt64(webworkResponse.Headers["Content-Length"]) :
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

        private void GotCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext ctx = this.listener.EndGetContext(ar);

                this.listener.BeginGetContext(this.GotCallback, null);

                var webworkRequest = this.ConvertRequestToWebworkRequest(ctx.Request);

                using (var webworkContext = this.HandleMessage(webworkRequest))
                {
                    try
                    {
                        this.ConvertWebworkResponseToResponse(webworkContext.Response, ctx.Response);
                    }
                    catch (Exception e)
                    {
                        SD.Log.Error("通信框架的出错",e);
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    this.listener.BeginGetContext(this.GotCallback, null);
                }
                catch
                {

                }

                SD.Log.Error("通信框架的出错", e);
            }
        }


    }
}
