using System;

namespace ShanDian.Common.Exceptions
{
    public class ShanDianException : Exception
    {
        public int Code { get; set; }

        public ShanDianException(int code)
        {
            Code = code;
        }

        public ShanDianException(int code, string msg) : base(msg)
        {
            Code = code;
        }

    }
}