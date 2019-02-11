namespace Clamp.Linker.Responses
{
    using Clamp;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    public class BundleFileResponse : Response
    {
        private static readonly byte[] ErrorText;

        static BundleFileResponse()
        {
            ErrorText = Encoding.UTF8.GetBytes("NOT FOUND");
        }

        public BundleFileResponse(RuntimeBundle runtimeBundle, string resourceName,string name)
        {
            this.ContentType = MimeTypes.GetMimeType(name);
            this.StatusCode = HttpStatusCode.OK;

            Stream content = runtimeBundle.GetResource(resourceName);

            if (content != null)
            {
                this.WithHeader("ETag", GenerateETag(content));

                content.Seek(0, SeekOrigin.Begin);
            }

            this.Contents = stream =>
            {
                if (content != null)
                {
                    content.CopyTo(stream);
                }
                else
                {
                    stream.Write(ErrorText, 0, ErrorText.Length);
                }
            };
        }
     
        private static string GenerateETag(Stream stream)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var hash = sha1.ComputeHash(stream);
                return string.Concat("\"", ByteArrayToString(hash), "\"");
            }
        }

        private static string ByteArrayToString(byte[] data)
        {
            var output = new StringBuilder(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                output.Append(data[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}
