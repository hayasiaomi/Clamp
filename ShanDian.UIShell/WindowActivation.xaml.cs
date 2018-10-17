using Newtonsoft.Json;
using ShanDian.UIShell.Forms;
using ShanDian.UIShell.Framework;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.Model;
using ShanDian.UIShell.Framework.Network;
using ShanDian.UIShell.Framework.Services;
using ShanDian.UIShell.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace ShanDian.UIShell
{
    /// <summary>
    /// WindowActivited.xaml 的交互逻辑
    /// </summary>
    public partial class WindowActivited : Window
    {
        public WindowActivited()
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

        private void TxtRestCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.txtRestCode.Text))
            {
                this.txtbRestCodeHint.Visibility = Visibility.Hidden;
            }
            else
            {
                this.txtbRestCodeHint.Visibility = Visibility.Visible;
            }

            this.txtRestCodeErrorMessage.Text = string.Empty;
            this.txtRestCodeErrorMessage.Visibility = Visibility.Hidden;

        }

        private void DisplayProgressBar(string busyText)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.loadingWait.BusyText = busyText;
                this.loadingWait.Visibility = Visibility.Visible;
            }));
        }

        private void HideProgressBar()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.loadingWait.Visibility = Visibility.Collapsed;
            }));
        }

        private void BtnActivited_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayProgressBar(SDResources.Activited_ValidateBusyText);

            this.txtRestCodeErrorMessage.Text = string.Empty;
            this.txtRestCodeErrorMessage.Visibility = Visibility.Hidden;

            string restCode = this.txtRestCode.Text;

            if (string.IsNullOrWhiteSpace(restCode))
            {
                this.DisplayRestCodeErrorMessage(SDResources.Activited_RestCodeNotBlank);
                this.HideProgressBar();
                return;
            }

            new Thread(new ThreadStart(() =>
            {
                if (this.ActivitedStore(restCode))
                {
                    Task.Factory.StartNew(() =>
                    {
                        MachineService.UploadSoftware();
                    });
                }
            })).Start();
        }

        public bool ActivitedStore(string storeId)
        {
            try
            {

                SDResponse<ActivitedInfo> response = InstallService.ActivitedStore(storeId);

                if (response.Flag || response.Result == null)
                {
                    this.DisplayRestCodeErrorMessage(string.Format(SDResources.Activited_RestCodeActivitedFailure, SDResources.HttpAccessor_BadData));

                    this.HideProgressBar();

                    return false;
                }

                ActivitedInfo activitedResult = response.Result;

                if (!activitedResult.IsBind)//没有被绑定，激活成功
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        SDShellHelper.SaveMerchantNo(storeId);

                        WindowAuthority windowAuthority = new WindowAuthority();

                        Application.Current.MainWindow = windowAuthority;

                        windowAuthority.Show();

                        this.Close();

                    }));

                    return true;
                }
                else
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.gridMainFrame.Visibility = Visibility.Hidden;
                        this.gridFinalStep.Visibility = Visibility.Visible;

                        string[] words = activitedResult.MainIp.Split(',');

                        if (words != null && words.Length > 0)
                        {
                            for (int i = 0; i < words.Length; i++)
                            {
                                this.txtbMainIpString.Inlines.Add(words[i]);

                                if (i != words.Length - 1)
                                {
                                    this.txtbMainIpString.Inlines.Add(new LineBreak());
                                }
                            }

                            this.txtbMainIpString.Tag = activitedResult.MainIp;
                        }
                        this.txtbMainInstallTime.Text = activitedResult.ActiveTime;
                        this.txtbMainOnLine.Text = activitedResult.Online;
                    }));

                    this.HideProgressBar();
                }
            }
            catch (Exception ex)
            {
                this.DisplayRestCodeErrorMessage(ex.Message);
                this.HideProgressBar();
            }

            return false;
        }

        private void DisplayRestCodeErrorMessage(string errorMessage)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.txtRestCodeErrorMessage.Visibility = Visibility.Visible;
                this.txtRestCodeErrorMessage.Text = errorMessage;
            }));
        }


        private void BtnMainKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (this.gridMainKeyboard.Visibility == Visibility.Visible)
            {
                this.gridMainKeyboard.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.gridMainKeyboard.Visibility = Visibility.Visible;
            }
        }

        private void BtnSubVirtualKey_Click(object sender, RoutedEventArgs e)
        {
            MiniKeyboard.Input(Convert.ToInt32((sender as Button).DataContext));
        }

        private void BtnMainVirtualKey_Click(object sender, RoutedEventArgs e)
        {
            MiniKeyboard.Input(Convert.ToInt32((sender as Button).DataContext));
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="stepMainframe_RestCodeNotBlank"></param>
        private void DisplaySubIpErrorMessage(string errorMessage)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.txtbSubErrorMessage.Visibility = Visibility;
                this.txtbSubErrorMessage.Text = errorMessage;
            }));
        }

        private void BtnInstallSub_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayProgressBar(SDResources.Activited_InstallSubBusyText);

            string ipString = this.txtbMainIpString.Tag != null ? Convert.ToString(this.txtbMainIpString.Tag) : string.Empty;

            new Thread(new ThreadStart(() =>
            {
                InstallSubInfo installSubInfo = InstallService.InstallSub("192.168.146.130");

                if (installSubInfo != null)
                {
                    if (installSubInfo.IsIntalled)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            WindowAuthority windowAuthority = new WindowAuthority();

                            Application.Current.MainWindow = windowAuthority;

                            windowAuthority.Show();

                            this.Close();

                        }));

                        this.HideProgressBar();
                    }
                    else
                    {
                        this.DisplaySubIpErrorMessage(installSubInfo.ErrorMessage);
                        this.HideProgressBar();
                    }
                }
                else
                {
                    this.DisplaySubIpErrorMessage(SDResources.Activited_InstallSubError);
                    this.HideProgressBar();
                }

            })).Start();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FrmSplash.CloseForm();
            this.Activate();
            this.txtRestCode.Focus();
        }

        private void ProxyPartial_OnProxyBackClick(object sender, ProxyRoutedEventArgs e)
        {
            this.gridActivitedView.Visibility = Visibility.Visible;
            this.gridProxyView.Visibility = Visibility.Hidden;
        }

        private void ProxyPartial_OnProxyOkClick(object sender, ProxyRoutedEventArgs e)
        {
            this.gridActivitedView.Visibility = Visibility.Visible;
            this.gridProxyView.Visibility = Visibility.Hidden;
        }

        private void BtnProxy_Click(object sender, RoutedEventArgs e)
        {
            this.gridActivitedView.Visibility = Visibility.Hidden;
            this.gridProxyView.Visibility = Visibility.Visible;
        }
    }
}
