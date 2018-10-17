using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Clamp.UIShell.Glow
{

    /// <summary>
    /// 阴影管理
    /// </summary>
    public static class GlowManager
    {
        static readonly DependencyProperty GlowInfoProperty = DependencyProperty.RegisterAttached("GlowInfo", typeof(GlowInfo), typeof(GlowManager));

        /// <summary>
        /// 阴影颜色的依赖属性
        /// </summary>
        public static readonly DependencyProperty GlowBrushProperty =
            DependencyProperty.RegisterAttached("GlowBrush",
                                                typeof(Brush),
                                                typeof(GlowManager),
                                                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0x7a, 0xcc))));

        /// <summary>
        /// 是否开启阴影功能
        /// </summary>
        public static readonly DependencyProperty EnableGlowProperty = DependencyProperty.RegisterAttached("EnableGlow", typeof(bool), typeof(GlowManager), new FrameworkPropertyMetadata(EnableGlowChangedCallback));

        /// <summary>
        /// 开启阴影功能的事件
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void EnableGlowChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                if ((bool)e.NewValue == true)
                    Assign((Window)d);
                else
                    Unassign((Window)d);
            }
        }

        /// <summary>
        /// 设置阴影的信息
        /// </summary>
        /// <param name="window"></param>
        public static void AssignGlows(Window window)
        {
            if (GetGlowInfo(window) != null)
                throw new InvalidOperationException("Glows have already been assigned.");

            GlowInfo glowInfo = new GlowInfo();

            Brush glowBrush = GetGlowBrush(window);

            glowInfo.glows.Add(new GlowWindow { Location = Location.Left, Foreground = glowBrush });
            glowInfo.glows.Add(new GlowWindow { Location = Location.Top, Foreground = glowBrush });
            glowInfo.glows.Add(new GlowWindow { Location = Location.Right, Foreground = glowBrush });
            glowInfo.glows.Add(new GlowWindow { Location = Location.Bottom, Foreground = glowBrush });

            foreach (GlowWindow glow in glowInfo.glows)
            {
                glow.Opacity = window.Opacity;
                glow.Owner = window;
                glow.OwnerChanged();
            }

            SetGlowInfo(window, glowInfo);
        }

        /// <summary>
        /// 分配窗体的四边阴影
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        static bool Assign(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");
            if (GetGlowInfo(window) != null)
                return false;
            else if (!window.IsLoaded)
                window.SourceInitialized += delegate
                {
                    AssignGlows(window);
                };
            else
            {
                AssignGlows(window);
            }
            return true;
        }

        /// <summary>
        /// 解除窗体的四边阴影
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        static bool Unassign(Window window)
        {
            GlowInfo info = GetGlowInfo(window);
            if (info == null)
                return false;
            else
            {
                foreach (GlowWindow glow in info.glows)
                {
                    try
                    {
                        glow.Close();
                    }
                    catch
                    {
                        // Do nothing
                    }
                }
                SetGlowInfo(window, null);
            }
            return true;
        }


        /// <summary>
        /// 获得阴影颜色
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static Brush GetGlowBrush(Window window)
        {
            return (Brush)window.GetValue(GlowBrushProperty);
        }

        /// <summary>
        /// 设置阴影的颜色
        /// </summary>
        /// <param name="window"></param>
        /// <param name="value"></param>
        public static void SetGlowBrush(Window window, Brush value)
        {
            window.SetValue(GlowBrushProperty, value);
        }

        /// <summary>
        /// 获得是否开启阴影
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool GetEnableGlow(Window window)
        {
            return (bool)window.GetValue(EnableGlowProperty);
        }

        /// <summary>
        /// 设置是否开启阴影
        /// </summary>
        /// <param name="window"></param>
        /// <param name="value"></param>
        public static void SetEnableGlow(Window window, bool value)
        {
            window.SetValue(EnableGlowProperty, value);
        }

        internal static GlowInfo GetGlowInfo(Window window)
        {
            return (GlowInfo)window.GetValue(GlowInfoProperty);
        }

        internal static void SetGlowInfo(Window window, GlowInfo info)
        {
            window.SetValue(GlowInfoProperty, info);
        }
    }

    class GlowInfo
    {
        public readonly Collection<GlowWindow> glows = new Collection<GlowWindow>();
    }
}
