using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.PrintTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.Print.Dto;

namespace ShanDian.AddIns.Print.Interface
{
    /// <summary>
    /// HTML打印服务
    /// </summary>
    public interface IHtmlPrintingServices
    {

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="printingName"></param>
        /// <param name="leift"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="copies"></param>
        void Print(PrintConfigDto printConfigDto, VoucherDto voucherDto, object data);

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="printingName"></param>
        /// <param name="leift"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="copies"></param>
        void Print(string path, string printingName, object data, int leift, int top, int width, int copies, string pcid);

        /// <summary>
        /// 轻餐开钱箱
        /// </summary>
        /// <param name="printConfigDto"></param>
        void LMOpenCashBox(PrintConfigDto printConfigDto);

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="printingName"></param>
        void Print(string path, string printingName, object data, string pcid);

        /// <summary>
        /// 打印支付凭证
        /// </summary>
        /// <param name="PaymentVoucherDto"></param>
        void PrintSfpz(PrintConfigDto printConfig, VoucherDto voucherDto, SfpzDto SfpzDto);

        /// <summary>
        /// 打印扫码点菜单的菜品
        /// </summary>
        /// <param name="SfpzDto"></param>
        void PrintSmdcd(PrintConfigDto printConfig, VoucherDto voucherDto, SmdcdDto smdcdDto);

        /// <summary>
        /// 打印退款凭证
        /// </summary>
        /// <param name="tkpzDto"></param>
        void PrintTkpz(PrintConfigDto printConfig, VoucherDto voucherDto, TkpzDto tkpzDto);

        /// <summary>
        /// 打印口碑预点单
        /// </summary>
        /// <param name="preOrderDto"></param>
        void PrintYddd(PrintConfigDto printConfig, VoucherDto voucherDto, PreOrderDto preOrderDto);

        /// <summary>
        /// 收银汇总单
        /// </summary>
        /// <param name="syhzdDto"></param>
        void PrintSyhzd(PrintConfigDto printConfig, VoucherDto voucherDto, SyhzdDto syhzdDto);

        /// <summary>
        /// 打印堂食汇总单
        /// </summary>
        /// <param name="tshzdDto"></param>
        void PrintTshzd(PrintConfigDto printConfig, VoucherDto voucherDto, TshzdDto tshzdDto);

        /// <summary>
        /// 打印机测试页
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="dycsDto"></param>
        void PrintDycs(PrintConfigDto printConfig, VoucherDto voucherDto, DycsDto dycsDto);

        /// <summary>
        /// 打印自动核销失败的单子
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="printFailDto"></param>
        void PrintPayFail(PrintConfigDto printConfig, VoucherDto voucherDto, PrintFailDto printFailDto);

        /// <summary>
        /// 打印轻餐版营业汇总单
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="printFailDto"></param>
        void PrintSummary(PrintConfigDto printConfig, VoucherDto voucherDto, PrintSummaryDto printFailDto);

        /// <summary>
        /// 本地打印
        /// </summary>
        /// <param name="printConfig">打印机参数信息</param>
        /// <param name="path">打印模板的路径</param>
        /// <param name="param">打印参数</param>
        void LocalPrintScheme(PrintConfigDto printConfig, string path, Object param, int num = 1);

        /// <summary>
        /// 通用打印
        /// </summary>
        /// <param name="printConfig">打印机参数信息</param>
        /// <param name="path">打印模板的路径</param>
        /// <param name="param">打印参数</param>
        void GlobalPrintScheme(PrintConfigDto printConfig, string path, Object param, int num );
        /// 打印轻餐菜品统计单
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="dishStatisticsDto"></param>
        void PrintDishStatistics(PrintConfigDto printConfig, VoucherDto voucherDto, DishStatisticsDto dishStatisticsDto);

        /// <summary>
        /// 通过本机打印的通用方法
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="dishStatisticsDto"></param>
        void PrintLocalMachine(PrintConfigDto printConfig, VoucherDto voucherDto, object data);

        /// <summary>
        /// 打印外卖
        /// </summary>
        /// <param name="preOrderDto"></param>
        void PrintTakeOrder(PrintConfigDto printConfig, VoucherDto voucherDto, TakeOutOrderDto takeOutOrderDto);
    }
}
