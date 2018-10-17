using ShanDian.Common.Commands;
using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Cmds
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
