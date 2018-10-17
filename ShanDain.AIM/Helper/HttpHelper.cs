using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShanDain.AIM.DTO;
using Newtonsoft.Json;

namespace ShanDain.AIM.Helper
{
    public class HttpHelper
    {
        private MainForm form = null;

        public HttpHelper(MainForm form)
        {
            this.form = form;
        }

        private static string Url = ConfigHelper.HostAddress;

        public void Search(string key, int start = 0, int size = 100)
        {
            var url = $"{Url}PackManager/GetPackageList?addinId={key}&start={start}&size={size}";
            var req = HttpWebRequest.Create(url);
            req.ContentType = "application/json";
            req.Method = "GET";
            req.BeginGetResponse(EndGetResponse, req);
            MLog.GetInstance().SendDebug("发起查询" + url);
        }

        private void EndGetResponse(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;
            try
            {
                var resp = req.EndGetResponse(result);
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    var str = sr.ReadToEnd();
                    var model = JsonConvert
                        .DeserializeObject<Result<Tuple<List<Tuple<string, Dictionary<string, AddInInfo>>>, int>>>(str);
                    if (model.ErrorCode != 200)
                    {
                        MessageBox.Show(model.ErrMsg);
                        MLog.GetInstance().SendDebug($"{model.ErrorCode} 查询错误: {model.ErrMsg}");
                    }

                    var data = model.Data;
                    foreach (var item in data.Item1)
                    {
                        foreach (var versioninfo in item.Item2)
                        {
                            MLog.GetInstance().SendDebug($"查询到结果:{item.Item1}:{versioninfo.Key}");
                        }
                    }

                    this.form.Invoke((Action) (() => { this.form.UpdateSearchResult(data.Item2, data.Item1); }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                MLog.GetInstance().SendDebug(e.ToString());
            }
            finally
            {
                this.form.EnableButtonSearch();
            }
        }
    }
}
