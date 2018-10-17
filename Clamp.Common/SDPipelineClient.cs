using Newtonsoft.Json;
using Clamp.Common.Commands;
using Clamp.Common.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common
{
    public class SDPipelineClient : NamedPipeClient<string>
    {
        public Func<SDPipelineCommand, object> HandleMessage { set; get; }

        public SDPipelineClient(string possessor) : base("SDPIPELINES")
        {
            this.Possessor = possessor;
        }

        protected override void OnReceiveMessage(NamedPipeConnection<string, string> connection, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                CommandPacket commandPacket = JsonConvert.DeserializeObject<CommandPacket>(message);

                if (commandPacket != null)
                {
                    if (commandPacket.CommandType == CommandType.Ask)
                    {
                        object result = null;

                        if (this.HandleMessage != null)
                        {
                            SDPipelineCommand command = new SDPipelineCommand();

                            command.CommandName = commandPacket.CommandName;
                            command.Id = commandPacket.Id;
                            command.Data = commandPacket.Data;

                            result = this.HandleMessage(command);
                        }

                        CommandPacket backPacket = new CommandPacket();
                        backPacket.CommandType = CommandType.Reply;
                        backPacket.Id = commandPacket.Id;
                        backPacket.Possessor = commandPacket.Possessor;

                        CommandResult commandResult = new CommandResult();
                        commandResult.IsSucceed = true;

                        backPacket.Data = JsonConvert.SerializeObject(commandResult);

                        string backMessage = JsonConvert.SerializeObject(backPacket);

                        connection.PushMessage(backMessage);
                    }
                }
            }
        }
    }

    public class SDPipelineCommand
    {
        public string Id { set; get; }

        public string CommandName { set; get; }

        public string Data { set; get; }

        public bool Compare(string cmdName)
        {
            return string.Equals(cmdName, this.CommandName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
