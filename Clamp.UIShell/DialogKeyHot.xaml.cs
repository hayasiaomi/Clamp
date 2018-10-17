using Clamp.UIShell.Framework.Helpers;
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
using System.Windows.Shapes;

namespace Clamp.UIShell
{
    /// <summary>
    /// DialogKeyHot.xaml 的交互逻辑
    /// </summary>
    public partial class DialogKeyHot : Window
    {
        public DialogKeyHot()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnDialogOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.ChangeChecked(false);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ChangeChecked(true);
        }

        private void ChangeChecked(bool result)
        {
            DBHelper.Store("KeyHotHint", result);
        }
    }
}
