using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.Printer.Dto;

namespace ShanDian.Printer
{
    public interface IPrinterService
    {
        void Print(FinalPrintInfoDto input);
    }
}
