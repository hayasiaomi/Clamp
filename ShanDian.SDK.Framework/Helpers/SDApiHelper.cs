using ShanDian.Common.Extensions;
using ShanDian.Common.HTTP;
using ShanDian.Common.Helpers;
using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ShanDian.SDK.Framework.Helpers
{
    public class SDApiHelper
    {

        public static HttpResponse SDRequest(string url, HttpMethod httpMethod, string data, string appid = "", string secureKey = "")
        {
            LoggingService.Info($"请求的URL:{url}-方式:{httpMethod}-对应的数据:{data}-appid:{appid}-secureKey{secureKey}");

            HttpRequest httpRequest = new HttpRequest(url, httpMethod);

            if (httpMethod == HttpMethod.POST || httpMethod == HttpMethod.PUT || httpMethod == HttpMethod.PATCH)
            {
                httpRequest.ContentType = HttpContentTypes.ApplicationJson;
            }
            else
            {
                httpRequest.ContentType = HttpContentTypes.ApplicationXWwwFormUrlEncoded;
            }

            Dictionary<string, string> sdHeads = GetSDHeader(appid, secureKey, data);

            foreach (string headerKey in sdHeads.Keys)
            {
                httpRequest.AddExtraHeader(headerKey, sdHeads[headerKey]);
            }

            httpRequest.Data = data;

            HttpResponse httpResponse = httpRequest.GetHttpResponse();

            LoggingService.Info($"请求的URL:{url}-返回码:{httpResponse.Code} - 说明:{httpResponse.StatusDescription}");
            LoggingService.Info($"请求的URL:{url}-返回内容:{httpResponse.RawText} ");
            return httpResponse;
        }

        private static Dictionary<string, string> GetSDHeader(string appid, string secureKey, string data)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("AppId", appid);
            headers.Add("Ts", DateTime.Now.Unix().ToString());//System.Web.HttpUtility.UrlEncode(jsonParamList, Encoding.UTF8)
            headers.Add("Lang", "zh-cn");
            headers.Add("Version", "1.0.0.0");

            StringBuilder signLine = new StringBuilder();

            if (!string.IsNullOrEmpty(data))
            {
                signLine.Append(data);
                signLine.Append("&");
            }

            foreach (var dicparam in headers.Select(x => $"{x.Key}={x.Value}"))
            {
                signLine.Append(dicparam);
                signLine.Append("&");
            }

            signLine.Append(secureKey);

            headers.Add("Sign", signLine.ToString().ToLower().Md5Encrypt().ToLower());

            return headers;
        }

        public static string GetSDApiUrl(string fragment)
        {
            MediumConfiguration mediumConfiguration = ObjectSingleton.GetRequiredInstance<MediumConfiguration>();

            return $"{mediumConfiguration.SDApiHost}{fragment}";
        }
        /// <summary>
        /// 激活过后才可以有请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static HttpResponse SDFragmentRequest(string fragment, HttpMethod httpMethod, string data)
        {
            SDDemand demand = SDHelper.GetDemand();

            HttpResponse httpResponse;

            if (demand != null)
            {
                httpResponse = SDRequest(GetSDApiUrl(fragment), httpMethod, data, demand.AppId, demand.SecureKey);
            }
            else
            {
                httpResponse = HttpResponse.NewHttpResponse(HttpStatusCode.Unauthorized, "当前的善点系统还没有激活，你没有权限访问");
            }

            return httpResponse;

        }
    }
}
