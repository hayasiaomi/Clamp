using Clamp.MUI.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            authorityVM.BeginLogin();

            if (authorityVM.CheckValidation())
            {
                Thread thread = new Thread(new ThreadStart(() =>
                {
                    if (authorityVM.Username != "0000" || authorityVM.Password != "1234")
                    {
                        authorityVM.Mistake = "用户或密码不正确";

                        authorityVM.EndLogin();

                        return;
                    }

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        MainWindow mainWindow = new MainWindow();

                        Application.Current.MainWindow = mainWindow;

                        mainWindow.Show();

                        this.Close();

                    }));

                    authorityVM.EndLogin();
                }));

                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                authorityVM.EndLogin();
            }
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
