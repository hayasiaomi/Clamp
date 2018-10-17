using CefSharp.Wpf;
using Clamp.UIShell.Assist.Brower;
using Clamp.UIShell.Assist.Helpers;
using Newtonsoft.Json;
using Clamp.Common;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework.InterProcess;
using Clamp.UIShell.Framework.Model;
using Clamp.UIShell.Framework.Services;
using Clamp.UIShell.Framework.Vo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Clamp.UIShell.Assist
{
    /// <summary>
    /// WindowAdvicesxaml.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAdvices : Window
    {
        private ProcessBinder processBinder;
        private ChromiumWebBrowser chromiumWebBrowser;
        private object closeLock = new object();
        private string mark;
        private bool isChecking = false;

        public int SDShellPID { set; get; }

        public WindowAdvices()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void InitializeChromium()
        {
            this.chromiumWebBrowser = new ChromiumWebBrowser();

            this.chromiumWebBrowser.RegisterJsObject("SD", new WinformBrowserObject(this));

            this.chromiumWebBrowser.Address = SDShellHelper.GetSDShellSettings().AdvicesUrl;

            NLogService.Info("消息框的URL-" + SDShellHelper.GetSDShellSettings().AdvicesUrl);

            this.chromiumWebBrowser.MenuHandler = new MenuHandler();
            this.chromiumWebBrowser.RequestHandler = new RequestHandler();
            this.chromiumWebBrowser.FrameLoadEnd += Browser_FrameLoadEnd;
            this.chromiumWebBrowser.LoadError += Browser_LoadError;

            this.chromiumWebBrowser.AllowDrop = false;
            this.chromiumWebBrowser.FrameLoadEnd += Browser_FrameLoadEnd;

            this.AddChild(this.chromiumWebBrowser);
        }

        private void Browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Hide();
                this.Opacity = 1;
            }));

        }

        private void Browser_LoadError(object sender, CefSharp.LoadErrorEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Opacity = 1;
            }));
        }

        public void DisplayNotice(Notice notice)
        {
            if (DateTime.Now.Subtract(notice.CreateDate) < TimeSpan.FromMinutes(1))
            {
                if (!this.IsVisible)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.Show();

                        bool topMost = this.Topmost;
                        this.Topmost = true;
                        this.Topmost = topMost;

                    }));
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        bool topMost = this.Topmost;
                        this.Topmost = true;
                        this.Topmost = topMost;
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

                NLogService.Info("Web:" + message);

                var script = @"(function () {
                                        var msg = ##message##;
                                        window.frameEvent.setList(JSON.stringify(msg));
                                       })(); ";

                script = Regex.Replace(script, "##message##", message);

                NLogService.Info("script:" + script);

                this.chromiumWebBrowser.GetBrowser().MainFrame.ExecuteJavaScriptAsync(script);

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
                this.Dispatcher.Invoke(new Action(() =>
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
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }



        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.IsVisible)
            {
                SoundPlayerHelper.Stop();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 5;
            this.Top = SystemParameters.PrimaryScreenHeight - this.Height - 5;

            this.Hide();
        }
    }
}
