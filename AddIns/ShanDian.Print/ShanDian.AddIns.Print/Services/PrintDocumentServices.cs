using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.LogUtility;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.Partial.Mqtt;
using Hydra.Framework.Services.Aop;
using Hydra.Framework.Services.Aop.RegistrationAttributes;
using Hydra.Framework.Services.Utility;
using Hydra.Framework.SqlContent;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.PrintDataDto;
using ShanDian.AddIns.Print.Dto.PrintTemplate;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Model;
using ShanDian.AddIns.Print.Data;
using Newtonsoft.Json;
using System.IO;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Module
{
    public class PrintDocumentServices
    {
        private IRepositoryContext _repositoryContext;
        private string _restaurantName = "";
        private string _restaurantSubName = "";
        private string _currency = "¥";

        public PrintDocumentServices()
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
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintScanCodeBill data:" + JsonConvert.SerializeObject(billData));
            var templateCode = "PRT_SO_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            billData.VoucherId = voucher.Id;
            var printListConfig = GetPrintListConfig(billData, voucher);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
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
            smdcdDto.PriceDetails = GetSmdcdPriceDetail(billData.PriceDetails);
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
            PrintEnumCode.TableTypeCode TableCode = new PrintEnumCode.TableTypeCode();
            TableCode = (PrintEnumCode.TableTypeCode)(billData.TableTypeCode);
            switch (TableCode)
            {
                case PrintEnumCode.TableTypeCode.ZPH:
                    smdcdDto.TableType = "桌牌号";
                    break;
                case PrintEnumCode.TableTypeCode.QCH:
                    smdcdDto.TableType = "取餐号";
                    break;
                default:
                    smdcdDto.TableType = "桌牌号";
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
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintSmdcd(item, voucher, smdcdDto);
            }
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
            PrintLogUtility.Writer.SendInfo("saleSummary data:" + JsonConvert.SerializeObject(saleSummary));
            string templateCode = "PRT_FI_0003";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            saleSummary.VoucherId = voucher.Id;
            var printListConfig = GetPrintListConfig(saleSummary, voucher);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }

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

            PrintLogUtility.Writer.SendInfo("ServicesPrint syhzd data:" + JsonConvert.SerializeObject(syhzdDto));

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintSyhzd(item, voucher, syhzdDto);
            }
            Flag = true;
        }

        /// <summary>
        /// 打印堂食汇总单
        /// </summary>
        public void PrintDineSummary(DineSummary dineSummary)
        {
            if (dineSummary == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("dineSummary data:" + JsonConvert.SerializeObject(dineSummary));
            string templateCode = "PRT_FI_0004";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            dineSummary.VoucherId = voucher.Id;
            var printListConfig = GetPrintListConfig(dineSummary, voucher);
            if (printListConfig == null || printListConfig.Count < 1)
            {
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

            PrintLogUtility.Writer.SendInfo("ServicesPrint tshzd data:" + JsonConvert.SerializeObject(tshzdDto));

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintTshzd(item, voucher, tshzdDto);
            }
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
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            string templateCode = "PRT_FI_0002";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            refundVoucher.VoucherId = voucher.Id;
            var printListConfig = GetPrintListConfig(refundVoucher, voucher);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }

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
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintTkpz(item, voucher, tkpzDto);
            }
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
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintPaymentVoucher 接收 data:" + JsonConvert.SerializeObject(paymentVoucher));
            string templateCode = "PRT_FI_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            paymentVoucher.VoucherId = voucher.Id;
            var printListConfig = GetPrintListConfig(paymentVoucher, voucher);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }

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
                Dishes = null,
                IsPrintDish = paymentVoucher.IsPrintDish,
                IsPrintDishAamount = paymentVoucher.IsPrintDishAamount
            };

            if (paymentVoucher.BillDish != null && paymentVoucher.BillDish.Any() && paymentVoucher.IsPrintDish)
            {
                sfpzDto.Dishes = GetSmdcdDishDtos(paymentVoucher.BillDish);
            }

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintSfpz(item, voucher, sfpzDto);
            }
            PrintLogUtility.Writer.SendInfo("PrintPaymentVoucher 打印完成");
            Flag = true;
        }

        /// <summary>
        /// 打印口碑预点单
        /// </summary>
        /// <param name="preOrder"></param>
        public void PrintKbPreOrder(KbPreOrder preOrder)
        {
            PrintLogUtility.Writer.SendInfo("PrintPreOrder 接收 data:" + JsonConvert.SerializeObject(preOrder));
            if (preOrder == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            string templateCode = "PRT_SO_1001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            preOrder.VoucherId = voucher.Id;
            var printListConfig = GetPrintListConfig(voucher.Id);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
            var preOrderDto = new PreOrderDto()
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                OrderTime = preOrder.OrderTime,
                TakeMealNum = preOrder.TakeMealNum,
                Scene = preOrder.Scene,
                CustomerName = preOrder.CustomerName,
                CustomerPhone = preOrder.CustomerPhone,
                EatingTime = preOrder.EatingTime,
                CustomerCount = preOrder.CustomerCount,
                Remark = preOrder.Remark,
                TotalAmount = preOrder.TotalAmount,
                ShouldAmount = preOrder.ShouldAmount
            };
            if (preOrder.Dishes != null)
            {
                foreach (var dish in preOrder.Dishes)
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
            if (preOrder.DiscountAmounts != null)
            {
                foreach (var discount in preOrder.DiscountAmounts)
                {
                    var preOrderDiscountAmountDto = new PreOrderDiscountAmountDto()
                    {
                        DiscountName = discount.DiscountName,
                        Price = discount.Price
                    };

                    preOrderDto.DiscountAmounts.Add(preOrderDiscountAmountDto);
                }
            }
            if (preOrder.AdditionalCharges != null)
            {
                foreach (var payDetail in preOrder.AdditionalCharges)
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
            foreach (var item in printListConfig)
            {
                PrintLogUtility.Writer.SendInfo("PrintPreOrder 打印 data:" + preOrder.TakeMealNum);
                htmlPrintingServices.PrintYddd(item, voucher, preOrderDto);
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
            var printConfig = GetPrintListConfigLocalMachine(voucher.Id);
            if (printConfig == null)
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

            htmlPrintingServices.PrintSmdcd(printConfig, voucher, mockScanCodeBill);
            Flag = true;
        }

        /// <summary>
        /// 测试打印单据(门店打印方案)
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="pcId"></param>
        public void PrintTestByTemplateCode(string templateCode, string pcId)
        {
            if (string.IsNullOrEmpty(templateCode))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            if (string.IsNullOrEmpty(pcId))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var printTestData = new PrintTestData { PcId = pcId };
            switch (templateCode)
            {
                case "PRT_SO_0001":
                    TestPrintScanCodeBill();
                    return;
                case "PRT_FI_0001":
                    TestPrintPaymentVoucher();
                    return;
                case "PRT_FI_0002":
                    TestPrintRefundVoucher();
                    return;
                case "PRT_FI_0003":
                    TestPrintSaleSummary();
                    return;
                case "PRT_SO_1001":
                    TestPrintKbPreOrder();
                    return;
                default:
                    Flag = false;
                    Message = "没有目标打印模板信息";
                    this.Code = PrintErrorCode.Code.VoucherNullError;
                    return;
            }
        }

        private void TestPrintScanCodeBill()
        {
            var templateCode = "PRT_SO_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printListConfig = GetPrintListConfig(voucher.Id);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
            var mockScanCodeBill = GetMockScanCodeBill();

            mockScanCodeBill.Footer = "本单为打印测试";
            mockScanCodeBill.FooterAlignment = "center";

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintSmdcd(item, voucher, mockScanCodeBill);
            }
            Flag = true;
        }

        private void TestPrintSaleSummary()
        {
            string templateCode = "PRT_FI_0003";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printListConfig = GetPrintListConfig(voucher.Id);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
            var mockSaleSummary = GetMockSaleSummary();

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintSyhzd(item, voucher, mockSaleSummary);
            }
            Flag = true;
        }

        private void TestPrintRefundVoucher()
        {
            string templateCode = "PRT_FI_0002";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printListConfig = GetPrintListConfig(voucher.Id);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
            var mockRefundVoucher = GetMockRefundVoucher();

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintTkpz(item, voucher, mockRefundVoucher);
            }
            Flag = true;
        }

        private void TestPrintPaymentVoucher()
        {
            string templateCode = "PRT_FI_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printListConfig = GetPrintListConfig(voucher.Id);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
            var mockPaymentVoucher = GetMockPaymentVoucher();

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintSfpz(item, voucher, mockPaymentVoucher);
            }
            Flag = true;
        }

        private void TestPrintKbPreOrder()
        {
            var templateCode = "PRT_SO_1001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            var printListConfig = GetPrintListConfig(voucher.Id);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
            var preOrderDto = GetMockKbPreOrder();
            var json = preOrderDto.ToJson();
            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintYddd(item, voucher, preOrderDto);
            }
            Flag = true;
        }

        private SmdcdDto GetMockScanCodeBill()
        {
            var smdcdDto = new SmdcdDto()
            {
                TableType = "桌牌号",
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
                ExceptionReason = string.Empty
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

        private SyhzdDto GetMockSaleSummary()
        {
            var syhzdDto = new SyhzdDto
            {
                ShopName = _restaurantName,
                SubShopName = _restaurantSubName,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                PayCount = 1,
                PayTotalAmount = 0.01m,
                PayDiscountAmount = 0,
                DiscountAmounts = new List<SyhzDiscountAmountDto>(),
                RefundTotalAmount = 0,
                ActualTotalAmount = 0.01m,
                IsPayDetail = true,
                PayDetails = new List<SyhzdPayDetailDto>(),
                RefundPayTypeAmounts = new List<SyhzdPayTypeAmountDto>(),
                ActualPayTypeAmounts = new List<SyhzdPayTypeAmountDto>()
            };
            syhzdDto.DiscountAmounts.Add(new SyhzDiscountAmountDto() { DiscountName = "扫码折扣(支付宝)", PayTypeName = "", Price = 0 });
            syhzdDto.PayDetails.Add(new SyhzdPayDetailDto() { ActualAmount = 0.01m, PayDiscount = 0, PaymentAmount = 0, PaymentDate = DateTime.Now, PaymentTypeName = "支付宝", PaymentVoucherId = "打印测试" });
            syhzdDto.RefundPayTypeAmounts.Add(new SyhzdPayTypeAmountDto { PayTypeName = "", Price = 0 });
            syhzdDto.ActualPayTypeAmounts.Add(new SyhzdPayTypeAmountDto() { PayTypeName = "支付宝", Price = 0.01m });
            return syhzdDto;
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
                Operator = "打印测试"
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
                BillRunningId = "打印测试"
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
                ShouldAmount = 140
            };
            preOrderDto.Dishes = new List<PreOrderDishDto>() { new PreOrderDishDto() { DishName = "菲力牛排", TakeCount = 1, DishDescription = String.Empty, UnitPrice = 58, WeightNames = new List<string>(), PracticeNames = new List<string>() { "8成熟", "番茄酱" } }, new PreOrderDishDto() { DishName = "牛肉炒饭", TakeCount = 1, DishDescription = String.Empty, UnitPrice = 12, WeightNames = new List<string>(), PracticeNames = new List<string>() }, new PreOrderDishDto() { DishName = "双人套餐", TakeCount = 1, DishDescription = String.Empty, UnitPrice = 88, WeightNames = new List<string>(), PracticeNames = new List<string>(), SubDishes = new List<PreOrderDishDto>() { new PreOrderDishDto() { DishName = "[套]排骨汤", TakeCount = 1 }, new PreOrderDishDto() { DishName = "[套]炒米粉", TakeCount = 2 }, new PreOrderDishDto() { DishName = "[套]上海青", TakeCount = 1 }, new PreOrderDishDto() { DishName = "[套]拌牛肚", TakeCount = 1 } } } };
            preOrderDto.DiscountAmounts = new List<PreOrderDiscountAmountDto>() { new PreOrderDiscountAmountDto() { DiscountName = "优惠券", Price = 20 } };
            preOrderDto.AdditionalCharges = new List<PreOrderAdditionalCostDto>() { new PreOrderAdditionalCostDto() { Name = "餐盒费", Price = 1 }, new PreOrderAdditionalCostDto() { Name = "配送费", Price = 1 } };
            return preOrderDto;
        }

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

        private Voucher GetVoucherById(int voucherId)
        {
            var voucher = _repositoryContext.QueryFirstOrDefault<Voucher>("select Id,VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Sort from tb_voucher Where Id=@Id", new { Id = voucherId });
            return voucher;
        }

        private VoucherDto GetVoucherByCode(string templateCode)
        {
            var voucher = _repositoryContext.QueryFirstOrDefault<Voucher>("select Id,VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Sort,Path from tb_voucher Where TemplateCode=@TemplateCode", new { TemplateCode = templateCode });
            var voucherDto = GetVoucherDto(voucher);
            return voucherDto;
        }

        /// <summary>
        /// 获取本地设置的打印方案
        /// </summary>
        /// <param name="printData">PcId存在就走本地打印方案，不存在就走门店方案</param>
        /// <param name="voucher"></param>
        /// <returns></returns>
        private List<PrintConfigDto> GetPrintListConfig(PrintData printData, VoucherDto voucher)
        {
            if (printData == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                Message = "参数不可为空";
                return new List<PrintConfigDto>();
            }
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ResultNull;
                Message = "模板不可为空";
                return new List<PrintConfigDto>();
            }
            if (!string.IsNullOrEmpty(printData.PcId))
            {
                printData.PcId = printData.PcId.Trim();
                var printSchemes = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where VoucherId=@VoucherId And PcId=@PcId And PrintNum>0 And LocalMachine=0", new { VoucherId = printData.VoucherId, PcId = printData.PcId });
                if (printSchemes == null || printSchemes.Count < 1)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                    Message = "未找到相关打印方案";
                    return new List<PrintConfigDto>();
                }
                List<string> ids = new List<string>();
                foreach (var printScheme in printSchemes)
                {
                    ids.Add(printScheme.PrintId);
                }
                var configs = _repositoryContext.GetSet<PrintConfig>("select printId,pcid,printName,alias,updated,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId IN @ids", new { ids = ids });

                Flag = true;
                var printConfigDtos = GetPrintConfigDtos(configs);
                foreach (var printScheme in printSchemes)
                {
                    var config = printConfigDtos.FirstOrDefault(x => x.PrintId == printScheme.PrintId && x.PrintNum == 0);
                    if (config != null)
                    {
                        config.PrintNum += printScheme.PrintNum;
                    }
                }
                var configDtoList = new List<PrintConfigDto>();
                foreach (var item in printConfigDtos)
                {
                    if (item.PrintNum > 0)
                    {
                        configDtoList.Add(item);
                    }
                }

                return configDtoList;
            }
            if (printData.TagList == null || printData.TagList.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return new List<PrintConfigDto>();
            }
            var printSchemelList = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme WHERE VoucherId=@VoucherId And LocalMachine!=0 And PrintNum>0", new { VoucherId = voucher.Id });
            if (printSchemelList == null || printSchemelList.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "未找到相关打印方案";
                return new List<PrintConfigDto>();
            }
            var list = new List<string>();
            foreach (var printScheme in printSchemelList)
            {
                list.Add(printScheme.SchemeCode);
            }

            var printSchemeLabels = _repositoryContext.GetSet<PrintSchemeLabel>("select Id,PrintSchemeId,LabelId,LabelGroupCode,SchemeCode from tb_printSchemeLabel t WHERE t.LabelId IN @lables And t.SchemeCode IN @codes", new { lables = printData.TagList, codes = list });
            List<string> idList = new List<string>();
            foreach (var label in printSchemeLabels)
            {
                idList.Add(label.SchemeCode);
            }
            List<string> configIds = new List<string>();
            foreach (var printScheme in printSchemelList)
            {
                var exists = idList.Exists(x => x == printScheme.SchemeCode);
                if (exists)
                {
                    configIds.Add(printScheme.PrintId);
                }
            }
            var configList = _repositoryContext.GetSet<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId IN @ids", new { ids = configIds });
            Flag = true;
            var dtos = GetPrintConfigDtos(configList);
            foreach (var printScheme in printSchemelList)
            {
                var config = dtos.FirstOrDefault(x => x.PrintId == printScheme.PrintId);
                if (config != null)
                {
                    config.PrintNum += printScheme.PrintNum;
                }
            }
            var configDtos = new List<PrintConfigDto>();
            foreach (var item in dtos)
            {
                if (item.PrintNum > 0)
                {
                    configDtos.Add(item);
                }
            }

            return configDtos;
        }

        /// <summary>
        /// 本机
        /// </summary>
        /// <param name="voucherId"></param>
        /// <returns></returns>
        private PrintConfigDto GetPrintListConfigLocalMachine(int voucherId)
        {
            var printScheme = _repositoryContext.QueryFirstOrDefault<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme  WHERE VoucherId =@VoucherId And LocalMachine=0", new { VoucherId = voucherId });
            if (printScheme == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "未找到相关打印方案";
                return null;
            }
            var config = _repositoryContext.QueryFirstOrDefault<PrintConfig>("select printId,pcid,printName,alias,updated,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig  WHERE printId=@id", new { id = printScheme.PrintId });
            if (config == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintConfigNullError;
                Message = "未找到相关打印机";
                return null;
            }

            Flag = true;
            var dto = GetPrintConfigDto(config);
            if (dto != null)
            {
                dto.PrintNum += printScheme.PrintNum;
            }
            return dto;
        }

        /// <summary>
        /// 门店
        /// </summary>
        /// <param name="voucherId"></param>
        /// <returns></returns>
        private List<PrintConfigDto> GetPrintListConfig(int voucherId)
        {
            var printSchemes = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme  WHERE VoucherId =@VoucherId And LocalMachine!=0", new { VoucherId = voucherId });
            if (printSchemes == null || printSchemes.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "未找到相关打印方案";
                return new List<PrintConfigDto>();
            }
            List<string> printIds = new List<string>();
            foreach (var printScheme in printSchemes)
            {
                printIds.Add(printScheme.PrintId);
            }
            var configs = _repositoryContext.GetSet<PrintConfig>("select printId,pcid,printName,alias,updated,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime, isDefault from tb_printConfig t WHERE t.printId IN @ids", new { ids = printIds });
            if (configs == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintConfigNullError;
                Message = "未找到相关打印机";
                return new List<PrintConfigDto>();
            }

            Flag = true;
            var dtos = GetPrintConfigDtos(configs);
            foreach (var printScheme in printSchemes)
            {
                var config = dtos.FirstOrDefault(x => x.PrintId == printScheme.PrintId);
                if (config != null)
                {
                    config.PrintNum += printScheme.PrintNum;
                }
            }
            var printConfigDtos = new List<PrintConfigDto>();
            foreach (var item in dtos)
            {
                if (item.PrintNum > 0)
                {
                    printConfigDtos.Add(item);
                }
            }
            return printConfigDtos;
        }

        private List<PrintConfigDto> GetPrintConfigDtos(List<PrintConfig> printConfigs)
        {
            var printConfigDtos = new List<PrintConfigDto>();
            if (printConfigs == null)
            {
                return printConfigDtos;
            }
            foreach (var item in printConfigs)
            {
                var printConfigDto = GetPrintConfigDto(item);
                printConfigDtos.Add(printConfigDto);
            }
            return printConfigDtos;
        }

        private PrintConfigDto GetPrintConfigDto(PrintConfig printConfig)
        {
            if (printConfig == null)
            {
                return new PrintConfigDto();
            }
            PrintConfigDto printConfigDto = new PrintConfigDto
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
                Enable = printConfig.Enable
            };
            return printConfigDto;
        }

        private VoucherDto GetVoucherDto(Voucher voucher)
        {
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

        private List<SmdcdDishDto> GetSmdcdDishDtos(List<OrderDishDto> billDishs)
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
                smdcdDishDtos.Add(smdcdDishDto);
            }
            return smdcdDishDtos;
        }

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
                    PracticeNames = new List<string>()
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

        private List<SmdcdPriceDetail> GetSmdcdPriceDetail(List<PriceDetail> prinDetail)
        {
            List<SmdcdPriceDetail> result = new List<SmdcdPriceDetail>();
            if (prinDetail != null && prinDetail.Count > 0)
            {
                foreach (var prtItem in prinDetail)
                {
                    SmdcdPriceDetail newResult = new SmdcdPriceDetail();
                    newResult.Price = prtItem.Price;
                    newResult.PriceTypeName = prtItem.PriceTypeName;
                    result.Add(newResult);
                }
            }
            return result;
        }

        public void PrintPayFail(PrintFailVoucher printFailVoucher)
        {
            if (printFailVoucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            PrintLogUtility.Writer.SendInfo("PrintPayFail 接收 data:" + JsonConvert.SerializeObject(printFailVoucher));
            string templateCode = "PRT_FI_0001";
            var voucher = GetVoucherByCode(templateCode);
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return;
            }
            printFailVoucher.VoucherId = voucher.Id;
            var printListConfig = GetPrintListConfig(printFailVoucher, voucher);
            if (printListConfig == null || printListConfig.Count < 1)
            {
                return;
            }
            string templateCodePath = "PRT_SO_0002";
            var voucherPath = GetVoucherByCode(templateCodePath);
            voucher.Path = voucherPath.Path;

            PrintFailDto printFailDto = new PrintFailDto
            {
                TableName = printFailVoucher.TableName,
                BillNo = printFailVoucher.BillNo,
                PayPattern = printFailVoucher.PayPattern,
                PayMoney = printFailVoucher.PayMoney,
                OrderPayNo = printFailVoucher.OrderPayNo
            };

            var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintPayFail(item, voucher, printFailDto);
            }
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
            var printListConfig = GetPrintConfig(summaryVoucher.PcId);
            if (printListConfig == null || printListConfig.Count < 1)
            {
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
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintSummary(item, voucher, printSummary);
            }
            Flag = true;
        }

        /// <summary>
        /// 获取本机打印方案
        /// </summary>
        public List<PrintConfigDto> GetPrintConfig(string pcid)
        {
            var localConfig = _repositoryContext.FirstOrDefault<LocalPrint>("select Id,PrintId,Machine from tb_localPrint t WHERE t.Machine = @Machine", new { Machine = pcid });

            if (localConfig == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "未找到相关打印方案(本机打印方案)";
                return new List<PrintConfigDto>();
            }
            var configItem = _repositoryContext.GetSet<PrintConfig>("select printId,pcid,printName,alias,paperType,terminalName,connStyle,paperWidth,topMargin,leftMargin,modifyTime from tb_printConfig t WHERE t.printid = @printid", new { printid = localConfig.PrintId });
            if (configItem == null || configItem.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "未找到相关打印方案(本机打印方案对应的打印方案)";
                return new List<PrintConfigDto>();
            }

            var printConfigList = new List<PrintConfigDto>();

            foreach (var item in configItem)
            {
                var printConfigDto = GetPrintConfigDto(item);
                printConfigList.Add(printConfigDto);
            }

            return printConfigList;
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
            var printListConfig = GetPrintConfig(dishStatistics.PcId);
            if (printListConfig == null || printListConfig.Count < 1)
            {
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
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintDishStatistics(item, voucher, dishStatisticsDto);
            }
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
            var printListConfig = GetPrintConfig(restaurantShifts.PcId);
            if (printListConfig == null || printListConfig.Count < 1)
            {
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
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintLocalMachine(item, voucher, restaurantShiftsDto);
            }
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
            var printListConfig = GetPrintListConfig(1);
            if (printListConfig == null || printListConfig.Count < 1)
            {
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
            foreach (var item in printListConfig)
            {
                htmlPrintingServices.PrintTakeOrder(item, voucher, tabkOrderDto);
            }
            Flag = true;
        }
    }
}
