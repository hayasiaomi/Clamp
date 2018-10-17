using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.Partial.Mqtt;
using Hydra.Framework.Services.Aop;
using Hydra.Framework.Services.Aop.RegistrationAttributes;
using Hydra.Framework.Services.Utility;
using Hydra.Framework.SqlContent;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.PrintDataDto;
using ShanDian.AddIns.Print.Dto.Print.Dto;
using ShanDian.AddIns.Print.Dto.PrintTemplate;
using ShanDian.AddIns.Print.Dto.PrintTemplate.Kitchen;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Model;
using ShanDian.AddIns.Print.Data;
using Newtonsoft.Json;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Module
{
    [As(typeof(IPrintDocumentServices))]
    public class PrintDocumentServicesV2 : IPrintDocumentServices
    {
        private IRepositoryContext _repositoryContext;
        private string _restaurantName = "";
        private string _restaurantSubName = "";
        private string _currency = "¥";

        public PrintDocumentServicesV2()
        {
            _repositoryContext = Global.RepositoryContext();
            IRestServices restaurantServices = null;
            var restaurantDto = restaurantServices.GetRestInfo();
            if (restaurantDto == null) return;
            _restaurantName = restaurantDto.Name;
            _restaurantSubName = restaurantDto.SubName;
        }

        /// <summary>
        /// 打印扫码点菜单
        /// </summary>
        /// <param name="billData"></param>
        public void PrintScanCodeBill(BillData billData)
        {
            if (billData == null)
            {
                Flag = false;
                Message = "扫码点菜单打印参数为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintScanCodeBill data:" + JsonConvert.SerializeObject(billData));
            var templateCode = "PRT_SO_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                Message = "扫码点菜单模板为空";
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }

            billData.VoucherId = voucher.Id;

            var Print = ServiceLocator.Instance.Resolve<IPrint>();

            var smdcdDto = new SmdcdDto
            {
                IsCheckout = billData.IsCheckout,
                IsFailured = billData.IsFailured,
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                TableNo = billData.TableName,
                BillNo = billData.OrderNo,
                PersonCount = billData.Person,
                OrderTakeDate = billData.CreateTime,
                Remark = billData.Remark,
                Dishes = new List<SmdcdDishDto>(),
                TotalAmount = billData.TotalMoney,
                DiscountAmounts = new List<SmdcdDiscountAmountDto>(),
                ShouldAmount = billData.RealMoney,
                PayTypeAmounts = new List<SmdcdPayTypeAmountDto>(),
                InvoiceTitle = billData.InvoiceTitle,
                TaxpayerIdentify = billData.TaxpayerNo,
                ExceptionDishes = new List<SmdcdDishDto>(),
                ExceptionReason = billData.FailReason,
                OrderOrigin = "扫码点菜",
            };
            var footer = Print.GetSMDCDFooter();
            if (footer != null)
            {
                smdcdDto.Footer = footer.Content;
                smdcdDto.Footer = footer.Content.Replace("\\r\\n", "<br />").Replace("\r\n", "<br />").Replace("\\n", "<br />").Replace("\n", "<br />").Replace(Environment.NewLine, "<br />");

                if (footer.Alignment == 1)
                {
                    smdcdDto.FooterAlignment = "center";
                }
                else
                {
                    smdcdDto.FooterAlignment = "left";
                }
            }
            smdcdDto.Dishes = GetSmdcdDishDtos(billData.BillDish, billData.Tags, billData.OrderSetMealDatas);
            smdcdDto.ExceptionDishes =
                GetSmdcdDishDtos(billData.FailBillDish, billData.Tags, billData.OrderSetMealDatas);

            if (billData.DiscountDetails != null)
            {
                foreach (var discount in billData.DiscountDetails)
                {
                    var smdcdDiscount = new SmdcdDiscountAmountDto
                    {
                        DiscountName = discount.Name,
                        Price = discount.Amount
                    };

                    smdcdDto.DiscountAmounts.Add(smdcdDiscount);
                }
            }
            if (billData.PayDetails != null)
            {
                foreach (var payDetail in billData.PayDetails)
                {
                    var payTypeAmountDto = new SmdcdPayTypeAmountDto
                    {
                        PayTypeName = payDetail.PayName,
                        Price = payDetail.Receiptamount
                    };
                    smdcdDto.PayTypeAmounts.Add(payTypeAmountDto);
                }
            }
            smdcdDto.TotalDishCount = smdcdDto.ExceptionDishes.Count;
            smdcdDto.TotalDishPrice = smdcdDto.ExceptionDishes.Sum(x => x.UnitPrice);
            if (billData.PriceDetails != null && billData.PriceDetails.Count > 0)
            {
                foreach (var prtItem in billData.PriceDetails)
                {
                    SmdcdPriceDetail newResult = new SmdcdPriceDetail();
                    newResult.Price = prtItem.Price;
                    newResult.PriceTypeName = prtItem.PriceTypeName;
                    smdcdDto.PriceDetails.Add(newResult);
                }
            }
            PrintEnumCode.CheckBillCode billCode = new PrintEnumCode.CheckBillCode();
            billCode = (PrintEnumCode.CheckBillCode)(billData.CheckBillCode);
            switch (billCode)
            {
                case PrintEnumCode.CheckBillCode.XDsbd:
                    smdcdDto.IsFailured = true;
                    smdcdDto.IsCheckout = false;
                    smdcdDto.IsMealCheckout = false;
                    break;
                case PrintEnumCode.CheckBillCode.SMjzd:
                    smdcdDto.IsCheckout = true;
                    smdcdDto.IsFailured = false;
                    smdcdDto.IsMealCheckout = false;
                    break;
                case PrintEnumCode.CheckBillCode.QCjzd:
                    smdcdDto.IsMealCheckout = true;
                    smdcdDto.IsCheckout = false;
                    smdcdDto.IsFailured = false;
                    break;
                default:
                    smdcdDto.IsCheckout = billData.IsCheckout;
                    smdcdDto.IsFailured = billData.IsFailured;
                    smdcdDto.IsMealCheckout = false;
                    break;
            }
            PrintEnumCode.TableTypeCode TableCode = (PrintEnumCode.TableTypeCode)(billData.TableTypeCode);
            switch (TableCode)
            {
                case PrintEnumCode.TableTypeCode.ZPH:
                    smdcdDto.TableType = "餐桌号";
                    break;
                case PrintEnumCode.TableTypeCode.QCH:
                    smdcdDto.TableType = "取餐号";
                    break;
                default:
                    smdcdDto.TableType = "餐桌号";
                    break;
            }
            smdcdDto.ErasingMoney = "";
            #region 区分尾数调整
            if (billData.ErasingMoney > 0)
            {
                smdcdDto.ErasingMoney = "+" + billData.ErasingMoney;
            }
            else if (billData.ErasingMoney != 0)
            {
                smdcdDto.ErasingMoney = billData.ErasingMoney.ToString();
            }
            #endregion
            if (!string.IsNullOrWhiteSpace(billData.OrderOrigin))
            {
                smdcdDto.OrderOrigin = billData.OrderOrigin;
            }

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            if (!string.IsNullOrWhiteSpace(billData.PcId))//pcid决定是否走本地打印机
            {
                var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
                var printListConfig = printSchemeServicesV2.GetLocalPrint(billData.PcId);
                if (printListConfig == null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                    Message = "打印方案为空";
                    return;
                }
                smdcdDto.PrintingDate = DateTime.Now;
                htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, smdcdDto);
            }
            else
            {
                List<V2PrintConfigDto> printListConfig = new List<V2PrintConfigDto>();
                PrintEnumCode.OriginCode origin = (PrintEnumCode.OriginCode)(billData.OriginCode);
                printListConfig = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtOrder, origin, billData.TagList);
                if (printListConfig == null || printListConfig.Count < 1)
                {
                    //默认打印机
                    var printDefaultConfig = GetPrintSchemeByDefault(PrintEnumCode.PrintGroupCode.PrtOrder);
                    if (printDefaultConfig == null)
                    {
                        Flag = false;
                        this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                        Message = "打印方案为空";
                    }
                    else
                    {
                        PrintWithDefaultScheme(printDefaultConfig, htmlPrintingServices, voucher.Path, smdcdDto);
                    }
                    return;
                }
                if (smdcdDto.IsFailured)//打印下单失败单的情况下默认打印一张
                {
                    List<PrintConfigDto> printConfigDtoList = GetPrintConfigBySet(printListConfig, PrintEnumCode.SetInfoKey.FailOrder);
                    foreach (var item in printConfigDtoList)
                    {
                        smdcdDto.PrintingDate = DateTime.Now;
                        htmlPrintingServices.GlobalPrintScheme(item, voucher.Path, smdcdDto, 1);
                    }
                }
                else
                {
                    List<PrintConfigDto> printConfigDtoList = new List<PrintConfigDto>();
                    if (smdcdDto.IsCheckout)//结账
                    {
                        printConfigDtoList = GetPrintConfigByVoucher(printListConfig, PrintEnumCode.VoucherCode.ChechOut);
                    }
                    else
                    {
                        printConfigDtoList = GetPrintConfigByVoucher(printListConfig, PrintEnumCode.VoucherCode.Order);
                    }

                    foreach (var item in printConfigDtoList)
                    {
                        smdcdDto.PrintingDate = DateTime.Now;
                        htmlPrintingServices.GlobalPrintScheme(item, voucher.Path, smdcdDto, item.PrintNum);
                    }
                }
            }

            PrintLogUtility.Writer.SendInfo("PrintScanCodeBill 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 自动核销失败单据
        /// </summary>
        /// <param name="printFailVoucher"></param>
        public void PrintPayFail(PrintFailVoucher printFailVoucher)
        {
            if (printFailVoucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintPayFail 接收 data:" + JsonConvert.SerializeObject(printFailVoucher));

            string templateCode = "PRT_SO_0002";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            printFailVoucher.VoucherId = voucher.Id;

            PrintFailDto printFailDto = new PrintFailDto
            {
                TableName = printFailVoucher.TableName,
                BillNo = printFailVoucher.BillNo,
                PayPattern = printFailVoucher.PayPattern,
                PayMoney = printFailVoucher.PayMoney,
                OrderPayNo = printFailVoucher.OrderPayNo
            };

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            if (!string.IsNullOrWhiteSpace(printFailVoucher.PcId)) //pcid决定是否走本地打印机
            {
                var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
                var printListConfig = printSchemeServicesV2.GetLocalPrint(printFailVoucher.PcId);
                if (printListConfig == null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                    Message = "打印方案为空";
                    return;
                }
                printFailDto.PrintDate = DateTime.Now;
                htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, printFailDto);
            }
            else
            {
                PrintEnumCode.OriginCode origin = (PrintEnumCode.OriginCode)(printFailVoucher.OriginCode);
                var printListConfig = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtOrder, origin, printFailVoucher.TagList);
                if (printListConfig == null || printListConfig.Count < 1)
                {
                    var printDefaultConfig = GetPrintSchemeByDefault(PrintEnumCode.PrintGroupCode.PrtOrder);
                    if (printDefaultConfig == null)
                    {
                        Flag = false;
                        this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                        Message = "打印方案为空";
                    }
                    else
                    {
                        PrintWithDefaultScheme(printDefaultConfig, htmlPrintingServices, voucher.Path, printFailDto);
                    }
                    return;
                }
                List<PrintConfigDto> printConfigDtoList = GetPrintConfigBySet(printListConfig, PrintEnumCode.SetInfoKey.PayFail);
                foreach (var item in printConfigDtoList)
                {
                    printFailDto.PrintDate = DateTime.Now;
                    htmlPrintingServices.GlobalPrintScheme(item, voucher.Path, printFailDto, 1);
                }
            }


            PrintLogUtility.Writer.SendInfo("PrintPayFail 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 打印退款凭证
        /// </summary>
        /// <param name="refundVoucher"></param>
        public void PrintRefundVoucher(RefundVoucher refundVoucher)
        {
            if (refundVoucher == null)
            {
                Flag = false;
                Message = "退款凭证打印参数为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            string templateCode = "PRT_FI_0002";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                Message = "退款凭证模板为空";
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            refundVoucher.VoucherId = voucher.Id;

            var tkpzDto = new TkpzDto
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                RefundAmount = refundVoucher.RefundAmount,
                RefundDate = refundVoucher.RefundTime,
                RefundAccount = refundVoucher.Account,
                RefundTypeName = refundVoucher.RefundTypeName,
                PaymentVoucherId = refundVoucher.TradeNo,
                RefundVoucherId = refundVoucher.RefundTradeNo,
                Operator = refundVoucher.Operator,
                RefundDescription = refundVoucher.Description
            };
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            if (!string.IsNullOrWhiteSpace(refundVoucher.PcId)) //pcid决定是否走本地打印机
            {
                var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
                var printListConfig = printSchemeServicesV2.GetLocalPrint(refundVoucher.PcId);
                if (printListConfig == null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                    Message = "打印方案为空";
                    return;
                }
                tkpzDto.PrintingDate = DateTime.Now;
                htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, tkpzDto);
            }
            else
            {
                PrintEnumCode.OriginCode origin = (PrintEnumCode.OriginCode)(refundVoucher.OriginCode);
                var printListConfig = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtPayment, origin, refundVoucher.TagList);
                if (printListConfig == null || printListConfig.Count < 1)
                {
                    var printDefaultConfig = GetPrintSchemeByDefault(PrintEnumCode.PrintGroupCode.PrtPayment);
                    if (printDefaultConfig == null)
                    {
                        Flag = false;
                        this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                        Message = "打印方案为空";
                    }
                    else
                    {
                        PrintWithDefaultScheme(printDefaultConfig, htmlPrintingServices, voucher.Path, tkpzDto);
                    }
                    return;
                }
                List<PrintConfigDto> printConfigDtoList = GetPrintConfigByVoucher(printListConfig, PrintEnumCode.VoucherCode.Refund);
                foreach (var item in printConfigDtoList)
                {
                    tkpzDto.PrintingDate = DateTime.Now;
                    htmlPrintingServices.GlobalPrintScheme(item, voucher.Path, tkpzDto, item.PrintNum);
                }
            }

            PrintLogUtility.Writer.SendInfo("PrintRefundVoucher 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 打印支付凭证
        /// </summary>
        /// <param name="paymentVoucher"></param>
        public void PrintPaymentVoucher(PaymentVoucher paymentVoucher)
        {
            if (paymentVoucher == null)
            {
                Flag = false;
                Message = "支付凭证打印参数为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintPaymentVoucher 接收 data:" + JsonConvert.SerializeObject(paymentVoucher));
            string templateCode = "PRT_FI_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                Message = "支付凭证模板为空";
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            paymentVoucher.VoucherId = voucher.Id;

            var sfpzDto = new SfpzDto
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                ShouldAmount = paymentVoucher.Totalamount,
                PayDiscount = paymentVoucher.DiscountAmount,
                ActualAmount = paymentVoucher.Receiptamount,
                UserPayment = paymentVoucher.UserPayAmount,
                PaymentDescription = paymentVoucher.Description,
                PaymentDate = paymentVoucher.PayTime,
                PaymentTypeName = paymentVoucher.PayName,
                PaymentAccount = paymentVoucher.Account,
                PaymentVoucherId = paymentVoucher.TradeNo,
                BillRunningId = paymentVoucher.BillRunningId,
                InvoiceTitle = paymentVoucher.InvoiceTitle,
                TaxpayerNo = paymentVoucher.TaxpayerNo,
                Dishes = new List<SmdcdDishDto>(),
                IsPrintDish = paymentVoucher.IsPrintDish,
                IsPrintDishAamount = paymentVoucher.IsPrintDishAamount
            };

            if (paymentVoucher.BillDish != null && paymentVoucher.BillDish.Any() && paymentVoucher.IsPrintDish)
            {
                foreach (var dish in paymentVoucher.BillDish)
                {
                    var smdcdDishDto = new SmdcdDishDto
                    {
                        DishName = dish.DishName,
                        TakeCount = dish.TakeCount,
                        DishDescription = dish.DishDescription,
                        UnitPrice = dish.UnitPrice,
                        WeightNames = new List<string>(),
                        PracticeNames = new List<string>()
                    };
                    if (dish.WeightNames != null)
                    {
                        foreach (var weight in dish.WeightNames)
                        {
                            smdcdDishDto.WeightNames.Add(weight);
                        }
                    }
                    if (dish.PracticeNames != null)
                    {
                        foreach (var practice in dish.PracticeNames)
                        {
                            smdcdDishDto.PracticeNames.Add(practice);
                        }
                    }
                    if (dish.SubDishes != null)
                    {
                        foreach (var setMeal in dish.SubDishes)
                        {
                            var setMealDto = new SmdcdDishDto
                            {
                                DishName = setMeal.DishName,
                                TakeCount = setMeal.TakeCount,
                                DishDescription = setMeal.DishDescription,
                                UnitPrice = setMeal.UnitPrice,
                                WeightNames = new List<string>(),
                                PracticeNames = new List<string>()
                            };
                            if (setMeal.WeightNames != null)
                            {
                                foreach (var weight in dish.WeightNames)
                                {
                                    setMealDto.WeightNames.Add(weight);
                                }
                            }
                            if (setMeal.PracticeNames != null)
                            {
                                foreach (var practice in dish.PracticeNames)
                                {
                                    setMealDto.PracticeNames.Add(practice);
                                }
                            }
                            smdcdDishDto.SubDishes.Add(setMealDto);
                        }
                    }
                    sfpzDto.Dishes.Add(smdcdDishDto);
                }
            }

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            if (!string.IsNullOrWhiteSpace(paymentVoucher.PcId)) //pcid决定是否走本地打印机
            {
                var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
                var printListConfig = printSchemeServicesV2.GetLocalPrint(paymentVoucher.PcId);
                if (printListConfig == null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                    Message = "打印方案为空";
                    return;
                }
                sfpzDto.PrintingDate = DateTime.Now;
                htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, sfpzDto);
            }
            else
            {
                PrintEnumCode.OriginCode origin = (PrintEnumCode.OriginCode)(paymentVoucher.OriginCode);
                var printListConfig = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtPayment, origin, paymentVoucher.TagList);
                if (printListConfig == null || printListConfig.Count < 1)
                {
                    var printDefaultConfig = GetPrintSchemeByDefault(PrintEnumCode.PrintGroupCode.PrtPayment);
                    if (printDefaultConfig == null)
                    {
                        Flag = false;
                        this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                        Message = "打印方案为空";
                    }
                    else
                    {
                        PrintWithDefaultScheme(printDefaultConfig, htmlPrintingServices, voucher.Path, sfpzDto);
                    }
                    return;
                }
                List<PrintConfigDto> printConfigDtoList = GetPrintConfigByVoucher(printListConfig, PrintEnumCode.VoucherCode.Payment);
                foreach (var item in printConfigDtoList)
                {
                    sfpzDto.PrintingDate = DateTime.Now;
                    htmlPrintingServices.GlobalPrintScheme(item, voucher.Path, sfpzDto, item.PrintNum);
                }
            }

            PrintLogUtility.Writer.SendInfo("PrintPaymentVoucher 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 打印口碑预点单
        /// </summary>
        /// <param name="kbPreOrder"></param>
        public void PrintKbPreOrder(KbPreOrder kbPreOrder)
        {
            PrintLogUtility.Writer.SendInfo("PrintKbPreOrder 接收 data:" + JsonConvert.SerializeObject(kbPreOrder));
            if (kbPreOrder == null)
            {
                Flag = false;
                Message = "口碑预点单打印参数为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            string templateCode = "PRT_SO_1001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                Message = "口碑预点单模板为空";
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            kbPreOrder.VoucherId = voucher.Id;

            var preOrderDto = new PreOrderDto()
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                OrderTime = kbPreOrder.OrderTime,
                TakeMealNum = kbPreOrder.TakeMealNum,
                Scene = kbPreOrder.Scene,
                CustomerName = kbPreOrder.CustomerName,
                CustomerPhone = kbPreOrder.CustomerPhone,
                EatingTime = kbPreOrder.EatingTime,
                CustomerCount = kbPreOrder.CustomerCount,
                Remark = kbPreOrder.Remark,
                TotalAmount = kbPreOrder.TotalAmount,
                ShouldAmount = kbPreOrder.ShouldAmount
            };
            if (kbPreOrder.Dishes != null)
            {
                foreach (var dish in kbPreOrder.Dishes)
                {
                    var preOrderDishDto = new PreOrderDishDto()
                    {
                        DishName = dish.DishName,
                        TakeCount = dish.TakeCount,
                        DishDescription = dish.DishDescription,
                        UnitPrice = dish.UnitPrice,
                        WeightNames = dish.WeightNames,
                        PracticeNames = dish.PracticeNames
                    };
                    if (dish.SubDishes != null)
                    {
                        foreach (var subDishe in dish.SubDishes)
                        {
                            var subDisheDto = new PreOrderDishDto()
                            {
                                DishName = subDishe.DishName,
                                TakeCount = subDishe.TakeCount,
                                DishDescription = subDishe.DishDescription,
                                UnitPrice = subDishe.UnitPrice,
                                WeightNames = subDishe.WeightNames,
                                PracticeNames = subDishe.PracticeNames,
                                SubDishes = new List<PreOrderDishDto>()
                            };
                            preOrderDishDto.SubDishes.Add(subDisheDto);
                        }
                    }
                    preOrderDto.Dishes.Add(preOrderDishDto);
                }
            }
            if (kbPreOrder.DiscountAmounts != null)
            {
                foreach (var discount in kbPreOrder.DiscountAmounts)
                {
                    var preOrderDiscountAmountDto = new PreOrderDiscountAmountDto()
                    {
                        DiscountName = discount.DiscountName,
                        Price = discount.Price
                    };

                    preOrderDto.DiscountAmounts.Add(preOrderDiscountAmountDto);
                }
            }
            if (kbPreOrder.AdditionalCharges != null)
            {
                foreach (var payDetail in kbPreOrder.AdditionalCharges)
                {
                    var preOrderAdditionalCostDto = new PreOrderAdditionalCostDto
                    {
                        Name = payDetail.Name,
                        Price = payDetail.Price
                    };
                    preOrderDto.AdditionalCharges.Add(preOrderAdditionalCostDto);
                }
            }
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            var printListConfig = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtPreOrder);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                var printDefaultConfig = GetPrintSchemeByDefault(PrintEnumCode.PrintGroupCode.PrtPreOrder);
                if (printDefaultConfig == null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                    Message = "打印方案为空";
                }
                else
                {
                    PrintWithDefaultScheme(printDefaultConfig, htmlPrintingServices, voucher.Path, preOrderDto);
                }
                return;
            }
            List<PrintConfigDto> printConfigDtoList = GetPrintConfigByVoucher(printListConfig, PrintEnumCode.VoucherCode.PreOrder);
            foreach (var item in printConfigDtoList)
            {
                preOrderDto.PrintingDate = DateTime.Now;
                htmlPrintingServices.GlobalPrintScheme(item, voucher.Path, preOrderDto, item.PrintNum);
            }

            PrintLogUtility.Writer.SendInfo("PrintKbPreOrder 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 打印收银汇总单
        /// </summary>
        /// <param name="saleSummary"></param>
        public void PrintSaleSummary(SaleSummary saleSummary)
        {
            if (saleSummary == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintSaleSummary data:" + JsonConvert.SerializeObject(saleSummary));
            string templateCode = "PRT_FI_0003";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            saleSummary.VoucherId = voucher.Id;

            var syhzdDto = new SyhzdDto
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                StartDate = saleSummary.StartDate,
                EndDate = saleSummary.EndDate,
                PayCount = saleSummary.PayCount,
                PayTotalAmount = saleSummary.PayTotalAmount,
                DiscountCount = saleSummary.DiscountCount,
                PayDiscountAmount = saleSummary.PayDiscountAmount,
                DiscountAmounts = new List<SyhzDiscountAmountDto>(),
                RefundTotalAmount = saleSummary.RefundTotalAmount,
                ActualTotalAmount = saleSummary.ActualTotalAmount,
                IsPayDetail = saleSummary.IsPayDetail,
                RefundCount = saleSummary.RefundCount,
                PayDetails = new List<SyhzdPayDetailDto>(),
                RefundPayTypeAmounts = new List<SyhzdPayTypeAmountDto>(),
                ActualPayTypeAmounts = new List<SyhzdPayTypeAmountDto>()
            };

            if (saleSummary.DiscountDetails != null)
            {
                if (saleSummary.DiscountCount != saleSummary.DiscountDetails.Count)
                {
                    saleSummary.DiscountCount = saleSummary.DiscountDetails.Count;
                }
                foreach (var sale in saleSummary.DiscountDetails)
                {
                    var amountDto = new SyhzDiscountAmountDto
                    {
                        DiscountName = sale.Name,
                        Price = sale.Amount
                    };

                    syhzdDto.DiscountAmounts.Add(amountDto);
                }
            }
            if (saleSummary.PayDetails != null)
            {
                foreach (var pay in saleSummary.PayDetails)
                {
                    var syhzdPayDetailDto = new SyhzdPayDetailDto
                    {
                        PaymentDate = pay.PayTime,
                        PaymentVoucherId = pay.TradeNo,
                        PaymentAmount = pay.UserPayAmount,
                        ActualAmount = pay.Receiptamount,
                        PayDiscount = pay.PayDiscount,
                        PaymentTypeName = pay.PayName
                    };

                    if (saleSummary.RefundDetails != null)
                    {
                        var refundDetails = saleSummary.RefundDetails.Where(x => x.TradeNo == pay.TradeNo);
                        if (refundDetails != null)
                        {
                            foreach (var refund in refundDetails)
                            {
                                var syhzdRefundDetailDto = new SyhzdRefundDetailDto
                                {
                                    RefundDate = refund.RefundTime,
                                    PaymentVoucherId = refund.TradeNo,
                                    RefundVoucherId = refund.RefundTradeNo,
                                    RefundTypeName = refund.RefundTypeName,
                                    RefundAmount = refund.RefundAmount
                                };
                                syhzdPayDetailDto.RefundDetails.Add(syhzdRefundDetailDto);
                            }
                        }
                    }
                    syhzdDto.PayDetails.Add(syhzdPayDetailDto);
                }
            }
            if (saleSummary.RefundDetails != null)
            {
                if (saleSummary.RefundCount != saleSummary.RefundDetails.Count)
                {
                    saleSummary.RefundCount = saleSummary.RefundDetails.Count;
                }
            }
            if (saleSummary.RefundPayTypeAmounts != null)
            {
                foreach (var refund in saleSummary.RefundPayTypeAmounts)
                {
                    var amountDto = new SyhzdPayTypeAmountDto
                    {
                        PayTypeName = refund.PayTypeName,
                        Price = refund.Price
                    };

                    syhzdDto.RefundPayTypeAmounts.Add(amountDto);
                }
            }
            if (saleSummary.ActualPayTypeAmounts != null)
            {
                foreach (var actual in saleSummary.ActualPayTypeAmounts)
                {
                    var amount = new SyhzdPayTypeAmountDto
                    {
                        PayTypeName = actual.PayTypeName,
                        Price = actual.Price
                    };

                    syhzdDto.ActualPayTypeAmounts.Add(amount);
                }
            }

            var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
            var printListConfig = printSchemeServicesV2.GetLocalPrint(saleSummary.PcId);
            if (printListConfig == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "打印方案为空";
                return;
            }
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            syhzdDto.PrintingDate = DateTime.Now;
            htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, syhzdDto);

            PrintLogUtility.Writer.SendInfo("PrintSaleSummary 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 打印堂食汇总单
        /// </summary>
        /// <param name="dineSummary"></param>
        public void PrintDineSummary(DineSummary dineSummary)
        {
            if (dineSummary == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintDineSummary data:" + JsonConvert.SerializeObject(dineSummary));
            string templateCode = "PRT_FI_0004";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            dineSummary.VoucherId = voucher.Id;
            var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
            var printListConfig = printSchemeServicesV2.GetLocalPrint(dineSummary.PcId);
            if (printListConfig == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "打印方案为空";
                return;
            }
            var tshzdDto = new TshzdDto()
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                StartDate = dineSummary.StartDate,
                EndDate = dineSummary.EndDate,
                PayCount = dineSummary.PayCount,
                PayTotalAmount = dineSummary.PayTotalAmount,
                AbnormalCount = dineSummary.AbnormalCount,
                AbnormalTotalAmount = dineSummary.AbnormalTotalAmount,
                AdjustmentCount = dineSummary.AdjustmentCount,
                AdjustmentTotalAmount = dineSummary.AdjustmentTotalAmount,
                DiscountCount = dineSummary.DiscountCount,
                PayDiscountAmount = dineSummary.PayDiscountAmount,
                DiscountAmounts = new List<SyhzDiscountAmountDto>(),
                RefundTotalAmount = dineSummary.RefundTotalAmount,
                ActualTotalAmount = dineSummary.ActualTotalAmount,
                IsPayDetail = dineSummary.IsPayDetail,
                RefundCount = dineSummary.RefundCount,
                PayDetails = new List<SyhzdPayDetailDto>(),
                RefundPayTypeAmounts = new List<SyhzdPayTypeAmountDto>(),
                ActualPayTypeAmounts = new List<SyhzdPayTypeAmountDto>()
            };

            #region 20180823新增打赏
            if (dineSummary.RewardsCount == 0)
            {
                tshzdDto.RewardsCount = "";
                tshzdDto.RewardsTotalAmount = dineSummary.RewardsTotalAmount;
            }
            else
            {
                tshzdDto.RewardsCount = dineSummary.RewardsCount.ToString();
                tshzdDto.RewardsTotalAmount = dineSummary.RewardsTotalAmount;
            }
            #endregion

            if (dineSummary.DiscountDetails != null)
            {
                if (dineSummary.DiscountCount != dineSummary.DiscountDetails.Count)
                {
                    dineSummary.DiscountCount = dineSummary.DiscountDetails.Count;
                }
                foreach (var sale in dineSummary.DiscountDetails)
                {
                    var amountDto = new SyhzDiscountAmountDto
                    {
                        DiscountName = sale.Name,
                        Price = sale.Amount
                    };

                    tshzdDto.DiscountAmounts.Add(amountDto);
                }
            }
            if (dineSummary.PayDetails != null)
            {
                foreach (var pay in dineSummary.PayDetails)
                {
                    var syhzdPayDetailDto = new SyhzdPayDetailDto
                    {
                        PaymentDate = pay.PayTime,
                        PaymentVoucherId = pay.TradeNo,
                        PaymentAmount = pay.UserPayAmount,
                        ActualAmount = pay.Receiptamount,
                        PayDiscount = pay.PayDiscount,
                        PaymentTypeName = pay.PayName
                    };

                    if (dineSummary.RefundDetails != null)
                    {
                        var refundDetails = dineSummary.RefundDetails.Where(x => x.TradeNo == pay.TradeNo);
                        if (refundDetails != null)
                        {
                            foreach (var refund in refundDetails)
                            {
                                var syhzdRefundDetailDto = new SyhzdRefundDetailDto
                                {
                                    RefundDate = refund.RefundTime,
                                    PaymentVoucherId = refund.TradeNo,
                                    RefundVoucherId = refund.RefundTradeNo,
                                    RefundTypeName = refund.RefundTypeName,
                                    RefundAmount = refund.RefundAmount
                                };
                                syhzdPayDetailDto.RefundDetails.Add(syhzdRefundDetailDto);
                            }
                        }
                    }
                    tshzdDto.PayDetails.Add(syhzdPayDetailDto);
                }
            }
            if (dineSummary.RefundDetails != null)
            {
                if (dineSummary.RefundCount != dineSummary.RefundDetails.Count)
                {
                    dineSummary.RefundCount = dineSummary.RefundDetails.Count;
                }
            }
            if (dineSummary.RefundPayTypeAmounts != null)
            {
                foreach (var refund in dineSummary.RefundPayTypeAmounts)
                {
                    var amountDto = new SyhzdPayTypeAmountDto
                    {
                        PayTypeName = refund.PayTypeName,
                        Price = refund.Price
                    };

                    tshzdDto.RefundPayTypeAmounts.Add(amountDto);
                }
            }
            if (dineSummary.ActualPayTypeAmounts != null)
            {
                foreach (var actual in dineSummary.ActualPayTypeAmounts)
                {
                    var amount = new SyhzdPayTypeAmountDto
                    {
                        PayTypeName = actual.PayTypeName,
                        Price = actual.Price
                    };

                    tshzdDto.ActualPayTypeAmounts.Add(amount);
                }
            }

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            tshzdDto.PrintingDate = DateTime.Now;
            htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, tshzdDto);

            PrintLogUtility.Writer.SendInfo("PrintDineSummary 打印完成");

            Flag = true;
        }

        /// <summary>
        /// 轻餐版营业汇总单
        /// </summary>
        /// <param name="summaryVoucher"></param>
        public void PrintSummary(SummaryVoucher summaryVoucher)
        {
            if (summaryVoucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintSummary 接收 data:" + JsonConvert.SerializeObject(summaryVoucher));
            string templateCode = "PRT_FI_1001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
            var printListConfig = printSchemeServicesV2.GetLocalPrint(summaryVoucher.PcId);
            if (printListConfig == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "打印方案为空";
                return;
            }

            PrintSummaryDto printSummary = new PrintSummaryDto
            {
                BillFlowCount = summaryVoucher.BillFlowCount,
                BeginTime = summaryVoucher.BeginTime,
                EndTime = summaryVoucher.EndTime,
                DishCostTotalAmount = summaryVoucher.DishCostTotalAmount,
                ReceiptsTotalAmount = summaryVoucher.ReceiptsTotalAmount,
                ReceiptsDetailList = summaryVoucher.ReceiptsDetailList,
                OrderDiscountList = summaryVoucher.OrderDiscountList
            };
            printSummary.ShopName = _restaurantName;
            printSummary.SubShopName = _restaurantSubName;
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            printSummary.PrintTime = DateTime.Now;
            htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, printSummary);

            PrintLogUtility.Writer.SendInfo("PrintSummary 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 轻餐开钱箱
        /// </summary>
        /// <param name="pcid"></param>
        public void LMOpenCashBox(string pcid)
        {
            if (string.IsNullOrWhiteSpace(pcid))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            try
            {
                PrintLogUtility.Writer.SendInfo("轻餐开钱箱" + "pcid:" + pcid);

                var Print = ServiceLocator.Instance.Resolve<IPrint>();
                var openCashBoxSetInfo = Print.GetOpenCashBoxSetInfoByPcid(pcid);
                if (openCashBoxSetInfo == null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.ResultNull;
                    Message = "没有配置钱箱";
                    return;
                }
                if (string.IsNullOrWhiteSpace(openCashBoxSetInfo.PrintId))
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.ResultNull;
                    Message = "没有配置钱箱";
                    return;
                }
                var printConfigDto = Print.GetPrintConfig(openCashBoxSetInfo.PrintId);
                if (printConfigDto == null || printConfigDto.Enable == 0)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.ResultNull;
                    Message = "没有配置钱箱";
                    return;
                }
                var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
                PrintLogUtility.Writer.SendInfo("轻餐开钱箱调用" + JsonConvert.SerializeObject(printConfigDto));
                htmlPrintingServices.LMOpenCashBox(printConfigDto);
            }
            catch (Exception ex)
            {
                Flag = false;
                PrintLogUtility.Writer.SendFullError(ex);
                this.Code = PrintErrorCode.Code.DatabaseError;
                return;
            }
        }

        /// <summary>
        /// 打印测试页
        /// </summary>
        /// <param name="printConfigDto"></param>
        public void PrintTest(PrintConfigDto printConfigDto)
        {
            if (printConfigDto == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            string templateCode = "PRT_TM_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            string pagerTypeName = string.Empty;
            switch (printConfigDto.PaperType)
            {
                case 0:
                    pagerTypeName = "未配置";
                    break;
                case 1:
                    pagerTypeName = "58mm";
                    break;
                case 2:
                    pagerTypeName = "76mm";
                    break;
                case 3:
                    pagerTypeName = "80mm";
                    break;
            }
            var dycsDto = new DycsDto()
            {
                TerminalName = printConfigDto.TerminalName,
                PrintingName = printConfigDto.PrintName,
                PagerTypeName = pagerTypeName,
                PaperWidth = (int)printConfigDto.PaperWidth,
                TopMargin = (int)printConfigDto.TopMargin,
                LeftMargin = (int)printConfigDto.LeftMargin,
                Alias = printConfigDto.Alias
            };
            if (printConfigDto.PrintNum == 0)
            {
                printConfigDto.PrintNum = 1;
            }
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            htmlPrintingServices.PrintDycs(printConfigDto, voucher, dycsDto);
            Flag = true;
        }

        /// <summary>
        /// 旧的测试打印单据(门店打印方案)
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="pcId"></param>
        public void PrintTestByTemplateCode(string templateCode, string pcId)
        {
            Flag = false;
            Message = "旧的打印方案不启用";
            this.Code = PrintErrorCode.Code.IllegalOperationError;
            return;
        }

        /// <summary>
        /// 测试打印扫码点菜单--页尾(本机打印方案)
        /// </summary>
        /// <param name="pcId"></param>
        /// <param name="content"></param>
        /// <param name="isMiddle"></param>
        public void TestPrintScanCodeBill(string pcId, string content, int isMiddle)
        {
            if (string.IsNullOrEmpty(pcId))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var templateCode = "PRT_SO_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
            var printListConfig = printSchemeServicesV2.GetLocalPrint(pcId);
            if (printListConfig == null)
            {
                return;
            }
            var mockScanCodeBill = GetMockScanCodeBill();
            mockScanCodeBill.Footer = content;
            if (isMiddle == 1)
            {
                mockScanCodeBill.FooterAlignment = "center";
            }
            else
            {
                mockScanCodeBill.FooterAlignment = "left";
            }

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            mockScanCodeBill.PrintingDate = DateTime.Now;
            mockScanCodeBill.Footer = mockScanCodeBill.Footer.Replace("\\r\\n", "<br />").Replace("\r\n", "<br />").Replace("\\n", "<br />").Replace("\n", "<br />").Replace(Environment.NewLine, "<br />");
            htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, mockScanCodeBill);
            Flag = true;
        }

        #region 接口拓展方法

        /// <summary>
        /// 通过模板编号获取对应的模板信息
        /// </summary>
        /// <param name="templateCode">模板编号</param>
        /// <returns></returns>
        private VoucherDto GetVoucherByCode(string templateCode)
        {
            var voucher = _repositoryContext.QueryFirstOrDefault<Voucher>("select Id,VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Sort,Path from tb_voucher Where TemplateCode=@TemplateCode", new { TemplateCode = templateCode });
            if (voucher == null)
            {
                return null;
            }
            var voucherDto = new VoucherDto
            {
                Id = voucher.Id,
                VoucherName = voucher.VoucherName,
                TemplateCode = voucher.TemplateCode,
                GroupCode = voucher.GroupCode,
                Enble = voucher.Enble,
                Overall = voucher.Overall,
                Sort = voucher.Sort,
                SchemeNum = 0,
                Path = voucher.Path
            };
            return voucherDto;
        }

        /// <summary>
        /// 通过打印方案组ID直接获取打印方案
        /// 1. 打印方案组
        /// 2. 餐桌来源
        /// 3. 餐桌信息
        /// </summary>
        /// <param name="code">打印方案组</param>
        /// <param name="tableCode">餐桌类型</param>
        /// <param name="areaCode">菜品分类类型</param>
        /// <param name="tagList">标签</param>
        /// <returns></returns>
        private List<V2PrintConfigDto> GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode code, PrintEnumCode.OriginCode Code, List<string> tagList)
        {
            string tableId = "";
            if (tagList != null)
            {
                tableId = tagList.FirstOrDefault();
            }

            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode", new { printCode = code });
            if (printGroup == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.GetPrintGroupNullError;
                Message = "打印方案组不存在";
                return new List<V2PrintConfigDto>();
            }

            var sqlStr = new StringBuilder();
            sqlStr.Append("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme ");
            sqlStr.Append($"where groupId = {printGroup.Id} and state != 0 and id in ");
            sqlStr.Append($"( select schemeId from tb_schemeTable ");
            switch (Code)
            {
                case PrintEnumCode.OriginCode.MK:
                    sqlStr.Append($"where MKTableID  in ('{tableId}') ) ");
                    break;
                case PrintEnumCode.OriginCode.ERP:
                    sqlStr.Append($"where ErpTableID in ('{tableId}') ) ");
                    break;
            }

            sqlStr.Append("order by createTime asc ");
            List<PrintGroupScheme> printGroupSchemes = _repositoryContext.GetSet<PrintGroupScheme>(sqlStr.ToString());
            if (printGroupSchemes == null || printGroupSchemes.Count <= 0)
            {
                //找不到打印方案的时候，选取默认打印方案(启用状态)
                //var itemObject = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select * from tb_printGroupScheme t where groupId = @groupId and t.isDefault = 1 and state = 1", new { groupId = printGroup.Id });
                //if (itemObject != null)
                //{
                //    printGroupSchemes = new List<PrintGroupScheme>();
                //    printGroupSchemes.Add(itemObject);
                //}
                //else
                //{
                //    Flag = false;
                //    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                //    Message = "打印方案为空";
                //}

                return new List<V2PrintConfigDto>();
            }

            var printConfigDtos = new List<V2PrintConfigDto>();
            if (printGroupSchemes != null && printGroupSchemes.Count > 0)
            {
                foreach (var scheme in printGroupSchemes)
                {
                    //获取打印机配置信息
                    var printConfig = _repositoryContext.FirstOrDefault<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId = @printId ", new { printId = scheme.PrintId });

                    if (printConfig == null)
                    {
                        continue;
                    }

                    V2PrintConfigDto printConfigDto = new V2PrintConfigDto
                    {
                        PrintId = printConfig.PrintId,
                        Pcid = printConfig.Pcid,
                        TerminalName = printConfig.TerminalName,
                        PrintName = printConfig.PrintName,
                        ConnStyle = printConfig.ConnStyle,
                        Alias = printConfig.Alias,
                        PaperType = printConfig.PaperType,
                        PaperWidth = printConfig.PaperWidth,
                        TopMargin = printConfig.TopMargin,
                        LeftMargin = printConfig.LeftMargin,
                        ModifyTime = printConfig.ModifyTime,
                        Updated = printConfig.Updated,
                        Enable = printConfig.Enable,
                        State = printConfig.State
                    };

                    printConfigDto.VoucherList.AddRange(GetSchemeVoucherList(scheme));
                    printConfigDto.SetInfoList.AddRange(GetSchemeSetInfoList(scheme));
                    //当前版本没有菜品配置
                    //printConfigDto.DishTypeCassify.AddRange(GetSchemeDishTypeList(scheme));

                    printConfigDtos.Add(printConfigDto);
                }
            }

            return printConfigDtos;
        }

        private List<V2PrintConfigDto> GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode code)
        {
            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode", new { printCode = code });
            if (printGroup == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.DatabaseError;
                Message = "找不到打印方案组";
                return new List<V2PrintConfigDto>();
            }

            var sqlStr = new StringBuilder();
            sqlStr.Append("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme ");
            sqlStr.Append($"where groupId = {printGroup.Id} and state != 0 ");
            sqlStr.Append("order by createTime asc ");
            var printGroupSchemes = _repositoryContext.GetSet<PrintGroupScheme>(sqlStr.ToString());
            if (printGroupSchemes == null || printGroupSchemes.Count <= 0)
            {
                //找不到打印方案的时候，选取默认打印方案(启用状态)
                //var itemObject = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select * from tb_printGroupScheme t where groupId = @groupId and t.isDefault = 1 and state = 1", new { groupId = printGroup.Id });
                //if (itemObject != null)
                //{
                //    printGroupSchemes = new List<PrintGroupScheme>();
                //    printGroupSchemes.Add(itemObject);
                //}
                //else
                //{
                //    Flag = false;
                //    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                //    Message = "打印方案为空";
                //}
                return new List<V2PrintConfigDto>();
            }

            var printConfigDtos = new List<V2PrintConfigDto>();
            if (printGroupSchemes != null && printGroupSchemes.Count > 0)
            {
                foreach (var scheme in printGroupSchemes)
                {
                    //获取打印机配置信息
                    var printConfig = _repositoryContext.FirstOrDefault<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId = @printId ", new { printId = scheme.PrintId });

                    if (printConfig == null)
                    {
                        continue;
                    }

                    V2PrintConfigDto printConfigDto = new V2PrintConfigDto
                    {
                        PrintId = printConfig.PrintId,
                        Pcid = printConfig.Pcid,
                        TerminalName = printConfig.TerminalName,
                        PrintName = printConfig.PrintName,
                        ConnStyle = printConfig.ConnStyle,
                        Alias = printConfig.Alias,
                        PaperType = printConfig.PaperType,
                        PaperWidth = printConfig.PaperWidth,
                        TopMargin = printConfig.TopMargin,
                        LeftMargin = printConfig.LeftMargin,
                        ModifyTime = printConfig.ModifyTime,
                        Updated = printConfig.Updated,
                        Enable = printConfig.Enable,
                        State = printConfig.State
                    };

                    //目前外卖，预点没有配置信息
                    printConfigDto.VoucherList.AddRange(GetSchemeVoucherList(scheme));
                    //printConfigDto.SetInfoList.AddRange(GetSchemeSetInfoList(scheme));
                    //printConfigDto.DishTypeCassify.AddRange(GetSchemeDishTypeList(scheme));

                    printConfigDtos.Add(printConfigDto);
                }
            }

            return printConfigDtos;
        }

        /// <summary>
        /// 获取默认打印方案
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private V2PrintConfigDto GetPrintSchemeByDefault(PrintEnumCode.PrintGroupCode code)
        {
            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode", new { printCode = code });
            if (printGroup == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.DatabaseError;
                Message = "找不到打印方案组";
                return new V2PrintConfigDto();
            }

            var sqlStr = new StringBuilder();
            sqlStr.Append("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme ");
            sqlStr.Append($"where groupId = {printGroup.Id} and state != 0 and isDefault = 1 ");
            var printGroupScheme = _repositoryContext.FirstOrDefault<PrintGroupScheme>(sqlStr.ToString(), new { });
            if (printGroupScheme == null)
            {
                //没有默认打印方案
                return new V2PrintConfigDto();
            }

            //获取打印机配置信息
            var printConfig = _repositoryContext.FirstOrDefault<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId = @printId ", new { printId = printGroupScheme.PrintId });

            if (printConfig == null)
            {
                return new V2PrintConfigDto();
            }

            V2PrintConfigDto printConfigDto = new V2PrintConfigDto
            {
                PrintId = printConfig.PrintId,
                Pcid = printConfig.Pcid,
                TerminalName = printConfig.TerminalName,
                PrintName = printConfig.PrintName,
                ConnStyle = printConfig.ConnStyle,
                Alias = printConfig.Alias,
                PaperType = printConfig.PaperType,
                PaperWidth = printConfig.PaperWidth,
                TopMargin = printConfig.TopMargin,
                LeftMargin = printConfig.LeftMargin,
                ModifyTime = printConfig.ModifyTime,
                Updated = printConfig.Updated,
                Enable = printConfig.Enable,
                State = printConfig.State
            };

            printConfigDto.VoucherList.AddRange(GetSchemeVoucherList(printGroupScheme));
            printConfigDto.SetInfoList.AddRange(GetSchemeSetInfoList(printGroupScheme));
            printConfigDto.DishTypeCassify.AddRange(GetSchemeDishTypeList(printGroupScheme));

            return printConfigDto;
        }

        /// <summary>
        /// 获取单据
        /// </summary>
        /// <param name="printGroupScheme"></param>
        /// <returns></returns>
        private List<SchemeVoucherDto> GetSchemeVoucherList(PrintGroupScheme printGroupScheme)
        {
            var schemeVoucher = _repositoryContext.GetSet<SchemeVoucher>("select id,name,voucherCode,templateCode,isEnabled,printNum,pattern,schemeId from tb_schemeVoucher t WHERE t.schemeId = @schemeId", new { schemeId = printGroupScheme.Id });

            if (schemeVoucher != null && schemeVoucher.Count > 0)
            {
                List<SchemeVoucherDto> schemeVoucherList = new List<SchemeVoucherDto>();

                foreach (var item in schemeVoucher)
                {
                    SchemeVoucherDto voucherItem = new SchemeVoucherDto();
                    voucherItem.Id = item.Id;
                    voucherItem.Name = item.Name;
                    voucherItem.Describe = item.Describe;
                    voucherItem.VoucherCode = item.VoucherCode;
                    voucherItem.TemplateCode = item.TemplateCode;
                    voucherItem.IsEnabled = item.IsEnabled;
                    voucherItem.PrintNum = item.PrintNum;
                    voucherItem.Pattern = item.Pattern;
                    voucherItem.SchemeId = item.SchemeId;

                    schemeVoucherList.Add(voucherItem);
                }

                return schemeVoucherList;
            }
            else
            {
                return new List<SchemeVoucherDto>();
            }
        }

        /// <summary>
        /// 获取打印方案配置
        /// </summary>
        /// <param name="printGroupScheme"></param>
        /// <returns></returns>
        private List<PrintSetInfoDto> GetSchemeSetInfoList(PrintGroupScheme printGroupScheme)
        {
            var schemeVoucher = _repositoryContext.GetSet<PrintSetInfo>("select id,name,key,value,range,combineId from tb_printSetInfo t WHERE t.combineId = @combineId", new { combineId = printGroupScheme.Id });

            if (schemeVoucher != null && schemeVoucher.Count > 0)
            {
                List<PrintSetInfoDto> printSetInfoList = new List<PrintSetInfoDto>();

                foreach (var item in schemeVoucher)
                {
                    PrintSetInfoDto voucherItem = new PrintSetInfoDto();
                    voucherItem.Id = item.Id;
                    voucherItem.Name = item.Name;
                    voucherItem.Describe = item.Describe;
                    voucherItem.Key = item.Key;
                    voucherItem.Value = item.Value;
                    voucherItem.Range = item.Range;
                    voucherItem.CombineId = item.CombineId;

                    printSetInfoList.Add(voucherItem);
                }

                return printSetInfoList;
            }
            else
            {
                return new List<PrintSetInfoDto>();
            }
        }

        /// <summary>
        /// 获取打印方案的菜品分类
        /// </summary>
        /// <param name="printGroupScheme"></param>
        /// <returns></returns>
        private List<SchemeDishTypeDto> GetSchemeDishTypeList(PrintGroupScheme printGroupScheme)
        {

            var schemeVoucher = _repositoryContext.GetSet<SchemeDishType>("select id,schemeId,mkDishTypeID,erpDishTypeID,dishTypeName from tb_schemeDishType t WHERE t.schemeId = @schemeId", new { schemeId = printGroupScheme.Id });

            if (schemeVoucher != null && schemeVoucher.Count > 0)
            {
                List<SchemeDishTypeDto> printDishTypes = new List<SchemeDishTypeDto>();

                foreach (var item in schemeVoucher)
                {
                    SchemeDishTypeDto dishType = new SchemeDishTypeDto();
                    dishType.Id = item.Id;
                    dishType.SchemeId = item.SchemeId;
                    dishType.MKDishTypeID = item.MKDishTypeID;
                    dishType.ErpDishTypeID = item.ErpDishTypeID;
                    dishType.DishTypeName = item.DishTypeName;

                    printDishTypes.Add(dishType);
                }

                return printDishTypes;
            }
            else
            {
                return new List<SchemeDishTypeDto>();
            }
        }

        /// <summary>
        /// 扫码点菜单菜品整理
        /// </summary>
        /// <param name="billDishs"></param>
        /// <param name="tags"></param>
        /// <param name="setMealDatas"></param>
        /// <returns></returns>
        private List<SmdcdDishDto> GetSmdcdDishDtos(List<BillDishData> billDishs, List<Tag> tags, List<OrderSetMealData> setMealDatas)
        {
            var smdcdDishDtos = new List<SmdcdDishDto>();
            if (billDishs == null)
            {
                return smdcdDishDtos;
            }
            foreach (var dish in billDishs)
            {
                var smdcdDishDto = new SmdcdDishDto
                {
                    DishName = dish.DishName,
                    TakeCount = dish.Amount,
                    DishDescription = dish.Remark,
                    UnitPrice = dish.Price,
                    WeightNames = new List<string>(),
                    PracticeNames = new List<string>(),
                    IsWeight = dish.IsWeight
                };
                if (tags != null)
                {
                    var tabList = tags.Where(x => x.OrderDishId == dish.BillDishId).ToList();
                    foreach (var tag in tabList)
                    {
                        switch (tag.Type)
                        {
                            case 0:
                                smdcdDishDto.WeightNames.Add(tag.TagName);
                                break;
                            case 1:
                                smdcdDishDto.PracticeNames.Add(tag.TagName);
                                break;
                        }
                    }
                }
                if (setMealDatas != null)
                {
                    var orderSetMealDatas = setMealDatas.Where(x => x.SetMealDishId == dish.BillDishId.ToString()).ToList();
                    foreach (var setMeal in orderSetMealDatas)
                    {
                        var setMealDto = new SmdcdDishDto
                        {
                            DishName = setMeal.DishName,
                            TakeCount = setMeal.Amount,
                            DishDescription = dish.Remark,
                            UnitPrice = setMeal.Price,
                            WeightNames = new List<string>(),
                            PracticeNames = new List<string>()
                        };
                        smdcdDishDto.SubDishes.Add(setMealDto);
                        if (tags != null)
                        {
                            var tabList = tags.Where(x => x.OrderDishId.ToString() == setMeal.BillDishId).ToList();
                            foreach (var tag in tabList)
                            {
                                switch (tag.Type)
                                {
                                    case 0:
                                        setMealDto.WeightNames.Add(tag.TagName);
                                        break;
                                    case 1:
                                        setMealDto.PracticeNames.Add(tag.TagName);
                                        break;
                                }
                            }
                        }
                    }
                }
                smdcdDishDtos.Add(smdcdDishDto);
            }
            return smdcdDishDtos;
        }

        /// <summary>
        /// 通过打印方案的配置信息来筛选打印机信息
        /// </summary>
        /// <param name="printConfigV2"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<PrintConfigDto> GetPrintConfigBySet(List<V2PrintConfigDto> printConfigV2, PrintEnumCode.SetInfoKey key)
        {
            List<PrintConfigDto> result = new List<PrintConfigDto>();

            foreach (var item in printConfigV2)
            {
                switch (key)
                {
                    case PrintEnumCode.SetInfoKey.FailOrder:
                        var tempFailOrder = item.SetInfoList.FirstOrDefault(t => t.Key == "101");
                        if (tempFailOrder == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempFailOrder.Value.ToLower() == "false")
                        {
                            continue;
                        }
                        break;
                    case PrintEnumCode.SetInfoKey.PayFail:
                        var tempPayFail = item.SetInfoList.FirstOrDefault(t => t.Key == "102");
                        if (tempPayFail == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempPayFail.Value.ToLower() == "false")
                        {
                            continue;
                        }
                        break;
                    default:
                        break;
                }

                PrintConfigDto printConfigDto = new PrintConfigDto();
                printConfigDto.PrintId = item.PrintId;
                printConfigDto.Pcid = item.Pcid;
                printConfigDto.PrintName = item.PrintName;
                printConfigDto.Alias = item.Alias;
                printConfigDto.ConnStyle = item.ConnStyle;
                printConfigDto.ConnAddress = item.ConnAddress;
                printConfigDto.ConnBrand = item.ConnBrand;
                printConfigDto.ConnPort = item.ConnPort;
                printConfigDto.PaperType = item.PaperType;
                printConfigDto.TerminalName = item.TerminalName;
                printConfigDto.PaperWidth = item.PaperWidth;
                printConfigDto.TopMargin = item.TopMargin;
                printConfigDto.LeftMargin = item.LeftMargin;
                printConfigDto.ModifyTime = item.ModifyTime;
                printConfigDto.IsDefault = item.IsDefault;
                printConfigDto.Updated = item.Updated;
                printConfigDto.Enable = item.Enable;
                printConfigDto.State = item.State;

                result.Add(printConfigDto);
            }

            return result;
        }

        /// <summary>
        /// 通过单据来筛选打印机信息
        /// 确定张数
        /// </summary>
        /// <param name="printConfigV2"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<PrintConfigDto> GetPrintConfigByVoucher(List<V2PrintConfigDto> printConfigV2, PrintEnumCode.VoucherCode code)
        {
            List<PrintConfigDto> result = new List<PrintConfigDto>();

            foreach (var item in printConfigV2)
            {
                int printNumInfo = 0;
                switch (code)
                {
                    case PrintEnumCode.VoucherCode.Order://点菜单
                        var tempOrder = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 10);
                        if (tempOrder == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempOrder.IsEnabled == 0)
                        {
                            continue;
                        }

                        printNumInfo = tempOrder.PrintNum;
                        break;
                    case PrintEnumCode.VoucherCode.ChechOut://结账单
                        var tempPayOrder = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 11);
                        if (tempPayOrder == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempPayOrder.IsEnabled == 0)
                        {
                            continue;
                        }
                        printNumInfo = tempPayOrder.PrintNum;
                        break;
                    case PrintEnumCode.VoucherCode.Payment://支付凭证
                        var tempPayment = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 20);
                        if (tempPayment == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempPayment.IsEnabled == 0)
                        {
                            continue;
                        }
                        printNumInfo = tempPayment.PrintNum;
                        break;
                    case PrintEnumCode.VoucherCode.Refund://退款凭证
                        var tempRefundr = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 21);
                        if (tempRefundr == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempRefundr.IsEnabled == 0)
                        {
                            continue;
                        }
                        printNumInfo = tempRefundr.PrintNum;
                        break;
                    case PrintEnumCode.VoucherCode.ByDesk://厨打总单
                        var tempByDesk = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 30);
                        if (tempByDesk == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempByDesk.IsEnabled == 0)
                        {
                            continue;
                        }
                        printNumInfo = tempByDesk.PrintNum;
                        break;
                    case PrintEnumCode.VoucherCode.ByDish://厨打分单
                        var tempByDish = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 31);
                        if (tempByDish == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempByDish.IsEnabled == 0)
                        {
                            continue;
                        }
                        printNumInfo = tempByDish.PrintNum;
                        break;
                    case PrintEnumCode.VoucherCode.PreOrder://厨打分单
                        var tempPreOrder = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 40);
                        if (tempPreOrder == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempPreOrder.IsEnabled == 0)
                        {
                            continue;
                        }
                        printNumInfo = tempPreOrder.PrintNum;
                        break;
                    case PrintEnumCode.VoucherCode.TKout://外卖单
                        var tempTKout = item.VoucherList.FirstOrDefault(t => t.VoucherCode == 50);
                        if (tempTKout == null)
                        {
                            continue;
                        }
                        //不启用
                        if (tempTKout.IsEnabled == 0)
                        {
                            continue;
                        }
                        printNumInfo = tempTKout.PrintNum;
                        break;
                    default:
                        break;
                }

                PrintConfigDto printConfigDto = new PrintConfigDto();
                printConfigDto.PrintId = item.PrintId;
                printConfigDto.Pcid = item.Pcid;
                printConfigDto.PrintName = item.PrintName;
                printConfigDto.Alias = item.Alias;
                printConfigDto.ConnStyle = item.ConnStyle;
                printConfigDto.ConnAddress = item.ConnAddress;
                printConfigDto.ConnBrand = item.ConnBrand;
                printConfigDto.ConnPort = item.ConnPort;
                printConfigDto.PaperType = item.PaperType;
                printConfigDto.TerminalName = item.TerminalName;
                printConfigDto.PaperWidth = item.PaperWidth;
                printConfigDto.TopMargin = item.TopMargin;
                printConfigDto.LeftMargin = item.LeftMargin;
                printConfigDto.ModifyTime = item.ModifyTime;
                printConfigDto.IsDefault = item.IsDefault;
                printConfigDto.Updated = item.Updated;
                printConfigDto.Enable = item.Enable;
                printConfigDto.State = item.State;
                printConfigDto.PrintNum = printNumInfo;//确定打印的数量

                result.Add(printConfigDto);
            }

            return result;
        }

        /// <summary>
        /// 套餐菜品的打印信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="origin"></param>
        /// <param name="setmealFlag"></param>
        /// <param name="table"></param>
        /// <param name="kitchenDishes"></param>
        /// <returns></returns>
        private List<KitchenSchemeByDishType> GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode code, PrintEnumCode.OriginCode origin, bool setmealFlag, string table, List<KitchenDish> kitchenDishes)
        {
            List<KitchenSchemeByDishType> result = new List<KitchenSchemeByDishType>();
            List<KitchenDish> needRemoveDishes = kitchenDishes;
            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode", new { printCode = code });
            if (printGroup == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.DatabaseError;
                Message = "找不到打印方案组";
                return new List<KitchenSchemeByDishType>();
            }

            var sqlTableStr = new StringBuilder();
            sqlTableStr.Append("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme ");
            sqlTableStr.Append($"where groupId = {printGroup.Id} and state != 0 and id in ");
            sqlTableStr.Append($"( select schemeId from tb_schemeTable ");
            switch (origin)
            {
                case PrintEnumCode.OriginCode.MK:
                    sqlTableStr.Append($"where MKTableID  in ('{table}') ) ");
                    break;
                case PrintEnumCode.OriginCode.ERP:
                    sqlTableStr.Append($"where ErpTableID in ('{table}') ) ");
                    break;
            }

            sqlTableStr.Append("order by createTime asc ");
            var printGroupSchemesTable = _repositoryContext.GetSet<PrintGroupScheme>(sqlTableStr.ToString());

            var kitchenNormalDish = kitchenDishes.Where(t => t.IsSetMeal == false).ToList();
            var kitchenSetmealDish = kitchenDishes.Where(t => t.IsSetMeal == true).ToList();
            if (printGroupSchemesTable != null && printGroupSchemesTable.Count > 0)
            {
                foreach (var tableScheme in printGroupSchemesTable)
                {
                    var sqlTableSchemeStr = new StringBuilder();
                    sqlTableSchemeStr.Append("select id,schemeId,mkDishTypeID,erpDishTypeID,dishTypeName from tb_schemeDishType ");
                    sqlTableSchemeStr.Append($"where schemeId = {tableScheme.Id}");
                    var printGroupSchemesDishType = _repositoryContext.GetSet<SchemeDishType>(sqlTableSchemeStr.ToString());
                    List<string> dishTypeIdList = new List<string>();
                    switch (origin)
                    {
                        case PrintEnumCode.OriginCode.MK:
                            dishTypeIdList = printGroupSchemesDishType.Select(t => t.MKDishTypeID).ToList();
                            break;
                        case PrintEnumCode.OriginCode.ERP:
                            dishTypeIdList = printGroupSchemesDishType.Select(t => t.ErpDishTypeID).ToList();
                            break;
                    }

                    #region 获取打印机配置信息
                    var printConfig = _repositoryContext.FirstOrDefault<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId = @printId ", new { printId = tableScheme.PrintId });

                    if (printConfig == null)
                    {
                        continue;
                    }

                    V2PrintConfigDto printConfigDto = new V2PrintConfigDto
                    {
                        PrintId = printConfig.PrintId,
                        Pcid = printConfig.Pcid,
                        TerminalName = printConfig.TerminalName,
                        PrintName = printConfig.PrintName,
                        ConnStyle = printConfig.ConnStyle,
                        Alias = printConfig.Alias,
                        PaperType = printConfig.PaperType,
                        PaperWidth = printConfig.PaperWidth,
                        TopMargin = printConfig.TopMargin,
                        LeftMargin = printConfig.LeftMargin,
                        ModifyTime = printConfig.ModifyTime,
                        Updated = printConfig.Updated,
                        Enable = printConfig.Enable,
                        State = printConfig.State
                    };

                    printConfigDto.VoucherList.AddRange(GetSchemeVoucherList(tableScheme));
                    printConfigDto.SetInfoList.AddRange(GetSchemeSetInfoList(tableScheme));
                    printConfigDto.DishTypeCassify.AddRange(GetSchemeDishTypeList(tableScheme));
                    var printConfigDtos = new List<V2PrintConfigDto>();
                    printConfigDtos.Add(printConfigDto);
                    #endregion

                    KitchenSchemeByDishType ksByDt = new KitchenSchemeByDishType();
                    ksByDt.PrintConfigs = GetPrintConfigByVoucher(printConfigDtos, PrintEnumCode.VoucherCode.ByDesk).FirstOrDefault();
                    ksByDt.KitchenDishes = kitchenNormalDish.Where(t => dishTypeIdList.Contains(t.DishTypeId)).ToList();
                    needRemoveDishes.RemoveAll(t => dishTypeIdList.Contains(t.DishTypeId));
                    foreach (var set in kitchenSetmealDish)//每次只会有一条数据进来，座椅直接添加
                    {
                        var inSet = set.SubDishes.Where(t => dishTypeIdList.Contains(t.SetMealTypeId)).ToList();
                        if (inSet != null && inSet.Count > 0)
                        {
                            var addSetmeal = ksByDt.KitchenDishes.FirstOrDefault(t => t.DishId == set.DishId && t.DishTypeId == set.DishTypeId && t.FlowId == set.FlowId);
                            if (addSetmeal == null)
                            {
                                ksByDt.KitchenDishes.Add(set);
                            }
                            else
                            {
                                foreach (var item in set.SubDishes)
                                {
                                    KitchenSetMeal addSet = new KitchenSetMeal();
                                    addSet.DishSetName = item.DishSetName;
                                    addSet.DishSetId = item.DishSetId;
                                    addSet.Amount = item.Amount;
                                    addSet.Unit = item.Unit;
                                    addSet.SetMealTypeId = item.SetMealTypeId;
                                    addSet.TagInfo = item.TagInfo;
                                    addSet.DishRemark = item.DishRemark;

                                    addSetmeal.SubDishes.Add(item);
                                    needRemoveDishes.RemoveAll(t => t.DishTypeId == set.DishTypeId);
                                    break;
                                }
                            }
                        }
                    }

                    ksByDt.VoucherList.AddRange(GetSchemeVoucherList(tableScheme));
                    result.Add(ksByDt);
                }
            }

            return result;
        }

        /// <summary>
        /// 普通的菜品的打印的信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="origin"></param>
        /// <param name="table"></param>
        /// <param name="kitchenDishes"></param>
        /// <returns></returns>
        private List<KitchenSchemeByDishType> GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode code, PrintEnumCode.OriginCode origin, string table, List<KitchenDish> kitchenDishes)
        {
            List<KitchenSchemeByDishType> result = new List<KitchenSchemeByDishType>();

            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode", new { printCode = code });
            if (printGroup == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.DatabaseError;
                Message = "找不到打印方案组";
                return new List<KitchenSchemeByDishType>();
            }

            var sqlTableStr = new StringBuilder();
            sqlTableStr.Append("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme ");
            sqlTableStr.Append($"where groupId = {printGroup.Id} and state != 0 and id in ");
            sqlTableStr.Append("( select schemeId from tb_schemeTable ");
            switch (origin)
            {
                case PrintEnumCode.OriginCode.MK:
                    sqlTableStr.Append($"where MKTableID  in ('{table}') ) ");
                    break;
                case PrintEnumCode.OriginCode.ERP:
                    sqlTableStr.Append($"where ErpTableID in ('{table}') ) ");
                    break;
            }

            sqlTableStr.Append("order by createTime asc ");
            var printGroupSchemesTable = _repositoryContext.GetSet<PrintGroupScheme>(sqlTableStr.ToString());

            List<SchemeAndDish> dishTypeSchemeIdList = new List<SchemeAndDish>();
            List<string> dishTypeWithNullSchemeList = new List<string>();//没有配置打印方案的菜品id
            List<string> dishList = kitchenDishes.Select(t => t.DishTypeId).ToList();
            foreach (var dishTag in dishList)
            {
                var sqlDTypeStr = new StringBuilder();
                sqlDTypeStr.Append("select id,schemeId,mkDishTypeID,erpDishTypeID,dishTypeName from tb_schemeDishType ");
                switch (origin)
                {
                    case PrintEnumCode.OriginCode.MK:
                        sqlDTypeStr.Append($"where mkDishTypeID = '{dishTag}' ) ");
                        break;
                    case PrintEnumCode.OriginCode.ERP:
                        sqlDTypeStr.Append($"where erpDishTypeID = '{dishTag}' ) ");
                        break;
                }
                var schemeDishType = _repositoryContext.GetSet<SchemeDishType>(sqlDTypeStr.ToString());
                if (schemeDishType == null && schemeDishType.Count <= 0)
                {
                    //当前菜品分类没有配置打印方案
                    //添加到默认打印机中
                    dishTypeWithNullSchemeList.Add(dishTag);
                }
                else
                {
                    foreach (var type in schemeDishType)
                    {
                        SchemeAndDish schemeAndDish = new SchemeAndDish()
                        {
                            IsSetmeal = false,
                            SchemeId = type.SchemeId
                        };

                        switch (origin)
                        {
                            case PrintEnumCode.OriginCode.MK:
                                schemeAndDish.DishTypeId = type.MKDishTypeID;
                                break;
                            case PrintEnumCode.OriginCode.ERP:
                                schemeAndDish.DishTypeId = type.ErpDishTypeID;
                                break;
                        }
                        dishTypeSchemeIdList.Add(schemeAndDish);
                    }
                }
            }

            if (printGroupSchemesTable != null && printGroupSchemesTable.Count > 0)
            {
                foreach (var tableScheme in printGroupSchemesTable)
                {
                    #region 获取打印机配置信息
                    var printConfig = _repositoryContext.FirstOrDefault<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId = @printId ", new { printId = tableScheme.PrintId });

                    if (printConfig == null)
                    {
                        continue;
                    }

                    V2PrintConfigDto printConfigDto = new V2PrintConfigDto
                    {
                        PrintId = printConfig.PrintId,
                        Pcid = printConfig.Pcid,
                        TerminalName = printConfig.TerminalName,
                        PrintName = printConfig.PrintName,
                        ConnStyle = printConfig.ConnStyle,
                        Alias = printConfig.Alias,
                        PaperType = printConfig.PaperType,
                        PaperWidth = printConfig.PaperWidth,
                        TopMargin = printConfig.TopMargin,
                        LeftMargin = printConfig.LeftMargin,
                        ModifyTime = printConfig.ModifyTime,
                        Updated = printConfig.Updated,
                        Enable = printConfig.Enable,
                        State = printConfig.State
                    };

                    printConfigDto.VoucherList.AddRange(GetSchemeVoucherList(tableScheme));
                    printConfigDto.SetInfoList.AddRange(GetSchemeSetInfoList(tableScheme));
                    printConfigDto.DishTypeCassify.AddRange(GetSchemeDishTypeList(tableScheme));
                    var printConfigDtos = new List<V2PrintConfigDto>();
                    printConfigDtos.Add(printConfigDto);
                    #endregion

                    var inTableScheme = dishTypeSchemeIdList.Where(t => t.SchemeId == tableScheme.Id).ToList();
                    if (inTableScheme != null && inTableScheme.Count > 0)
                    {
                        var distinctId = inTableScheme.Select(x => x.DishTypeId).Distinct();//当前餐桌对应的餐桌方案中所有的菜品分类找出来

                        KitchenSchemeByDishType ksByDt = new KitchenSchemeByDishType();
                        ksByDt.PrintConfigs = GetPrintConfigByVoucher(printConfigDtos, PrintEnumCode.VoucherCode.ByDesk).FirstOrDefault();
                        ksByDt.KitchenDishes = kitchenDishes.Where(t => distinctId.Contains(t.DishTypeId)).ToList();
                        ksByDt.VoucherList.AddRange(GetSchemeVoucherList(tableScheme));
                        result.Add(ksByDt);
                    }
                    else
                    {
                        //餐桌菜系对应不上不打印
                        continue;
                    }
                }
            }

            if (dishTypeWithNullSchemeList != null && dishTypeWithNullSchemeList.Count > 0)
            {
                var printGroupScheme = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme WHERE state = 1 and isDefault = 1 and groupId = @groupId ", new { groupId = printGroup.Id });

                if (printGroupScheme != null)
                {
                    var printConfig = _repositoryContext.FirstOrDefault<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId = @printId ", new { printId = printGroupScheme.PrintId });

                    if (printConfig != null)
                    {
                        V2PrintConfigDto printConfigDto = new V2PrintConfigDto
                        {
                            PrintId = printConfig.PrintId,
                            Pcid = printConfig.Pcid,
                            TerminalName = printConfig.TerminalName,
                            PrintName = printConfig.PrintName,
                            ConnStyle = printConfig.ConnStyle,
                            Alias = printConfig.Alias,
                            PaperType = printConfig.PaperType,
                            PaperWidth = printConfig.PaperWidth,
                            TopMargin = printConfig.TopMargin,
                            LeftMargin = printConfig.LeftMargin,
                            ModifyTime = printConfig.ModifyTime,
                            Updated = printConfig.Updated,
                            Enable = printConfig.Enable,
                            State = printConfig.State
                        };

                        printConfigDto.VoucherList.AddRange(GetSchemeVoucherList(printGroupScheme));
                        printConfigDto.SetInfoList.AddRange(GetSchemeSetInfoList(printGroupScheme));
                        printConfigDto.DishTypeCassify.AddRange(GetSchemeDishTypeList(printGroupScheme));
                        var printConfigDtos = new List<V2PrintConfigDto>();
                        printConfigDtos.Add(printConfigDto);

                        KitchenSchemeByDishType ksByDt = new KitchenSchemeByDishType();
                        ksByDt.PrintConfigs = GetPrintConfigByVoucher(printConfigDtos, PrintEnumCode.VoucherCode.ByDesk).FirstOrDefault();
                        foreach (var typeId in dishTypeWithNullSchemeList.Distinct())
                        {
                            ksByDt.KitchenDishes.AddRange(kitchenDishes.Where(t => t.DishTypeId == typeId).ToList());
                        }
                        ksByDt.VoucherList.AddRange(GetSchemeVoucherList(printGroupScheme));

                        result.Add(ksByDt);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取是否按照套餐子菜品打印
        /// </summary>
        /// <returns></returns>
        private bool GetKitchenPrintInSetMeal(PrintEnumCode.PrintGroupCode code)
        {
            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode", new { printCode = code });
            if (printGroup == null)
            {
                //不存在打印方案组
                return false;
            }

            var prittSetInfo = _repositoryContext.FirstOrDefault<PrintSetInfo>("select id,name,key,value,range,combineId from tb_printSetInfo where range = @range  and combineId = @combineId ", new { range = 1, combineId = printGroup.Id });
            if (prittSetInfo == null)
            {
                //不存在该打印方案组的设置信息
                return false;
            }

            try
            {
                return Convert.ToBoolean(prittSetInfo.Value);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 跟着默认打印方案打印
        /// </summary>
        /// <param name="printDefaultConfig"></param>
        /// <param name="htmlPrintingServices"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        private void PrintWithDefaultScheme(V2PrintConfigDto printDefaultConfig, IHtmlPrintingServices htmlPrintingServices, string path, object data)
        {
            PrintConfigDto printConfigDto = new PrintConfigDto();
            printConfigDto.PrintId = printDefaultConfig.PrintId;
            printConfigDto.Pcid = printDefaultConfig.Pcid;
            printConfigDto.PrintName = printDefaultConfig.PrintName;
            printConfigDto.Alias = printDefaultConfig.Alias;
            printConfigDto.ConnStyle = printDefaultConfig.ConnStyle;
            printConfigDto.ConnAddress = printDefaultConfig.ConnAddress;
            printConfigDto.ConnBrand = printDefaultConfig.ConnBrand;
            printConfigDto.ConnPort = printDefaultConfig.ConnPort;
            printConfigDto.PaperType = printDefaultConfig.PaperType;
            printConfigDto.TerminalName = printDefaultConfig.TerminalName;
            printConfigDto.PaperWidth = printDefaultConfig.PaperWidth;
            printConfigDto.TopMargin = printDefaultConfig.TopMargin;
            printConfigDto.LeftMargin = printDefaultConfig.LeftMargin;
            printConfigDto.ModifyTime = printDefaultConfig.ModifyTime;
            printConfigDto.IsDefault = printDefaultConfig.IsDefault;
            printConfigDto.Updated = printDefaultConfig.Updated;
            printConfigDto.Enable = printDefaultConfig.Enable;
            printConfigDto.State = printDefaultConfig.State;
            htmlPrintingServices.LocalPrintScheme(printConfigDto, path, data);
        }

        #endregion

        #region 测试打印
        /// <summary>
        /// 测试打印单据(点菜打印方案)
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="printConfig"></param>
        /// <param name="voucherCode"></param>
        public void PrintTestByTemplateCodeV2(string templateCode, PrintConfigDto printConfig, int voucherCode)
        {
            if (string.IsNullOrEmpty(templateCode))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            if (string.IsNullOrEmpty(printConfig.Pcid))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }

            object mockVoucherData = null;
            switch (templateCode)
            {
                case "PRT_SO_0001":
                    var mockScanCodeBill = GetMockScanCodeBill();
                    //打印结账单
                    if (voucherCode == 11)
                    {
                        mockScanCodeBill.IsCheckout = true;
                    }
                    mockScanCodeBill.Footer = "本单为打印测试";
                    mockScanCodeBill.FooterAlignment = "center";
                    mockVoucherData = mockScanCodeBill;
                    break;
                case "PRT_FI_0001":
                    var mockPaymentVoucher = GetMockPaymentVoucher();
                    mockVoucherData = mockPaymentVoucher;
                    break;
                case "PRT_FI_0002":
                    var mockRefundVoucher = GetMockRefundVoucher();
                    mockVoucherData = mockRefundVoucher;
                    break;
                case "PRT_SO_1001":
                    var preOrderDto = GetMockKbPreOrder();
                    mockVoucherData = preOrderDto;
                    break;
                case "PRT_TO_0001":
                    var tkOrderDto = GetMockKbTakeOutOrder();
                    mockVoucherData = tkOrderDto;
                    break;
                default:
                    Flag = false;
                    Message = "没有目标打印模板信息";
                    this.Code = PrintErrorCode.Code.VoucherNullError;
                    break;
            }

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            htmlPrintingServices.GlobalPrintScheme(printConfig, voucher.Path, mockVoucherData, 1);
            Flag = true;
        }

        private SmdcdDto GetMockScanCodeBill()
        {
            var smdcdDto = new SmdcdDto()
            {
                TableType = "餐桌号",
                IsFailured = false,
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                TableNo = "A01",
                BillNo = "测试单",
                PersonCount = 3,
                OrderTakeDate = DateTime.Now,
                Remark = "不吃香菜",
                Dishes = new List<SmdcdDishDto>(),
                TotalAmount = 158,
                DiscountAmounts = new List<SmdcdDiscountAmountDto>(),
                ShouldAmount = 138,
                PayTypeAmounts = new List<SmdcdPayTypeAmountDto>(),
                InvoiceTitle = string.Empty,
                TaxpayerIdentify = string.Empty,
                ExceptionDishes = new List<SmdcdDishDto>(),
                ExceptionReason = string.Empty,
                PrintingDate = DateTime.Now
            };
            smdcdDto.BillNo = DateTime.Now.ToString("yyyyMMdd") + "0000";
            smdcdDto.Dishes.Add(new SmdcdDishDto()
            {
                DishName = "菲力牛排",
                TakeCount = 1,
                DishDescription = String.Empty,
                UnitPrice = 58,
                WeightNames = new List<string>(),
                PracticeNames = new List<string>() { "8成熟", "番茄酱" },
                SubDishes = new List<SmdcdDishDto>()
            });
            smdcdDto.Dishes.Add(new SmdcdDishDto()
            {
                DishName = " &#60打包&#62蛋炒饭",
                TakeCount = 1,
                DishDescription = string.Empty,
                UnitPrice = 12,
                WeightNames = new List<string>(),
                PracticeNames = new List<string>(),
                SubDishes = new List<SmdcdDishDto>()
            });
            smdcdDto.Dishes.Add(new SmdcdDishDto()
            {
                DishName = "双人套餐",
                TakeCount = 1,
                DishDescription = string.Empty,
                UnitPrice = 88,
                WeightNames = new List<string>(),
                PracticeNames = new List<string>(),
                SubDishes = new List<SmdcdDishDto>() { new SmdcdDishDto()
                {
                    DishName = "·排骨汤",
                    TakeCount = 1,
                    DishDescription = String.Empty,
                    UnitPrice = 0.01m,
                    WeightNames = new List<string>(),
                    PracticeNames = new List<string>(),
                    SubDishes = new List<SmdcdDishDto>()
                },new SmdcdDishDto()
                {
                    DishName = "·炒米粉",
                    TakeCount = 2,
                    DishDescription = String.Empty,
                    UnitPrice = 0.02m,
                    WeightNames = new List<string>(),
                    PracticeNames = new List<string>(),
                    SubDishes = new List<SmdcdDishDto>()
                },new SmdcdDishDto()
                    {
                        DishName = "·上海青",
                        TakeCount = 1,
                        DishDescription = String.Empty,
                        UnitPrice = 0.01m,
                        WeightNames = new List<string>(),
                        PracticeNames = new List<string>(),
                        SubDishes = new List<SmdcdDishDto>()
                    },new SmdcdDishDto()
                    {
                        DishName = "·拌牛肚",
                        TakeCount = 1,
                        DishDescription = String.Empty,
                        UnitPrice = 0.01m,
                        WeightNames = new List<string>(),
                        PracticeNames = new List<string>(),
                        SubDishes = new List<SmdcdDishDto>()
                    }}
            });
            smdcdDto.DiscountAmounts.Add(new SmdcdDiscountAmountDto()
            {
                DiscountName = "优惠券",
                PayTypeName = string.Empty,
                Price = 20
            });
            smdcdDto.PayTypeAmounts.Add(new SmdcdPayTypeAmountDto()
            {
                PayTypeName = "支付宝",
                Price = 138
            });
            return smdcdDto;
        }

        private TkpzDto GetMockRefundVoucher()
        {
            var tkpzDto = new TkpzDto()
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                RefundAmount = 0.01m,
                RefundDate = DateTime.Now,
                RefundAccount = "打印测试",
                RefundTypeName = "支付宝(打印测试)",
                PaymentVoucherId = "打印测试",
                RefundVoucherId = "打印测试",
                Operator = "打印测试",
                PrintingDate = DateTime.Now
            };

            return tkpzDto;
        }

        private SfpzDto GetMockPaymentVoucher()
        {
            var sfpzDto = new SfpzDto
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                ShouldAmount = 0.01m,
                PayDiscount = 0,
                ActualAmount = 0.01m,
                UserPayment = 0.01m,
                PaymentDescription = "打印测试",
                PaymentDate = DateTime.Now,
                PaymentTypeName = "支付宝(打印测试)",
                PaymentAccount = "打印测试",
                PaymentVoucherId = "55555666666666465461321635416513213241651321354123435",
                BillRunningId = "打印测试",
                PrintingDate = DateTime.Now
            };

            return sfpzDto;
        }

        private PreOrderDto GetMockKbPreOrder()
        {
            var preOrderDto = new PreOrderDto()
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                OrderTime = DateTime.Now,
                TakeMealNum = "0000",
                Scene = "堂食",
                CustomerName = "米小客",
                CustomerPhone = "12345678901",
                EatingTime = DateTime.Now,
                CustomerCount = 4,
                Remark = "本单为打印测试",
                TotalAmount = 160,
                ShouldAmount = 140,
                PrintingDate = DateTime.Now
            };
            preOrderDto.Dishes = new List<PreOrderDishDto>() { new PreOrderDishDto() { DishName = "菲力牛排", TakeCount = 1, DishDescription = String.Empty, UnitPrice = 58, WeightNames = new List<string>(), PracticeNames = new List<string>() { "8成熟", "番茄酱" } }, new PreOrderDishDto() { DishName = "牛肉炒饭", TakeCount = 1, DishDescription = String.Empty, UnitPrice = 12, WeightNames = new List<string>(), PracticeNames = new List<string>() }, new PreOrderDishDto() { DishName = "双人套餐", TakeCount = 1, DishDescription = String.Empty, UnitPrice = 88, WeightNames = new List<string>(), PracticeNames = new List<string>(), SubDishes = new List<PreOrderDishDto>() { new PreOrderDishDto() { DishName = "[套]排骨汤", TakeCount = 1 }, new PreOrderDishDto() { DishName = "[套]炒米粉", TakeCount = 2 }, new PreOrderDishDto() { DishName = "[套]上海青", TakeCount = 1 }, new PreOrderDishDto() { DishName = "[套]拌牛肚", TakeCount = 1 } } } };
            preOrderDto.DiscountAmounts = new List<PreOrderDiscountAmountDto>() { new PreOrderDiscountAmountDto() { DiscountName = "优惠券", Price = 20 } };
            preOrderDto.AdditionalCharges = new List<PreOrderAdditionalCostDto>() { new PreOrderAdditionalCostDto() { Name = "餐盒费", Price = 1 }, new PreOrderAdditionalCostDto() { Name = "配送费", Price = 1 } };
            return preOrderDto;
        }

        private TakeOutOrderDto GetMockKbTakeOutOrder()
        {
            TakeOutOrderDto tabkOrderDto = new TakeOutOrderDto
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                OrderTime = DateTime.Now,
                BillOrderOrigin = "美团外卖#1",
                SendMethord = "美团配送",
                InvoiceTitle = "913123231313MAAS1231331",
                TaxpayerNo = "福建米客互联网科技有限公司",
                TakeMealTimer = "立即送餐",
                Remark = "不要葱",
                Dishes = new List<TkOrderDishDto>(),
                Others = new List<TkOtherListDto>(),
                PrintTime = DateTime.Now,
                TotalAmount = _currency + "105",
                ShouldAmount = _currency + "105",
                CustomerName = "米小客",
                CustomerPhone = "13012345678",
                CustomerAddress = @"西洪路528号云座1号7楼 <br />本单为打印测试",
            };
            tabkOrderDto.Dishes.Add(new TkOrderDishDto()
            {
                DishName = "菲力牛排",
                Amount = "1",
                Price = "50",
                IsSetMeal = false,
                TagInfo = "8成熟、番茄酱",
                SubDishes = new List<TkOrderDishSetDto>(),
                DishRemark = ""
            });
            tabkOrderDto.Dishes.Add(new TkOrderDishDto()
            {
                DishName = "&#60打包&#62蛋炒饭",
                Amount = "1",
                Price = "10",
                IsSetMeal = false,
                TagInfo = "",
                SubDishes = new List<TkOrderDishSetDto>(),
                DishRemark = ""
            });
            tabkOrderDto.Dishes.Add(new TkOrderDishDto()
            {
                DishName = "双人套餐",
                Amount = "1",
                Price = "40",
                IsSetMeal = true,
                TagInfo = "",
                SubDishes = new List<TkOrderDishSetDto>()
                {
                    new TkOrderDishSetDto()
                    {
                        DishSetNameInfo = "·排骨汤",
                        TagSetInfo = "",
                        DishSetRemark = String.Empty,
                    },
                    new TkOrderDishSetDto()
                    {
                        DishSetNameInfo = "·炒米粉",
                        TagSetInfo = "",
                        DishSetRemark = String.Empty,
                    },
                    new TkOrderDishSetDto()
                    {
                        DishSetNameInfo = "·上海青",
                        TagSetInfo = "",
                        DishSetRemark = String.Empty,
                    },
                    new TkOrderDishSetDto()
                    {
                        DishSetNameInfo = "·拌牛肚",
                        TagSetInfo = "",
                        DishSetRemark = String.Empty,
                    }
                },
                DishRemark = ""
            });
            tabkOrderDto.Others.Add(new TkOtherListDto()
            {
                Name = "配送费",
                Price = _currency + 3
            });
            tabkOrderDto.Others.Add(new TkOtherListDto()
            {
                Name = "餐盒费",
                Price = _currency + 2
            });

            return tabkOrderDto;
        }

        #endregion

        /// <summary>
        /// 厨房打印
        /// </summary>
        /// <param name="keKitchenData"></param>
        public void PrintKitchenBill(KitchenData kitchenData)
        {
            if (kitchenData == null)
            {
                Flag = false;
                Message = "厨房打印参数为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            if (kitchenData.KitchenDishes == null && kitchenData.KitchenDishes.Count <= 0)
            {
                Flag = false;
                Message = "厨房菜品的打印参数为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintKitchen data:" + JsonConvert.SerializeObject(kitchenData));

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            #region 基础数据
            KitchenDataDto kitchenDataDto = new KitchenDataDto()
            {
                OrderTime = kitchenData.OrderTime,
                TableType = kitchenData.TableTypeCode == 1 ? "餐桌号" : "取餐号",
                TableName = kitchenData.TableName,
                OrderNo = kitchenData.OrderNo,
                Person = kitchenData.Person,
                Remark = kitchenData.Remark,
                KitchenDishes = new List<KitchenDishDto>(),
            };
            #endregion

            List<KitchenSchemeByDishType> printListConfigAll = new List<KitchenSchemeByDishType>();

            var setmealFlag = GetKitchenPrintInSetMeal(PrintEnumCode.PrintGroupCode.PrtKitchen);
            string table = kitchenData.TableId;
            PrintEnumCode.OriginCode OriginCode = (PrintEnumCode.OriginCode)(kitchenData.OriginCode);
            if (setmealFlag)
            {
                var printKitchenList = kitchenData.KitchenDishes.Where(t => t.IsSetMeal == false).ToList();
                var printSetmealDishList = kitchenData.KitchenDishes.Where(t => t.IsSetMeal == true).ToList();//套餐
                //把套餐子菜品整理成一道道菜品
                foreach (var item in printSetmealDishList)
                {
                    KitchenDish printKitchenDish = new KitchenDish();
                    printKitchenDish.DishName = item.DishName;
                    printKitchenDish.DishId = item.DishId;
                    printKitchenDish.DishTypeId = "";//套餐状态下的菜品分类不用
                    printKitchenDish.Amount = item.Amount;
                    printKitchenDish.Unit = item.Unit;
                    printKitchenDish.Price = item.Price;
                    printKitchenDish.IsWeight = item.IsWeight;
                    printKitchenDish.IsSetMeal = item.IsSetMeal;
                    printKitchenDish.WeightUnit = item.WeightUnit;
                    printKitchenDish.TagInfo = item.TagInfo;
                    printKitchenDish.SubDishes = new List<KitchenSetMeal>();
                    printKitchenDish.DishRemark = item.DishRemark;

                    foreach (var set in item.SubDishes)
                    {
                        printKitchenDish.SubDishes = new List<KitchenSetMeal>();

                        KitchenSetMeal inSet = new KitchenSetMeal();
                        inSet.DishSetName = set.DishSetName;
                        inSet.DishSetId = set.DishSetId;
                        inSet.Amount = set.Amount;
                        inSet.Unit = set.Unit;
                        inSet.SetMealTypeId = set.SetMealTypeId;
                        inSet.TagInfo = set.TagInfo;
                        inSet.DishRemark = set.DishRemark;

                        printKitchenDish.SubDishes.Add(inSet);

                        printKitchenList.Add(printKitchenDish);
                    }
                }

                printListConfigAll = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtKitchen, OriginCode, setmealFlag, table, printKitchenList);
            }
            else
            {

                printListConfigAll = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtKitchen, OriginCode, table, kitchenData.KitchenDishes);
            }

            //是否存在打印方案
            if (printListConfigAll != null && printListConfigAll.Count > 0)
            {
                foreach (var prints in printListConfigAll)
                {
                    foreach (var dish in prints.KitchenDishes)
                    {
                        KitchenDishDto printKitchenDishDto = new KitchenDishDto();
                        printKitchenDishDto.DishName = dish.DishName;
                        printKitchenDishDto.DishTypeId = dish.DishTypeId;
                        printKitchenDishDto.Amount = dish.Amount;
                        printKitchenDishDto.Unit = dish.Unit;
                        printKitchenDishDto.Price = dish.Price;
                        printKitchenDishDto.IsWeight = dish.IsWeight;
                        printKitchenDishDto.IsSetMeal = dish.IsSetMeal;
                        printKitchenDishDto.WeightUnit = dish.WeightUnit;
                        printKitchenDishDto.TagInfo = dish.TagInfo;
                        printKitchenDishDto.DishRemark = dish.DishRemark;
                        printKitchenDishDto.SubDishes = new List<KitchenSetMealDto>();

                        foreach (var set in dish.SubDishes)
                        {
                            KitchenSetMealDto inSet = new KitchenSetMealDto();
                            inSet.DishSetName = set.DishSetName;
                            inSet.Amount = set.Amount;
                            inSet.Unit = set.Unit;
                            inSet.SetMealTypeId = set.SetMealTypeId;
                            inSet.DishRemark = set.DishRemark;
                            inSet.TagInfo = set.TagInfo;

                            printKitchenDishDto.SubDishes.Add(inSet);
                        }

                        kitchenDataDto.KitchenDishes.Add(printKitchenDishDto);
                    }

                    var printByDesk = prints.VoucherList.FirstOrDefault(t => t.VoucherCode == 30);
                    if (printByDesk != null && printByDesk.IsEnabled == 1)
                    {
                        var templateCode = "PRT_SO_3001";
                        var voucher = GetVoucherByCode(templateCode);
                        if (voucher == null)
                        {
                            Flag = false;
                            Message = "厨房打印模板为空";
                            this.Code = PrintErrorCode.Code.VoucherNullError;
                            return;
                        }
                        kitchenData.VoucherId = voucher.Id;
                        kitchenDataDto.PrintTime = DateTime.Now;
                        //htmlPrintingServices.GlobalPrintScheme(prints.PrintConfigs, voucher.Path, kitchenDataDto, printByDesk.PrintNum);
                    }

                    var printByDish = prints.VoucherList.FirstOrDefault(t => t.VoucherCode == 31);
                    if (printByDish != null && printByDish.IsEnabled == 1)
                    {
                        var templateCode = "PRT_SO_3002";
                        var voucher = GetVoucherByCode(templateCode);
                        if (voucher == null)
                        {
                            Flag = false;
                            Message = "厨房打印模板为空";
                            this.Code = PrintErrorCode.Code.VoucherNullError;
                            return;
                        }
                        kitchenData.VoucherId = voucher.Id;
                        KitchenDataDto separateKitchenDataDto = new KitchenDataDto()
                        {
                            OrderTime = kitchenData.OrderTime,
                            TableType = kitchenData.TableTypeCode == 1 ? "餐桌号" : "取餐号",
                            TableName = kitchenData.TableName,
                            OrderNo = kitchenData.OrderNo,
                            Person = kitchenData.Person,
                            Remark = kitchenData.Remark,
                            KitchenDishes = new List<KitchenDishDto>(),
                        };
                        foreach (var printDish in kitchenDataDto.KitchenDishes)
                        {
                            KitchenDishDto separateDish = new KitchenDishDto();
                            separateDish.DishName = printDish.DishName;
                            separateDish.DishTypeId = printDish.DishTypeId;
                            separateDish.Amount = printDish.Amount;
                            separateDish.Unit = printDish.Unit;
                            separateDish.Price = printDish.Price;
                            separateDish.IsWeight = printDish.IsWeight;
                            separateDish.IsSetMeal = printDish.IsSetMeal;
                            separateDish.WeightUnit = printDish.WeightUnit;
                            separateDish.TagInfo = printDish.TagInfo;
                            separateDish.DishRemark = printDish.DishRemark;
                            separateDish.SubDishes = new List<KitchenSetMealDto>();

                            foreach (var set in printDish.SubDishes)
                            {
                                KitchenSetMealDto inSet = new KitchenSetMealDto();
                                inSet.DishSetName = set.DishSetName;
                                inSet.Amount = set.Amount;
                                inSet.Unit = set.Unit;
                                inSet.SetMealTypeId = set.SetMealTypeId;
                                inSet.DishRemark = set.DishRemark;
                                inSet.TagInfo = set.TagInfo;

                                separateDish.SubDishes.Add(inSet);
                            }
                            separateKitchenDataDto.KitchenDishes.Add(separateDish);
                            separateKitchenDataDto.PrintTime = DateTime.Now;
                            htmlPrintingServices.GlobalPrintScheme(prints.PrintConfigs, voucher.Path, separateKitchenDataDto, printByDish.PrintNum);
                        }
                    }
                }
            }

            PrintLogUtility.Writer.SendInfo("PrintKitchenBill 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 打印轻餐菜品总计
        /// </summary>
        /// <param name="printDishStatisticsDto"></param>
        public void PrintDishStatistics(DishStatistics dishStatistics)
        {
            if (dishStatistics == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintDishStatistics 接收 data:" + JsonConvert.SerializeObject(dishStatistics));
            string templateCode = "PRT_FI_0005";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
            var printListConfig = printSchemeServicesV2.GetLocalPrint(dishStatistics.PcId);
            if (printListConfig == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "打印方案为空";
                return;
            }

            DishStatisticsDto dishStatisticsDto = new DishStatisticsDto
            {
                StatisticalTypeName = "分类统计",
                TypeFlag = true,
                StartTime = dishStatistics.StartTime,
                EndTime = dishStatistics.EndTime,
                Dishes = new List<DishStatisticsList>()
            };

            if (dishStatistics.Dishes != null && dishStatistics.Dishes.Count > 0)
            {
                foreach (var dish in dishStatistics.Dishes)
                {
                    DishStatisticsList dishItem = new DishStatisticsList();
                    dishItem.Name = dish.Name;
                    dishItem.TotalDishAmount = dish.TotalDishAmount;

                    dishStatisticsDto.Dishes.Add(dishItem);
                }
            }

            PrintEnumCode.DishTypeCode dishCode = new PrintEnumCode.DishTypeCode();
            dishCode = (PrintEnumCode.DishTypeCode)(dishStatistics.StatisticalType);
            switch (dishCode)
            {
                case PrintEnumCode.DishTypeCode.Sort:
                    dishStatisticsDto.StatisticalTypeName = "分类统计";
                    break;
                case PrintEnumCode.DishTypeCode.Sale:
                    dishStatisticsDto.StatisticalTypeName = "菜品销量";
                    dishStatisticsDto.TypeFlag = false;
                    break;
                case PrintEnumCode.DishTypeCode.Back:
                    dishStatisticsDto.StatisticalTypeName = "退菜数量";
                    dishStatisticsDto.TypeFlag = false;
                    break;
                default:
                    dishStatisticsDto.StatisticalTypeName = "分类统计";
                    break;
            }

            dishStatisticsDto.ShopName = _restaurantName;
            dishStatisticsDto.SubShopName = _restaurantSubName;
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            dishStatisticsDto.PrintTime = DateTime.Now;
            htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, dishStatisticsDto);

            PrintLogUtility.Writer.SendInfo("PrintDishStatistics 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 交班单
        /// </summary>
        /// <param name="restaurantShifts"></param>
        public void PrintRestaurantShifts(RestaurantShifts restaurantShifts)
        {
            if (restaurantShifts == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintRestaurantShifts 接收 data:" + JsonConvert.SerializeObject(restaurantShifts));
            string templateCode = "PRT_FI_0006";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printSchemeServicesV2 = ServiceLocator.Instance.Resolve<IPrintSchemeServicesV2>();
            var printListConfig = printSchemeServicesV2.GetLocalPrint(restaurantShifts.PcId);
            if (printListConfig == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "打印方案为空";
                return;
            }

            RestaurantShiftsDto restaurantShiftsDto = new RestaurantShiftsDto
            {
                TimeWork = restaurantShifts.TimeWork,
                TimeShift = restaurantShifts.TimeShift,
                ShiftOperator = restaurantShifts.ShiftOperator,
                ShiftMachine = restaurantShifts.ShiftMachine,
                WorkBalance = restaurantShifts.WorkBalance,
                ReceiptsCash = restaurantShifts.ReceiptsCash,
                Turnover = restaurantShifts.Turnover,
                PettyCash = restaurantShifts.PettyCash,
                IsPrintSummary = restaurantShifts.IsPrintSummary,
                IsPrintDish = restaurantShifts.IsPrintDish,
                IsPrintRefundDish = restaurantShifts.IsPrintRefundDish,
                Dishes = new List<DishStatisticsList>(),
                PrintTime = DateTime.Now
            };
            if (restaurantShifts.PrintSummaryShifts != null)
            {
                restaurantShiftsDto.PrintSummaryShifts = new SummaryShiftsDto()
                {
                    BillFlowCount = restaurantShifts.PrintSummaryShifts.BillFlowCount,
                    DishCostTotalAmount = restaurantShifts.PrintSummaryShifts.DishCostTotalAmount,
                    ReceiptsTotalAmount = restaurantShifts.PrintSummaryShifts.ReceiptsTotalAmount,
                    ReceiptsDetailList = new List<ReceiptsList>(),
                    OrderDiscountList = new List<OrderCostDscription>()
                };
                foreach (var item in restaurantShifts.PrintSummaryShifts.ReceiptsDetailList)
                {
                    ReceiptsList receipts = new ReceiptsList();
                    receipts.PayName = item.PayName;
                    receipts.Price = item.Price;

                    restaurantShiftsDto.PrintSummaryShifts.ReceiptsDetailList.Add(receipts);
                }
                foreach (var item in restaurantShifts.PrintSummaryShifts.OrderDiscountList)
                {
                    OrderCostDscription discountSimple = new OrderCostDscription();
                    discountSimple.TotalDiscountMoney = item.TotalDiscountMoney;
                    discountSimple.DiscountName = item.DiscountName;

                    restaurantShiftsDto.PrintSummaryShifts.OrderDiscountList.Add(discountSimple);
                }
            }

            foreach (var item in restaurantShifts.Dishes)
            {
                DishStatisticsList dishItem = new DishStatisticsList();
                dishItem.Name = item.Name;
                dishItem.TotalDishAmount = item.TotalDishAmount;

                restaurantShiftsDto.Dishes.Add(dishItem);
            }
            foreach (var item in restaurantShifts.RefundDish)
            {
                DishStatisticsList dishItem = new DishStatisticsList();
                dishItem.Name = item.Name;
                dishItem.TotalDishAmount = item.TotalDishAmount;

                restaurantShiftsDto.RefundDish.Add(dishItem);
            }

            restaurantShiftsDto.IsPrintSummary = restaurantShifts.IsPrintSummary;
            restaurantShiftsDto.ShopName = _restaurantName;
            restaurantShiftsDto.SubShopName = _restaurantSubName;
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            restaurantShiftsDto.PrintTime = DateTime.Now;
            htmlPrintingServices.LocalPrintScheme(printListConfig, voucher.Path, restaurantShiftsDto);

            PrintLogUtility.Writer.SendInfo("PrintRestaurantShifts 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 外卖打印
        /// </summary>
        /// <param name="tabOutOrder"></param>
        public void PrintTakeOutOrder(TakeOutOrder takeOutOrder)
        {
            if (takeOutOrder == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintTakeOutOrder 接收 data:" + JsonConvert.SerializeObject(takeOutOrder));
            string templateCode = "PRT_TO_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }

            TakeOutOrderDto tabkOrderDto = new TakeOutOrderDto
            {
                OrderTime = takeOutOrder.OrderTime,
                BillOrderOrigin = takeOutOrder.BillOrderOrigin,
                SendMethord = takeOutOrder.SendMethord,
                InvoiceTitle = takeOutOrder.InvoiceTitle,
                TaxpayerNo = takeOutOrder.TaxpayerNo,
                TakeMealTimer = takeOutOrder.TakeMealTimer,
                Remark = takeOutOrder.Remark,
                Dishes = new List<TkOrderDishDto>(),
                Others = new List<TkOtherListDto>(),
                PrintTime = DateTime.Now,
                TotalAmount = _currency + takeOutOrder.TotalAmount,
                ShouldAmount = _currency + takeOutOrder.ShouldAmount,
                CustomerName = takeOutOrder.CustomerName,
                CustomerPhone = takeOutOrder.CustomerPhone,
                CustomerAddress = takeOutOrder.CustomerAddress,
            };

            foreach (var item in takeOutOrder.Dishes)
            {
                TkOrderDishDto tkOrder = new TkOrderDishDto();
                tkOrder.DishName = item.DishName;
                tkOrder.Amount = item.Amount.ToString("0.##");
                tkOrder.Price = _currency + item.Price;
                tkOrder.IsSetMeal = item.IsSetMeal;
                tkOrder.SubDishes = new List<TkOrderDishSetDto>();
                tkOrder.DishRemark = item.DishRemark;

                foreach (var set in item.SubDishes)
                {
                    TkOrderDishSetDto tkOrderSet = new TkOrderDishSetDto();
                    tkOrderSet.DishSetNameInfo = set.DishSetNameInfo;
                    tkOrderSet.TagSetInfo = set.TagInfo;
                    tkOrderSet.DishSetRemark = set.DishRemark;

                    tkOrder.SubDishes.Add(tkOrderSet);
                }

                if (item.IsWeight)
                {
                    tkOrder.TagInfo += item.Amount + item.WeightUnit + "、";
                }

                tkOrder.TagInfo += item.TagInfo;

                tabkOrderDto.Dishes.Add(tkOrder);
            }

            foreach (var item in takeOutOrder.Others)
            {
                TkOtherListDto tkOther = new TkOtherListDto();
                tkOther.Name = item.Name;

                if (item.Price < 0)
                {
                    tkOther.Price = "-" + _currency + Math.Abs(item.Price);
                }
                else
                {
                    tkOther.Price = _currency + item.Price;
                }
                tabkOrderDto.Others.Add(tkOther);
            }

            var TKOrderCode = (PrintEnumCode.TKOrderCode)(takeOutOrder.TkOrderCode);
            if (TKOrderCode == PrintEnumCode.TKOrderCode.TK2)
            {
                tabkOrderDto.IsFailured = true;
            }
            else
            {
                tabkOrderDto.IsFailured = false;
            }

            tabkOrderDto.ShopName = _restaurantName;
            tabkOrderDto.SubShopName = _restaurantSubName;
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            var printListConfig = GetPrintSchemeListByCode(PrintEnumCode.PrintGroupCode.PrtTKOrder);
            if (printListConfig == null)
            {
                var printDefaultConfig = GetPrintSchemeByDefault(PrintEnumCode.PrintGroupCode.PrtTKOrder);
                if (printDefaultConfig == null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                    Message = "打印方案为空";
                }
                else
                {
                    PrintWithDefaultScheme(printDefaultConfig, htmlPrintingServices, voucher.Path, tabkOrderDto);
                }
                return;
            }
            List<PrintConfigDto> printConfigDtoList = GetPrintConfigByVoucher(printListConfig, PrintEnumCode.VoucherCode.TKout);
            foreach (var item in printConfigDtoList)
            {
                tabkOrderDto.PrintTime = DateTime.Now;
                htmlPrintingServices.GlobalPrintScheme(item, voucher.Path, tabkOrderDto, item.PrintNum);
            }

            PrintLogUtility.Writer.SendInfo("PrintTakeOutOrder 打印完成");
            Flag = true;
        }
    }
}
