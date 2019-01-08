using Clamp.MUI.WF.Windows;
using System;
using System.Windows.Forms;

namespace Clamp.MUI.WF.CFX
{
    public delegate bool BrowserForwardAction(ref Message message);

    internal class BrowserWidgetMessageInterceptor : NativeWindow
    {
        private BrowserForwardAction forwardAction;
        private readonly IntPtr parentHandle;

        internal BrowserWidgetMessageInterceptor(Control parentControl, IntPtr parentHandle, IntPtr chromeHostHandle, BrowserForwardAction forwardAction)
        {
            this.parentHandle = parentHandle;
            AssignHandle(chromeHostHandle);
            parentControl.HandleDestroyed += BrowserHandleDestroyed;
            this.forwardAction = forwardAction;
        }

        private void BrowserHandleDestroyed(object sender, EventArgs e)
        {
            ReleaseHandle();
            var browser = (Control)sender;
            browser.HandleDestroyed -= BrowserHandleDestroyed;
            forwardAction = null;
        }

        protected override void WndProc(ref Message m)
        {
            if (forwardAction == null)
            {
                return;
            }


            try
            {

                if (m.Msg == (int)WindowsMessages.WM_ACTIVATE)
                {
                    User32.PostMessage(parentHandle, (uint)WindowsMessages.WM_ACTIVATE, m.WParam, m.LParam);
                }

                var result = forwardAction(ref m);

                if (!result)
                {
                    base.WndProc(ref m);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                base.WndProc(ref m);

            }
        }
    }
}