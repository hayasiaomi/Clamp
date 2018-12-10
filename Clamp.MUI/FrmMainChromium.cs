using Chromium.WebBrowser;
using Clamp.MUI.Framework.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Clamp.MUI.ChromiumCore;
using Clamp.MUI.Properties;
using Clamp.MUI.Helpers;
using System.Diagnostics;
using Clamp.MUI.Biz;
using Chromium.Remote;

namespace Clamp.MUI
{
    internal partial class FrmMainChromium : FrmChromium
    {
        public NotifyIcon NotifyIcon { private set; get; }

        public FrmMainChromium() : base(ChromiumSettings.AuthorizeUrl)
        {
            this.WinFormBridge = new WinFormBridge(this);
            this.ChromiumWebBrowser.GlobalObject.Add("SD", new JsWinformObject(this.WinFormBridge));

            InitializeComponent();

            InitializeNotifyIcon();
        }

        #region 托盘业务

        /// <summary>
        /// 初始化托盘
        /// </summary>
        private void InitializeNotifyIcon()
        {
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon();

            System.Windows.Forms.MenuItem openMenuItem = new System.Windows.Forms.MenuItem(StringResources.FrmMainChromium_NotifyIcon_Open);
            openMenuItem.Click += OpenMenuItem_Click;

            System.Windows.Forms.MenuItem exitMenuItem = new System.Windows.Forms.MenuItem(StringResources.FrmMainChromium_NotifyIcon_Exit);
            exitMenuItem.Click += ExitMenuItem_Click;

            System.Windows.Forms.MenuItem floatMenuItem = new System.Windows.Forms.MenuItem(StringResources.FrmMainChromium_NotifyIcon_FloatDisplay);
            floatMenuItem.Name = "floatMenuItem";
            floatMenuItem.Click += FloatMenuItem_Click;

            System.Windows.Forms.MenuItem[] m = new System.Windows.Forms.MenuItem[] { floatMenuItem, openMenuItem, exitMenuItem, };

            this.NotifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(m);
            this.NotifyIcon.Icon = new System.Drawing.Icon(this.GetType().Assembly.GetManifestResourceStream("Clamp.MUI.Resources.Logo.ico"));
            this.NotifyIcon.Visible = false;

            this.NotifyIcon.DoubleClick += NotifyIcon_DoubleClick;

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

        private void FrmMain_Shown(object sender, EventArgs e)
        {
        }

        #endregion


        /// <summary>
        /// 显示窗体，用于托盘时候调用
        /// </summary>
        internal void DisplayNormal()
        {
            this.Show();

            if (this.WindowState != FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;

            this.TopMost = true;

            this.Focus();

            this.TopMost = false;
        }

        #region 悬浮窗的开关
        /// <summary>
        /// 悬浮窗的开关
        /// </summary>
        /// <param name="state"></param>
        internal void SwitchFloatWindow(bool @switch, bool notice = true)
        {
            DBHelper.Store("FloatSwitch", @switch);

            this.Invoke(new Action(() =>
            {
                if (@switch)
                {
                    if (ChromiumSettings.ChildProcess == null)
                    {
                        ChromiumSettings.ChildProcess = new ChildProcess("ClampSuspend.exe");
                        ChromiumSettings.ChildProcess.OnFloatExisted += ChildProcess_OnFloatExisted;
                        ChromiumSettings.ChildProcess.OnReceived += ChildProcess_OnReceived;
                    }

                    ChromiumSettings.ChildProcess.Open();

                    this.NotifyIcon.ContextMenu.MenuItems["floatMenuItem"].Text = StringResources.FrmMainChromium_NotifyIcon_FloatHide;

                    if (notice)
                        EventsHelper.Emit(this.ChromiumWebBrowser, "FloatSwitchEvent", "true");
                }
                else
                {
                    if (ChromiumSettings.ChildProcess != null)
                        ChromiumSettings.ChildProcess.Exit();

                    this.NotifyIcon.ContextMenu.MenuItems["floatMenuItem"].Text = StringResources.FrmMainChromium_NotifyIcon_FloatDisplay;

                    if (notice)
                        EventsHelper.Emit(this.ChromiumWebBrowser, "FloatSwitchEvent", "true");
                }
            }));
        }

        private void ChildProcess_OnReceived(string message)
        {
            if (message == "Click")
            {
                this.Invoke(new Action(() =>
                {
                    this.DisplayNormal();
                }));
            }
        }

        private void ChildProcess_OnFloatExisted(Process obj)
        {
            if (!ChromiumSettings.IsExiting)
            {
                object floatSwitchValue = DBHelper.AcquireValue("FloatSwitch");

                if (Convert.ToBoolean(floatSwitchValue))
                {
                    ChromiumSettings.ChildProcess.Exit();
                    ChromiumSettings.ChildProcess.Open();
                }
            }
        }
        #endregion

        #region WinForm内部事件
        private void FrmMainChromium_Load(object sender, EventArgs e)
        {
            this.Activate();
        }
        #endregion


        #region 重写FrmChromium

        /// <summary>
        /// UI线程关闭
        /// </summary>
        public override void InvokeClose()
        {
            this.Invoke(new Action(() =>
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }));
        }


        public override void InvokeOpen(string url, string title, int width, int height, double left, double top, Action<string> openCallback)
        {
            this.Invoke(new Action(() =>
            {
                FrmChromium frmChromium = ChromiumSettings.ChildrenChromiums.FirstOrDefault(cf => String.Compare(cf.Url, url, true) == 0);

                if (frmChromium == null)
                {
                    frmChromium = new FrmSimpleChromium(url);
                }

                frmChromium.Show();

                if (openCallback != null)
                    openCallback(frmChromium.Identity);

            }));
        }

        #endregion
    }


}
