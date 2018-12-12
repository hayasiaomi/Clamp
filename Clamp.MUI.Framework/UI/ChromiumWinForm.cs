using Chromium;
using Chromium.Event;
using Chromium.Remote;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Clamp.MUI.Framework.UI;
using Clamp.MUI.Framework.UI.Imports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Chromium.Remote.CfrRuntime;

namespace Clamp.MUI.Framework.UI
{
    public class ChromiumWinForm : Form
    {
        private ChromiumWebBrowser browser;
        private Region draggableRegion = null;
        private BrowserWidgetMessageInterceptor messageInterceptor;
        private float scaleFactor = 1.0f;
        private int borderSize = 1;
        private Color borderColor = Color.Gray;
        private ChromiumShadowDecorator chromiumShadowDecorator;

        protected readonly bool IsDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        protected IntPtr FormHandle { get; private set; }
        protected IntPtr BrowserHandle { get; private set; }

        public string Identity { private set; get; }

        public string Url { private set; get; }

        public CfrV8Context V8Context { private set; get; }

        public ChromiumWebBrowser ChromiumWebBrowser { get { return this.browser; } }


        public bool IsLoading { get { return this.browser.IsLoading; } }

        public bool CanGoBack { get { return this.browser.CanGoBack; } }

        public bool CanGoForward { get { return this.browser.CanGoForward; } }

        public double FrmOpacity
        {
            set
            {
                this.Opacity = value;

                if (this.OnFrmOpacityChanged != null)
                {
                    this.OnFrmOpacityChanged(this, new EventArgs());
                }
            }
            get { return this.Opacity; }
        }


        private int CornerAreaSize
        {
            get
            {
                return borderSize < 3 ? 3 : borderSize;
            }
        }

        public bool Resizable { set; get; }

        public event Action<ChromiumWinForm, ChromiumWebBrowser> OnChromiumLoadCompleted;
        public event Action<ChromiumWinForm, EventArgs> OnFrmOpacityChanged;

        public ChromiumWinForm() : this(null)
        {

        }

        public ChromiumWinForm(string initialUrl)
        {
            if (!this.IsDesignMode)
            {


                this.Identity = Guid.NewGuid().ToString("N");
                this.scaleFactor = 1.0f / User32.GetOriginalDeviceScaleFactor(FormHandle);
                this.Url = initialUrl;

                if (string.IsNullOrWhiteSpace(initialUrl))
                {
                    this.browser = new ChromiumWebBrowser();
                }
                else
                {
                    this.browser = new ChromiumWebBrowser(initialUrl);
                }

                this.browser.Dock = DockStyle.Fill;
                this.browser.RemoteCallbackInvokeMode = JSInvokeMode.Inherit;

                this.Resizable = true;

                this.Controls.Add(this.browser);

                this.BrowserHandle = this.browser.Handle;

                //this.browser.BrowserCreated += this.AttachInterceptorToChromiumBrowser;
                this.browser.KeyboardHandler.OnPreKeyEvent += KeyboardHandler_OnPreKeyEvent;
                this.browser.DragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;
                this.browser.DragHandler.OnDragEnter += DragHandler_OnDragEnter;
                this.browser.LoadHandler.OnLoadEnd += LoadHandler_OnLoadEnd;
                this.browser.LoadHandler.OnLoadError += LoadHandler_OnLoadError;
                this.browser.RequestHandler.OnBeforeResourceLoad += RequestHandler_OnBeforeResourceLoad;
                this.browser.ContextMenuHandler.OnBeforeContextMenu += ContextMenuHandler_OnBeforeContextMenu;
                this.browser.ContextMenuHandler.OnContextMenuCommand += ContextMenuHandler_OnContextMenuCommand;
                this.browser.OnV8ContextCreated += Browser_OnV8ContextCreated;
            }
        }

        private void Browser_OnV8ContextCreated(object sender, Chromium.Remote.Event.CfrOnContextCreatedEventArgs e)
        {
            this.V8Context = e.Context;
        }

