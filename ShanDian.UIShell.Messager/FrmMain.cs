using CefSharp.WinForms;
using ShanDian.UIShell.Assist.Brower;
using ShanDian.UIShell.Assist.Helpers;
using Newtonsoft.Json;
using ShanDian.Common;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.InterProcess;
using ShanDian.UIShell.Framework.Model;
using ShanDian.UIShell.Framework.Services;
using ShanDian.UIShell.Framework.Vo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShanDian.UIShell.Assist
{
    public partial class FrmMain : Form
    {
        private ProcessBinder processBinder;
        private ChromiumWebBrowser browser;
        private object closeLock = new object();
        private string mark;
        private bool isChecking = false;

        public int SDShellPID { set; get; }

        public FrmMain()
        {
            InitializeComponent();
        }

        private void Browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                this.Hide();
                this.Opacity = 1;
            }));

        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            browser = new ChromiumWebBrowser(SDShellHelper.GetSDShellSettings().AdvicesUrl)
            {
                Dock = DockStyle.Fill,
            };

            this.browser.MenuHandler = new MenuHandler();
            this.browser.RequestHandler = new RequestHandler();
            this.browser.FrameLoadEnd += Browser_FrameLoadEnd;
            this.browser.LoadError += Browser_LoadError;

            this.browser.RegisterJsObject("SD", new WinformBrowserObject(this));

            this.Controls.Add(browser);
        }

        private void Browser_LoadError(object sender, CefSharp.LoadErrorEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                this.Opacity = 1;
            }));
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width - 5;
            this.Top = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 5;

            SDPipelineHelper.Setup("SDShellMessager");
            SDPipelineHelper.HandlePipelineCommand = this.HandlePipelineCommand;
        }

        private object HandlePipelineCommand(SDPipelineCommand command)
        {
            if (command != null)
            {
                Log4netService.Info("command.Data:" + command.Data);

                if (string.Equals("DispalyNotice", command.CommandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Notice notice = JsonConvert.DeserializeObject<Notice>(command.Data);

                    if (notice != null)
                        this.DisplayNotice(notice);
                }
            }

            return null;
        }

        private void DisplayNotice(Notice notice)
        {
            if (DateTime.Now.Subtract(notice.CreateDate) < TimeSpan.FromMinutes(1))
            {
                if (!this.Visible)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.Show();

                        bool topMost = this.TopMost;
                        this.TopMost = true;
                        this.TopMost = topMost;

                    }));
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        bool topMost = this.TopMost;
                        this.TopMost = true;
                        this.TopMost = topMost;
                    }));
                }


                NoticeVo noticeVo = new NoticeVo();

                noticeVo.Id = notice.Id;
                noticeVo.SerialNumber = notice.SerialNumber;
                noticeVo.Title = notice.Title;
                noticeVo.NoticeCategory = (int)notice.Category;
                noticeVo.Content = notice.Content;
                noticeVo.ShutCount = notice.ShutCount;
                noticeVo.CreateDate = notice.CreateDate.ToString("HH:mm");
                noticeVo.IconName = notice.IconName;
                noticeVo.UrlString = notice.UrlString;

                string message = JsonConvert.SerializeObject(noticeVo);

                Log4netService.Info("Web:" + message);

                var script = @"(function () {
                                        var msg = ##message##;
                                        window.frameEvent.setList(JSON.stringify(msg));
                                       })(); ";

                script = Regex.Replace(script, "##message##", message);

                Log4netService.Info("script:" + script);

                this.browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);

                //this.HanldeSound(noticeVo.NoticeCategory, noticeVo.SerialNumber);
            }
        }


        //public void HanldeSound(int category, string serialNumber)
        //{
        //    bool noticeSoundSwitch = GetNoticeValue(ConfigsDBHelper.AcquireValue("NoticeSoundSwitch"));

        //    Log4netService.Info($"NoticeSoundSwitch {noticeSoundSwitch}");

        //    if (noticeSoundSwitch)
        //    {
        //        if (!string.IsNullOrWhiteSpace(serialNumber))
        //        {
        //            string serialCode = serialNumber.ToUpper();

        //            string soundFileName = null;

        //            if (serialCode.StartsWith("MSG_SO"))
        //            {
        //                bool noticeSoundOrder = GetNoticeValue(ConfigsDBHelper.AcquireValue("NoticeSoundOrder"));

        //                if (noticeSoundOrder)
        //                {
        //                    if (serialCode != "MSG_SO_0005")
        //                    {
        //                        soundFileName = this.GetSoundFileName("order-tohes");
        //                    }
        //                    else
        //                    {
        //                        soundFileName = this.GetSoundFileName("order-failure-tohes");
        //                    }
        //                }
        //                else
        //                {
        //                    soundFileName = this.GetSoundFileName("unify");
        //                }

        //            }
        //            else if (serialCode.StartsWith("MSG_FI"))
        //            {
        //                bool noticeSoundPayment = GetNoticeValue(ConfigsDBHelper.AcquireValue("NoticeSoundPayment"));

        //                if (noticeSoundPayment)
        //                {
        //                    if (serialCode != "MSG_FI_0003")
        //                    {
        //                        soundFileName = this.GetSoundFileName("proceeds-tohes");
        //                    }
        //                }
        //                else
        //                {
        //                    soundFileName = this.GetSoundFileName("unify");
        //                }
        //            }
        //            else
        //            {
        //                soundFileName = this.GetSoundFileName("unify");
        //            }

        //            if (!string.IsNullOrWhiteSpace(soundFileName))
        //                SoundPlayerHelper.AddAndPlay(new SoundPlayer(soundFileName));
        //        }

        //    }
        //}

        //private string GetSoundFileName(string name)
        //{
        //    string[] mediaFiles = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media"), name + ".*", SearchOption.TopDirectoryOnly);

        //    if (mediaFiles != null && mediaFiles.Length > 0)
        //        return mediaFiles[0];
        //    return null;
        //}


        /// <summary>
        /// 把对象变成布尔类型
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        private bool GetNoticeValue(object objValue)
        {
            if (objValue != null)
                return Convert.ToBoolean(objValue);
            return false;
        }

        /// <summary>
        /// 隐藏窗体
        /// </summary>
        public void UnDisplay()
        {
            lock (this.closeLock)
            {
                this.Invoke(new Action(() =>
                {
                    this.Hide();
                }));

            }

        }

        /// <summary>
        /// 写信息给主进程
        /// </summary>
        /// <param name="message"></param>
        public void RedirectSettings()
        {

        }

        public void RedirectDetails(string url)
        {
            string message = Convert.ToBase64String(Encoding.UTF8.GetBytes(url));


        }

        public void RedirectHistory()
        {

        }

        public void BindMainProcesss(int processId)
        {
            this.SDShellPID = processId;

            new Thread(new ThreadStart(() =>
            {
                if (processBinder == null)
                {
                    processBinder = new ProcessBinder(this.SDShellPID);
                    processBinder.ProcessExited += this.SDShellProcess_Exited;
                    processBinder.Bind();
                }

            })).Start();
        }

        private void SDShellProcess_Exited(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }

        private void FrmMain_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                SoundPlayerHelper.Stop();
            }
        }
    }
}
