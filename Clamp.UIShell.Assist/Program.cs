using CefSharp;
using Clamp.UIShell.Assist.Brower;
using Newtonsoft.Json;
using Clamp.Common;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework.InterProcess;
using Clamp.UIShell.Framework.Model;
using Clamp.UIShell.Framework.Services;
using Clamp.UIShell.Assist;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Management;

namespace Clamp.UIShell.Assist
{
    class Program
    {
        private static Mutex mutex = new Mutex(true, "CB76303F-9AA7-461F-BD7A-BE09CBDABFA6");

        internal static WindowAdvices WindowAdvices;

        [STAThread]
        public static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.FromSeconds(2), true))
            {
                NLogService.Info("args:" + args);

                if (args != null && args.Length > 0)
                {
                    string dataString = Encoding.UTF8.GetString(Convert.FromBase64String(args[0]));

                    NLogService.Info("dataString:" + dataString);

                    if (!string.IsNullOrWhiteSpace(dataString))
                    {
                        ProcessCredential processCredential = JsonConvert.DeserializeObject<ProcessCredential>(dataString);

                        if (processCredential != null && string.Equals(processCredential.Token, "ShanDian", StringComparison.CurrentCultureIgnoreCase))
                        {

                            SDShellHelper.GetSDShellSettings();
                            SDShellHelper.GetDemand();

                            Cef.EnableHighDPISupport();

                            CefBrower.Init(true, true);

                            NLogService.Info("Token:" + processCredential.Token);

                            SDPipelineHelper.Setup("UIShellAssist");
                            SDPipelineHelper.HandlePipelineCommand = HandlePipelineCommand;

                            App app = new App();
                            app.InitializeComponent();

                            MainWindow mainWindow = new MainWindow();

                            mainWindow.BindMainProcesss(processCredential.ProcessId);

                            NLogService.Info("UIShellPID:" + mainWindow.SDShellPID);

                            app.MainWindow = mainWindow;
                            app.Run(mainWindow);

                            Cef.Shutdown();

                            mutex.ReleaseMutex();

                        }
                    }
                }
            }
        }

        private static object HandlePipelineCommand(SDPipelineCommand command)
        {
            if (command != null)
            {
                NLogService.Info("command.Data:" + command.Data);

                if (string.Equals("DispalyNotice", command.CommandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Notice notice = JsonConvert.DeserializeObject<Notice>(command.Data);

                    if (notice != null && Program.WindowAdvices != null)
                        Program.WindowAdvices.DisplayNotice(notice);

                    return null;
                }
                else if (string.Equals("ApplicationExit", command.CommandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Application.Current.MainWindow.Close();

                    return null;
                }
                else if (string.Equals("GetPrinterInfos", command.CommandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    List<PrinterInfo> printerInfos = new List<PrinterInfo>();

                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");

                    foreach (ManagementObject service in searcher.Get())
                    {
                        PrinterInfo printerInfo = new PrinterInfo();

                        printerInfo.PrintName = service.Properties["Name"].Value.ToString();
                        printerInfo.State = Convert.ToBoolean(service.Properties["WorkOffline"].Value) ? 1 : 0;

                        printerInfos.Add(printerInfo);
                    }

                    return JsonConvert.SerializeObject(printerInfos);
                }

            }

            return null;
        }
    }
}
