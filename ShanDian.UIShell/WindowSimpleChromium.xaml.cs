using CefSharp;
using CefSharp.Wpf;
using ShanDian.UIShell.Brower;
using ShanDian.UIShell.Properties;
using ShanDian.UIShell.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShanDian.UIShell
{
    /// <summary>
    /// ChromiumSimple.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSimpleChromium : Window
    {
        private ChromiumWebBrowser chromiumWebBrowser;
        private Region region;
        private WindowBrowserObject windowBrowserObject;
        private DispatcherTimer dispatcherTimer;

        public string Url { set; get; }

        public bool IsMain { set; get; }

        public WindowBrowserObject WindowBrowserObject { get { return windowBrowserObject; } }

        public ChromiumWebBrowser ChromiumWebBrowser { get { return this.chromiumWebBrowser; } }

        public event Action<ChromiumWebBrowser> OnChromiumViewLoadEnd;
        public event EventHandler<FrameLoadStartEventArgs> OnChromiumViewLoadStart;

        public WindowSimpleChromium(string url)
        {
            InitializeComponent();

            this.Url = url;
            this.windowBrowserObject = new WindowBrowserObject(this);
            this.chromiumWebBrowser = new ChromiumWebBrowser();
            this.chromiumWebBrowser.RegisterJsObject("SD", this.windowBrowserObject, false);
            this.chromiumWebBrowser.Address = url;
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
                //var errorBody = string.Format("<html><body bgcolor=\"white\"> <div style=\"margin: 0 auto; \"><h2>Failed to load URL {0} with error {1} ({2}).</div></h2></body></html>", args.FailedUrl, args.ErrorText, args.ErrorCode);

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
                                browserHost.SendKeyEvent((int)WM.CHAR, (int)character, 0);
                            }
                        }
                    }
                }

                e.Handled = true;
            };

            this.chromiumWebBrowser.AllowDrop = false;
            this.chromiumWebBrowser.FrameLoadEnd += Browser_FrameLoadEnd;
            this.chromiumWebBrowser.FrameLoadStart += Browser_FrameLoadStart;

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
        /// 网页加载前的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            if (this.OnChromiumViewLoadStart != null)
                this.OnChromiumViewLoadStart(this, e);

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

            if (this.OnChromiumViewLoadEnd != null)
                this.OnChromiumViewLoadEnd(this.chromiumWebBrowser);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            SDShell.AddChromium(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                SDShell.RemoveChromium(this.Url);
            }
        }
    }

    /// <summary>
    /// 实现对前端的接口
    /// </summary>
    public partial class WindowSimpleChromium : IChromiumWindow
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
                this.chromiumWebBrowser.Address = url;
            }));
        }



        /// <summary>
        /// UI线程关闭
        /// </summary>
        public virtual void InvokeClose()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
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

        public void InvokeQuit()
        {
            this.InvokeClose();
        }
    }
}
