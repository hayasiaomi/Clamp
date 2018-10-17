using Newtonsoft.Json;
using ShanDian.Common.Commands;
using ShanDian.SDK.Framework.Advices;
using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Cmds
{
    public class SHELLACTIVITE : CommandBase
    {
        IWinFormService winFormService;

        public SHELLACTIVITE()
        {
            winFormService = SD.GetRequiredInstance<IWinFormService>();
        }

        protected override object DoHandle(string data)
        {
            winFormService.ActiviteSDShell();
            return null;
        }
    }
}
