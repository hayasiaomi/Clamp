using Chromium;
using Chromium.WebBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.Handlers
{
    public class ClampHandlerContext
    {
        public ChromiumWebBrowser ChromiumWebBrowser { set; get; }

        public string Url { set; get; }

        public List<string> Datas { set; get; }

        public ClampHandlerContext()
        {

        }
    }
}
