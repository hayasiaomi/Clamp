using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.NancyExpand;

namespace ShanDian.AddIns.Print.Data
{
    public class PrintErrorCode:SystemError<PrintErrorCode>
    {
        /// <summary>
        /// 违规操作
        /// </summary>
        public int IllegalOperationError = 2001;

        /// <summary>
        /// 结果为空
        /// </summary>
        public int ResultNull = 2002;

        /// <summary>
        /// 数据更新失败
        /// </summary>
        public int UpdateError = 2003;

        /// <summary>
        /// 重复添加
        /// </summary>
        public int RepeatedError = 2004;

        /// <summary>
        /// 模板为空
        /// </summary>
        public int VoucherNullError = 2010;

        /// <summary>
        /// 打印方案为空
        /// 未配置打印方案
        /// </summary>
        public int PrintSchemeNullError = 2011;

        /// <summary>
        /// 打印机为空
        /// </summary>
        public int PrintConfigNullError = 2012;

        /// <summary>
        /// 同一凭证不可添加相同打印机
        /// </summary>
        public int SamePrinterError = 2013;

        /// <summary>
        /// 获取到的打印方案组为空
        /// </summary>
        public int PrintGroupNullError = 3001;

        /// <summary>
        /// 同一方案不可添加相同打印机
        /// </summary>
        public int SamePrintSchemeError = 3002;

        /// <summary>
        /// 编辑高级设置失败
        /// </summary>
        public int EditTopSetInfoError = 3003;

        /// <summary>
        /// 添加打印机时间戳错误
        /// </summary>
        public int TimeStampError = 3004;

        /// <summary>
        /// 当前编辑的打印方案错误
        /// </summary>
        public int GetModifyPrintSchemeError = 3005;

        /// <summary>
        /// 编辑打印方案失败
        /// </summary>
        public int ModifyPrintSchemeError = 3006;

        /// <summary>
        /// 打印方案组不存在
        /// </summary>
        public int GetPrintGroupNullError = 3007;

        /// <summary>
        /// 请配置打印方案
        /// </summary>
        public int GetPrintGroupSchemeNullError = 3008;
    }
}
