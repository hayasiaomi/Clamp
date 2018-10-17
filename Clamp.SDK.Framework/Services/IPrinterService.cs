using Clamp.SDK.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Services
{
    public interface IPrinterService
    {
        List<PrinterInfo> GetPrinters();

        PrinterInfo GetPrinter(string pcid, string printerName);

        void AddPrinter(string pcid, string printerName, string address, int port, int state);

        void AddPrinter(PrinterInfo printerInfo);
    }
}
