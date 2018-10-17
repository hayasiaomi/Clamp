using System;
using System.Collections.Generic;
using System.Linq;
using ShanDian.Common.Extensions;
using ShanDian.Print;
using ShanDian.Printer.Config;
using ShanDian.Printer.Converter;
using ShanDian.Printer.Dto;
using ShanDian.SDK.Framework.Services;

namespace ShanDian.Printer.Printer
{
    public class EscPrinter
    {
        private static Dictionary<PrintType, EscPrinter> _printerDict 
            = new  Dictionary<PrintType, EscPrinter>
            {
                {PrintType.Net,new NetPrinter() },
                {PrintType.LPT,new LPTPrinter() },
                {PrintType.COM,new ComPrinter() },
            };

        public virtual void Print(string ip, string port, byte[] data)
        {
            
        }

        public static void Print(FinalPrintInfoDto input)
        {
            //todo 打印时异常处理
            LoggingService.Debug($"调用指令打印接口，input：{input?.ToJson()}");
            var pConfig = PrinterConfigManager.GetConfig(input.PrintBrand, input.PrintType);
            var content = input.PrintingBody;
            LoggingService.Debug($"获取打印机指令配置信息，pConfig：{pConfig?.ToJson()}");
            string[] separator = { "\r\n" };
            var clist = content.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

            var sendData = EscConvert.ToEscByte(clist, pConfig, input.RowSize, input.Left, input.Width);
            LoggingService.Debug($"转化指令字节，sendData：{string.Join(",", sendData)}");

            byte[] arrData = sendData.ToArray();
            var printer = _printerDict[input.PrintType];

            for (var i = 0; i < input.Copies; i++)
            {
                LoggingService.Debug($"打印第{i + 1}次");
                printer.Print(input.PrintingName,input.PrintPort, arrData);
            }

           
        }
    }
}