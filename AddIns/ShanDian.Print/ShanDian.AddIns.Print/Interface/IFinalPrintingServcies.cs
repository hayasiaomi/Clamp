using Hydra.Framework.NancyExpand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Interface
{
    /// <summary>
    /// 终端打印服务类
    /// </summary>
    [RoutePrefix("", "FinalPrinter")]
    public interface IFinalPrintingServcies
    {
        /// <summary>
        /// 终端打印
        /// </summary>
        /// <param name="message"></param>
        [Get("FinalPrinting")]
        void FinalPrinting(string message);
    }
}
