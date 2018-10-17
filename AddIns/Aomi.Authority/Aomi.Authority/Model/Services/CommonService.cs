using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aomi.Common.Services
{
    public class CommonService : ICommonService
    {
        public string Hello(string name)
        {
            return $"你好! {name}";
        }
    }
}
