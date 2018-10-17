using Hydra.AomiCss.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydra.AomiCss
{
    public sealed class AoGenius
    {
        static AoGenius()
        {
            DebugHelper.Init(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs"));
        }
            
        public static void Printing(string html, string name, int width = 190, int leftMargin = 0, int topMargin = 0, int copies = 1)
        {
            PrintingEngine printingEngine = new DriverPrintingEngine(name);

            printingEngine.PaperWidth = width;
            printingEngine.LeftMargin = leftMargin;
            printingEngine.TopMargin = topMargin;
            printingEngine.Copies = copies;

            printingEngine.Engine(html);

        }
    }
}
