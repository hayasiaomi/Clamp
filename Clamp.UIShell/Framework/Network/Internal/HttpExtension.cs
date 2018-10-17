using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Clamp.UIShell.Framework.Network.Internal
{
    internal static class HttpExtension
    {
        public static HttpResponse AsHttpResponse(this HttpRequest httpRequest)
        {
            try
            {
                HttpWebRequest httpWebRequest = httpRequest.PrepareRequest();
                return HttpResponse.GetResponse(httpWebRequest);
            }
            catch (WebException we)
            {
                return HttpResponse.NewHttpResponse(HttpStatusCode.ExpectationFailed, we.Message);
            }
        }

        public static T AsDeserializeBody<T>(this HttpRequest httpRequest) 
        {

            HttpResponse httpResponse = AsHttpResponse(httpRequest);

            if (httpResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(httpResponse.RawText))
            {
                return httpResponse.DeserializeBody<T>();
            }

            return default(T);
        }
    }
}
