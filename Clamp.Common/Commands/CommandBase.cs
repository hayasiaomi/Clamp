using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Commands
{
    public abstract class CommandBase : ICommand
    {
        public CommandResult Handle(string data)
        {
            CommandResult commandResult = new CommandResult();

            try
            {
                commandResult.Paramters = this.DoHandle(data);

                commandResult.IsSucceed = true;
            }
            catch (Exception ex)
            {
                commandResult.IsSucceed = false;
                commandResult.Mistake = ex.Message;
            }

            return commandResult;
        }

        protected abstract object DoHandle(string data);
    }
}
