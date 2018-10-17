using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.AIM.DTO
{
    public class Result<T>
    {
        public int ErrorCode { get; set; }
        public string ErrMsg { get; set; }
        public T Data { get; set; }
    }
}
