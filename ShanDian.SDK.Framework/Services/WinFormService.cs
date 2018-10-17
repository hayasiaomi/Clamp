using Newtonsoft.Json;
using ShanDian.Common.Commands;
using ShanDian.Common.Pipelines;
using ShanDian.SDK.Framework.Advices;
using ShanDian.SDK.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Management;

namespace ShanDian.SDK.Framework.Services
{
    public class WinFormService : IWinFormService
    {
        private NamedPipeServer<string> namedPipeServer;

        public void ActiviteSDShell()
        {
            if (namedPipeServer != null)
            {
                //var connection = namedPipeServer.GetConnection("SDShell");
                var commander = namedPipeServer.GetCommanderWithResult(Constants.PipelineSDShellName);
                if (commander != null && commander.IsConnected)
                {
                    CommandPacket commandPacket = new CommandPacket();

                    commandPacket.Id = Guid.NewGuid().ToString("N");
                    commandPacket.CommandName = "ShellActivate";
                    commandPacket.Possessor = Constants.PipelineSDShellName;
                    commandPacket.CommandType = CommandType.Ask;

                    string message = JsonConvert.SerializeObject(commandPacket);

                    //connection.PushMessage(message);

                    //connection.ReadMessage();
                    commander.GetResponse(message);
                }
            }
        }

        public void DisplayNotice(Notice notice)
        {
            if (namedPipeServer != null)
            {
                //var connection = namedPipeServer.GetConnection("UIShellAssist");
                var commander = namedPipeServer.GetCommanderWithResult(Constants.PipelineSDShellAssitName);
                if (commander != null && commander.IsConnected)
                {
                    CommandPacket commandPacket = new CommandPacket();

                    commandPacket.Id = Guid.NewGuid().ToString("N");
                    commandPacket.CommandName = "DispalyNotice";
                    commandPacket.Possessor = Constants.PipelineSDShellAssitName;
                    commandPacket.CommandType = CommandType.Ask;
                    commandPacket.Data = JsonConvert.SerializeObject(notice);

                    string message = JsonConvert.SerializeObject(commandPacket);

                    //connection.PushMessage(message);

                    //connection.ReadMessage();
                    commander.GetResponse(message);
                }
            }
        }

        public void Upgrade(string versionCode)
        {
            if (namedPipeServer != null)
            {
                //var connection = namedPipeServer.GetConnection("SDShell");
                var commander = namedPipeServer.GetCommanderWithResult(Constants.PipelineSDShellName);

                if (commander != null && commander.IsConnected)
                {
                    CommandPacket commandPacket = new CommandPacket();

                    commandPacket.Id = Guid.NewGuid().ToString("N");
                    commandPacket.CommandName = "Upgrade";
                    commandPacket.Possessor = Constants.PipelineSDShellName;
                    commandPacket.CommandType = CommandType.Ask;
                    commandPacket.Data = versionCode;

                    string message = JsonConvert.SerializeObject(commandPacket);

                    //connection.PushMessage(message);

                    //connection.ReadMessage();
                    commander.GetResponse(message);
                }
            }
        }

        public void Setup(NamedPipeServer<string> namedPipeServer)
        {
            this.namedPipeServer = namedPipeServer;
        }

        public List<LocalPrinterInfo> GetLocalPrinterInfos()
        {
            List<LocalPrinterInfo> printerInfos = new List<LocalPrinterInfo>();

            //if (namedPipeServer != null)
            //{
            //    var commander = namedPipeServer.GetCommanderWithResult(Constants.PipelineSDShellName);

            //    if (commander != null && commander.IsConnected)
            //    {
            //        CommandPacket commandPacket = new CommandPacket();

            //        commandPacket.Id = Guid.NewGuid().ToString("N");
            //        commandPacket.CommandName = "GetPrinterInfos";
            //        commandPacket.Possessor = Constants.PipelineSDShellName;
            //        commandPacket.CommandType = CommandType.Ask;

            //        string cmdReulst = commander.GetResponse(JsonConvert.SerializeObject(commandPacket));

            //        if (!string.IsNullOrWhiteSpace(cmdReulst))
            //        {
            //            List<LocalPrinterInfo> mPrinterInfos = JsonConvert.DeserializeObject<List<LocalPrinterInfo>>(cmdReulst);

            //            if (mPrinterInfos != null && mPrinterInfos.Count > 0)
            //            {
            //                printerInfos.AddRange(mPrinterInfos);
            //            }

            //        }

            //        return printerInfos;
            //    }
            //}

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");

            foreach (ManagementObject service in searcher.Get())
            {
                LocalPrinterInfo printerInfo = new LocalPrinterInfo();

                printerInfo.PrintName = service.Properties["Name"].Value.ToString();
                printerInfo.State = Convert.ToBoolean(service.Properties["WorkOffline"].Value) ? 0 : 1;

                printerInfos.Add(printerInfo);
            }

            return printerInfos;
        }

        public bool IsConnected(string name)
        {
            if (namedPipeServer != null)
            {
                var commander = namedPipeServer.GetCommanderWithResult(name);

                if (commander != null && commander.IsConnected)
                {
                    return commander.IsConnected;
                }
            }

            return false;
        }

    }
}
