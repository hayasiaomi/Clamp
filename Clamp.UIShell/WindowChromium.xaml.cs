using CefSharp;
using CefSharp.Wpf;
using Clamp.UIShell.Framework.DB;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework.InterProcess;
using Clamp.UIShell.Framework.Model;
using Clamp.UIShell.Framework.Vo;
using Clamp.UIShell.Brower;
using Clamp.UIShell.Framework;
using Clamp.UIShell.Framework.Shortcut;
using Clamp.UIShell.Forms;
using Clamp.UIShell.Properties;
using Clamp.UIShell.Views;
using Clamp.UIShell.Win32;
using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Threading;
using Clamp.UIShell.Glow;
using Clamp.UIShell.Framework.Brower;
using Clamp.Common.Commands;
using Clamp.Common;

namespace Clamp.UIShell
{
    /// <summary>
    /// ChromiumWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowChromium : Window
    {
        private bool IsKeyboardCtrlDown = false;
        private DateTime? keyDownDataTime;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private bool isClosed = false;
        private bool IsRendered = false;
        private ChromiumWebBrowser chromiumWebBrowser;
        private Region region;
        private WindowBrowserObject windowBrowserObject;
        private DispatcherTimer dispatcherTimer;

        public WindowAuthority WindowAuthority { set; get; }

        #region 消息气泡业务相关（2018年7月25日）

        private BackgroundWorker backgroundWorker;
        private FileSystemWatcher fwatcher;
        private object closeLock = new object();
        private bool isChecking = false;

        #endregion


        public WindowChromium()
        {
            HotkeyHelper.HotkeyBinder.OnHotkeyAlreadyRegistered += HotkeyBinder_HotkeyAlreadyRegistered;
            this.Identity = Guid.NewGuid().ToString("N");
            this.Title = SDResources.MainWindow_ProductName + SDShell.FullVersion;
            this.Icon = BitmapFrame.Create(this.GetType().Assembly.GetManifestResourceStream("ShanDian.UIShell.Resources.Logo.ico"));
            SDShell.ChromiumWindow = this;

            InitializeComponent();
            InitializeChromium();
            InitializeNotifyIcon();
        }
        #region 浏览器业务
        private void InitializeChromium()
        {
            this.windowBrowserObject = new WindowBrowserObject(this);
            this.chromiumWebBrowser = new ChromiumWebBrowser();
            this.chromiumWebBrowser.RegisterJsObject("SD", this.windowBrowserObject, false);
            this.chromiumWebBrowser.Address = SDShell.SDShellSettings.InitializeUrl;
            this.chromiumWebBrowser.AllowDrop = false;
            this.chromiumWebBrowser.BrowserSettings.Databases = CefState.Disabled;
            this.chromiumWebBrowser.BrowserSettings.Plugins = CefState.Disabled;
            this.chromiumWebBrowser.BrowserSettings.ApplicationCache = CefState.Disabled;


            this.chromiumWebBrowser.RequestHandler = new RequestHandler();
            this.chromiumWebBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler();
            this.chromiumWebBrowser.MenuHandler = new MenuHandler();

            DragHandler dragHandler = new DragHandler();

            dragHandler.RegionsChanged += DragHandler_RegionsChanged;

            this.chromiumWebBrowser.DragHandler = dragHandler;
            this.chromiumWebBrowser.LoadHandler = new LoadHandler();
            this.chromiumWebBrowser.LoadingStateChanged += ChromiumWebBrowser_LoadingStateChanged;
            this.chromiumWebBrowser.LoadError += (sender, args) =>
            {
                if (args.ErrorCode == CefErrorCode.Aborted)
                {
                    return;
                }
                args.Frame.LoadStringForUrl(SDResources.default_404, args.FailedUrl);
            };

            this.chromiumWebBrowser.PreviewTextInput += (o, e) =>
            {
                foreach (var character in e.Text)
                {
                    if (this.chromiumWebBrowser != null)
                    {
                        IBrowser iBrowser = this.chromiumWebBrowser.GetBrowser();

                        if (iBrowser != null)
                        {
                            IBrowserHost browserHost = iBrowser.GetHost();

                            if (browserHost != null)
                            {
                                browserHost.SendKeyEvent((int)CefSharp.Wpf.WM.CHAR, (int)character, 0);
                            }
                        }
                    }
                }

                e.Handled = true;
            };

            this.chromiumWebBrowser.AllowDrop = false;
            this.chromiumWebBrowser.FrameLoadEnd += Browser_FrameLoadEnd;

            this.AddChild(this.chromiumWebBrowser);
        }

        private void ChromiumWebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                if (this.dispatcherTimer != null)
                {
                    if (this.dispatcherTimer.IsEnabled)
                    {
                        this.dispatcherTimer.Stop();
                    }

                    this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    this.dispatcherTimer.Start();
                }
            }
        }

        /// <summary>
        /// 处理网页上拖动的区域
        /// </summary>
        /// <param name="region"></param>
        private void DragHandler_RegionsChanged(Region region)
        {
            if (region != null)
            {
                if (this.region == null)
                {
                    this.chromiumWebBrowser.PreviewMouseLeftButtonDown += Browser_PreviewMouseLeftButtonDown;
                }

                this.region = region;
            }
        }

        /// <summary>
        /// 处理网页鼠标左键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(this.chromiumWebBrowser);

            if (region.IsVisible((float)point.X, (float)point.Y))
            {
                if (e.ClickCount != 2)
                {
                    var window = Window.GetWindow(this);

                    window.DragMove();
                }
                else
                {
                    var window = Window.GetWindow(this);

                    if (window.WindowState == WindowState.Normal)
                    {
                        window.WindowState = WindowState.Maximized;
                    }
                    else if (window.WindowState == WindowState.Maximized)
                    {
                        window.WindowState = WindowState.Normal;
                    }
                }

                e.Handled = true;
            }
        }
        /// <summary>
        /// 网页加载后的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (!"about:blank".Equals(e.Url, StringComparison.CurrentCultureIgnoreCase))
            {
                if (this.dispatcherTimer == null)
                {
                    this.dispatcherTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, this.Dispatcher);
                    this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    this.dispatcherTimer.Tick += DispatcherTimer_Tick;
                    this.dispatcherTimer.Start();
                }
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.dispatcherTimer.Tick -= this.DispatcherTimer_Tick;
            this.dispatcherTimer.Stop();
            this.dispatcherTimer = null;

            if (!this.IsRendered)
            {
                this.Opacity = 1;
                this.ShowInTaskbar = true;
                this.WindowState = WindowState.Normal;
                this.SetValue(GlowManager.EnableGlowProperty, true);
                this.Activate();

                this.IsRendered = true;
            }

            if (this.WindowAuthority != null)
            {
                this.WindowAuthority.Close();
                this.WindowAuthority = null;
            }
        }

        #endregion

        #region 托盘业务
        /// <summary>
        /// 初始化托盘
        /// </summary>
        private void InitializeNotifyIcon()
        {
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();

            System.Windows.Forms.MenuItem openMenuItem = new System.Windows.Forms.MenuItem(SDResources.MainWindow_NotifyIcon_Open);
            openMenuItem.Click += OpenMenuItem_Click;

            System.Windows.Forms.MenuItem exitMenuItem = new System.Windows.Forms.MenuItem(SDResources.MainWindow_NotifyIcon_Exit);
            exitMenuItem.Click += ExitMenuItem_Click;

            System.Windows.Forms.MenuItem floatMenuItem = new System.Windows.Forms.MenuItem(SDResources.MainWindow_NotifyIcon_FloatDisplay);
            floatMenuItem.Name = "floatMenuItem";
            floatMenuItem.Click += FloatMenuItem_Click;

            System.Windows.Forms.MenuItem[] m = new System.Windows.Forms.MenuItem[] { floatMenuItem, openMenuItem, exitMenuItem, };

            this.notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(m);
            this.notifyIcon.Icon = new System.Drawing.Icon(this.GetType().Assembly.GetManifestResourceStream("ShanDian.UIShell.Resources.Logo.ico"));
            this.notifyIcon.Visible = true;

            this.notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        /// <summary>
        /// 托盘的退出菜单的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.InvokeQuit();
        }

        /// <summary>
        /// 悬浮窗菜单的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FloatMenuItem_Click(object sender, EventArgs e)
        {
            object floatSwitchValue = DBHelper.AcquireValue("FloatSwitch");

            if (floatSwitchValue != null && Convert.ToBoolean(floatSwitchValue))
            {
                this.SwitchFloatWindow(false);
            }
            else
            {
                this.SwitchFloatWindow(true);
            }
        }
        /// <summary>
        /// 托盘的打开菜单的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            this.DisplayNormal();
        }

        /// <summary>
        /// 双击托盘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.DisplayNormal();
        }

        #endregion

        #region Window内部


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //启动监听消息气泡信息的后台线程
            if (this.backgroundWorker == null)
            {
                this.backgroundWorker = new BackgroundWorker();
                this.backgroundWorker.DoWork += BackgroundWorker_DoWork;
                this.backgroundWorker.WorkerSupportsCancellation = true;
                this.backgroundWorker.RunWorkerAsync();
            }

            this.Activate();


        }

        #region 消息气泡业务相关

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string fwatcherPath = System.IO.Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Commands");

            if (!Directory.Exists(fwatcherPath))
            {
                Directory.CreateDirectory(fwatcherPath);
            }

            DebugHelper.WriteLine("发生变化 {0}", fwatcherPath);

            this.fwatcher = new FileSystemWatcher(fwatcherPath);

            this.fwatcher.Filter = "cmd-*.db";
            this.fwatcher.NotifyFilter = NotifyFilters.Size;
            this.fwatcher.IncludeSubdirectories = false;
            this.fwatcher.Changed += Fwatcher_Changed;
            this.fwatcher.EnableRaisingEvents = true;
        }

        private void Fwatcher_Changed(object sender, FileSystemEventArgs e)
        {
            this.fwatcher.Changed -= Fwatcher_Changed;

            Task.Factory.StartNew(HandleCommand, e.FullPath);
        }

        /// <summary>
        /// 处理消息气泡信息
        /// </summary>
        private void HandleCommand(object state)
        {
            string fullPath = Convert.ToString(state);

            DebugHelper.WriteLine("（消息气泡通知）发生变化 {0}", fullPath);
            using (var db = new LiteDatabase(fullPath))
            {
                LiteCollection<UICommand> cmdCollection = db.GetCollection<UICommand>();
                List<UICommand> cmds = cmdCollection.Find(n => n.IsHandled == false).ToList();

                if (cmds != null && cmds.Count > 0)
                {
                    lock (closeLock)
                    {

                        List<MsgBubbleNoticeDto> msgBubbleNoticeDtoVoList = new List<MsgBubbleNoticeDto>();

                        foreach (var cmd in cmds)
                        {
                            cmd.IsHandled = true;

                            if (cmd.Name == "MsgBubble")
                            {

                                DebugHelper.WriteLine("（消息气泡通知）IsDispose:" + JsonConvert.SerializeObject(cmd));

                                if (!string.IsNullOrWhiteSpace(cmd.Parameters))
                                {
                                    MsgBubbleNotice msgBubbleNotice = JsonConvert.DeserializeObject<MsgBubbleNotice>(cmd.Parameters);

                                    if (msgBubbleNotice != null)
                                    {
                                        MsgBubbleNoticeDto noticeVo = new MsgBubbleNoticeDto();
                                        noticeVo.code = msgBubbleNotice.Code;
                                        noticeVo.total = msgBubbleNotice.TotalNum;
                                        msgBubbleNoticeDtoVoList.Add(noticeVo);
                                    }
                                }

                            }
                            else if (cmd.Name == "Mqtt")
                            {
                                if (!string.IsNullOrWhiteSpace(cmd.Parameters))
                                {
                                    MqttState mqttState = JsonConvert.DeserializeObject<MqttState>(cmd.Parameters);

                                    if (mqttState != null)
                                    {
                                        if (this.chromiumWebBrowser != null)
                                        {

                                            var script = @"(function () {
                                                        console.log(##message##);
                                                        window.frameEvent.networkStatus(##message##);
                                                       })(); ";

                                            if (mqttState.Connected)
                                                script = Regex.Replace(script, "##message##", "false");
                                            else
                                                script = Regex.Replace(script, "##message##", "true");

                                            this.chromiumWebBrowser.ExecuteScriptAsync(script);

                                            DebugHelper.WriteLine("（MQTTSTATE）script:" + mqttState.Connected);
                                        }
                                    }
                                }
                            }
                        }

                        if (msgBubbleNoticeDtoVoList.Count > 0)
                        {
                            //执行脚本
                            if (this.chromiumWebBrowser != null)
                            {
                                string message = JsonConvert.SerializeObject(msgBubbleNoticeDtoVoList);

                                var script = @"(function () {
                                        console.log(##message##);
                                        window.frameEvent.navMsgPush(##message##);
                                       })(); ";

                                script = Regex.Replace(script, "##message##", message);

                                this.chromiumWebBrowser.ExecuteScriptAsync(script);

                                DebugHelper.WriteLine("（消息气泡通知）script:" + script);
                            }
                        }

                        cmdCollection.Update(cmds);
                    }
                }

                #region 删除过期的数据库
                try
                {
                    if (!this.isChecking)
                    {
                        this.isChecking = true;
                        Task.Factory.StartNew((obj) =>
                        {
                            string dir = System.IO.Path.GetDirectoryName(Convert.ToString(obj));

                            if (Directory.Exists(dir))
                            {
                                string[] aFiles = Directory.GetFiles(dir, "MsgBubble-*.db", SearchOption.TopDirectoryOnly);

                                if (aFiles != null && aFiles.Length > 0)
                                {
                                    foreach (string aFile in aFiles)
                                    {
                                        FileInfo fi = new FileInfo(aFile);

                                        if (fi.Exists && DateTime.Now.Subtract(fi.CreationTime) > TimeSpan.FromDays(2))
                                        {
                                            fi.Delete();
                                        }
                                    }
                                }
                            }
                            this.isChecking = false;

                        }, fullPath);
                    }
                }
                catch (Exception ex)
                {

                }
                #endregion
            }

            this.fwatcher.Changed += Fwatcher_Changed;
            DebugHelper.WriteLine("（消息气泡通知）结束操作");
        }

        #endregion

        private void Window_Initialized(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                if (HotkeyHelper.GetShortcutsEnabled())
                {
                    List<DictionaryInfo> dictionaryInfos = HotkeyHelper.AcquireAll();

                    if (dictionaryInfos != null & dictionaryInfos.Count > 0)
                    {
                        foreach (DictionaryInfo dictionaryInfo in dictionaryInfos)
                        {
                            if (dictionaryInfo.Value != null)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    HotkeyRegisterResult hrr = HotkeyHelper.RegisterHotKey(dictionaryInfo.KeyName, Convert.ToString(dictionaryInfo.Value), true, this.HotkeyCallback);
                                }));
                            }
                        }
                    }
                }

            });

            SDPipelineHelper.Setup("SDShell");

            SDPipelineHelper.HandlePipelineCommand = this.HandlePipelineCommand;

        }

        private object HandlePipelineCommand(SDPipelineCommand command)
        {
            if (command != null)
            {
                if (command.Compare("ShellActivate"))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (this.WindowState != WindowState.Maximized)
                            this.WindowState = WindowState.Normal;

                        this.Activate();
                        this.Topmost = true;
                        Thread.Sleep(20);
                        this.Topmost = false;

                    }));
                }
            }

            return null;
        }

        /// <summary>
        /// 注册消息队列
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        /// <summary>
        /// 一般用于打开应用的时候，发现存在，就只要激活主框
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeConstant.WM_SHOWME)
            {
                DebugHelper.WriteLine("WM_SHOWME");
                this.DisplayNormal();
            }

            return IntPtr.Zero;
        }



        #endregion

        #region 快捷业务

        /// <summary>
        /// 验证是否存在快捷键冲突
        /// </summary>
        internal void ValidateConflictHotkeys()
        {
            object khhObj = DBHelper.AcquireValue("KeyHotHint");

            //等于空就是表示没有指定。那就提醒
            if (khhObj == null || Convert.ToBoolean(khhObj))
            {
                if (HotkeyHelper.HaveConflict())
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        DialogKeyHot dialog = new DialogKeyHot();
                        dialog.Owner = this;
                        bool? result = dialog.ShowDialog();

                        if (result != null && result.HasValue && result.Value)
                        {
                            if (this.chromiumWebBrowser != null)
                            {
                                var script = @"(function () {
                                           window.frameEvent.openShortKey();
                                        })();";

                                this.chromiumWebBrowser.ExecuteScriptAsync(script);
                            }
                        }
                    }));
                }
            }



        }

        internal void ValidateAdvicesMessage()
        {
            Task.Factory.StartNew(() =>
            {
                bool haveUnDisplayNotice = false;

                string advicesPath = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Advices");

                if (!System.IO.Directory.Exists(advicesPath))
                {
                    System.IO.Directory.CreateDirectory(advicesPath);
                }

                using (var db = new LiteDB.LiteDatabase(System.IO.Path.Combine(advicesPath, NotcieHelper.GetFileName())))
                {
                    LiteCollection<Notice> noticeCollection = db.GetCollection<Notice>();

                    if (noticeCollection.Count(n => n.IsDisplay == false) > 0)
                        haveUnDisplayNotice = true;
                    else
                        haveUnDisplayNotice = false;
                }

                this.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (this.chromiumWebBrowser != null)
                    {
                        DebugHelper.WriteLine("判断是否有消息:" + Convert.ToString(haveUnDisplayNotice));

                        var script = @"(function () {
                                        window.frameEvent.messagePush(##message##);
                                       })(); ";

                        script = Regex.Replace(script, "##message##", Convert.ToString(haveUnDisplayNotice).ToLower());

                        this.chromiumWebBrowser.ExecuteScriptAsync(script);
                    }
                }));
            });
        }

        private void HotkeyBinder_HotkeyAlreadyRegistered(Hotkey hotkeyCombo, HotkeyAlreadyBoundException e)
        {

        }

        private void KeyboardKeyUpEventHandler(object sender, RawKeyEventArgs args)
        {
            if (this.IsKeyboardCtrlDown && (args.Key == Key.LeftCtrl || args.Key == Key.RightCtrl))
            {
                DebugHelper.WriteLine("KeyUp");

                this.IsKeyboardCtrlDown = false;
            }
        }

        public void KeyboardKeyDownEventHandler(object sender, RawKeyEventArgs args)
        {
            if (args.Key == Key.LeftCtrl || args.Key == Key.RightCtrl)
            {
                if (!this.IsKeyboardCtrlDown)
                {
                    this.IsKeyboardCtrlDown = true;

                    DebugHelper.WriteLine("KeyDown");

                    if (this.keyDownDataTime != null && this.keyDownDataTime.HasValue)
                    {
                        int milliseconds = DateTime.Now.Subtract(this.keyDownDataTime.Value).Milliseconds;

                        DebugHelper.WriteLine("milliseconds " + milliseconds);

                        if (milliseconds > 100 && milliseconds < 500)
                        {
                            DebugHelper.WriteLine("double ctrl");
                            this.DisplayNormal();
                        }
                        else
                        {
                            this.keyDownDataTime = null;
                        }
                    }
                    else
                    {
                        this.keyDownDataTime = DateTime.Now;
                    }
                }
                else
                {
                    this.keyDownDataTime = null;
                }

            }
            else
            {
                this.keyDownDataTime = null;
            }

        }

        private void HotkeyCallback(Hotkey hotkey)
        {
            if (HotkeyHelper.GetShortcutsEnabled())
            {
                if ("OpenShanDian" == hotkey.KeyName)
                {
                    this.DisplayNormal();
                }
                else
                {
                    if (this.windowBrowserObject != null)
                        this.windowBrowserObject.events.emit("HotkeyEvent", hotkey.KeyName);
                }
            }
        }

        public HotkeyRegisterResult RegisterHotkey(string keyName, string value)
        {
            return HotkeyHelper.RegisterHotKey(keyName, value, this.HotkeyCallback);
        }

        #endregion

        #region 对外访问业务
        /// <summary>
        /// 主框登出
        /// </summary>
        internal void Logout()
        {
            CDBHelper.Modify("user_auth", "");

            this.Dispatcher.Invoke(new Action(() =>
            {
                WindowAuthority windowAuthority = new WindowAuthority();
                System.Windows.Application.Current.MainWindow = windowAuthority;
                windowAuthority.Show();
                this.Close(true);
            }));
        }
        /// <summary>
        /// 设置是否正真退出
        /// </summary>
        /// <param name="isClosed"></param>
        internal void Close(bool isClosed)
        {
            this.isClosed = true;
            this.Close();
        }

        /// <summary>
        /// 显示窗体，用于托盘时候调用
        /// </summary>
        internal void DisplayNormal()
        {
            if (this.Visibility != Visibility.Visible)
            {
                this.Visibility = Visibility.Visible;
            }

            this.Show();

            if (this.WindowState != WindowState.Maximized)
                this.WindowState = WindowState.Normal;

            this.Activate();

            bool topmost = this.Topmost;
            this.Topmost = true;
            this.Topmost = topmost;

            //if (this.displayChromiumView != null)
            //    this.displayChromiumView.ViewFocus();
        }

        /// <summary>
        /// 打开新的窗体
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <param name="callback"></param>
        internal void CreateChildrenChromium(string url, string title, int width, int height, double top, double left, Action<string> callback)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                WindowSimpleChromium nBrowser = SDShell.GetChromium(url);

                if (nBrowser == null)
                {
                    nBrowser = new WindowSimpleChromium(url);

                    nBrowser.Title = title;
                    nBrowser.Width = width;
                    nBrowser.Height = height;
                    nBrowser.Left = left;
                    nBrowser.Top = top;
                }

                DebugHelper.WriteLine("打开一个新窗体");

                if (nBrowser.WindowState != WindowState.Normal)
                    nBrowser.WindowState = WindowState.Normal;

                bool topmost = nBrowser.Topmost;

                nBrowser.Topmost = true;

                nBrowser.Topmost = topmost;

                nBrowser.Show();

                nBrowser.Activate();
                nBrowser.Focus();

                if (callback != null)
                    callback(nBrowser.Identity);
            }));

        }

        /// <summary>
        /// 悬浮窗的开关
        /// </summary>
        /// <param name="state"></param>
        internal void SwitchFloatWindow(bool @switch, bool notice = true)
        {
            DBHelper.Store("FloatSwitch", @switch);

            this.Dispatcher.Invoke(new Action(() =>
            {
                if (@switch)
                {
                    SDShell.OpenSDSuspender();

                    this.notifyIcon.ContextMenu.MenuItems["floatMenuItem"].Text = SDResources.MainWindow_NotifyIcon_FloatHide;

                    if (this.windowBrowserObject != null && notice)
                        this.windowBrowserObject.events.emit("FloatSwitchEvent", "true");
                }
                else
                {
                    SDShell.ExitSDSuspender();

                    this.notifyIcon.ContextMenu.MenuItems["floatMenuItem"].Text = SDResources.MainWindow_NotifyIcon_FloatDisplay;

                    if (this.windowBrowserObject != null && notice)
                        this.windowBrowserObject.events.emit("FloatSwitchEvent", "false");
                }
            }));
        }
        #endregion

        private void BeginProcessAdvices()
        {
            Task.Factory.StartNew(() =>
            {
                if (SDShell.ProcessAdvices == null)
                {
                    SDShell.ProcessAdvices = new ProcessMonitor(System.IO.Path.Combine(SDShell.RootPath, "ShanDianAdvicesHelper.exe"));
                    SDShell.ProcessAdvices.OnProcessMonitorReceived += ProcessAdvices_OnProcessMonitorReceived;
                    SDShell.ProcessAdvices.OnProcessMonitorExist += ProcessAdvices_OnProcessMonitorExist;
                    SDShell.ProcessAdvices.Start();
                }
            });
        }


        private void ProcessAdvices_OnProcessMonitorExist(object sender, EventArgs e)
        {
            if (!SDShell.IsExiting)
            {
                SDShell.ProcessAdvices.Start();
            }
        }

        private void ProcessAdvices_OnProcessMonitorReceived(object sender, ProcessMonitorReceivedEventArgs e)
        {
            DebugHelper.WriteLine(" ProcessAdvices:" + e.Message);

            if (e.Message.StartsWith("advicesmessage", StringComparison.CurrentCultureIgnoreCase))
            {
                if (this.chromiumWebBrowser != null)
                {
                    var script = @"(function () {
                                          window.frameEvent.messagePush(true);
                                        })();";
                    this.chromiumWebBrowser.ExecuteScriptAsync(script);

                }
            }
            else if (e.Message.StartsWith("redirectsettings", StringComparison.CurrentCultureIgnoreCase))
            {
                if (this.chromiumWebBrowser != null)
                {
                    var script = @"(function () {
                                          window.frameEvent.openMsgSetting();
                                        })();";
                    this.chromiumWebBrowser.ExecuteScriptAsync(script);

                }
            }
            else if (e.Message.StartsWith("redirectHistory", StringComparison.CurrentCultureIgnoreCase))
            {
                if (this.chromiumWebBrowser != null)
                {
                    var script = @"(function () {
                                          window.frameEvent.openMsgCenter();
                                        })();";
                    this.chromiumWebBrowser.ExecuteScriptAsync(script);
                }
            }
            else if (e.Message.StartsWith("redirectdetails", StringComparison.CurrentCultureIgnoreCase))
            {
                string[] words = e.Message.Split('|');

                if (words != null && words.Length > 1)
                {
                    string urlString = Encoding.UTF8.GetString(Convert.FromBase64String(words[1]));

                    Uri uri;

                    if (Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out uri))
                    {
                        string query = uri.Query;

                        if (string.IsNullOrWhiteSpace(query))
                        {
                            urlString = urlString + "?1=1";
                        }

                        string userValue = CDBHelper.Get("user_auth");

                        if (!string.IsNullOrWhiteSpace(userValue))
                        {
                            CoreUserInfo coreUserInfo = JsonConvert.DeserializeObject<CoreUserInfo>(userValue);

                            string userId = coreUserInfo.UserId.ToString();
                            string userName = coreUserInfo.UserName;
                            string phone = coreUserInfo.Mobile;
                            string currentTime = DateTime.Now.ToString("yyyyMMddHHmm");
                            string sign = Logistics.GetSign(userId, userName, phone, currentTime);

                            urlString = urlString + string.Format("&userId={0}&userName={1}&phone={2}&currentTime={3}&sign={4}", userId, userName, phone, currentTime, sign);
                        }

                        DebugHelper.WriteLine("UrlString:" + urlString);

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            WindowSimpleChromium windowBrowser = new WindowSimpleChromium(urlString);

                            windowBrowser.WindowStyle = WindowStyle.ToolWindow;
                            windowBrowser.ShowInTaskbar = false;
                            windowBrowser.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                            windowBrowser.Show();

                        }));
                    }
                }

            }

        }

        private void ProcessAdvices_OnReceived(string message)
        {
            DebugHelper.WriteLine(" ProcessAdvices:" + message);

            if (this.chromiumWebBrowser != null)
            {
                var script = @"(function () {
                                           window.frameEvent.openShortKey();
                                        })();";

                this.chromiumWebBrowser.ExecuteScriptAsync(script);
            }
        }


        /// <summary>
        /// 修改代理信息
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public void ChangeProxyInfo(string proxyType, string proxyServer, string proxyPort)
        {
            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                if (!string.IsNullOrWhiteSpace(proxyServer) && !string.IsNullOrWhiteSpace(proxyPort))
                {
                    var rc = this.chromiumWebBrowser.GetBrowser().GetHost().RequestContext;
                    Dictionary<string, object> v = new Dictionary<string, object>();
                    v["mode"] = "fixed_servers";
                    v["server"] = string.Format("{0}:{1}", proxyServer, proxyPort);
                    v["bypass_list"] = "127.*";
                    string error;
                    rc.SetPreference("proxy", v, out error);
                }
                else
                {
                    var rc = this.chromiumWebBrowser.GetBrowser().GetHost().RequestContext;
                    Dictionary<string, object> v = new Dictionary<string, object>();
                    v["mode"] = "direct";
                    v["server"] = "";
                    v["bypass_list"] = "127.*";
                    string error;
                    rc.SetPreference("proxy", v, out error);
                }
                //success=true,error=""
            });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!this.isClosed)
            {
                this.InvokeClose();
                e.Cancel = true;
            }
        }
    }


    /// <summary>
    /// 实现对前端的接口
    /// </summary>
    public partial class WindowChromium : IChromiumWindow
    {
        public string Identity { private set; get; }

        /// <summary>
        /// 窗口横坐标
        /// </summary>
        public double ClientOffsetLeft
        {
            get
            {
                double locationX = -1;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    locationX = this.PointToScreen(new System.Windows.Point(0, 0)).X;
                }));

                return locationX;
            }
        }


        /// <summary>
        /// 窗口纵坐标
        /// </summary>
        public double ClientOffsetTop
        {
            get
            {
                double locationY = -1;

                this.Dispatcher.Invoke(new Action(() =>
                {
                    locationY = this.PointToScreen(new System.Windows.Point(0, 0)).Y;
                }));

                return locationY;
            }
        }

        public void InvokeRedirect(string url)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                //this.Redirect(url);
            }));
        }

        /// <summary>
        /// UI线程关闭
        /// </summary>
        public virtual void InvokeClose()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.WindowState = WindowState.Minimized;
                this.Hide();
            }));
        }

        public virtual void InvokeQuit()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {

                SDShell.IsExiting = true;

                Dialog exitDialog = new Dialog();
                exitDialog.DialogText = SDResources.Dialog_Exit_Text;
                exitDialog.Owner = this;

                bool? dialogResult = exitDialog.ShowDialog();

                if (dialogResult != null && dialogResult.HasValue && dialogResult.Value)
                {
                    this.notifyIcon.Dispose();

                    this.Close(true);

                    SDShell.Exit();
                }
                else
                {
                    SDShell.IsExiting = false;
                }


            }));
        }

        /// <summary>
        /// UI线程执行最小化
        /// </summary>
        public virtual void InvokeMinimized()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.WindowState = WindowState.Minimized;
            }));
        }

        /// <summary>
        /// UI线程执行最大化
        /// </summary>
        public virtual void InvokeMaximized()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            }));
        }

        /// <summary>
        /// UI线程移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public virtual void InvokeMove(int width, int height, double left, double top)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Width = width;
                this.Height = height;
                this.Top = top;
                this.Left = left;
            }));
        }

        public virtual void InvokeOpenScreenKeyboard()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                SDShell.ShowVirtualKeyboard();
            }));
        }
    }
}
