using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Commands
{
    interface ICommand
    {
        CommandResult Handle(string data);
    }
}
