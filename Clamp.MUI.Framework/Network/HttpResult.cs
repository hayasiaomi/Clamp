using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.Network
{
    public class HttpResult<TResult>
    {
        public bool Flag { set; get; }

        public TResult Result { set; get; }

        public string Message { set; get; }

        public int Code { set; get; }

        //{"Flag":true,"Result":{"UserId":13,"Token":"f7a1c88a-2580-47c2-8211-c41d11a5e1fd"},"Message":null,"Code":0}
    }
}
