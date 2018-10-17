using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Clamp.UIShell.Views
{
    /// <summary>
    /// LoadingWait.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWait : UserControl
    {
        private readonly DispatcherTimer animationTimer;
        private const double offset = Math.PI;
        private const double step = Math.PI * 2 / 8.0;

        public string BusyText
        {
            get
            {
                return this.txtbBusyText.Text;
            }
            set
            {
                this.txtbBusyText.Text = value;
            }
        }

        public LoadingWait()
        {
            InitializeComponent();

            animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher);
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 75);
        }

        private void Start()
        {
            animationTimer.Tick += HandleAnimationTick;
            animationTimer.Start();
        }

        private void Stop()
        {
            animationTimer.Stop();
            animationTimer.Tick -= HandleAnimationTick;
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            SetPosition(C0, offset, 0.0, step);
            SetPosition(C1, offset, 1.0, step);
            SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
        }

        private void SetPosition(Ellipse ellipse, double offset, double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 40.0 + Math.Sin(offset + posOffSet * step) * 40.0);
            ellipse.SetValue(Canvas.TopProperty, 40 + Math.Cos(offset + posOffSet * step) * 40.0);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            if (isVisible)
                Start();
            else
                Stop();
        }
    }
}
