using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Network
{
    public class HResponse<T>
    {
        public string Msg { set; get; }

        public int Code { set; get; }

        public T Data { set; get; }

    }
}
