using Clamp.MUI.WPF.ViewModel;
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

namespace Clamp.MUI.WPF
{
    /// <summary>
    /// WindowAuthority.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAuthority : Window
    {
        public WindowAuthority()
        {
            InitializeComponent();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            AuthorityVM authorityVM = this.DataContext as AuthorityVM;

            authorityVM.Password = this.PBPassword.Password;

            authorityVM.Login();
        }

        private void TBUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            (this.DataContext as AuthorityVM).ClearFault();
        }

        private void PBPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (this.DataContext as AuthorityVM).ClearFault();
        }
    }
}
