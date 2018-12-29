using Chromium.Event;
using Clamp.AppCenter;
using Clamp.MUI.WPF.CFX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using Forms = System.Windows.Forms;

namespace Clamp.MUI.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr windowHandle;
        private IntPtr chromeWidgetHostHandle;
        private BrowserWidgetMessageInterceptor chromeWidgetMessageInterceptor;
        private Region draggableRegion = null;
        private IntPtr browserHandle;
        private Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += Window_Loaded;

            this.ChromiumWebBrowser.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
            this.ChromiumWebBrowser.RequestHandler.OnQuotaRequest += RequestHandler_OnQuotaRequest;

            var dragHandler = this.ChromiumWebBrowser.DragHandler;

            dragHandler.OnDragEnter += (o, e) => { e.SetReturnValue(true); };
            dragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;
        }

        private void RequestHandler_OnQuotaRequest(object sender, CfxOnQuotaRequestEventArgs e)
        {
            e.SetReturnValue(true);
        }

        private void DragHandler_OnDraggableRegionsChanged(object sender, Chromium.Event.CfxOnDraggableRegionsChangedEventArgs args)
        {
            this.draggableRegion = args.Regions.Aggregate(new Region(), (current, region) =>
            {
                var rect = new Rectangle(region.Bounds.X, region.Bounds.Y, region.Bounds.Width, region.Bounds.Height);

                if (region.Draggable)
                    current.Union(rect);
                else
                    current.Exclude(rect);

                return current;
            });

            this.draggableRegion.Transform(this.matrix);
        }

        private void ChromiumWebBrowser_BrowserCreated(object sender, Chromium.WebBrowser.Event.BrowserCreatedEventArgs e)
        {
            this.browserHandle = e.Browser.Host.WindowHandle;

            new Resilient(() => ChromeWidgetHandleFinder.TryFindHandle(this.browserHandle, out this.chromeWidgetHostHandle)).WithTimeOut(100).StartIn(100);

            this.chromeWidgetMessageInterceptor = new BrowserWidgetMessageInterceptor(this.ChromiumWebBrowser, this.chromeWidgetHostHandle, OnWebBroswerMessage);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Window_Loaded;

            if (AppManager.Current.ClampConfs.ContainsKey(AppCenterConstant.CFX_INIT_URL))
            {
                this.ChromiumWebBrowser.LoadUrl(AppManager.Current.ClampConfs[AppCenterConstant.CFX_INIT_URL]);
            }
            else
            {
                this.ChromiumWebBrowser.LoadUrl("about:blank");
            }

            this.windowHandle = new WindowInteropHelper(this).Handle;
            this.Closed += Window_Closed;
            this.LocationChanged += Window_LocationChanged;
            this.StateChanged += Window_StateChanged;
            this.UpdateDpiMatrix();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
                return;

            Thread.Sleep(10);

            this.ChromiumWebBrowser.Refresh();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            this.UpdateDpiMatrix();
        }

        private void UpdateDpiMatrix()
        {
            this.matrix = HdiHelper.GetDisplayScaleFactor(windowHandle);
        }

        private bool OnWebBroswerMessage(Forms.Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WindowsMessage.WM_LBUTTONDBLCLK:
                    if (!IsInDragRegion(message))
                        break;

                    this.Dispatcher.BeginInvoke(new Action(ToogleMaximize));
                    return true;

                case NativeMethods.WindowsMessage.WM_LBUTTONDOWN:
                    if (!IsInDragRegion(message))
                        return false;

                    NativeMethods.ReleaseCapture();
                    NativeMethods.PostMessage(windowHandle, NativeMethods.WindowsMessage.WM_NCLBUTTONDOWN, (IntPtr)NativeMethods.HitTest.HTCAPTION, IntPtr.Zero);
                    return true;
            }
            return false;
        }

        private bool IsInDragRegion(Forms.Message message)
        {
            if (this.draggableRegion == null)
                return false;

            var point = GetPoint(message);
            return this.draggableRegion.IsVisible(point);
        }

        private static System.Drawing.Point GetPoint(Forms.Message message)
        {
            var lparam = message.LParam;
            var x = NativeMethods.LoWord(lparam.ToInt32());
            var y = NativeMethods.HiWord(lparam.ToInt32());
            return new System.Drawing.Point(x, y);
        }

        private void ToogleMaximize()
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.Closed -= Window_Closed;
            this.LocationChanged -= Window_LocationChanged;
            this.StateChanged -= Window_StateChanged;
            this.chromeWidgetMessageInterceptor?.ReleaseHandle();
            this.chromeWidgetMessageInterceptor?.DestroyHandle();
            this.chromeWidgetMessageInterceptor = null;
        }

    }
}
