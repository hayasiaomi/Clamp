using ShanDain.AIM.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShanDain.AIM.Helper
{
    public class NetGetter : IManagerGetter
    {
        public AddInInfo GetAddInInfo(string addinId, string version)
        {
            var req = HttpWebRequest.Create(
                    $"{ConfigHelper.HostAddress}PackManager/GetPackageInfo?addinId={addinId}&version={version}") as
                HttpWebRequest;
            req.ContentType = "application/json";
            req.Method = "GET";
            var response = req.GetResponse();
            var sr = new StreamReader(response.GetResponseStream());
            var str = sr.ReadToEnd();
            sr.Close();
            var data = JsonConvert.DeserializeObject<Result<AddInInfo>>(str);
            if (data.ErrorCode == 200 && data.Data != null)
            {
                return data.Data;
            }

            return null;
        }
    }
}
