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
    public class DISPLAYNOTICE : CommandBase
    {
        IWinFormService winFormService;

        public DISPLAYNOTICE()
        {
            winFormService = SD.GetRequiredInstance<IWinFormService>();
        }

        protected override object DoHandle(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                Notice notice = JsonConvert.DeserializeObject<Notice>(data);

                winFormService.DisplayNotice(notice);
            }

            return null;
        }
    }
}
