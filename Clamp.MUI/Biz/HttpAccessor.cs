using Clamp.MUI.Framework.Network;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Clamp.MUI.Biz
{
    internal class HttpAccessor
    {
        public const string Version = "1.0.0.0";
        public const string HttpAccessTemplate = "http://{0}:{1}/{2}/api/{3}/{4}";

        public static HttpResult<TokenInfo> Token(string employeeNo, string password)
        {
            string url = string.Format(HttpAccessTemplate, ChromiumSettings.ServerAddress, ChromiumSettings.Port, HttpModule.Account, Version, "Token");

            HttpRequest httpRequest = HttpWorker.Post(url);

            httpRequest.Data = JsonConvert.SerializeObject(new { employeeNo = employeeNo, password = MD5Hash(password) });

            HttpResponse httpResponse = httpRequest.AsHttpResponse();

            if (!string.IsNullOrWhiteSpace(httpResponse.RawText))
                return JsonConvert.DeserializeObject<HttpResult<TokenInfo>>(httpResponse.RawText);

            return null;
        }

        public static HttpResult<UserInfo> UserInfo(int userId)
        {
            string url = string.Format(HttpAccessTemplate, ChromiumSettings.ServerAddress, ChromiumSettings.Port, HttpModule.Account, Version, "UserInfo/UserId");

            HttpRequest httpRequest = HttpWorker.Get(url + "?userId=" + userId);

            HttpResponse httpResponse = httpRequest.AsHttpResponse();


            if (!string.IsNullOrWhiteSpace(httpResponse.RawText))
                return JsonConvert.DeserializeObject<HttpResult<UserInfo>>(httpResponse.RawText);

            return null;

        }

        public static HttpResult<UserPermissionInfo> UserPermissions(int userId)
        {
            string url = string.Format(HttpAccessTemplate, ChromiumSettings.ServerAddress, ChromiumSettings.Port, HttpModule.Account, Version, "UserPermissions/UserId");


            HttpRequest httpRequest = HttpWorker.Get(url + "?userId=" + userId);

            HttpResponse httpResponse = httpRequest.AsHttpResponse();


            if (!string.IsNullOrWhiteSpace(httpResponse.RawText))
                return JsonConvert.DeserializeObject<HttpResult<UserPermissionInfo>>(httpResponse.RawText);

            return null;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }




    }
}
