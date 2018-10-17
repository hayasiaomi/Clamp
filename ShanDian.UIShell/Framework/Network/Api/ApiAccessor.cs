using ShanDian.UIShell.Framework.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Network.Api
{
    internal class ApiAccessor
    {
        private static readonly DateTime BaseTime = new DateTime(1970, 1, 1);

        public static ApiResult UploadSoftware(SoftwareInfo softwareInfo)
        {

            //string url = $"{HydraSystemConfig.SdApiHost}/v1/rests/clientData";

            //DebugHelper.WriteLine("开始访问URL({0})", url);

            //var param = JsonConvert.SerializeObject(softwareInfo);
            //var loader = new CloudLoader();
            //var header = loader.CreateHeadDictionary("1.0.0.0", null, param);
            //var client = new HttpSender(url, param, HttpMethod.Post, header);
            //string result = client.GetResponse();

            //DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, string.IsNullOrWhiteSpace(result) ? "null" : result);

            ////string url = "http://shandianapi.chidaoni.com/v1/rests/clientData";

            ////DebugHelper.WriteLine("开始访问URL({0})", url);

            ////HttpRequest httpRequest = HttpWorker.Post(url);

            ////httpRequest.ContentType = HttpContentTypes.ApplicationJson;

            ////string param = JsonConvert.SerializeObject(softwareInfo);

            ////DebugHelper.WriteLine("开始访问URL({0})=> param:{1}", url, param);

            ////Dictionary<string, string> headers = CreateHeadDictionary("v1", null, param);

            ////foreach (string headerName in headers.Keys)
            ////{
            ////    httpRequest.AddExtraHeader(headerName, headers[headerName]);
            ////}

            ////httpRequest.Data = param;

            ////HttpResponse httpResponse = httpRequest.AsHttpResponse();

            ////DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, httpResponse != null ? httpResponse.RawText : "null");

            //if (!string.IsNullOrWhiteSpace(result))
            //    return JsonConvert.DeserializeObject<ApiResult>(result);

            return new ApiResult() { Code = -1 };
        }

       


        private static long Unix(DateTime dateTime, bool millisecond = false)
        {
            TimeSpan ret = (dateTime - (TimeZone.CurrentTimeZone.ToLocalTime(BaseTime)));
            if (millisecond)
                return (long)ret.TotalMilliseconds;
            else
                return (long)ret.TotalSeconds;
        }

        public static string Md5Encrypt(string obj)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(obj, "MD5");
        }
    }
}
