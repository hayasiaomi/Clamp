using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Clamp.Printer.Printer;
using Clamp.SDK.Framework.Services;

namespace Clamp.Print
{
    public class NetPrinter : EscPrinter
    {
        public override void Print(string ip,string port,byte[] data)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            LoggingService.Debug($"开始连接网络打印机:ip:{ip},port:{port}");
            socket.Connect(IPAddress.Parse(ip), Convert.ToInt32(port));
            LoggingService.Debug($"开始发送指令，length：{data.Length}");
            var sendCount = socket.Send(data);
            LoggingService.Debug($"发送成功，sendCount：{sendCount}");
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            LoggingService.Debug($"关闭连接");
        }

        
    }
}