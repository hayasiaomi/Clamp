using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Common.Commands
{
    interface ICommand
    {
        CommandResult Handle(string data);
    }
}
