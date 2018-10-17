using Clamp.Common;
using Clamp.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Helpers
{
    public class SDPipelineHelper
    {
        private static bool IsInstalled = false;
        private static SDPipelineClient SDPipelineClient;

        public static Func<SDPipelineCommand, object> HandlePipelineCommand;

        public static void Setup(string possessor)
        {
            if (!IsInstalled)
            {
                SDPipelineClient = new SDPipelineClient(possessor);
                SDPipelineClient.HandleMessage = HandlePipelineMessage;
                SDPipelineClient.Start();
                IsInstalled = true;
            }
        }

        public static void Shutdown()
        {
            if (SDPipelineClient != null)
                SDPipelineClient.Stop();

            IsInstalled = false;
        }

        private static object HandlePipelineMessage(SDPipelineCommand command)
        {
            if (HandlePipelineCommand != null)
                return HandlePipelineCommand(command);
            return null;
        }
    }
}
