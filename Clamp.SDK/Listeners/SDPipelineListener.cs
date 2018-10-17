using Newtonsoft.Json;
using Clamp.Common.Commands;
using Clamp.Common.Pipelines;
using Clamp.SDK.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Pipelines
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
