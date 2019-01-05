using Chromium;
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
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace Clamp.MUI.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<ContextMenuItem> menuCommands = new List<ContextMenuItem>();
        private readonly List<int> menuSeparatorIndex = new List<int>();

        private IntPtr windowHandle;
        private IntPtr chromeWidgetHostHandle;
        private BrowserWidgetMessageInterceptor chromeWidgetMessageInterceptor;
        private Region draggableRegion = null;
        private IntPtr browserHandle;
        private Matrix matrix = new Matrix(1, 0, 0, 1, 0, 0);
        private IntPtr _DebugWindowHandle = IntPtr.Zero;

        private CfxLifeSpanHandler debugCfxLifeSpanHandler;
        private CfxClient debugCfxClient;

        public MainWindow()
        {
            InitializeComponent();

            this.menuCommands.Add(new ContextMenuItem(() =>
            {
                this.OpenDebugTools();

            }, "开发工具"));

            this.Loaded += Window_Loaded;

            this.ChromiumWebBrowser.BrowserCreated += ChromiumWebBrowser_BrowserCreated;
            this.ChromiumWebBrowser.LoadHandler.OnLoadError += LoadHandler_OnLoadError;
            this.ChromiumWebBrowser.RequestHandler.OnQuotaRequest += RequestHandler_OnQuotaRequest;
            this.ChromiumWebBrowser.ContextMenuHandler.OnBeforeContextMenu += OnBeforeContextMenu;
            this.ChromiumWebBrowser.ContextMenuHandler.OnContextMenuCommand += ContextMenuHandler_OnContextMenuCommand;

            var dragHandler = this.ChromiumWebBrowser.DragHandler;

            dragHandler.OnDragEnter += (o, e) => { e.SetReturnValue(true); };
            dragHandler.OnDraggableRegionsChanged += DragHandler_OnDraggableRegionsChanged;
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


        #region 私有方法  右击菜单


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

                this.chromeWidgetMessageInterceptor = new BrowserWidgetMessageInterceptor(this.ChromiumWebBrowser, this.chromeWidgetHostHandle, OnWebBroswerMessage);
            });

            thread.IsBackground = true;
            thread.Start();
        }
        /// <summary>
        /// Window加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Window 状态发生变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        #endregion





    }
}
