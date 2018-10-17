
namespace ShanDian.UIShell.Framework.Network.Internal
{
    internal class HttpWorker
    {
        public static HttpRequest Get(string url)
        {
            return new HttpRequest(HttpMethod.GET, url);
        }

        public static HttpRequest Post(string url)
        {
            return new HttpRequest(HttpMethod.POST, url);
        }

        public static HttpRequest Delete(string url)
        {
            return new HttpRequest(HttpMethod.DELETE, url);
        }

        public static HttpRequest Patch(string url)
        {
            return new HttpRequest(HttpMethod.PATCH, url);
        }

        public static HttpRequest Put(string url)
        {
            return new HttpRequest(HttpMethod.PUT, url);
        }
    }
}
