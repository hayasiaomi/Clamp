using Clamp.UIShell.Framework.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Network.Service
{
    internal class ServiceAccessor
    {
        public const string Version = "1.0.0.0";
        public const string HttpAccessTemplate = "http://{0}:{1}/{2}/api/{3}/{4}";


        public static ServiceResult<UserInfo> Authorized(string username, string password)
        {
            //string url = string.Format(HttpAccessTemplate, ChromiumSettings.Demand.Server, ChromiumSettings.Port, ServiceModule.Account, Version, "users/login");

            //DebugHelper.WriteLine("开始访问URL({0})", url);

            //DebugHelper.WriteLine("访问参数username={0}&password={1}", username, password);

            //var param = JsonConvert.SerializeObject(new { loginName = username, pwd = Logistics.MD5Hash(password) });
            //var loader = new CloudLoader();
            //var header = loader.CreateHeadDictionary("1.0.0.0", null, param);
            //var client = new HttpSender(url, param, HttpMethod.Post, header);
            //string result = client.GetResponse();

            //DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");

            //if (!string.IsNullOrWhiteSpace(result))
            //    return JsonConvert.DeserializeObject<ServiceResult<UserInfo>>(result);

            return null;
        }

        public static ServiceResult<List<MachineInfo>> GetAllMachines()
        {
            //string url = string.Format(HttpAccessTemplate, ChromiumSettings.Demand.Server, ChromiumSettings.Port, ServiceModule.Restaurant, Version, "Machines");

            //DebugHelper.WriteLine("开始访问URL({0})", url);
            //var loader = new CloudLoader();
            //var header = loader.CreateHeadDictionary("1.0.0.0", null, "");
            //var client = new HttpSender(url, "", HttpMethod.Get, header);
            //string result = client.GetResponse();

            //DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");

            //if (!string.IsNullOrWhiteSpace(result))
            //    return JsonConvert.DeserializeObject<ServiceResult<List<MachineInfo>>>(result);

            return null;
        }


        public static ServiceResult<InitConfigInfo> GetInitConfig()
        {
            //string url = string.Format(HttpAccessTemplate, ChromiumSettings.Demand.Server, ChromiumSettings.Port, ServiceModule.Parts, Version, "GetInitConfig");

            //DebugHelper.WriteLine("开始访问URL({0})", url);
            //var loader = new CloudLoader();
            //var header = loader.CreateHeadDictionary("1.0.0.0", null, "");
            //var client = new HttpSender(url, "", HttpMethod.Get, header);
            //string result = client.GetResponse();

            //DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");

            //if (!string.IsNullOrWhiteSpace(result))
            //    return JsonConvert.DeserializeObject<ServiceResult<InitConfigInfo>>(result);

            return null;
        }

        public static ServiceResult<string> InsertMachine(string codeValue, string nameValue, int typeValue, string ipValue)
        {
            //string url = string.Format(HttpAccessTemplate, ChromiumSettings.Demand.Server, ChromiumSettings.Port, ServiceModule.Restaurant, Version, "Machine");

            //DebugHelper.WriteLine("开始访问URL({0})", url);
            //var loader = new CloudLoader();

            //var param = JsonConvert.SerializeObject(new { code = codeValue, name = nameValue, type = typeValue, ip = ipValue });

            //var header = loader.CreateHeadDictionary("1.0.0.0", null, param);
            //var client = new HttpSender(url, param, HttpMethod.Post, header);
            //string result = client.GetResponse();

            //DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");

            //if (!string.IsNullOrWhiteSpace(result))
            //    return JsonConvert.DeserializeObject<ServiceResult<string>>(result);

            return null;
        }

        public static void LoginNoticeUpdate()
        {
            try
            {
                //string url = string.Format(HttpAccessTemplate, ChromiumSettings.Demand.Server, ChromiumSettings.Port, ServiceModule.ScanCode, Version, "Config/Config/LoginInform");

                //DebugHelper.WriteLine("开始访问URL({0})", url);
                //var loader = new CloudLoader();
                //var header = loader.CreateHeadDictionary("1.0.0.0", null, "");
                //var client = new HttpSender(url, "", HttpMethod.Get, header);
                //string result = client.GetResponse();

                //DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");

            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
        }

    }
}
