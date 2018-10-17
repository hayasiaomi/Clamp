using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.StandardVerSite
{
    public class BaseResult
    {
        public BaseResult()
        {
            ResultType = "0";
        }

        /// <summary>
        /// 成功：1 ，失败：0
        /// </summary>
        public string ResultType { get; set; }

        /// <summary>
        /// 错误编号
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 失败消息
        /// </summary>
        public string Message { get; set; }

        public void toSusccess()
        {
            ResultType = "1";
        }
    }
}
