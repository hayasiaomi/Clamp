using Newtonsoft.Json;
using ShanDian.Common.Commands;
using ShanDian.Common.Pipelines;
using ShanDian.SDK.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Pipelines
{
    internal class SDPipelineListener : NamedPipeServer<string>, ISDListener<CommandPacket, object>
    {
        private readonly Dictionary<string, CommandPacket> unhandledCmdPacket = new Dictionary<string, CommandPacket>();

        public SDPipelineListener() : base("SDPIPELINES")
        {

        }

        public bool IsListening { get { return this.IsRunning; } }

        public Func<CommandPacket, object> HandleMessage { set; get; }

        public void Shutdown()
        {
            this.Stop();
        }

        public void StartListen()
        {
            this.Start();
        }
    }
}
