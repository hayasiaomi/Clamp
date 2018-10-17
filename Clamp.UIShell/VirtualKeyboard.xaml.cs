using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindowsInput;
using WindowsInput.Native;

namespace Clamp.UIShell
{
    /// <summary>
    /// VirtualKeyboard.xaml 的交互逻辑
    /// </summary>
    public partial class VirtualKeyboard : Window, INotifyPropertyChanged
    {
        private bool _showNumericKeyboard;
        private InputSimulator input = new InputSimulator();
        public bool ShowNumericKeyboard
        {
            get { return _showNumericKeyboard; }
            set { _showNumericKeyboard = value; this.OnPropertyChanged("ShowNumericKeyboard"); }
        }

        public VirtualKeyboard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.Register("IsDragging", typeof(bool), typeof(VirtualKeyboard), new UIPropertyMetadata(false));

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            private set { SetValue(IsDraggingProperty, value); }
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


        protected override void OnClosed(EventArgs e)
        {
            SDShell.VirtualKeyboard = null;
            base.OnClosed(e);
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }



        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                string cmdParameter = button.CommandParameter.ToString();

                string[] cmd = cmdParameter.Split('+');
                if (cmd != null && cmd.Length > 0)
                {
                    if (cmd[0] != "KEYBOARDCLOSE")
                    {
                        if (cmd.Length > 1)
                        {
                            VirtualKeyCode keyDownCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[0]);

                            input.Keyboard
                                .KeyDown(keyDownCode)
                                .KeyPress((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[1]))
                                .KeyUp(keyDownCode);

                        }
                        else
                        {
                            input.Keyboard.KeyPress((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[0]));
                        }

                        if (cmd[0] == "RETURN")
                        {
                            //this.IsOpen = false;
                        }
                    }
                    else
                    {
                        //this.IsOpen = false;
                    }
                }

            }
        }

        private void DragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //this.HorizontalOffset += e.HorizontalChange;
            //this.VerticalOffset += e.VerticalChange;
        }
    }
}
