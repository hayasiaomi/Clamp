using Hydra.Framework.MqttUtility;
using Hydra.Framework.Partial.Mqtt;
using Hydra.Framework.Services.Aop.RegistrationAttributes;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Services.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hydra.Framework.LogUtility;
using ShanDian.AddIns.Print.Dto.PrintTemplate;
using Hydra.Framework.Utility;
using ShanDian.AddIns.Print.Dto.Print.Dto;

namespace ShanDian.AddIns.Print.Services.Module
{
    /// <summary>
    /// 用于打印相关的凭证打印类
    /// </summary>
    [As(typeof(IHtmlPrintingServices))]
    public class HtmlPrintingServices : CommunicationServer, IHtmlPrintingServices
    {
        /// <summary>
        /// 初始化HTML引擎的时间格式
        /// </summary>
        static HtmlPrintingServices()
        {
            HtmlTemplate.RegisterHelper("DateFormat", (writer, context, args) =>
            {
                if (args != null && args.Length > 0)
                {
                    try
                    {
                        DateTime date = Convert.ToDateTime(args[0]);

                        if (args.Length > 1)
                        {
                            string dateformat = Convert.ToString(args[1]);
                            writer.Write(date.ToString(dateformat));
                        }
                        else
                        {
                            writer.Write(date.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    catch (Exception)
                    {
                        writer.Write(args[0]);
                    }

                }
            });
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="printConfigDto"></param>
        /// <param name="voucherDto"></param>
        /// <param name="data"></param>
        public void Print(PrintConfigDto printConfigDto, VoucherDto voucherDto, object data)
        {
            string templatePath = Path.Combine(SystemUtility.ProjectDirectory("PrintTemplate"), voucherDto.Path);

            //this.Print(templatePath, printConfigDto.PrintName, data, 0, 0, 0, 1);
            this.Print(templatePath, printConfigDto.PrintName, data,
                Convert.ToInt32(printConfigDto.LeftMargin),
                Convert.ToInt32(printConfigDto.TopMargin),
                Convert.ToInt32(printConfigDto.PaperWidth),
                printConfigDto.PrintNum, printConfigDto.Pcid);
            //this.Print(voucherDto.Path, printConfigDto.PrintName, data,
            //    Convert.ToInt32(printConfigDto.LeftMargin),
            //    Convert.ToInt32(printConfigDto.TopMargin),
            //    Convert.ToInt32(printConfigDto.PaperWidth),
            //    voucherDto.PrintNum);
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="printingName"></param>
        public void Print(string path, string printingName, object data, string pcid)
        {
            this.Print(path, printingName, data, 0, 0, -1, 1, pcid);
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="printingName"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="copies"></param>
        public void Print(string path, string printingName, object data, int left, int top, int width, int copies, string pcid)
        {
            if (File.Exists(path))
            {
                this.PrintHtml(File.ReadAllText(path, Encoding.UTF8), data, printingName, left, top, width, copies, pcid);
            }
        }

        /// <summary>
        /// 轻餐开钱箱
        /// </summary>
        /// <param name="printConfigDto"></param>
        public void LMOpenCashBox(PrintConfigDto printConfigDto)
        {
            if (printConfigDto != null)
            {
                this.PrintHtml(printConfigDto.PrintName, Convert.ToInt32(printConfigDto.LeftMargin), Convert.ToInt32(printConfigDto.TopMargin), Convert.ToInt32(printConfigDto.PaperWidth), 1, printConfigDto.Pcid);
            }
        }

        /// <summary>
        /// 通知客户端打印
        /// </summary>
        /// <param name="htmlTemplate"></param>
        /// <param name="data"></param>
        /// <param name="printingName"></param>    
        /// <param name="leift"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="copies"></param>
        private void PrintHtml(string htmlTemplate, object data, string printingName, int left, int top, int width, int copies, string pcid)
        {
            try
            {
                PrintLogUtility.Writer.SendInfo("Print data:" + JsonConvert.SerializeObject(data));

                var template = HtmlTemplate.Compile(htmlTemplate);
                string html = template(data);
                FinalPrintInfoDto finalPrintInfoDto = new FinalPrintInfoDto();

                finalPrintInfoDto.Left = left;
                finalPrintInfoDto.Top = top;
                finalPrintInfoDto.Width = width;
                finalPrintInfoDto.PrintingBody = html;
                finalPrintInfoDto.PrintingName = printingName;
                finalPrintInfoDto.Copies = copies;
                finalPrintInfoDto.Pcid = pcid;

                PrintLogUtility.Writer.SendInfo("Print data pcid:" + pcid);
                PrintLogUtility.Writer.SendInfo($"Print data machine: printingName:{printingName} -- left:{left} -- top;{top} -- width:{width} -- copies:{copies} .");
                PrintLogUtility.Writer.SendInfo("Print data html:" + html);

                IFinalPrintingServcies finalPrintingServcies = MLandLoader.Load<IFinalPrintingServcies>(Global.PartName);

                finalPrintingServcies.FinalPrinting(JsonConvert.SerializeObject(finalPrintInfoDto));
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendError(ex);
            }
        }

        #region 旧的打印接口
        /// <summary>
        /// 开钱箱（临时解决方案）
        /// </summary>
        /// <param name="printingName"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="copies"></param>
        /// <param name="pcid"></param>
        private void PrintHtml(string printingName, int left, int top, int width, int copies, string pcid)
        {
            try
            {
                PrintLogUtility.Writer.SendInfo("开钱箱");


                //PrintLogUtility.Writer.SendDebug(string.Format("第三方打印1：成功"));
                //PrintLogUtility.Writer.SendDebug(string.Format("第三方打印2：{0}", html));
                FinalPrintInfoDto finalPrintInfoDto = new FinalPrintInfoDto();

                finalPrintInfoDto.Left = left;
                finalPrintInfoDto.Top = top;
                finalPrintInfoDto.Width = width;
                finalPrintInfoDto.PrintingBody = "<div> </div>";
                finalPrintInfoDto.PrintingName = printingName;
                finalPrintInfoDto.Copies = copies;
                finalPrintInfoDto.Pcid = pcid;

                PrintLogUtility.Writer.SendInfo("Print data pcid:" + pcid);

                PrintLogUtility.Writer.SendInfo("开钱箱");

                IFinalPrintingServcies finalPrintingServcies = MLandLoader.Load<IFinalPrintingServcies>(Global.PartName);

                finalPrintingServcies.FinalPrinting(JsonConvert.SerializeObject(finalPrintInfoDto));
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendError(ex);
            }
        }

        /// <summary>
        /// 打印支付凭证
        /// </summary>
        /// <param name="sfpzDto"></param>
        public void PrintSfpz(PrintConfigDto printConfig, VoucherDto voucherDto, SfpzDto sfpzDto)
        {
            sfpzDto.PrintingDate = DateTime.Now;

            this.Print(printConfig, voucherDto, sfpzDto);
        }

        /// <summary>
        ///  打印 扫码点菜单的菜品
        /// </summary>
        /// <param name="smdcdDto"></param>
        public void PrintSmdcd(PrintConfigDto printConfig, VoucherDto voucherDto, SmdcdDto smdcdDto)
        {
            smdcdDto.PrintingDate = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(smdcdDto.Footer))
            {
                smdcdDto.Footer = smdcdDto.Footer.Replace("\\r\\n", "<br />").Replace("\r\n", "<br />").Replace("\\n", "<br />").Replace("\n", "<br />").Replace(Environment.NewLine, "<br />");
            }

            this.Print(printConfig, voucherDto, smdcdDto);
        }

        /// <summary>
        /// 打印退款凭证
        /// </summary>
        /// <param name="tkpzDto"></param>
        public void PrintTkpz(PrintConfigDto printConfig, VoucherDto voucherDto, TkpzDto tkpzDto)
        {
            tkpzDto.PrintingDate = DateTime.Now;

            this.Print(printConfig, voucherDto, tkpzDto);
        }

        /// <summary>
        /// 打印口碑预点单
        /// </summary>
        /// <param name="preOrderDto"></param>
        public void PrintYddd(PrintConfigDto printConfig, VoucherDto voucherDto, PreOrderDto preOrderDto)
        {
            preOrderDto.PrintingDate = DateTime.Now;

            this.Print(printConfig, voucherDto, preOrderDto);
        }

        /// <summary>
        /// 打印收银汇总单
        /// </summary>
        /// <param name="syhzdDto"></param>
        public void PrintSyhzd(PrintConfigDto printConfig, VoucherDto voucherDto, SyhzdDto syhzdDto)
        {
            syhzdDto.PrintingDate = DateTime.Now;

            this.Print(printConfig, voucherDto, syhzdDto);
        }

        /// <summary>
        /// 打印堂食汇总单
        /// </summary>
        /// <param name="tshzdDto"></param>
        public void PrintTshzd(PrintConfigDto printConfig, VoucherDto voucherDto, TshzdDto tshzdDto)
        {
            tshzdDto.PrintingDate = DateTime.Now;

            this.Print(printConfig, voucherDto, tshzdDto);
        }

        /// <summary>
        /// 打印机测试页
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        public void PrintDycs(PrintConfigDto printConfig, VoucherDto voucherDto, DycsDto dycsDto)
        {
            this.Print(printConfig, voucherDto, dycsDto);
        }

        public void PrintPayFail(PrintConfigDto printConfig, VoucherDto voucherDto, PrintFailDto printFailDto)
        {
            printFailDto.PrintDate = DateTime.Now;

            this.Print(printConfig, voucherDto, printFailDto);
        }

        /// <summary>
        /// 清仓版营业汇总单
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="printSummaryDto"></param>
        public void PrintSummary(PrintConfigDto printConfig, VoucherDto voucherDto, PrintSummaryDto printSummaryDto)
        {
            printSummaryDto.PrintTime = DateTime.Now;

            string templatePath = Path.Combine(SystemUtility.ProjectDirectory("PrintTemplate"), voucherDto.Path);

            //默认打印一张
            this.Print(templatePath, printConfig.PrintName, printSummaryDto,
                Convert.ToInt32(printConfig.LeftMargin),
                Convert.ToInt32(printConfig.TopMargin),
                Convert.ToInt32(printConfig.PaperWidth),
               1, printConfig.Pcid);
        }
        #endregion

        public void LocalPrintScheme(PrintConfigDto printConfig, string path, object param, int num = 1)
        {
            string templatePath = Path.Combine(SystemUtility.ProjectDirectory("PrintTemplate"), path);

            //默认打印一张
            this.Print(templatePath, printConfig.PrintName, param,
                Convert.ToInt32(printConfig.LeftMargin),
                Convert.ToInt32(printConfig.TopMargin),
                Convert.ToInt32(printConfig.PaperWidth),
                num, printConfig.Pcid);
        }

        public void GlobalPrintScheme(PrintConfigDto printConfig, string path, object param, int num)
        {
            string templatePath = Path.Combine(SystemUtility.ProjectDirectory("PrintTemplate"), path);

            //默认打印一张
            this.Print(templatePath, printConfig.PrintName, param,
                Convert.ToInt32(printConfig.LeftMargin),
                Convert.ToInt32(printConfig.TopMargin),
                Convert.ToInt32(printConfig.PaperWidth),
                num, printConfig.Pcid);
        }

        /// <summary>
        /// 轻餐打印菜品统计
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="dishStatisticsDto"></param>
        public void PrintDishStatistics(PrintConfigDto printConfig, VoucherDto voucherDto, DishStatisticsDto dishStatisticsDto)
        {
            dishStatisticsDto.PrintTime = DateTime.Now;

            string templatePath = Path.Combine(SystemUtility.ProjectDirectory("PrintTemplate"), voucherDto.Path);

            //默认打印一张
            this.Print(templatePath, printConfig.PrintName, dishStatisticsDto,
                Convert.ToInt32(printConfig.LeftMargin),
                Convert.ToInt32(printConfig.TopMargin),
                Convert.ToInt32(printConfig.PaperWidth),
                1, printConfig.Pcid);
        }

        /// <summary>
        /// 通过本机打印的通用方法
        /// </summary>
        /// <param name="printConfig"></param>
        /// <param name="voucherDto"></param>
        /// <param name="data"></param>
        public void PrintLocalMachine(PrintConfigDto printConfig, VoucherDto voucherDto, object data)
        {
            string templatePath = Path.Combine(SystemUtility.ProjectDirectory("PrintTemplate"), voucherDto.Path);

            //默认打印一张
            this.Print(templatePath, printConfig.PrintName, data,
                Convert.ToInt32(printConfig.LeftMargin),
                Convert.ToInt32(printConfig.TopMargin),
                Convert.ToInt32(printConfig.PaperWidth),
                1, printConfig.Pcid);
        }

        public void PrintTakeOrder(PrintConfigDto printConfig, VoucherDto voucherDto, TakeOutOrderDto takeOutOrderDto)
        {
            takeOutOrderDto.PrintTime = DateTime.Now;

            this.Print(printConfig, voucherDto, takeOutOrderDto);
        }
    }
}
