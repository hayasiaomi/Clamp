using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.MUI.Framework.Network
{
    public class HttpResponse
    {
        private HttpWebResponse httpWebResponse;

        public string CharacterSet { get; private set; }
        public string ContentType { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        public CookieCollection Cookies { get; private set; }
        public int Age { get; private set; }
        public HttpMethod[] Allow { get; private set; }
        public string ContentEncoding { get; private set; }
        public string ContentLanguage { get; private set; }
        public long ContentLength { get; private set; }
        public string ContentLocation { get; private set; }
        public string ContentDisposition { get; private set; }
        public int Code { get; private set; }
        public Dictionary<String, String> Headers { get; private set; }
        public Stream Raw { get; private set; }
        public DateTime Date { get; private set; }
        public string ETag { get; private set; }
        public DateTime Expires { get; private set; }
        public DateTime LastModified { get; private set; }
        public string Location { get; private set; }
        public string Server { get; private set; }
        public WebHeaderCollection RawHeaders { get; private set; }
        public Stream ResponseStream
        {
            get { return httpWebResponse.GetResponseStream(); }
        }



        public string RawText { get; private set; }
        private HttpResponse()
        {

        }

        private HttpResponse(HttpWebResponse httpWebResponse, string rawText)
        {
            this.httpWebResponse = httpWebResponse;
            this.RawText = rawText;
            this.CharacterSet = this.httpWebResponse.CharacterSet;
            this.ContentType = this.httpWebResponse.ContentType;
            this.StatusCode = this.httpWebResponse.StatusCode;
            this.StatusDescription = this.httpWebResponse.StatusDescription;
            this.Cookies = this.httpWebResponse.Cookies;
            this.ContentEncoding = this.httpWebResponse.ContentEncoding;
            this.ContentLength = this.httpWebResponse.ContentLength;
            this.Date = DateTime.Now;
            this.LastModified = this.httpWebResponse.LastModified;
            this.Server = this.httpWebResponse.Server;

            if (!String.IsNullOrEmpty(this.GetHeader("Age")))
            {
                this.Age = Convert.ToInt32(this.GetHeader("Age"));
            }

            this.ContentLanguage = this.GetHeader("Content-Language");
            this.ContentLocation = this.GetHeader("Content-Location");
            this.ContentDisposition = this.GetHeader("Content-Disposition");
            this.ETag = this.GetHeader("ETag");
            this.Location = this.GetHeader("Location");

            if (!String.IsNullOrEmpty(this.GetHeader("Expires")))
            {
                DateTime expires;
                if (DateTime.TryParse(this.GetHeader("Expires"), out expires))
                {
                    this.Expires = expires;
                }
            }

            this.RawHeaders = this.httpWebResponse.Headers;
        }

        private string GetHeader(string header)
        {
            return this.httpWebResponse.GetResponseHeader(header).Replace("\"", "");
        }

        public T DeserializeBody<T>()
        {
            return JsonConvert.DeserializeObject<T>(this.RawText);
        }

        public static HttpResponse GetResponse(WebRequest request, string characterSet = null)
        {
            HttpWebResponse response;
            string rawText;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                {
                    throw;
                }
                response = (HttpWebResponse)webException.Response;
            }

            using (var stream = response.GetResponseStream())
            {
                if (stream == null) return null;

                var encoding = string.IsNullOrEmpty(characterSet) ? Encoding.UTF8 : Encoding.GetEncoding(characterSet);
                using (var reader = new StreamReader(stream, encoding))
                {
                    rawText = reader.ReadToEnd();
                }
            }

            return new HttpResponse(response, rawText);
        }

        public static HttpResponse NewHttpResponse(HttpStatusCode statusCode, string statusDescription)
        {
            HttpResponse httpResponse = new HttpResponse();

            httpResponse.StatusCode = statusCode;
            httpResponse.StatusDescription = statusDescription;

            return httpResponse;
        }

    }
}
