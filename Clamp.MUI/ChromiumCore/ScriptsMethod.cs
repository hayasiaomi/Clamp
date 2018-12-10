using Chromium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.ChromiumCore
{
    class ScriptsMethod
    {
        public CfrV8Value Method { private set; get; }
        public CfrV8Context V8Context { private set; get; }
        public ScriptsMethod(CfrV8Context context, CfrV8Value method)
        {
            this.V8Context = context;
            this.Method = method;
        }
    }
}
