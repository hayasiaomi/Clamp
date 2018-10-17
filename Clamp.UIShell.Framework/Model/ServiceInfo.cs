using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Model
{
    public class ServiceInfo<TResult>
    {
        public bool Flag { set; get; }

        public TResult Result { set; get; }

        public string Message { set; get; }

        public int Code { set; get; }
    }
}
