using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Clamp.UIShell.Framework
{

    public delegate void ProxyRoutedEventHandler(object sender, ProxyRoutedEventArgs e);

    public class ProxyRoutedEventArgs : RoutedEventArgs
    {
        public string TypeName { set; get; }
        public string Server { set; get; }

        public string Port { set; get; }
    }

    public class ProxyInfo
    {
        public string TypeName { set; get; }
        public string Server { set; get; }

        public string Port { set; get; }
    }
}
