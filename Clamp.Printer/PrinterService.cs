using System;
using System.Threading;
using Clamp.Printer.Config;
using Clamp.Printer.Dto;
using Clamp.Printer.Printer;
using Clamp.SDK.Framework.Services;
using System.Threading.Tasks;

namespace Clamp.Printer
{
    public class PrinterService : IPrinterService
    {
        public void Print(FinalPrintInfoDto input)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (input.PrintType == PrintType.Unkown)
                    {
                        for (var i = 0; i < input.Copies; i++)
                        {
                            Hydra.AomiCss.AoGenius.Printing(input.PrintingBody,
                                input.PrintingName, input.Width, input.Left, input.Top);
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        EscPrinter.Print(input);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error("打印异常", ex);
                }
            });

            
            
            
            
        }
    }
}