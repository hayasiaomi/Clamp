using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamp.Printer.Dto;

namespace Clamp.Printer
{
    public interface IPrinterService
    {
        void Print(FinalPrintInfoDto input);
    }
}
