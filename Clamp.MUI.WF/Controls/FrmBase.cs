using Chromium;
using Chromium.Event;
using Chromium.WebBrowser;
using Clamp.AppCenter;
using Clamp.AppCenter.Handlers;
using Clamp.MUI.WF.CFX;
using Clamp.MUI.WF.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI.WF.Controls
{
    public partial class FrmBase : Form, IClampHandlerFactory
    {
        private readonly List<ContextMenuItem> menuCommands = new List<ContextMenuItem>();
        private readonly List<int> menuSeparatorIndex = new List<int>();

        private IntPtr chromeWidgetHostHandle;
        private BrowserWidgetMessageInterceptor chromeWidgetMessageInterceptor;
        private Region draggableRegion = null;
        private IntPtr browserHandle;
        private Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);
        private IntPtr _DebugWindowHandle = IntPtr.Zero;

        private CfxLifeSpanHandler debugCfxLifeSpanHandler;
        private CfxClient debugCfxClient;
        private IntPtr formHandle;
        private FormGlowBorderDecorator shadowDecorator;
        private bool creatingHandle = false;
        private FormWindowState displayWindowState;

        protected bool IsDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        protected IntPtr BrowserHandle
        {
            get
            {
                return this.chromium.BrowserHost.WindowHandle;
            }
        }

        protected IntPtr FormHandle
        {
            get
            {
                return this.formHandle;
            }
        }

        public ChromiumWebBrowser ChromiumWebBrowser { get { return this.chromium; } }


        public FrmBase()
        {
            InitializeComponent();

            if (!IsDesignMode)
            {
                this.chromium.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
                this.chromium.LoadHandler.OnLoadError += LoadHandler_OnLoadError;
                this.chromium.RequestHandler.OnQuotaRequest += RequestHandler_OnQuotaRequest;
                this.chromium.ContextMenuHandler.OnBeforeContextMenu += OnBeforeContextMenu;
                this.chromium.ContextMenuHandler.OnContextMenuCommand += ContextMenuHandler_OnContextMenuCommand;

                var dragHandler = this.chromium.DragHandler;

                dragHandler.OnDragEnter += (o, e) => { e.SetReturnValue(true); };
                dragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;

                this.OnShadowEffectChanged();
            }
        }

        #region 公有方法
        public void RegisterContextMenuItem(IEnumerable<ContextMenuItem> contextMenuItens)
        {
            this.menuCommands.AddRange(contextMenuItens);
            this.menuSeparatorIndex.Insert(0, this.menuCommands.Count);
        }

        /// <summary>
        /// 打开开发工具
        /// </summary>
        /// <returns></returns>
        public bool OpenDebugTools()
        {
            if (_DebugWindowHandle != IntPtr.Zero)
            {
                NativeWindowHelper.BringToFront(_DebugWindowHandle);
                return true;
            }

            DisplayDebug();
            return true;
        }

        /// <summary>
        /// 关闭开发工具
        /// </summary>
        public void CloseDebugTools()
        {
            this.chromium.BrowserHost.CloseDevTools();
        }

        /// <summary>
        /// 加载指定的URL
        /// </summary>
        /// <param name="url"></param>
        public void LoadUrl(string url)
        {
            this.chromium.LoadUrl(url);
        }

        #endregion


        #region 私有方法

        #region 私有方法 开发工具

        /// <summary>
        /// 显示开发工具
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="style"></param>
        private void DisplayDebug(CfxGetLoadHandlerEventHandler handler = null, Chromium.WindowStyle style = Chromium.WindowStyle.WS_OVERLAPPEDWINDOW | Chromium.WindowStyle.WS_CLIPCHILDREN | Chromium.WindowStyle.WS_CLIPSIBLINGS | Chromium.WindowStyle.WS_VISIBLE)
        {
            var cfxWindowInfo = new CfxWindowInfo
            {
                Style = style,
                ParentWindow = IntPtr.Zero,
                WindowName = "开发工具",
                X = 200,
                Y = 200,
                Width = 800,
                Height = 600
            };

            debugCfxClient = new CfxClient();
            debugCfxClient.GetLifeSpanHandler += DebugClient_GetLifeSpanHandler;
            if (handler != null) debugCfxClient.GetLoadHandler += handler;
            this.chromium.BrowserHost.ShowDevTools(cfxWindowInfo, debugCfxClient, new CfxBrowserSettings(), null);
        }


        private void DebugClient_GetLifeSpanHandler(object sender, CfxGetLifeSpanHandlerEventArgs e)
        {
            if (debugCfxLifeSpanHandler == null)
            {
                debugCfxLifeSpanHandler = new CfxLifeSpanHandler();
                debugCfxLifeSpanHandler.OnAfterCreated += DebugLifeSpan_OnAfterCreated;
                debugCfxLifeSpanHandler.OnBeforeClose += DebugLifeSpan_OnBeforeClose;
            }
            e.SetReturnValue(debugCfxLifeSpanHandler);
        }

        private void DebugLifeSpan_OnBeforeClose(object sender, CfxOnBeforeCloseEventArgs e)
        {
            if (_DebugWindowHandle == IntPtr.Zero)
                return;

            _DebugWindowHandle = IntPtr.Zero;
            debugCfxClient = null;
        }

        private void DebugLifeSpan_OnAfterCreated(object sender, CfxOnAfterCreatedEventArgs e)
        {
            _DebugWindowHandle = e.Browser.Host.WindowHandle;
        }

        #endregion


        #region 私有方法  右击菜单

        private void OnShadowEffectChanged()
        {
            var isInitd = true;
            var isEnabled = true;

            if (shadowDecorator != null)
            {
                isInitd = shadowDecorator.IsInitialized;
                isEnabled = shadowDecorator.IsEnabled;

                shadowDecorator.Dispose();
            }

            shadowDecorator = new FormGlowBorderDecorator(this, isEnabled);

            shadowDecorator.InactiveColor = Color.Black;
            shadowDecorator.ActiveColor = Color.Black;

            if (isInitd)
            {
                shadowDecorator.InitializeShadows();
            }
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
            Thread thread = new Thread(() =>
            {
                this.browserHandle = e.Browser.Host.WindowHandle;

                new Resilient(() => ChromeWidgetHandleFinder.TryFindHandle(this.browserHandle, out this.chromeWidgetHostHandle)).WithTimeOut(100).StartIn(100);

                this.chromeWidgetMessageInterceptor = new BrowserWidgetMessageInterceptor(this.chromium, this.formHandle, this.chromeWidgetHostHandle, OnWebBroswerMessage);
            });

            thread.IsBackground = true;
            thread.Start();
        }

        private void OnBeforeContextMenu(object sender, CfxOnBeforeContextMenuEventArgs e)
        {
            var model = e.Model;

            for (var index = model.Count - 1; index >= 0; index--)
            {
                if (!CfxContextMenu.IsEdition(model.GetCommandIdAt(index)))
                    model.RemoveAt(index);
            }

            if (model.Count != 0)
                return;

            var rank = (int)ContextMenuId.MENU_ID_USER_FIRST;

            menuCommands.ForEach(command =>
            {
                model.AddItem(rank, command.Name);
                model.SetEnabled(rank++, command.Enabled);
            });

            menuSeparatorIndex.ForEach(index => model.InsertSeparatorAt(index));
        }

        private void ContextMenuHandler_OnContextMenuCommand(object sender, CfxOnContextMenuCommandEventArgs e)
        {
            if (!CfxContextMenu.IsUserDefined(e.CommandId))
                return;

            var command = menuCommands[e.CommandId - (int)ContextMenuId.MENU_ID_USER_FIRST].Command;
            command.Invoke();
        }

        #endregion


        private void UpdateDpiMatrix()
        {
            this.matrix = HdiHelper.GetDisplayScaleFactor(this.formHandle);
        }

        /// <summary>
        /// 加载出错事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadHandler_OnLoadError(object sender, CfxOnLoadErrorEventArgs e)
        {
            if (e.ErrorCode == CfxErrorCode.Aborted)
            {
                //Aborted is raised during hot-reload
                //We will not poluate log nor stop the application
                return;
            }

            if (!e.Frame.IsMain)
                return;
        }


        #region 消息处理

        /// <summary>
        /// 处理CEF传来的消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool OnWebBroswerMessage(ref Message message)
        {
            if (BrowserHandle != IntPtr.Zero)
            {
                var msg = (WindowsMessages)message.Msg;
                if (msg == WindowsMessages.WM_MOUSEMOVE)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var mode = GetSizeMode(pt);

                    if (mode != HitTest.HTCLIENT)
                    {
                        User32.ClientToScreen(this.formHandle, ref pt);
                        User32.PostMessage(this.formHandle, (uint)WindowsMessages.WM_NCHITTEST, IntPtr.Zero, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));
                        return true;
                    }
                }

                if (msg == WindowsMessages.WM_LBUTTONDOWN)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var dragable = (this.draggableRegion != null && this.draggableRegion.IsVisible(new Point(pt.x, pt.y)));

                    var mode = GetSizeMode(pt);
                    if (mode != HitTest.HTCLIENT)
                    {

                        User32.ClientToScreen(this.formHandle, ref pt);

                        User32.PostMessage(this.formHandle, (uint)WindowsMessages.WM_NCLBUTTONDOWN, (IntPtr)mode, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));

                        return true;

                    }
                    else if (dragable && !(FormBorderStyle == FormBorderStyle.None && WindowState == FormWindowState.Maximized))
                    {
                        this.chromium.Browser.Host.NotifyMoveOrResizeStarted();

                        User32.PostMessage(this.formHandle, (uint)DefMessages.WM_NANUI_DRAG_APP_REGION, IntPtr.Zero, IntPtr.Zero);

                        return true;

                    }
                }

                if (msg == WindowsMessages.WM_LBUTTONDBLCLK)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var dragable = (this.draggableRegion != null && this.draggableRegion.IsVisible(new Point(pt.x, pt.y)));
                    if (dragable)
                    {
                        User32.SendMessage(this.formHandle, (uint)WindowsMessages.WM_NCLBUTTONDBLCLK, (IntPtr)HitTest.HTCAPTION, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));
                        return true;
                    }
                }

                if (msg == WindowsMessages.WM_RBUTTONDOWN)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var dragable = (this.draggableRegion != null && this.draggableRegion.IsVisible(new Point(pt.x, pt.y)));
                    if (dragable)
                    {

                        User32.SendMessage(this.formHandle, (uint)DefMessages.WM_NANUI_APP_REGION_RBUTTONDOWN, IntPtr.Zero, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));
                        return true;
                    }
                }
            }
            return false;
        }



        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == (int)DefMessages.WM_NANUI_DRAG_APP_REGION)
            {

                User32.ReleaseCapture();
                User32.SendMessage(Handle, (uint)WindowsMessages.WM_NCLBUTTONDOWN, (IntPtr)HitTest.HTCAPTION, (IntPtr)0);
            }

            if (m.Msg == (int)DefMessages.WM_NANUI_APP_REGION_RBUTTONDOWN)
            {
                var pt = Win32.GetPostionFromPtr(m.LParam);

                var ptToScr = PointToScreen(new Point(pt.x, pt.y));

                ShowSystemMenu(this, ptToScr);
            }

            base.DefWndProc(ref m);
        }

        private bool ShowSystemMenu(Form frm, Point pt)
        {
            const int TPM_LEFTALIGN = 0x0000, TPM_TOPALIGN = 0x0000, TPM_RETURNCMD = 0x0100;
            if (frm == null)
                return false;
            IntPtr menuHandle = GetSystemMenu(frm.Handle, false);
            IntPtr command = User32.TrackPopupMenu(menuHandle, TPM_RETURNCMD | TPM_TOPALIGN | TPM_LEFTALIGN, pt.X, pt.Y, 0, frm.Handle, IntPtr.Zero);
            if (frm.IsDisposed)
                return false;
            User32.PostMessage(frm.Handle, (uint)WindowsMessages.WM_SYSCOMMAND, command, IntPtr.Zero);
            return true;
        }

        private HitTest GetSizeMode(POINT point)
        {
            HitTest mode = HitTest.HTCLIENT;

            int x = point.x, y = point.y;

            var CornerAreaSize = Win32.CornerAreaSize;

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


        [DllImport("USER32.dll")]
        private static extern bool DestroyMenu(IntPtr menu);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private static extern IntPtr GetSystemMenuCore(IntPtr hWnd, bool bRevert);
        [System.Security.SecuritySafeCritical]
        private static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert)
        {
            return GetSystemMenuCore(hWnd, bRevert);
        }

        #endregion

        #endregion

        #region 重写方法

        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.displayWindowState = this.WindowState;
            this.formHandle = this.Handle;

            this.UpdateDpiMatrix();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);

            this.UpdateDpiMatrix();
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (this.WindowState != this.displayWindowState)
            {
                this.displayWindowState = this.WindowState;

                if (this.WindowState == FormWindowState.Minimized)
                    return;

                Thread.Sleep(10);

                this.chromium.Refresh();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.chromeWidgetMessageInterceptor?.ReleaseHandle();
            this.chromeWidgetMessageInterceptor?.DestroyHandle();
            this.chromeWidgetMessageInterceptor = null;

            this.chromium.Dispose();
        }

        #endregion

        public virtual IClampHandler GetClampHandler()
        {
            return null;
        }

    }
}