        private void RequestHandler_OnBeforeResourceLoad(object sender, CfxOnBeforeResourceLoadEventArgs e)
        {
            Uri callUri;

            if (Uri.TryCreate(e.Request.Url, UriKind.Absolute, out callUri))
            {
                if (callUri.DnsSafeHost.EndsWith("SD-proxy.chidaoni.com", StringComparison.OrdinalIgnoreCase))
                {

                    Dictionary<string, string> acHeaders = new Dictionary<string, string>();

                    acHeaders.Add("Access-Control-Allow-Origin", "*");
                    acHeaders.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS，PUT,DELETE");
                    acHeaders.Add("Access-Control-Max-Age", "30000");
                    acHeaders.Add("Access-Control-Allow-Headers", "*");


                    List<string[]> headerMaps = e.Request.GetHeaderMap();

                    foreach (string keyName in acHeaders.Keys)
                    {
                        if (headerMaps != null && headerMaps.Count > 0)
                        {
                            string[] header = headerMaps.FirstOrDefault(headers =>
                            {
                                if (headers.Length > 0)
                                    return String.Compare(headers[0], keyName, true) == 0;
                                return false;
                            });

                            if (header != null)
                            {
                                if (header.Length > 1)
                                {
                                    header[1] = acHeaders[keyName];
                                }
                            }
                            else
                            {
                                header = new string[2];
                                header[0] = keyName;
                                header[1] = acHeaders[keyName];

                                headerMaps.Add(header);
                            }
                        }
                    }


                    e.Request.SetHeaderMap(headerMaps);

                    e.Request.Url = $"{callUri.Scheme}://127.0.0.1:31234{callUri.PathAndQuery}";

                    Console.WriteLine(e.Request.Method + ":" + e.Request.Url);
                }

                e.Callback.Continue(true);
            }
            else
            {
                Console.WriteLine("无效URL :" + e.Request.Url);

                //e.Callback.Continue(false);
            }
        }

        private void ChromiumForm_Execute(object sender, Chromium.Remote.Event.CfrV8HandlerExecuteEventArgs e)
        {
            Debug.WriteLine("Aomi:" + e.Arguments != null);

            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                Debug.WriteLine("AomiValue:" + e.Arguments[0].StringValue);
            }
        }



        #region Private




        private void ContextMenuHandler_OnBeforeContextMenu(object sender, CfxOnBeforeContextMenuEventArgs e)
        {
            e.Model.Clear();
            e.Model.AddItem(1000, "开发工具");
        }

        private void ContextMenuHandler_OnContextMenuCommand(object sender, CfxOnContextMenuCommandEventArgs e)
        {
            if (e.CommandId == 1000)
            {
                this.ShowDebugTools();
            }
        }

        private void KeyboardHandler_OnPreKeyEvent(object sender, CfxOnPreKeyEventEventArgs e)
        {
            if (e.Event.IsSystemKey)
                e.SetReturnValue(true);
        }


        private void DragHandler_OnDragEnter(object sender, CfxOnDragEnterEventArgs e)
        {
            e.SetReturnValue(true);
        }

        private void DragHandler_OnDraggableRegionsChanged(object sender, CfxOnDraggableRegionsChangedEventArgs e)
        {
            var regions = e.Regions;

            if (regions.Length > 0)
            {
                foreach (var region in regions)
                {
                    var rect = new Rectangle(region.Bounds.X, region.Bounds.Y, region.Bounds.Width, region.Bounds.Height);

                    if (this.draggableRegion == null)
                    {
                        this.draggableRegion = new Region(rect);
                    }
                    else
                    {
                        if (region.Draggable)
                        {
                            this.draggableRegion.Union(rect);
                        }
                        else
                        {
                            this.draggableRegion.Exclude(rect);
                        }
                    }
                }



            }

            //if (this.draggableRegion != null)
            //{
            //    this.browser.mo
            //}
        }


        private void LoadHandler_OnLoadError(object sender, Chromium.Event.CfxOnLoadErrorEventArgs e)
        {

        }

