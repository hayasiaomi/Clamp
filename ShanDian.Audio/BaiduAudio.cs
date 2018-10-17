using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ShanDian.Common.Exceptions;
using ShanDian.Common.HTTP;

namespace ShanDian.Audio
{
    public class BaiduAudio
    {

        private static string client_id = "tETFfeZC9Iqw5QcjMThNpWOH";
        private static string client_secret = "feff16b10f182db738b1909a7382649b";

        public static string Access_token = "";
        private static DateTime _expiresTime = DateTime.MinValue;
        

        public static void GrantToken()
        {
            if (DateTime.Now > _expiresTime)
            {
                var url =
                    $"http://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={client_id}&client_secret={client_secret}";
                var httpRequest = new ShanDian.Common.HTTP.HttpRequest(url,HttpMethod.GET);
                var response = httpRequest.GetHttpResponse();
                //
                var dict = response.AsDeserializeBody<Dictionary<string, object>>();
                if (!dict.ContainsKey("access_token"))
                {
                    throw new ShanDianException(500, "");
                }
                Access_token = dict["access_token"].ToString();
                _expiresTime = DateTime.Now.AddDays(7);

            }
            
        }

        /// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <returns></returns>
        public  static void DownloadAudio(string param,string path,bool isOverride = false)
        {
            var url = "http://tsn.baidu.com/text2audio";
            if (File.Exists(path) )
            {
                if (isOverride)
                {
                    File.Delete(path); //存在则删除
                }
                else
                {
                    return;
                }
            }
            var tempPath = Path.GetDirectoryName(path) + @"\temp";
            Directory.CreateDirectory(tempPath);  //创建临时文件目录
            var tempFile = tempPath + @"\" + Path.GetFileName(path) + ".temp"; //临时文件
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);    //存在则删除
            }
          
                // 设置参数
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                var reqStreamWriter = new StreamWriter(request.GetRequestStream());
                reqStreamWriter.Write(param);
                reqStreamWriter.Close();

                //发送请求并获取相应回应数据
                var response = request.GetResponse() as HttpWebResponse;
                var contentType = response.Headers.GetValues("Content-Type").FirstOrDefault();
                if (contentType.StartsWith("audio"))
                {
                    var fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    //直到request.GetResponse()程序才开始向目标网页发送Post请求
                    var responseStream = response.GetResponseStream();
                    //创建本地文件写入流
                    //Stream stream = new FileStream(tempFile, FileMode.Create);
                    byte[] bArr = new byte[1024];
                    int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                    while (size > 0)
                    {
                        //stream.Write(bArr, 0, size);
                        fs.Write(bArr, 0, size);
                        size = responseStream.Read(bArr, 0, (int)bArr.Length);
                    }
                    //stream.Close();
                    fs.Close();
                    responseStream.Close();
                    File.Move(tempFile, path);
                    return;
                }
                if (contentType.Equals("application/json"))
                {
                    using (var responseStreamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = responseStreamReader.ReadToEnd();
                        //输出错误日志

                        responseStreamReader.Close();
                    }
                }
                throw new ShanDianException(400,"语音合成失败");

        }
    }
}