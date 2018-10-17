using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Clamp.UIShell.Glow
{
    /// <summary>
    /// 阴影位置
    /// </summary>
    enum Location
    {
        Left, Top, Right, Bottom
    }

    /// <summary>
    /// 边的阴影
    /// </summary>
    class GlowWindow : Window
    {
        public double glowThickness = 10d, tolerance = 72d;

        public Location Location;
        public Action Update;
        public Func<Point, Cursor> GetCursor;
        public Func<Point, HT> GetHT;
        private Border glowBorder;
        private Func<bool> canResize;


        public void NotifyResize(HT ht)
        {
            NativeMethods.SendNotifyMessage(new WindowInteropHelper(Owner).Handle, (int)WM.NCLBUTTONDOWN, (IntPtr)ht, IntPtr.Zero);
        }

        public GlowWindow()
        {
            InitializeComponent();

            this.Background = Brushes.Transparent;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.CanMinimize;
            this.AllowsTransparency = true;
            this.SnapsToDevicePixels = true;
            this.ShowInTaskbar = false;

            this.IsVisibleChanged += GlowWindow_IsVisibleChanged;
        }

        private void GlowWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("Window Visible " + e.NewValue);
        }

        private void InitializeComponent()
        {
            Border bgBorder = new Border() { Background = Brushes.Transparent };

            Grid bgGrid = new Grid();

            this.glowBorder = new Border();
            this.glowBorder.CornerRadius = new CornerRadius(5);

            Binding foregroundBinding = new Binding();

            foregroundBinding.Path = new PropertyPath(ForegroundProperty);
            foregroundBinding.Source = this;

            this.glowBorder.SetBinding(BackgroundProperty, foregroundBinding);

            this.glowBorder.Effect = new BlurEffect() { Radius = 10 };

            bgGrid.Children.Add(this.glowBorder);

            bgBorder.Child = bgGrid;

            this.Content = bgBorder;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Update();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int ws_ex = NativeMethods.GetWindowLong(hwnd, (int)GWL.EXSTYLE);
            ws_ex |= (int)WS_EX.TOOLWINDOW;
            NativeMethods.SetWindowLong(hwnd, (int)GWL.EXSTYLE, ws_ex);
            HwndSource.FromHwnd(hwnd).AddHook(WinMain);
        }

        public virtual IntPtr WinMain(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((WM)msg)
            {
                case WM.MOUSEACTIVATE:
                    {
                        handled = true;
                        return new IntPtr(3); // MA_NOACTIVATE
                    }
                case WM.NCHITTEST:
                    {
                        if (canResize())
                        {
                            Point ptScreen = NativeMethods.LPARAMTOPOINT(lParam);
                            Point ptClient = PointFromScreen(ptScreen);
                            Cursor = GetCursor(ptClient);
                        }
                        break;
                    }
                case WM.LBUTTONDOWN:
                    {
                        POINT ptScreenWin32;
                        NativeMethods.GetCursorPos(out ptScreenWin32);
                        Point ptScreen = new Point(ptScreenWin32.x, ptScreenWin32.y);
                        Point ptClient = PointFromScreen(ptScreen);
                        HT result = GetHT(ptClient);
                        IntPtr ownerHwnd = new WindowInteropHelper(Owner).Handle;
                        NativeMethods.SendNotifyMessage(ownerHwnd, (int)WM.NCLBUTTONDOWN, (IntPtr)result, IntPtr.Zero);
                        break;
                    }
                case WM.GETMINMAXINFO:
                    {
                        MINMAXINFO info = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
                        info.ptMaxSize = info.ptMaxTrakSize = new POINT { x = int.MaxValue, y = int.MaxValue };
                        Marshal.StructureToPtr(info, lParam, true);
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        public void OwnerChanged()
        {
            canResize = () => Owner.ResizeMode == ResizeMode.CanResize ? true : Owner.ResizeMode == ResizeMode.CanResizeWithGrip ? true : false;

            switch (Location)
            {
                case Location.Left:
                    {
                        GetCursor = pt =>
                            new Rect(new Point(0, 0), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            Cursors.SizeNWSE :
                            new Rect(new Point(0, ActualHeight - tolerance), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            Cursors.SizeNESW :
                            Cursors.SizeWE;

                        GetHT = pt => new Rect(new Point(0, 0), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            HT.TOPLEFT :
                            new Rect(new Point(0, ActualHeight - tolerance), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            HT.BOTTOMLEFT :
                            HT.LEFT;

                        Update = delegate
                        {
                            if (glowBorder != null)
                                glowBorder.Margin = new Thickness(glowThickness, glowThickness, -glowThickness, glowThickness);
                            Left = Owner.Left - glowThickness;
                            Top = Owner.Top - glowThickness;
                            Width = glowThickness;
                            Height = Owner.ActualHeight + glowThickness * 2;
                        };
                        break;
                    }

                case Location.Top:
                    {
                        GetCursor = pt =>
                            new Rect(new Point(0, 0), new Size(tolerance, glowThickness)).Contains(pt) ?
                            Cursors.SizeNWSE :
                            new Rect(new Point(ActualWidth - tolerance, 0), new Size(tolerance, ActualHeight)).Contains(pt) ?
                            Cursors.SizeNESW :
                            Cursors.SizeNS;

                        GetHT = pt =>
                            new Rect(new Point(0, 0), new Size(tolerance, glowThickness)).Contains(pt) ?
                            HT.TOPLEFT :
                            new Rect(new Point(ActualWidth - tolerance, 0), new Size(tolerance, ActualHeight)).Contains(pt) ?
                            HT.TOPRIGHT :
                            HT.TOP;

                        Update = delegate
                        {
                            if (glowBorder != null)
                                glowBorder.Margin = new Thickness(glowThickness, glowThickness, glowThickness, -glowThickness);
                            Left = Owner.Left - glowThickness;
                            Top = Owner.Top - glowThickness;
                            Width = Owner.ActualWidth + glowThickness * 2;
                            Height = glowThickness;
                        };
                        break;
                    }

                case Location.Right:
                    {
                        GetCursor = pt =>
                            new Rect(new Point(0, 0), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            Cursors.SizeNESW :
                            new Rect(new Point(0, ActualHeight - tolerance), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            Cursors.SizeNWSE :
                            Cursors.SizeWE;

                        GetHT = pt => new Rect(new Point(0, 0), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            HT.TOPRIGHT :
                            new Rect(new Point(0, ActualHeight - tolerance), new Size(ActualWidth, tolerance)).Contains(pt) ?
                            HT.BOTTOMRIGHT :
                            HT.RIGHT;


                        Update = delegate
                        {
                            if (glowBorder != null)
                                glowBorder.Margin = new Thickness(-glowThickness, glowThickness, glowThickness, glowThickness);
                            Left = Owner.Left + Owner.ActualWidth;
                            Top = Owner.Top - glowThickness;
                            Width = glowThickness;
                            Height = Owner.ActualHeight + glowThickness * 2;
                        };
                        break;
                    }

                case Location.Bottom:
                    {
                        GetCursor = pt =>
                            new Rect(new Point(0, 0), new Size(tolerance, glowThickness)).Contains(pt) ?
                            Cursors.SizeNESW :
                            new Rect(new Point(ActualWidth - tolerance, 0), new Size(tolerance, ActualHeight)).Contains(pt) ?
                            Cursors.SizeNWSE :
                            Cursors.SizeNS;

                        GetHT = pt =>
                            new Rect(new Point(0, 0), new Size(tolerance, glowThickness)).Contains(pt) ?
                            HT.BOTTOMLEFT :
                            new Rect(new Point(ActualWidth - tolerance, 0), new Size(tolerance, ActualHeight)).Contains(pt) ?
                            HT.BOTTOMRIGHT :
                            HT.BOTTOM;

                        Update = delegate
                        {
                            if (glowBorder != null)
                                glowBorder.Margin = new Thickness(glowThickness, -glowThickness, glowThickness, glowThickness);
                            Left = Owner.Left - glowThickness;
                            Top = Owner.Top + Owner.ActualHeight;
                            Width = Owner.ActualWidth + glowThickness * 2;
                            Height = glowThickness;
                        };
                        break;
                    }
            }

            Owner.LocationChanged += delegate
            {
                Update();
            };
            Owner.SizeChanged += delegate
            {
                Update();
            };
            Owner.IsVisibleChanged += delegate
            {
                if (Owner.IsVisible)
                {
                    this.Opacity = 1;
                    //this.Hide();
                    Debug.WriteLine("Window IsVisibleChanged Hide ");
                }
                else
                {
                    this.Opacity = 0;
                    //this.Show();
                    Debug.WriteLine("Window IsVisibleChanged Show ");
                }
            };
            Owner.StateChanged += delegate
            {
                Debug.WriteLine("Window state :" + Owner.WindowState);

                if (Owner.WindowState == WindowState.Maximized || Owner.WindowState == WindowState.Minimized)
                {
                    this.Opacity = 0;
                    //this.Hide();
                    Debug.WriteLine("Window StateChanged Hide ");
                }
                else
                {
                    this.Opacity = 1;
                    //this.Show();
                    Debug.WriteLine("Window StateChanged Show ");
                }
            };


            Debug.WriteLine("Window Show ");

            this.Update();
            this.Show();
        }
    }
}
