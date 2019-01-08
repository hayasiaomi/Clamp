using Chromium;
using Chromium.Event;
using Clamp.AppCenter;
using Clamp.MUI.WF.CFX;
using Clamp.MUI.WF.Controls;
using Clamp.MUI.WF.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI.WF
{
    public partial class FrmMain : Form
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
        private IntPtr windowHandle;
        private FormGlowBorderDecorator shadowDecorator;
        private bool creatingHandle = false;
        public bool CanResize { get { return true; } }

        protected IntPtr BrowserHandle
        {
            get
            {
                return this.ChromiumWebBrowser.BrowserHost.WindowHandle;
            }
        }

        public FrmMain()
        {
            InitializeComponent();

            this.menuCommands.Add(new ContextMenuItem(() =>
            {
                this.OpenDebugTools();

            }, "开发工具"));

            this.Load += FrmMain_Load;

            this.ChromiumWebBrowser.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
            this.ChromiumWebBrowser.LoadHandler.OnLoadError += LoadHandler_OnLoadError;
            this.ChromiumWebBrowser.RequestHandler.OnQuotaRequest += RequestHandler_OnQuotaRequest;
            this.ChromiumWebBrowser.ContextMenuHandler.OnBeforeContextMenu += OnBeforeContextMenu;
            this.ChromiumWebBrowser.ContextMenuHandler.OnContextMenuCommand += ContextMenuHandler_OnContextMenuCommand;

            var dragHandler = this.ChromiumWebBrowser.DragHandler;

            dragHandler.OnDragEnter += (o, e) => { e.SetReturnValue(true); };
            dragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;

            this.OnShadowEffectChanged();
        }




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
            this.ChromiumWebBrowser.BrowserHost.CloseDevTools();
        }


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
            this.ChromiumWebBrowser.BrowserHost.ShowDevTools(cfxWindowInfo, debugCfxClient, new CfxBrowserSettings(), null);
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

                this.chromeWidgetMessageInterceptor = new BrowserWidgetMessageInterceptor(this.ChromiumWebBrowser, this.windowHandle, this.chromeWidgetHostHandle, OnWebBroswerMessage);
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
        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Load -= FrmMain_Load;

            this.windowHandle = this.Handle;

            if (AppManager.Current.ClampConfs.ContainsKey(AppCenterConstant.CFX_INIT_URL))
            {
                this.ChromiumWebBrowser.LoadUrl(AppManager.Current.ClampConfs[AppCenterConstant.CFX_INIT_URL]);
            }
            else
            {
                this.ChromiumWebBrowser.LoadUrl("about:blank");
            }

            this.Closed += FrmMain_Closed;
            this.LocationChanged += Window_LocationChanged;
            this.ClientSizeChanged += Window_StateChanged;
            this.UpdateDpiMatrix();
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
        /// <summary>
        /// Window 状态发生变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
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
            this.matrix = HdiHelper.GetDisplayScaleFactor(this.windowHandle);
        }

        private bool OnWebBroswerMessage(ref Message message)
        {
            if (BrowserHandle != IntPtr.Zero)
            {
                var msg = (WindowsMessages)message.Msg;
                if (CanResize && msg == WindowsMessages.WM_MOUSEMOVE)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var mode = GetSizeMode(pt);

                    if (mode != HitTest.HTCLIENT)
                    {
                        User32.ClientToScreen(this.windowHandle, ref pt);
                        User32.PostMessage(this.windowHandle, (uint)WindowsMessages.WM_NCHITTEST, IntPtr.Zero, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));
                        return true;
                    }
                }

                if (msg == WindowsMessages.WM_LBUTTONDOWN)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var dragable = (this.draggableRegion != null && this.draggableRegion.IsVisible(new Point(pt.x, pt.y)));

                    var mode = GetSizeMode(pt);
                    if (CanResize && mode != HitTest.HTCLIENT)
                    {

                        User32.ClientToScreen(this.windowHandle, ref pt);

                        User32.PostMessage(this.windowHandle, (uint)WindowsMessages.WM_NCLBUTTONDOWN, (IntPtr)mode, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));

                        return true;

                    }
                    else if (dragable && !(FormBorderStyle == FormBorderStyle.None && WindowState == FormWindowState.Maximized))
                    {
                        this.ChromiumWebBrowser.Browser.Host.NotifyMoveOrResizeStarted();

                        User32.PostMessage(this.windowHandle, (uint)DefMessages.WM_NANUI_DRAG_APP_REGION, IntPtr.Zero, IntPtr.Zero);

                        return true;

                    }
                }

                if (CanResize && msg == WindowsMessages.WM_LBUTTONDBLCLK)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var dragable = (this.draggableRegion != null && this.draggableRegion.IsVisible(new Point(pt.x, pt.y)));
                    if (dragable)
                    {
                        User32.SendMessage(this.windowHandle, (uint)WindowsMessages.WM_NCLBUTTONDBLCLK, (IntPtr)HitTest.HTCAPTION, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));
                        return true;
                    }
                }

                if (msg == WindowsMessages.WM_RBUTTONDOWN)
                {
                    var pt = Win32.GetPostionFromPtr(message.LParam);
                    var dragable = (this.draggableRegion != null && this.draggableRegion.IsVisible(new Point(pt.x, pt.y)));
                    if (dragable)
                    {

                        User32.SendMessage(this.windowHandle, (uint)DefMessages.WM_NANUI_APP_REGION_RBUTTONDOWN, IntPtr.Zero, Win32.MakeParam((IntPtr)pt.x, (IntPtr)pt.y));
                        return true;
                    }
                }
            }
            return false;
        }

        protected HitTest GetSizeMode(POINT point)
        {
            HitTest mode = HitTest.HTCLIENT;

            int x = point.x, y = point.y;

            var CornerAreaSize = Win32.CornerAreaSize;

            if (WindowState == FormWindowState.Normal && CanResize)
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

        #region 消息处理

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

        protected bool ShowSystemMenu(Form frm, Point pt)
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


        [DllImport("USER32.dll")]
        private static extern bool DestroyMenu(IntPtr menu);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private static extern IntPtr GetSystemMenuCore(IntPtr hWnd, bool bRevert);
        [System.Security.SecuritySafeCritical]
        internal static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert)
        {
            return GetSystemMenuCore(hWnd, bRevert);
        }

        #endregion

        //private bool IsInDragRegion(Message message)
        //{
        //    if (this.draggableRegion == null)
        //        return false;

        //    var point = GetPoint(message);
        //    return this.draggableRegion.IsVisible(point);
        //}

        //private static System.Drawing.Point GetPoint(Message message)
        //{
        //    var lparam = message.LParam;
        //    var x = NativeMethods.LoWord(lparam.ToInt32());
        //    var y = NativeMethods.HiWord(lparam.ToInt32());
        //    return new System.Drawing.Point(x, y);
        //}

        private void ToogleMaximize()
        {
            this.WindowState = (this.WindowState == FormWindowState.Maximized) ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void FrmMain_Closed(object sender, System.EventArgs e)
        {
            this.Closed -= FrmMain_Closed;
            this.LocationChanged -= Window_LocationChanged;
            this.ClientSizeChanged -= Window_StateChanged;
            this.chromeWidgetMessageInterceptor?.ReleaseHandle();
            this.chromeWidgetMessageInterceptor?.DestroyHandle();
            this.chromeWidgetMessageInterceptor = null;
        }


    }
}
