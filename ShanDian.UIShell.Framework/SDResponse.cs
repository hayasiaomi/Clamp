using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework
{
    public class SDResponse<TResult>
    {
        public bool Flag { set; get; }

        public TResult Result { set; get; }

        public string Message { set; get; }

        public int Code { set; get; }

        public string SerializeObject()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
