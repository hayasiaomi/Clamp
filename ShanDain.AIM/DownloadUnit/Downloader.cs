using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using Clamp.AIM.Exceptions;

namespace Clamp.AIM.DownloadUnit
{

    public class Downloader
    {
        /// <summary>
        /// 传输块大小
        /// </summary>
        private const int BLOCKSIZE = 4096;
        /// <summary>
        /// 最大重试次数
        /// </summary>
        public const int MAXRETRT = 3;
        /// <summary>
        /// Http连接超时时间
        /// </summary>
        private const int TIMEOUT = 10 * 1000;
        /// <summary>
        /// 下载文件目录
        /// </summary>
        public static readonly string DownloadPath;
        /// <summary>
        /// 临时文件路径
        /// </summary>
        public static readonly string DownloadTempPath;

        private Url DownloadUrl = null;
        private string FileName = null;
        private byte[] CheckSum = null;

        private DownloadInfo preDownloadInfo = null;
        /// <summary>
        /// 更新下载进度
        /// </summary>
        public event Action<long, long> DownloadUpdate;
        /// <summary>
        /// 下载完毕
        /// </summary>
        public event Action<Downloader> OnComplited;
        /// <summary>
        /// 校验完毕
        /// </summary>
        public event Action<Downloader, bool> CheckSumResult;
        /// <summary>
        /// 下载成功
        /// </summary>
        public event Action<Downloader> DownloadSuccess;
        /// <summary>
        /// 下载失败
        /// </summary>
        public event Action<Downloader, Exception> DownloadError;

        private int RetryCount = 0;
        /// <summary>
        /// 下载文件路径
        /// </summary>
        private string DownloadFile
        {
            get { return DownloadPath + this.FileName; }
        }

        private string DownloadTempFile
        {
            get { return DownloadTempPath + this.FileName + ".tmp"; }
        }

        static Downloader()
        {
            var basicPath = AppDomain.CurrentDomain.BaseDirectory;
            DownloadPath = basicPath + "Download\\";
            if (!Directory.Exists(DownloadPath))
            {
                Directory.CreateDirectory(DownloadPath);
            }
            DownloadTempPath = basicPath + "DownloadTemp\\";
            if (!Directory.Exists(DownloadTempPath))
            {
                Directory.CreateDirectory(DownloadTempPath);
            }
        }


        public Downloader(string url, string filename, byte[] checkSum)
        {
            this.DownloadUrl = new Url(url);
            this.FileName = filename;
            if (checkSum == null || checkSum.Length != 32)
                throw new ArgumentException(nameof(checkSum));
            this.CheckSum = checkSum;
        }

        public void StartDownload()
        {
            if (File.Exists(this.DownloadFile))
            {
                throw new FileExistException(this.DownloadFile);
            }

            if (File.Exists(this.DownloadTempFile))
            {
                preDownloadInfo = ReadPreDownloadInfo();
                ContinueToDownload();
            }
            else
            {
                GetServerInfoAsync(ContinueToDownload);
            }
        }

        private void ContinueToDownload()
        {

            if (this.preDownloadInfo.IsComplited)
            {
                OnComplited?.Invoke(this);
                StartToCheck();
                return;
            }
            var req = HttpWebRequest.Create(this.DownloadUrl.Value) as HttpWebRequest;
            req.Method = "GET";
            req.AddRange(this.preDownloadInfo.FileDownloaded, this.preDownloadInfo.FileDownloaded + BLOCKSIZE - 1);
            req.Timeout = TIMEOUT;
            req.BeginGetResponse(EndDownloadBlock, req);
        }

        private void EndDownloadBlock(IAsyncResult result)
        {
            var req = result.AsyncState as HttpWebRequest;
            try
            {
                var response = req.EndGetResponse(result);
                var stream = response.GetResponseStream();
                var buffer = new byte[response.ContentLength];
                var totalRead = 0;
                while (totalRead < buffer.Length)
                {
                    var realRead = stream.Read(buffer, totalRead, buffer.Length - totalRead);
                    totalRead += realRead;
                    if (realRead == 0)
                        break;
                }
                stream.Close();
                //更新临时文件
                using (var tempfile = new FileStream(this.DownloadTempFile, FileMode.Open))
                {
                    var longBuf = new byte[8];
                    tempfile.Seek(5, SeekOrigin.Begin);
                    var taglength = tempfile.ReadByte();
                    var seekPosition = taglength + 55 + this.preDownloadInfo.FileDownloaded;
                    tempfile.Seek(seekPosition, SeekOrigin.Begin);
                    tempfile.Write(buffer, 0, buffer.Length);
                    this.preDownloadInfo.UpdateDownload(buffer.Length);
                    this.DownloadUpdate?.Invoke(this.preDownloadInfo.FileDownloaded, this.preDownloadInfo.FileSize);
                    tempfile.Seek(46 + taglength, SeekOrigin.Begin);
                    var t = BitConverter.GetBytes(this.preDownloadInfo.FileDownloaded);
                    tempfile.Write(t, 0, t.Length);
                }
                ContinueToDownload();
            }
            catch (Exception e)
            {
                this.RetryCount++;
                if (this.RetryCount > MAXRETRT)
                {
                    DownloadError?.Invoke(this, e);
                    return;
                }
                this.ContinueToDownload();
            }
        }

        public void StartToCheck()
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var file = new FileStream(this.DownloadTempFile, FileMode.Open);
                file.Seek(5, SeekOrigin.Begin);
                var taglength = file.ReadByte();
                file.Seek(55 + taglength, SeekOrigin.Begin);
                var result = md5.ComputeHash(file);
                file.Close();
                StringBuilder sc = new StringBuilder();
                for (int index = 0; index < result.Length; index++)
                {
                    sc.Append(result[index].ToString("x2"));
                }

