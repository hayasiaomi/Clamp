using ShanDian.Common.Helpers;
using ShanDian.Common.HTTP;
using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Helpers;
using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;

namespace ShanDian.SDK.Updater
{
    class SubUpdateChecker : UpdateChecker
    {
        private UpgradeWebClient webClient;
        private string tempFile;
        private int mainListener;

        public SubUpdateChecker(string updateProcessPath, string checkURL, string updateDownloadPath, int mainListener)
        {
            this.ProcessPath = updateProcessPath;
            this.CheckURL = checkURL;
            this.UpdateDownloadPath = updateDownloadPath;
            this.mainListener = mainListener;
        }

        public override UpdateStatus CheckUpdate()
        {
            this.UpdateInfo = this.GetUpdateInfo(this.CheckURL);

            this.LatestVersion = Version.Parse(this.UpdateInfo.VersionCode);

            if (CurrentVersion == null)
            {
                CurrentVersion = Version.Parse(RevisionClass.FullVersion);
            }

            UpdateStatus status = UpdateStatus.UpToDate;

            if (CurrentVersion != null && LatestVersion != null && Helper.CompareVersion(CurrentVersion, LatestVersion) < 0)
            {
                status = UpdateStatus.UpdateAvailable;
            }

            return status;
        }

        private UpdateInfo GetUpdateInfo(string url)
        {
            HttpRequest httpRequest = new HttpRequest(url, HttpMethod.GET);

            HttpResponse httpResponse = httpRequest.GetHttpResponse();

            UpdateInfo updateInfo = httpResponse.AsDeserializeBody<UpdateInfo>();

            if (updateInfo == null)
            {
                updateInfo = new UpdateInfo()
                {
                    VersionCode = RevisionClass.FullVersion,
                };
            }

            return updateInfo;
        }

        public override void DownloadUpdate()
        {
            if (this.UpdateInfo != null)
            {
                if (!Directory.Exists(Path.Combine(this.UpdateDownloadPath, this.UpdateInfo.VersionCode)))
                {
                    var uri = new Uri($"http://{SDHelper.GetDemand().Server}:{this.mainListener}/sd/upgrades/{this.UpdateInfo.VersionCode}/SDSetup.zip");

                    if (string.IsNullOrEmpty(this.UpdateDownloadPath))
                    {
                        tempFile = Path.GetTempFileName();
                    }
                    else
                    {
                        tempFile = Path.Combine(this.UpdateDownloadPath, $"{Guid.NewGuid().ToString()}.tmp");

                        if (!Directory.Exists(this.UpdateDownloadPath))
                        {
                            Directory.CreateDirectory(this.UpdateDownloadPath);
                        }
                    }

                    webClient = new UpgradeWebClient { CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore) };

                    webClient.Proxy = this.Proxy;


                    webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;

                    webClient.DownloadFileAsync(uri, tempFile);
                }
                else
                {
                    this.NoticeToUIShell(this.UpdateInfo.VersionCode);
                }
            }
        }

        private void NoticeToUIShell(string versionCode)
        {
            ObjectSingleton.GetRequiredInstance<IWinFormService>().Upgrade(versionCode);
        }


        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            if (e.Error != null)
            {
                SD.Log.Error("下载升级文件失败", e.Error);
                webClient = null;
                return;
            }

            string fileName;
            string contentDisposition = webClient.ResponseHeaders["Content-Disposition"] ?? string.Empty;

            if (string.IsNullOrEmpty(contentDisposition))
            {
                fileName = Path.GetFileName(webClient.ResponseUri.LocalPath);
            }
            else
            {
                fileName = this.TryToFindFileName(contentDisposition, "filename=");

                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = this.TryToFindFileName(contentDisposition, "filename*=UTF-8''");
                }
            }

            string fileDownloadPath = Path.Combine(UpdateDownloadPath, this.UpdateInfo.VersionCode);
            string updateFilename = Path.Combine(fileDownloadPath, fileName);

            if (!Directory.Exists(fileDownloadPath))
                Directory.CreateDirectory(fileDownloadPath);

            try
            {
                if (File.Exists(updateFilename))
                {
                    File.Delete(updateFilename);
                }

                File.Move(tempFile, updateFilename);

                File.WriteAllText(Path.Combine(fileDownloadPath, "changelog.txt"), this.UpdateInfo.UpdateLog, Encoding.UTF8);

            }
            catch (Exception ex)
            {
                if (Directory.Exists(fileDownloadPath))
                    Directory.Delete(fileDownloadPath, true);

                SD.Log.Error("在转移下载文件时出错", ex);

                webClient = null;
                return;
            }

            this.NoticeToUIShell(this.UpdateInfo.VersionCode);
        }

        private string TryToFindFileName(string contentDisposition, string lookForFileName)
        {
            string fileName = String.Empty;
            if (!string.IsNullOrEmpty(contentDisposition))
            {
                var index = contentDisposition.IndexOf(lookForFileName, StringComparison.CurrentCultureIgnoreCase);
                if (index >= 0)
                    fileName = contentDisposition.Substring(index + lookForFileName.Length);
                if (fileName.StartsWith("\""))
                {
                    var file = fileName.Substring(1, fileName.Length - 1);
                    var i = file.IndexOf("\"", StringComparison.CurrentCultureIgnoreCase);
                    if (i != -1)
                    {
                        fileName = file.Substring(0, i);
                    }
                }
            }
            return fileName;
        }
    }
}
