using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDain.AIM.Helper;

namespace ShanDain.AIM.DownloadUnit
{
    public class DownloadTask
    {
        private Downloader downloader = null;
        private int downloadPercentage = 0;
        private static string downloadurlTemplate = ConfigHelper.HostAddress + "Plugins/{0}/{1}/{2}.zip";
        public string downloadurl { get; private set; }

        /// <summary>
        /// 更新下载进度(百分比)
        /// </summary>
        public event Action<DownloadTask, int> UpdateDownload;
        /// <summary>
        /// 下载成功或者失败
        /// </summary>
        public event Action<DownloadTask, bool> DownloadResult;

        public string AddInName { get; private set; }
        private TaskList TaskList;

        public DownloadTask(string filename, string checkSum, TaskList tasklist)
        {
            this.TaskList = tasklist;
            this.AddInName = filename;
            var splited = filename.Split('_');
            downloadurl = string.Format(downloadurlTemplate, splited[0], splited[1], filename);
            downloader = new Downloader(downloadurl, filename + ".zip", Encoding.ASCII.GetBytes(checkSum));
            downloader.CheckSumResult += Downloader_CheckSumResult;
            downloader.DownloadUpdate += Downloader_DownloadUpdate;
            downloader.DownloadSuccess += Downloader_DownloadSuccess;
            downloader.DownloadError += Downloader_DownloadError;
        }

        private void Downloader_DownloadError(Downloader obj, Exception ex)
        {
            this.TaskList.SetTaskResult(this.AddInName, false);
            MLog.GetInstance().SendDebug($"下载失败:{ex.ToString()}");
            DownloadResult?.Invoke(this, false);
        }

        private void Downloader_DownloadSuccess(Downloader obj)
        {
            this.TaskList.SetTaskResult(this.AddInName, true);
            MLog.GetInstance().SendDebug($"下载成功:{this.downloadurl}");
            DownloadResult?.Invoke(this, true);
        }

        private void Downloader_DownloadUpdate(long downloaded, long size)
        {
            size = (size == 0 ? 1 : size);
            var i = (downloaded / size) * 100;
            if (i != downloadPercentage)
            {
                i = downloadPercentage;
                UpdateDownload?.Invoke(this, (int)i);
            }
        }

        private void Downloader_CheckSumResult(Downloader obj, bool issuccess)
        {
            MLog.GetInstance().SendDebug($"{this.downloadurl} 校验和结果:{issuccess}");
            if (issuccess == false)
            {
                this.TaskList.SetTaskResult(this.AddInName, false);
                DownloadResult?.Invoke(this, false);
            }
        }

        public void Start()
        {
            MLog.GetInstance().SendDebug($"启动下载:{this.downloadurl}");
            downloader.StartDownload();
        }
    }
}
