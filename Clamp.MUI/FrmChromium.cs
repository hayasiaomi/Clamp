using Clamp.MUI.Biz;
using Clamp.MUI.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clamp.MUI
{
    internal partial class FrmChromium : ChromiumWinForm, IChromiumWinForm
    {
        public WinFormBridge WinFormBridge { protected set; get; }

        public FrmChromium(string initialUrl) : base(initialUrl)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Activate();
        }

        #region IChromiumForm接口实现

        public virtual void InvokeRedirect(string url)
        {
            this.Invoke(new Action(() =>
            {
                this.LoadUrl(url);
            }));
        }

        /// <summary>
        /// UI线程关闭
        /// </summary>
        public virtual void InvokeClose()
        {
            this.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }

        /// <summary>
        /// 退出
        /// </summary>
        public virtual void InvokeQuit()
        {
            this.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }

        /// <summary>
        /// UI线程执行最小化
        /// </summary>
        public virtual void InvokeMinimized()
        {
            this.Invoke(new Action(() =>
            {
                this.WindowState = FormWindowState.Minimized;
            }));
        }

        /// <summary>
        /// UI线程执行最大化
        /// </summary>
        public virtual void InvokeMaximized()
        {
            this.Invoke(new Action(() =>
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                else
                {
                    this.WindowState = FormWindowState.Maximized;
                }
            }));
        }

        public virtual void InvokeOpenScreenKeyboard()
        {
            this.Invoke(new Action(() =>
            {
                //this.virtualKeyboard.PlacementTarget = this;
                //this.virtualKeyboard.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                //this.virtualKeyboard.IsOpen = true;
            }));
        }

        public virtual object InvokeMethod(Delegate method)
        {
            return this.Invoke(method);
        }

        public virtual object InvokeMethod(Delegate method, params object[] args)
        {
            return this.Invoke(method, args);
        }

        public virtual void InvokeOpen(string url, string title, int width, int height, double left, double top, Action<string> openCallback)
        {

        }

        #endregion
    }
}
