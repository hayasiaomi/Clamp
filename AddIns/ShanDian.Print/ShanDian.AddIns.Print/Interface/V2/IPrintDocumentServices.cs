using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.PrintDataDto;

namespace ShanDian.AddIns.Print.Interface
{
    [RoutePrefix("", "Print")]
    public interface IPrintDocumentServices : ICommunicationServer
    {
        [Post("PrintScanCodeBill", "打印扫码点菜单")]
        void PrintScanCodeBill(BillData billData);

        [Post("PrintSaleSummary", "打印收银汇总单")]
        void PrintSaleSummary(SaleSummary saleSummary);

        [Post("PrintDineSummary", "打印堂食汇总单")]
        void PrintDineSummary(DineSummary dineSummary);

        [Post("PrintRefundVoucher", "打印退款凭证")]
        void PrintRefundVoucher(RefundVoucher refundVoucher);

        [Post("PrintPaymentVoucher", "打印支付凭证")]
        void PrintPaymentVoucher(PaymentVoucher paymentVoucher);

        [Post("PrintKbPreOrder", "打印口碑预点单")]
        void PrintKbPreOrder(KbPreOrder kbPreOrder);

        [Post("PrintTest", "打印测试页")]
        void PrintTest(PrintConfigDto printConfigDto);

        [Post("PrintTest/TemplateCode", "旧的测试打印单据(门店打印方案)")]
        void PrintTestByTemplateCode(string templateCode, string pcId);

        [Post("PrintTest/TemplateCodeV2", "新的打印方案下的测试单据")]
        void PrintTestByTemplateCodeV2(string templateCode, PrintConfigDto printConfig, int voucherCode);

        [Post("PrintTestScanCodeBill/TemplateCode", "测试打印扫码点菜单--页尾(本机打印方案)")]
        void TestPrintScanCodeBill(string pcId, string content, int isMiddle);

        [Post("PrintPayFail", "自动核销失败单据")]
        void PrintPayFail(PrintFailVoucher printFailVoucher);

        [Post("PrintSummary", "轻餐版营业汇总单")]
        void PrintSummary(SummaryVoucher summaryVoucher);

        [Post("LMOpenCashBox", "轻餐开钱箱")]
        void LMOpenCashBox(string pcid);

        [Post("PrintKitchen", "厨房打印")]
        void PrintKitchenBill(KitchenData kitchenData);

        [Post("PrintDishStatistics", "轻餐菜品统计单据")]
        void PrintDishStatistics(DishStatistics dishStatistics);

        [Post("PrintRestaurantShifts", "交接班")]
        void PrintRestaurantShifts(RestaurantShifts restaurantShifts);

        [Post("PrintTakeOutOrder", "外卖打印")]
        void PrintTakeOutOrder(TakeOutOrder takeOutOrder);
    }
}
