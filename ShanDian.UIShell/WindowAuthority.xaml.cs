using Newtonsoft.Json;
using ShanDian.UIShell.Brower;
using ShanDian.UIShell.Forms;
using ShanDian.UIShell.Framework;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.Services;
using ShanDian.UIShell.Properties;
using ShanDian.UIShell.ViewModel;
using ShanDian.UIShell.Framework.Brower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ShanDian.Common.Commands;
using ShanDian.Common;
using ShanDian.UIShell.Framework.InterProcess;
using System.Diagnostics;

namespace ShanDian.UIShell
{
    /// <summary>
    /// WindowAuthority.xaml 的交互逻辑
    /// </summary>
    public partial class WindowAuthority : Window
    {
        private List<Ellipse> ellipsesDots = new List<Ellipse>();
        private Timer dispatcherTimer;

        public WindowAuthority()
        {
            InitializeComponent();
        }

        private void WindowHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void BtnWindowClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMainVirtualKey_Click(object sender, RoutedEventArgs e)
        {
            MiniKeyboard.Input(Convert.ToInt32((sender as Button).DataContext));
        }

        /// <summary>
        /// 显示记住密码用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRememberUser_Click(object sender, RoutedEventArgs e)
        {
            if (this.gridRememberUsers.Visibility != Visibility.Visible)
            {
                this.DisplayRememberUsers();
            }
            else
            {
                this.UnDisplayRememberUsers();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FrmSplash.CloseForm();

            this.Activate();

            this.txtbSoftVersion.Text = string.Format("V {0}", SDShell.FullVersion);

            this.ellipsesDots.Clear();

            this.ellipsesDots.Add(this.Dot1);
            this.ellipsesDots.Add(this.Dot2);
            this.ellipsesDots.Add(this.Dot3);

            Thickness thickness = new Thickness(0, 0, 0, 0);

            thickness.Top = this.gridHeader.Margin.Top + this.gridHeader.ActualHeight;
            thickness.Top += this.gridMark.Margin.Top + this.gridMark.ActualHeight;
            thickness.Top += this.spLoginZone.Margin.Top;
            thickness.Top += this.borderInputZone.Margin.Top;
            thickness.Top += this.gridUserName.Margin.Top + this.gridUserName.ActualHeight;

            this.borderRememberUsersList.Margin = thickness;

            this.RefreshRememberUsers();


            ProxyInfo proxyInfo = SDShell.GetProxyInfo();

            if (proxyInfo != null)
            {
                SDShell.ChromiumWindow.ChangeProxyInfo(proxyInfo.TypeName, proxyInfo.Server, proxyInfo.Port);
            }


        }



        private void DispatcherTimer_Tick(object sender)
        {
            this.dispatcherTimer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (this.Dot1.Visibility != Visibility.Visible)
                    {
                        this.Dot1.Visibility = Visibility.Visible;
                    }
                    else if (this.Dot2.Visibility != Visibility.Visible)
                    {
                        this.Dot2.Visibility = Visibility.Visible;
                    }
                    else if (this.Dot3.Visibility != Visibility.Visible)
                    {
                        this.Dot3.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.Dot1.Visibility = Visibility.Collapsed;
                        this.Dot2.Visibility = Visibility.Collapsed;
                        this.Dot3.Visibility = Visibility.Collapsed;
                    }
                }));

            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
            this.dispatcherTimer.Change(700, 700);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.UnDisplayRememberUsers();
        }

        private void BtnKeyboard_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            Thickness thickness = new Thickness(0, 0, 0, 0);

            thickness.Top = this.gridHeader.Margin.Top + this.gridHeader.ActualHeight;
            thickness.Top += this.spLoginZone.Margin.Top;
            thickness.Top += this.borderInputZone.Margin.Top;
            thickness.Top += this.gridUserName.Margin.Top + this.gridUserName.ActualHeight;

            if (this.gridMainKeyboard.Visibility == Visibility.Visible)
            {
                this.gridMainKeyboard.Visibility = Visibility.Collapsed;
                this.gridMark.Visibility = Visibility.Visible;
                this.imgKeyboardAngle.Angle = 0;
                thickness.Top += this.gridMark.Margin.Top + this.gridMark.ActualHeight;

            }
            else
            {
                this.gridMainKeyboard.Visibility = Visibility.Visible;
                this.gridMark.Visibility = Visibility.Collapsed;
                this.imgKeyboardAngle.Angle = 180;
            }

            this.borderRememberUsersList.Margin = thickness;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayButtonLogining();

            string username = this.txtUsername.Text.Trim();
            string password = this.txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username))
            {
                this.DisplayButtonLogin();
                this.DisplayErrorMessage(SDResources.AuthorityView_UsernameBlank);
                return;
            }

            if (!string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                this.DisplayButtonLogin();

                this.txtPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtPassword.Password))
            {
                this.DisplayButtonLogin();
                this.DisplayErrorMessage(SDResources.AuthorityView_PasswordBlank);
                return;
            }


            bool rememberPassword = this.cbRememberMe.IsChecked.HasValue ? this.cbRememberMe.IsChecked.Value : false;

            new Thread(new ThreadStart(() =>
            {
                string errorMessage;

                //登录成功
                if (AuthorityService.Login(username, password, rememberPassword, out errorMessage))
                {
                    string licenseNumberValues = CDBHelper.Get("license_number");

                    if (!string.IsNullOrWhiteSpace(licenseNumberValues))
                    {
                        List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

                        List<RememberUserViewModel> rememberUsers = new List<RememberUserViewModel>();

                        foreach (LicenseNumber licenseNumber in licensenumbers)
                        {
                            rememberUsers.Add(new RememberUserViewModel()
                            {
                                Username = licenseNumber.Username,
                                Password = licenseNumber.Password,
                                IsMemberkMe = licenseNumber.IsMemberkPassword
                            });
                        }

                        if (rememberUsers.Count > 0)
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                AuthorityViewModel authorityViewModel = this.DataContext as AuthorityViewModel;

                                authorityViewModel.RememberUsers.Clear();

                                foreach (RememberUserViewModel rememberUser in rememberUsers)
                                {
                                    authorityViewModel.RememberUsers.Add(rememberUser);
                                }
                            }));
                        }
                    }

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Logistics.UploadSoftware();
                        }
                        catch (Exception ex)
                        {
                            DebugHelper.WriteException(ex);
                        }

                        try
                        {
                            Logistics.LoginNoticeUpdate();
                        }
                        catch (Exception ex)
                        {
                            DebugHelper.WriteException(ex);
                        }

                    });

                    this.Dispatcher.BeginInvoke(new Action(() =>
                           {
                               WindowChromium windowChromium = new WindowChromium();

                               windowChromium.WindowAuthority = this;

                               Application.Current.MainWindow = windowChromium;

                               windowChromium.Show();

                           }));

                }
                else
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.DisplayErrorMessage(errorMessage);
                        this.DisplayButtonLogin();
                    }));
                    return;
                }
            })).Start();
        }

        private void RefreshRememberUsers()
        {
            Task.Factory.StartNew(() =>
            {
                string licenseNumberValues = CDBHelper.Get("license_number");

                if (!string.IsNullOrWhiteSpace(licenseNumberValues))
                {
                    List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

                    List<RememberUserViewModel> rememberUsers = new List<RememberUserViewModel>();

                    foreach (LicenseNumber licenseNumber in licensenumbers)
                    {
                        rememberUsers.Add(new RememberUserViewModel()
                        {
                            Username = licenseNumber.Username,
                            Password = licenseNumber.Password,
                            IsMemberkMe = licenseNumber.IsMemberkPassword
                        });
                    }

                    if (rememberUsers.Count > 0)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            AuthorityViewModel authorityViewModel = this.DataContext as AuthorityViewModel;

                            authorityViewModel.RememberUsers.Clear();

                            foreach (RememberUserViewModel rememberUser in rememberUsers)
                            {
                                authorityViewModel.RememberUsers.Add(rememberUser);
                            }
                        }));
                    }
                }
            });
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Logout()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {

                this.txtUsername.Text = string.Empty;
                this.txtPassword.Password = string.Empty;
                this.cbRememberMe.IsChecked = false;
                this.DisplayButtonLogin();
            }));
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Dispatcher.BeginInvoke(
                DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    this.txtUsername.Focus();
                }));
            }

        }
        /// <summary>
        /// 弹出记录用户的框
        /// </summary>
        public void DisplayRememberUsers()
        {
            AuthorityViewModel authorityViewModel = this.DataContext as AuthorityViewModel;

            if (authorityViewModel != null && authorityViewModel.RememberUsers.Count > 0)
            {
                this.gridRememberUsers.Visibility = Visibility.Visible;
                this.pathRememberUserAngle.Angle = 180;
            }
        }

        /// <summary>
        /// 隐藏记录用户的框
        /// </summary>
        public void UnDisplayRememberUsers()
        {

            this.gridRememberUsers.Visibility = Visibility.Collapsed;
            this.pathRememberUserAngle.Angle = 0;
        }
        /// <summary>
        /// 显示正在登录中...
        /// </summary>
        private void DisplayButtonLogining()
        {
            if (this.dispatcherTimer == null)
            {
                this.dispatcherTimer = new Timer(this.DispatcherTimer_Tick, null, 700, 700);
            }
            this.btnLogin.IsEnabled = false;
            this.txtbLoginText.Text = SDResources.AuthorityView_LoginingText;
        }

        /// <summary>
        /// 显示正常的登录
        /// </summary>
        private void DisplayButtonLogin()
        {
            if (this.dispatcherTimer != null)
            {
                this.dispatcherTimer.Change(Timeout.Infinite, Timeout.Infinite);
                this.dispatcherTimer.Dispose();
                this.dispatcherTimer = null;
            }

            try
            {

                this.Dot1.Visibility = Visibility.Collapsed;
                this.Dot2.Visibility = Visibility.Collapsed;
                this.Dot3.Visibility = Visibility.Collapsed;

                this.btnLogin.IsEnabled = true;
                this.txtbLoginText.Text = SDResources.AuthorityView_LoginText;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
        }

        /// <summary>
        /// 显示错误
        /// </summary>
        /// <param name="error"></param>
        private void DisplayErrorMessage(string error)
        {
            this.txtbErrorMessage.Text = error;
            this.txtbErrorMessage.Visibility = Visibility.Visible;
        }
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(this.txtPassword.Password))
            {
                this.txtbPasswordHint.Visibility = Visibility.Hidden;
            }
            else
            {
                this.txtbPasswordHint.Visibility = Visibility.Visible;
            }

            this.txtbErrorMessage.Text = string.Empty;
            this.txtbErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void TxtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtUsername.Text))
            {
                this.txtbUsernameHint.Visibility = Visibility.Hidden;
            }
            else
            {
                this.txtbUsernameHint.Visibility = Visibility.Visible;
            }

            if (!string.IsNullOrWhiteSpace(this.txtPassword.Password))
                this.txtPassword.Password = string.Empty;

            if (this.cbRememberMe.IsChecked.HasValue && this.cbRememberMe.IsChecked.Value)
                this.cbRememberMe.IsChecked = false;

            this.txtbErrorMessage.Text = string.Empty;
            this.txtbErrorMessage.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// 选中记录用户框中的一个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListDishesTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UnDisplayRememberUsers();

            if (e.AddedItems.Count > 0)
            {
                RememberUserViewModel rememberUserViewModel = e.AddedItems[0] as RememberUserViewModel;

                this.txtUsername.Text = rememberUserViewModel.Username;

                if (rememberUserViewModel.IsMemberkMe)
                {
                    this.txtPassword.Password = PasswordHelper.Encrypt(rememberUserViewModel.Password);
                    this.cbRememberMe.IsChecked = rememberUserViewModel.IsMemberkMe;
                }
            }
        }

        private void BtnDeleteRememberUser_Click(object sender, RoutedEventArgs e)
        {
            this.UnDisplayRememberUsers();

            Button button = sender as Button;

            if (button.DataContext != null)
            {
                RememberUserViewModel rememberUserViewModel = button.DataContext as RememberUserViewModel;

                if (rememberUserViewModel != null && !string.IsNullOrWhiteSpace(rememberUserViewModel.Username))
                {
                    string licenseNumberValues = CDBHelper.Get("license_number");

                    if (!string.IsNullOrWhiteSpace(licenseNumberValues))
                    {
                        List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);
                        if (licensenumbers != null)
                        {
                            LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == rememberUserViewModel.Username);

                            licensenumbers.Remove(licenseNumber);

                            CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));

                            AuthorityViewModel authorityViewModel = this.DataContext as AuthorityViewModel;

                            authorityViewModel.RememberUsers.Remove(rememberUserViewModel);
                        }
                    }
                }

            }

        }

        private void TxtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            string username = this.txtUsername.Text;

            if (!string.IsNullOrWhiteSpace(username))
            {
                AuthorityViewModel authorityViewModel = this.DataContext as AuthorityViewModel;

                if (authorityViewModel.RememberUsers != null && authorityViewModel.RememberUsers.Count > 0)
                {
                    RememberUserViewModel rememberUserViewModel = authorityViewModel.RememberUsers.FirstOrDefault(ru => ru.Username == username);

                    if (rememberUserViewModel != null && rememberUserViewModel.IsMemberkMe)
                    {
                        this.txtPassword.Password = PasswordHelper.Encrypt(rememberUserViewModel.Password);
                        this.cbRememberMe.IsChecked = rememberUserViewModel.IsMemberkMe;
                    }
                }
            }
        }

        private void BtnProxy_Click(object sender, RoutedEventArgs e)
        {
            this.gridAuthoriyView.Visibility = Visibility.Hidden;
            this.gridProxyView.Visibility = Visibility.Visible;
        }


        private void ProxyPartial_OnProxyBackClick(object sender, ProxyRoutedEventArgs e)
        {
            this.gridAuthoriyView.Visibility = Visibility.Visible;
            this.gridProxyView.Visibility = Visibility.Hidden;
        }


        private void ProxyPartial_OnProxyOkClick(object sender, ProxyRoutedEventArgs e)
        {
            this.gridAuthoriyView.Visibility = Visibility.Visible;
            this.gridProxyView.Visibility = Visibility.Hidden;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            SDPipelineHelper.Setup("SDShell");

            SDPipelineHelper.HandlePipelineCommand = this.HandlePipelineCommand;

            object floatSwitchValue = DBHelper.AcquireValue("FloatSwitch");

            if (floatSwitchValue != null && Convert.ToBoolean(floatSwitchValue))
            {
                Task.Factory.StartNew(() =>
                {
                    SDShell.OpenSDSuspender();

                }, TaskCreationOptions.LongRunning);
            }

            Task.Factory.StartNew(() =>
            {
                SDShell.OpenSDMessager();
            }, TaskCreationOptions.LongRunning);
        }


        private object HandlePipelineCommand(SDPipelineCommand command)
        {
            if (command != null)
            {
                if (command.Compare("ShellActivate"))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (this.WindowState != WindowState.Maximized)
                            this.WindowState = WindowState.Normal;

                        this.Activate();
                        this.Topmost = true;
                        Thread.Sleep(20);
                        this.Topmost = false;

                    }));
                }
                else if (command.Compare("Upgrade"))
                {
                    Process[] upgraderProcesses = Process.GetProcessesByName("SDUpgrader");

                    if (upgraderProcesses == null || upgraderProcesses.Length == 0)
                    {
                        string startup = System.IO.Path.Combine(SDShell.SDRootPath, "SDUpgrader.exe");

                        UpgradeInfo upgradeInfo = new UpgradeInfo();
                        upgradeInfo.Name = "shandian";
                        upgradeInfo.ExitProcesses.Add("SDShell");
                        upgradeInfo.ExitProcesses.Add("SDSuspender");
                        upgradeInfo.ExitWindowServices.Add("ShanDian");
                        upgradeInfo.UpgradedFilePath = System.IO.Path.Combine(SDShell.SDRootPath, "Upgrades", command.Data, "SDSetup.zip");
                        upgradeInfo.UpgradedTargetPath = SDShell.SDRootPath;

                        string dataString = JsonConvert.SerializeObject(upgradeInfo);

                        string cmdLine = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataString));

                        Process process = new Process();

                        ProcessStartInfo psi = new ProcessStartInfo(startup);

                        psi.Arguments = cmdLine;

                        process.StartInfo = psi;

                        process.Start();

                        process.WaitForInputIdle(10000);
                    }
                }
            }


            return null;
        }
    }
}