                var camd5 = sc.ToString();
                var oldCheckSum = Encoding.ASCII.GetString(this.CheckSum);
                var checkSumIsSame = camd5 == oldCheckSum;
                this.CheckSumResult?.Invoke(this, checkSumIsSame);
                if (checkSumIsSame)
                {
                    CopyToReal(55 + taglength);
                }
            }
        }

        public void CopyToReal(int tempFileSeek)
        {
            var buf = new byte[4096];
            using (var dest = new FileStream(this.DownloadFile, FileMode.Create))
            {
                using (var source = new FileStream(this.DownloadTempFile, FileMode.Open))
                {
                    source.Seek(tempFileSeek, SeekOrigin.Begin);
                    while (true)
                    {
                        var realread = source.Read(buf, 0, buf.Length);
                        if (realread == 0)
                            break;
                        dest.Write(buf, 0, realread);
                    }
                    dest.Flush();
                }
            }
            File.Delete(this.DownloadTempFile);
            DownloadSuccess?.Invoke(this);
        }

        private void GetServerInfoAsync(Action continueAct)
        {
            HttpWebRequest req = HttpWebRequest.Create(this.DownloadUrl.Value) as HttpWebRequest;
            req.Method = "GET";
            //尝试获取1个字节数据(如果IIS支持,可以考虑使用Head方法)
            req.AddRange(0, 1);
            req.Timeout = TIMEOUT;
            req.BeginGetResponse(EndGetServerInfo, new Tuple<HttpWebRequest, Action>(req, continueAct));
        }

        private void EndGetServerInfo(IAsyncResult result)
        {
            var context = result.AsyncState as Tuple<HttpWebRequest, Action>;
            var req = context.Item1;
            try
            {
                using (var response = req.EndGetResponse(result))
                {
                    var acceptRange = response.Headers["Accept-Ranges"];
                    var lastModfied = response.Headers["Last-Modified"];
                    if (string.IsNullOrEmpty(acceptRange) ||
                        "none".Equals(acceptRange, StringComparison.CurrentCultureIgnoreCase))
                    {
                        this.preDownloadInfo = new DownloadInfo(lastModfied, this.CheckSum, false, 0, 0);
                    }
                    else
                    {
                        var data = response.Headers["Content-Range"];
                        Tuple<long, long, long> tupleData = null;
                        if (data != null)
                            tupleData = ParseContentRange(data);
                        else
                        {
                            var templength = response.ContentLength;
                            tupleData = new Tuple<long, long, long>(0, templength - 1, templength);
                        }

                        this.preDownloadInfo = new DownloadInfo(lastModfied, this.CheckSum, true, 0, tupleData.Item3);
                    }
                    //将信息写入临时文件
                    this.preDownloadInfo.WriteToFile(this.DownloadTempFile);
                    context.Item2();
                }
            }
            catch (Exception e)
            {
                this.RetryCount++;
                if (this.RetryCount > MAXRETRT)
                {
                    DownloadError?.Invoke(this, e);
                    return;
                }

                GetServerInfoAsync(context.Item2);
            }
        }

        private Tuple<long, long, long> ParseContentRange(string data)
        {
            var temp = data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length != 2)
                throw new FormatException("ContentRangeFormatError:" + data);
            var temp1 = temp[1];
            var info = temp1.Split(new char[] { '-', '/' }, StringSplitOptions.None);
            if (info.Length != 3)
                throw new FormatException("ContentRangeFormatError:" + data);
            var start = long.Parse(info[0]);
            var lastPosit = string.IsNullOrEmpty(info[1]) ? -1 : long.Parse(info[1]);
            var length = string.IsNullOrEmpty(info[2]) ? -1 : long.Parse(info[2]);
            return new Tuple<long, long, long>(start, lastPosit, length);
        }

        private DownloadInfo ReadPreDownloadInfo()
        {
            using (var file = new FileStream(this.DownloadTempFile, FileMode.Open))
            {
                var magicNum = new byte[5];
                var realRead = file.Read(magicNum, 0, magicNum.Length);
                if (realRead != 5)
                    throw new TempFileFormatterException(this.DownloadTempFile);
                //1, magicNum校验
                if (Encoding.ASCII.GetString(magicNum, 0, magicNum.Length) != "sddlf")
                    throw new TempFileFormatterException(this.DownloadTempFile);

                realRead = file.Read(magicNum, 0, 1);
                if (realRead != 1)
                    throw new TempFileFormatterException(this.DownloadTempFile);
                var tagLength = (int)magicNum[0];

                string tagValue = "";
                if (tagLength > 0)
                {
                    var buf = new byte[tagLength];
                    realRead = file.Read(buf, 0, tagLength);
                    if (realRead != tagLength)
                        throw new TempFileFormatterException(this.DownloadTempFile);
                    tagValue = Encoding.ASCII.GetString(buf);
                }

                var checkSumBytes = new byte[32];
                realRead = file.Read(checkSumBytes, 0, checkSumBytes.Length);
                if (realRead != checkSumBytes.Length)
                    throw new TempFileFormatterException(this.DownloadTempFile);
                var longtemp = new byte[16];
                realRead = file.Read(longtemp, 0, longtemp.Length);
                if (realRead != longtemp.Length)
                    throw new TempFileFormatterException(this.DownloadTempFile);
                var size = BitConverter.ToInt64(longtemp, 0);
                var posit = BitConverter.ToInt64(longtemp, 8);
                realRead = file.Read(longtemp, 0, 1);
                if (realRead != 1)
                    throw new TempFileFormatterException(this.DownloadTempFile);
                return new DownloadInfo(tagValue, checkSumBytes, longtemp[0] == 0x01, posit, size);
            }
        }
    }
}
