using ShanDian.Common.Commands;
using ShanDian.SDK.Cmds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShanDian.SDK
{
    public class SDCmdManager
    {
        private static Dictionary<string, CommandBase> cmdCache = new Dictionary<string, CommandBase>();

        public static CommandResult Handle(string cmdName, string data)
        {
            CommandBase command;

            if (cmdCache.ContainsKey(cmdName))
            {
                command = cmdCache[cmdName];
            }
            else
            {
                string cmdFullname = $"{ typeof(SDExecutor).Namespace }.Cmds.{cmdName.ToUpper()}";

                Type cmdType = typeof(SDCmdManager).Assembly.GetType(cmdFullname, false, true);

                if (cmdType != null)
                {
                    command = Activator.CreateInstance(cmdType) as CommandBase;
                }
                else
                {
                    return new CommandResult() { IsSucceed = false, Mistake = "找不到对应的命令" };
                }
            }

            return command.Handle(data);
        }
    }
}