        private void LoadHandler_OnLoadEnd(object sender, Chromium.Event.CfxOnLoadEndEventArgs e)
        {
            if (this.messageInterceptor != null)
            {
                this.messageInterceptor.ReleaseHandle();
                this.messageInterceptor.DestroyHandle();
            }

            this.AttachInterceptorToChromiumBrowser(sender, new BrowserCreatedEventArgs(e.Browser));

            this.Invoke(new Action(() =>
            {
                Debug.WriteLine(this.browser.Handle);
            }));

            if (e.HttpStatusCode != 0)
            {
                if (this.OnChromiumLoadCompleted != null)
                    this.OnChromiumLoadCompleted(this, this.browser);
            }


        }

        private void AttachInterceptorToChromiumBrowser(object sender, BrowserCreatedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    while (true)
                    {
                        IntPtr chromeWidgetHostHandle = IntPtr.Zero;

                        if (BrowserWidgetHandleFinder.TryFindHandle(this.BrowserHandle, out chromeWidgetHostHandle))
                        {
                            messageInterceptor = new BrowserWidgetMessageInterceptor(browser, chromeWidgetHostHandle, OnWebBroswerMessage);
                            break;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
                catch
                {

                }
            });
        }

        private void SetCursor(HitTest mode)
        {
            IntPtr handle = IntPtr.Zero;

            switch (mode)
            {
                case HitTest.HTTOP:
                case HitTest.HTBOTTOM:
                    handle = User32.LoadCursor(IntPtr.Zero, (int)IdcStandardCursors.IDC_SIZENS);
                    break;
                case HitTest.HTLEFT:
                case HitTest.HTRIGHT:
                    handle = User32.LoadCursor(IntPtr.Zero, (int)IdcStandardCursors.IDC_SIZEWE);
                    break;
                case HitTest.HTTOPLEFT:
                case HitTest.HTBOTTOMRIGHT:
                    handle = User32.LoadCursor(IntPtr.Zero, (int)IdcStandardCursors.IDC_SIZENWSE);
                    break;
                case HitTest.HTTOPRIGHT:
                case HitTest.HTBOTTOMLEFT:
                    handle = User32.LoadCursor(IntPtr.Zero, (int)IdcStandardCursors.IDC_SIZENESW);
                    break;
            }

            if (handle != IntPtr.Zero)
            {
                User32.SetCursor(handle);
            }
        }

        #endregion

        #region Protected

