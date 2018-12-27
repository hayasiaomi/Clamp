using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Clamp.MUI.WPF.Controls
{
    public class TextBoxHelper
    {
        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(TextBoxHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextBoxHelper), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.RegisterAttached("WatermarkAlignment", typeof(TextAlignment), typeof(TextBoxHelper), new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.RegisterAttached("WatermarkTrimming", typeof(TextTrimming), typeof(TextBoxHelper), new FrameworkPropertyMetadata(TextTrimming.None, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty WatermarkWrappingProperty = DependencyProperty.RegisterAttached("WatermarkWrapping", typeof(TextWrapping), typeof(TextBoxHelper), new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextBoxHelper), new UIPropertyMetadata(0));
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty WatermarkForegroundProperty = DependencyProperty.RegisterAttached("WatermarkForeground", typeof(Brush), typeof(TextBoxHelper), new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));
        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static Brush GetWatermarkForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermarkForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(WatermarkProperty, value);
        }


        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static TextAlignment GetWatermarkAlignment(DependencyObject obj)
        {
            return (TextAlignment)obj.GetValue(WatermarkAlignmentProperty);
        }


        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetWatermarkAlignment(DependencyObject obj, TextAlignment value)
        {
            obj.SetValue(WatermarkAlignmentProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static TextTrimming GetWatermarkTrimming(DependencyObject obj)
        {
            return (TextTrimming)obj.GetValue(WatermarkTrimmingProperty);
        }


        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static void SetWatermarkTrimming(DependencyObject obj, TextTrimming value)
        {
            obj.SetValue(WatermarkTrimmingProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static TextWrapping GetWatermarkWrapping(DependencyObject obj)
        {
            return (TextWrapping)obj.GetValue(WatermarkWrappingProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static void SetWatermarkWrapping(DependencyObject obj, TextWrapping value)
        {
            obj.SetValue(WatermarkWrappingProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static bool GetHasText(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasTextProperty);
        }

        public static void SetHasText(DependencyObject obj, bool value)
        {
            obj.SetValue(HasTextProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = (TextBox)d;
                if ((bool)e.NewValue)
                {
                    txtBox.Dispatcher.BeginInvoke(new Action(() => TextChanged(txtBox, new TextChangedEventArgs(TextBox.TextChangedEvent, UndoAction.None))));

                    txtBox.TextChanged += TextChanged;
                    txtBox.GotFocus += TextBoxGotFocus;
                }
                else
                {
                    txtBox.TextChanged -= TextChanged;
                    txtBox.GotFocus -= TextBoxGotFocus;
                }
            }
            else if (d is PasswordBox)
            {
                var passBox = (PasswordBox)d;

                if ((bool)e.NewValue)
                {
                    passBox.Dispatcher.BeginInvoke(new Action(() => PasswordChanged(passBox, new RoutedEventArgs(PasswordBox.PasswordChangedEvent, passBox))));

                    passBox.PasswordChanged += PasswordChanged;
                    passBox.GotFocus += PasswordGotFocus;
                }
                else
                {
                    passBox.PasswordChanged -= PasswordChanged;
                    passBox.GotFocus -= PasswordGotFocus;
                }
            }
        }

        private static void SetTextLength<TDependencyObject>(TDependencyObject sender, Func<TDependencyObject, int> funcTextLength) where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                var value = funcTextLength(sender);

                sender.SetValue(TextLengthProperty, value);
                sender.SetValue(HasTextProperty, value >= 1);
            }
        }

        private static void TextChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as TextBox, textBox => textBox.Text.Length);
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as PasswordBox, passwordBox => passwordBox.Password.Length);
        }

        private static void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as TextBox, textBox => textBox.SelectAll());
        }

        private static void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as PasswordBox, passwordBox => passwordBox.SelectAll());
        }

        private static void ControlGotFocus<TDependencyObject>(TDependencyObject sender, Action<TDependencyObject> action) where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                if (GetSelectAllOnFocus(sender))
                {
                    sender.Dispatcher.BeginInvoke(action, sender);
                }
            }
        }
    }
}
