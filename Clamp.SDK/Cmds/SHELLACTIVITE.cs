using Newtonsoft.Json;
using Clamp.Common.Commands;
using Clamp.SDK.Framework.Advices;
using Clamp.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Cmds
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