        protected virtual bool OnWebBroswerMessage(Message message)
        {
            if (message.Msg == (int)WindowsMessages.WM_MOUSEACTIVATE)
            {
                var topLevelWindowHandle = message.WParam;

                User32.PostMessage(topLevelWindowHandle, (int)WindowsMessages.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
                User32.SendMessage(topLevelWindowHandle, (int)WindowsMessages.WM_NCLBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            }

            if (message.Msg == (int)WindowsMessages.WM_LBUTTONDOWN)
            {
                var x = (int)User32.LoWord(message.LParam);
                var y = (int)User32.HiWord(message.LParam);

                var sx = (int)((int)User32.LoWord(message.LParam) * scaleFactor);
                var sy = (int)((int)User32.HiWord(message.LParam) * scaleFactor);

                var ax = x;
                var ay = y;

                if (scaleFactor != 1.0f)
                {
                    ax = sx;
                    ay = sy;
                }


                var dragable = (draggableRegion != null && draggableRegion.IsVisible(new Point(sx, sy)));

                var dir = GetSizeMode(new POINT(x, y));

                //if (dir != HitTest.HTCLIENT/* && BorderSize == 0*/)
                //{
                //    User32.PostMessage(FormHandle, (uint)DefMessages.WM_CEF_RESIZE_CLIENT, (IntPtr)dir, message.LParam);
                //    return true;
                //}
                //else 
                if (dragable)
                {
                    User32.PostMessage(FormHandle, (uint)DefMessages.WM_CEF_DRAG_APP, message.WParam, message.LParam);
                    return true;
                }
            }

            if (message.Msg == (int)WindowsMessages.WM_LBUTTONDBLCLK && Resizable)
            {
                var x = (int)User32.LoWord(message.LParam);
                var y = (int)User32.HiWord(message.LParam);

                var sx = (int)((int)User32.LoWord(message.LParam) * scaleFactor);
                var sy = (int)((int)User32.HiWord(message.LParam) * scaleFactor);

                var ax = x;
                var ay = y;

                if (scaleFactor != 1.0f)
                {
                    ax = sx;
                    ay = sy;
                }

                var dragable = (draggableRegion != null && draggableRegion.IsVisible(new Point(sx, sy)));

                if (dragable)
                {
                    User32.PostMessage(FormHandle, (uint)DefMessages.WM_CEF_TITLEBAR_LBUTTONDBCLICK, message.WParam, message.LParam);

                    return true;
                }

            }

            if (message.Msg == (int)WindowsMessages.WM_MOUSEMOVE/* &&  BorderSize == 0*/)
            {

                //var x = (int)User32.LoWord(message.LParam);
                //var y = (int)User32.HiWord(message.LParam);

                //var sx = (int)((int)User32.LoWord(message.LParam) * scaleFactor);
                //var sy = (int)((int)User32.HiWord(message.LParam) * scaleFactor);

                //var ax = x;
                //var ay = y;

                //if (scaleFactor != 1.0f)
                //{
                //    ax = sx;
                //    ay = sy;
                //}


                //    var dragable = (draggableRegion != null && draggableRegion.IsVisible(new Point(sx, sy)));

                //    //Debug.WriteLine($"x:{x}\ty:{y}\t|\tax:{ax}\tay:{ay}");

                //    if (Resizable)
                //    {
                //        var dir = GetSizeMode(new POINT(x, y));


                //        if (dir != HitTest.HTCLIENT)
                //        {
                //            User32.PostMessage(FormHandle, (uint)DefMessages.WM_CEF_EDGE_MOVE, (IntPtr)dir, message.LParam);
                //            return true;
                //        }

                //    }

                User32.SendMessage(FormHandle, (uint)WindowsMessages.WM_MOUSEMOVE, message.WParam, message.LParam);

            }


            return false;

        }

        private HitTest GetSizeMode(POINT point)
        {
            HitTest mode = HitTest.HTCLIENT;

            int x = point.x, y = point.y;

            if (WindowState == FormWindowState.Normal)
            {
                if (x < CornerAreaSize & y < CornerAreaSize)
                {
                    mode = HitTest.HTTOPLEFT;
                }
                else if (x < CornerAreaSize & y + CornerAreaSize > this.Height - CornerAreaSize)
                {
                    mode = HitTest.HTBOTTOMLEFT;

                }
                else if (x + CornerAreaSize > this.Width - CornerAreaSize & y + CornerAreaSize > this.Height - CornerAreaSize)
                {
                    mode = HitTest.HTBOTTOMRIGHT;

                }
                else if (x + CornerAreaSize > this.Width - CornerAreaSize & y < CornerAreaSize)
                {
                    mode = HitTest.HTTOPRIGHT;

                }
                else if (x < CornerAreaSize)
                {
                    mode = HitTest.HTLEFT;

                }
                else if (x + CornerAreaSize > this.Width - CornerAreaSize)
                {
                    mode = HitTest.HTRIGHT;

                }
                else if (y < CornerAreaSize)
                {
                    mode = HitTest.HTTOP;

                }
                else if (y + CornerAreaSize > this.Height - CornerAreaSize)
                {
                    mode = HitTest.HTBOTTOM;

                }

            }
            return mode;
        }
        #endregion

        #region Override

        private const int WS_MINIMIZEBOX = 0x00020000;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            FormHandle = this.Handle;
            base.OnHandleCreated(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.chromiumShadowDecorator = new ChromiumShadowDecorator(this);
            this.chromiumShadowDecorator.RefreshShadow();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.messageInterceptor?.ReleaseHandle();
            this.messageInterceptor?.DestroyHandle();
            this.messageInterceptor = null;

            //this.browser.Dispose();
            this.chromiumShadowDecorator.Close();

            base.OnClosed(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (!IsDesignMode)
            {
                switch (m.Msg)
                {
                    case (int)WindowsMessages.WM_SHOWWINDOW:
                        {
                            //if (StartPosition == FormStartPosition.CenterParent && Owner != null)
                            //{
                            //    Location = new Point(Owner.Location.X + Owner.Width / 2 - Width / 2,
                            //    Owner.Location.Y + Owner.Height / 2 - Height / 2);


                            //}
                            //else if (StartPosition == FormStartPosition.CenterScreen || (StartPosition == FormStartPosition.CenterParent && Owner == null))
                            //{
                            //    var currentScreen = Screen.FromHandle(this.Handle);
                            //    Location = new Point(currentScreen.WorkingArea.Left + (currentScreen.WorkingArea.Width / 2 - this.Width / 2), currentScreen.WorkingArea.Top + (currentScreen.WorkingArea.Height / 2 - this.Height / 2));

                            //}

                            Activate();
                            BringToFront();

                            base.WndProc(ref m);
                        }
                        break;
                    case (int)WindowsMessages.WM_MOVE:
                        {

                            browser?.BrowserHost?.NotifyScreenInfoChanged();


                            base.WndProc(ref m);
                        }
                        break;
                    default:
                        {
                            base.WndProc(ref m);
                        }
                        break;
                }

            }
            else
            {
                base.WndProc(ref m);
            }



        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == (int)DefMessages.WM_CEF_TITLEBAR_LBUTTONDBCLICK)
            {
                User32.ReleaseCapture();

                if (WindowState == FormWindowState.Maximized)
                {
                    User32.SendMessage(FormHandle, (uint)WindowsMessages.WM_SYSCOMMAND, (IntPtr)SystemCommandFlags.SC_RESTORE, IntPtr.Zero);
                }
                else
                {
                    User32.SendMessage(FormHandle, (uint)WindowsMessages.WM_SYSCOMMAND, (IntPtr)SystemCommandFlags.SC_MAXIMIZE, IntPtr.Zero);
                }
            }

            if (m.Msg == (int)DefMessages.WM_CEF_DRAG_APP && !(FormBorderStyle == FormBorderStyle.None && WindowState == FormWindowState.Maximized))
            {
                User32.ReleaseCapture();
                User32.SendMessage(Handle, (uint)WindowsMessages.WM_NCLBUTTONDOWN, (IntPtr)HitTest.HTCAPTION, (IntPtr)0);
            }
            if (m.Msg == (int)DefMessages.WM_CEF_RESIZE_CLIENT && Resizable && WindowState == FormWindowState.Normal)
            {
                User32.ReleaseCapture();

                SetCursor((HitTest)m.WParam.ToInt32());

                User32.SendMessage(Handle, (int)WindowsMessages.WM_NCLBUTTONDOWN, m.WParam, (IntPtr)0);
            }

            if (m.Msg == (int)DefMessages.WM_CEF_EDGE_MOVE && Resizable && WindowState == FormWindowState.Normal)
            {
                SetCursor((HitTest)m.WParam.ToInt32());
            }


            base.DefWndProc(ref m);
        }
        #endregion

        #region Public

        public bool ExecuteJavascript(string code)
        {
            return this.browser.ExecuteJavascript(code);
        }

        public void GoForward()
        {
            this.browser.GoForward();
        }

        public void LoadUrl(string url)
        {
            this.browser.LoadUrl(url);
        }

        public void LoadString(string stringVal, string url)
        {
            this.browser.LoadString(stringVal, url);
        }

        public void LoadString(string stringVal)
        {
            this.browser.LoadString(stringVal);
        }

        public void ShowDebugTools()
        {
            var windowInfo = new CfxWindowInfo
            {
                Style = Chromium.WindowStyle.WS_OVERLAPPEDWINDOW | Chromium.WindowStyle.WS_CLIPCHILDREN | Chromium.WindowStyle.WS_CLIPSIBLINGS | Chromium.WindowStyle.WS_VISIBLE,
                ParentWindow = IntPtr.Zero,
                WindowName = "Dev Tools",
                X = 200,
                Y = 200,
                Width = 800,
                Height = 600
            };

            this.browser.BrowserHost.ShowDevTools(windowInfo, new CfxClient(), new CfxBrowserSettings(), null);
        }

        public void CloseDebugTools()
        {
            this.browser.BrowserHost.CloseDevTools();
        }


        #endregion

    }
}
