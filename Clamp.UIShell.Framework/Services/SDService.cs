using Clamp.Common.HTTP;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clamp.UIShell.Framework.Services
{
    public class SDService
    {
        public static DemandInfo GetDemandInfo()
        {
            int timer = 0;

            do
            {
                HttpResponse httpResponse = HttpRequest.Get(SDShellHelper.GetSelfHost("/demand"));

                if (httpResponse != null)
                {
                    return httpResponse.AsDeserializeBody<DemandInfo>();
                }

                timer++;

                Thread.Sleep(800);

            } while (timer < 3);


            return null;
        }


        public static SystemInfo GetSystemInfo()
        {
            int timer = 0;

            do
            {
                HttpResponse httpResponse = HttpRequest.Get(SDShellHelper.GetSelfHost("/status"));

                if (httpResponse != null)
                {
                    SystemInfo systemInfo = httpResponse.AsDeserializeBody<SystemInfo>();

                    if (systemInfo != null)
                        return systemInfo;
                }

                timer++;

            } while (timer < 3);


            return null;
        }

    }
}
