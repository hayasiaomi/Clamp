using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Model
{
    public class SDApiResponse<T>
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public long Ts { get; set; }
        public T Data { get; set; }

        public bool IsSuccess()
        {
            return Code == 200 || Code == 0;
        }
    }
}
