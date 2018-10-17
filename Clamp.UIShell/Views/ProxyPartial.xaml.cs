using Clamp.UIShell.Framework;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Clamp.UIShell.Views
{
    /// <summary>
    /// ProxyPartial.xaml 的交互逻辑
    /// </summary>
    public partial class ProxyPartial : UserControl
    {
        public event ProxyRoutedEventHandler OnProxyBackClick;
        public event ProxyRoutedEventHandler OnProxyOkClick;

        public ProxyPartial()
        {
            InitializeComponent();
        }

        private void WindowHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window.GetWindow(this).DragMove();
        }

        private void BtnProxyBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnProxyOkClick != null)
                this.OnProxyOkClick(this, new ProxyRoutedEventArgs());
        }

        private void BtnProxyOk_Click(object sender, RoutedEventArgs e)
        {
            this.DisplayMessgae("", Brushes.Green);

            if (!string.IsNullOrWhiteSpace(this.txtProxyAddress.Text))
            {
                IPAddress ip;

                bool validateIP = IPAddress.TryParse(this.txtProxyAddress.Text, out ip);

                if (!validateIP)
                {
                    this.DisplayMessgae(SDResources.AuthorityView_InValidteIpText, Brushes.Red);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(this.txtProxyPort.Text))
            {
                int port;

                if (!int.TryParse(this.txtProxyPort.Text, out port))
                {
                    this.DisplayMessgae(SDResources.AuthorityView_InValidtePortText, Brushes.Red);
                    return;
                }
            }

            string proxyFile = System.IO.Path.Combine(SDShell.SDRootPath, "Proxy.json");

            ProxyRoutedEventArgs args = new ProxyRoutedEventArgs();

            if (!string.IsNullOrWhiteSpace(this.txtProxyAddress.Text) && !string.IsNullOrWhiteSpace(this.txtProxyPort.Text))
            {
                args.Server = this.txtProxyAddress.Text;
                args.TypeName = this.txtProxyProtocol.Text;
                args.Port = this.txtProxyPort.Text;

                ProxyInfo proxyInfo = new ProxyInfo();

                proxyInfo.TypeName = args.TypeName;
                proxyInfo.Server = args.Server;
                proxyInfo.Port = args.Port ?? "15802";

                File.WriteAllText(proxyFile, JsonConvert.SerializeObject(proxyInfo));

                SDShell.ChromiumWindow.ChangeProxyInfo(proxyInfo.TypeName, proxyInfo.Server, proxyInfo.Port);

                this.DisplayMessgae(SDResources.AuthorityView_ProxySuccessText, Brushes.Green);
            }
            else
            {
                if (File.Exists(proxyFile))
                    File.Delete(proxyFile);
            }

            if (this.OnProxyOkClick != null)
                this.OnProxyOkClick(this, args);
        }

        private void BtnProxyTest_Click(object sender, RoutedEventArgs e)
        {
            string typeName = this.txtProxyProtocol.Text;
            string server = this.txtProxyAddress.Text;
            string port = this.txtProxyPort.Text;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.baidu.com");
                WebProxy myproxy = new WebProxy(server, Convert.ToInt32(port));
                myproxy.BypassProxyOnLocal = false;
                request.Proxy = myproxy;
                request.Method = "GET";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    this.DisplayMessgae(SDResources.AuthorityView_ProxySuccessText, Brushes.Green);
                }
                else
                {
                    this.DisplayMessgae(SDResources.AuthorityView_ProxyFailureText, Brushes.Red);
                }

            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);

                this.DisplayMessgae(SDResources.AuthorityView_ProxyFailureText, Brushes.Red);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string proxyFile = System.IO.Path.Combine(SDShell.SDRootPath, "Proxy.json");

            if (File.Exists(proxyFile))
            {
                string proxyJson = File.ReadAllText(proxyFile, Encoding.UTF8);

                ProxyInfo proxyInfo = JsonConvert.DeserializeObject<ProxyInfo>(proxyJson);

                if (proxyInfo != null)
                {
                    this.txtProxyAddress.Text = proxyInfo.Server;
                    this.txtProxyPort.Text = proxyInfo.Port;
                }
            }
        }

        private void DisplayMessgae(string message, SolidColorBrush solidColor)
        {
            this.txtbProxyMessage.Foreground = solidColor;
            this.txtbProxyMessage.Text = message;
        }
    }



}
