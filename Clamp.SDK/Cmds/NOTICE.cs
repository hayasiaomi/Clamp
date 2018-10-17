using Clamp.Common.Commands;
using Clamp.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Cmds
{
    public class NOTICE : CommandBase
    {
        protected override object DoHandle(string @params)
        {
            IWinFormService winFormService = SD.GetRequiredInstance<IWinFormService>();

            //winFormService.Notice("i am aomi");

            return null;
        }
    }
}
