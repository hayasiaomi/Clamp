using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ShanDian.UIShell.Framework.Network.Internal
{
    internal class HttpRequest
    {
        private HttpWebRequest httpWebRequest;
        private CookieContainer cookieContainer;
        private HttpRequestCachePolicy _cachePolicy;

        public Uri URL { get; protected set; }

        public bool AllowAutoRedirect { get; set; }

        public bool PersistCookies { get; set; }

        public HttpMethod Method { get; set; }

        public string Accept { get; set; }
        public string AcceptCharSet { get; set; }
        public string AcceptEncoding { get; set; }
        public string AcceptLanguage { get; set; }
        public bool KeepAlive { get; set; }

        public string ContentLength { get; private set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }

        public string UserAgent { get; set; }

        public string Data { get; set; }

        public string Referer { get; set; }

        public int Timeout { get; set; }

        public bool Expect { get; set; }

        public string From { get; set; }

        public string IfMatch { get; set; }

        public CookieCollection Cookies { get; set; }

        public Dictionary<string, object> RawHeaders { get; protected set; }

        public Dictionary<string, string> Fields { get; protected set; }

        public HttpRequest(HttpMethod method, string url)
        {
            this.URL = new Uri(url, UriKind.RelativeOrAbsolute);
            this.Method = method;
            this.RawHeaders = new Dictionary<string, object>();
            this.Fields = new Dictionary<string, string>();
            this.UserAgent = "Aomi";
            this.Accept = String.Join(";", HttpContentTypes.TextHtml, HttpContentTypes.ApplicationXml, HttpContentTypes.ApplicationJson);
            this.Timeout = 10000;
            this.ContentType = HttpContentTypes.ApplicationXWwwFormUrlEncoded;
            this.AllowAutoRedirect = true;
        }

        public void AddExtraHeader(string header, object value)
        {
            if (value != null && !RawHeaders.ContainsKey(header))
            {
                RawHeaders.Add(header, value);
            }
        }

        public HttpWebRequest PrepareRequest()
        {

            if (!PersistCookies || cookieContainer == null)
                cookieContainer = new CookieContainer();

            httpWebRequest = (HttpWebRequest)WebRequest.Create(this.URL);
            httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;
            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = ContentType;
            httpWebRequest.Accept = Accept;
            httpWebRequest.Method = Method.ToString();
            httpWebRequest.UserAgent = UserAgent;
            httpWebRequest.Referer = Referer;
            httpWebRequest.CachePolicy = _cachePolicy;
            httpWebRequest.KeepAlive = KeepAlive;
            httpWebRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;

            ServicePointManager.Expect100Continue = Expect;
            ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;

            if (this.Timeout > 0)
            {
                httpWebRequest.Timeout = Timeout;
            }

            if (this.Cookies != null && this.Cookies.Count > 0)
            {
                httpWebRequest.CookieContainer.Add(Cookies);
            }

            AddExtraHeader("From", From);
            AddExtraHeader("Accept-CharSet", AcceptCharSet);
            AddExtraHeader("Accept-Encoding", AcceptEncoding);
            AddExtraHeader("Accept-Language", AcceptLanguage);
            AddExtraHeader("If-Match", IfMatch);
            AddExtraHeader("Content-Encoding", ContentEncoding);

            foreach (var header in RawHeaders)
            {
                httpWebRequest.Headers.Add(String.Format("{0}: {1}", header.Key, header.Value));
            }

            if (!string.IsNullOrWhiteSpace(this.Data))
            {
                var bytes = Encoding.UTF8.GetBytes(this.Data);

                if (bytes.Length > 0)
                {
                    httpWebRequest.ContentLength = bytes.Length;
                }

                using (var requestStream = httpWebRequest.GetRequestStream())
                {
                    if (requestStream != null)
                        requestStream.Write(bytes, 0, bytes.Length);
                }
            }

            return httpWebRequest;
        }

        private bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


    }
}
