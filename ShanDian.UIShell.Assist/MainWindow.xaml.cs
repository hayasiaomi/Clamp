using ShanDian.Common;
using ShanDian.Common.HTTP;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.InterProcess;
using ShanDian.UIShell.Framework.Services;
using ShanDian.UIShell.Assist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShanDian.UIShell.Assist
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessBinder processBinder;
        private double dragWith;
        private double dragHeight;
        /// <summary>
        /// 主进程的PID
        /// </summary>
        public int SDShellPID { set; get; }

        public MainWindow()
        {
            InitializeComponent();

            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width - 10;
            this.Top = 10;
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((Math.Abs(this.Left - this.dragWith) < 2 || Math.Abs(this.Top - this.dragHeight) < 2))
            {
                HttpRequest.Post($"http://127.0.0.1:{SDShellHelper.GetMainListener()}/sd/command", new { CmdName = "SHELLACTIVITE" });
            }
        }


        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.dragHeight = this.Top;
            this.dragWith = this.Left;

            this.DragMove();

            e.Handled = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Program.WindowAdvices != null)
                Program.WindowAdvices.Close();

            SDPipelineHelper.Shutdown();
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 0x0003;


        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            SetWindowLong(source.Handle, GWL_EXSTYLE, GetWindowLong(source.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return (IntPtr)MA_NOACTIVATE;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }

        private void SDShellProcess_Exited(object sender, EventArgs e)
        {
            this.InvokeClose();
            NLogService.Info("关闭应用");
        }


        public void InvokeClose()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }

        public void BindMainProcesss(int processId)
        {
            this.SDShellPID = processId;

            new Thread(new ThreadStart(() =>
            {
                if (processBinder == null)
                {
                    processBinder = new ProcessBinder(this.SDShellPID);
                    processBinder.ProcessExited += this.SDShellProcess_Exited;
                    processBinder.Bind();
                }
            })).Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Program.WindowAdvices = new WindowAdvices();

            Program.WindowAdvices.Show();
        }
    }
}
