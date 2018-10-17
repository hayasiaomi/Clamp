using CefSharp;
using ShanDian.UIShell.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShanDian.UIShell.Brower
{
    internal class CefSharpSchemeHandler : IResourceHandler
    {
        private static readonly IDictionary<string, byte[]> ResourceDictionary;

        private string mimeType;
        private MemoryStream stream;

        static CefSharpSchemeHandler()
        {
            ResourceDictionary = new Dictionary<string, byte[]>
            {
                { "/assets/image/Connect.svg", SDResources.Connect }
            };
        }

        bool IResourceHandler.ProcessRequest(IRequest request, ICallback callback)
        {
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;

            byte[] bytes;

            if (ResourceDictionary.TryGetValue(fileName, out bytes) && bytes != null && bytes.Length > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    using (callback)
                    {
                        stream = new MemoryStream(bytes);

                        var fileExtension = Path.GetExtension(fileName);
                        mimeType = ResourceHandler.GetMimeType(fileExtension);

                        callback.Continue();
                    }
                });

                return true;
            }
            else
            {
                callback.Dispose();
            }

            return false;
        }


        void IResourceHandler.GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = stream == null ? 0 : stream.Length;
            redirectUrl = null;

            response.StatusCode = (int)HttpStatusCode.OK;
            response.StatusText = "OK";
            response.MimeType = mimeType;
        }

        bool IResourceHandler.ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
        {
            //Dispose the callback as it's an unmanaged resource, we don't need it in this case
            callback.Dispose();

            if (stream == null)
            {
                bytesRead = 0;
                return false;
            }

            //Data out represents an underlying buffer (typically 32kb in size).
            var buffer = new byte[dataOut.Length];
            bytesRead = stream.Read(buffer, 0, buffer.Length);

            dataOut.Write(buffer, 0, buffer.Length);

            return bytesRead > 0;
        }

        bool IResourceHandler.CanGetCookie(CefSharp.Cookie cookie)
        {
            return false;
        }

        bool IResourceHandler.CanSetCookie(CefSharp.Cookie cookie)
        {
            return false;
        }

        void IResourceHandler.Cancel()
        {

        }
    }
}
