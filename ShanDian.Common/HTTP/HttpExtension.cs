using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ShanDian.Common.HTTP
{
    public static class HttpExtension
    {
        public static T AsDeserializeBody<T>(this HttpResponse httpResponse)
        {
            if (httpResponse.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(httpResponse.RawText))
            {
                return httpResponse.DeserializeBody<T>();
            }

            return default(T);
        }
    }
}
