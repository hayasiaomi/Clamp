using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Common.Commands
{
    public class CommandResult
    {
        public bool IsSucceed { set; get; }

        public object Paramters { set; get; }

        public string Mistake { set; get; }
    }
}
