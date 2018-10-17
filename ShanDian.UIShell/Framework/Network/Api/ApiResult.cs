using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Network.Api
{
    internal class ApiResult
    {
        public string Message { set; get; }

        public int Code { set; get; }

        public string Data { set; get; }
    }
}
