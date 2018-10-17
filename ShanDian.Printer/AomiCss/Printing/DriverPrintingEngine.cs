using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace Hydra.AomiCss.Printing
{
    public class DriverPrintingEngine : PrintingEngine
    {

        public DriverPrintingEngine()
        {

        }

        public DriverPrintingEngine(string name) : base(name)
        {

        }

        private Size MeasureHtmlByRestrictions(HtmlContainer htmlContainer, Graphics graphics, Size minSize, Size maxSize)
        {
            htmlContainer.PerformLayout(graphics);

            if (maxSize.Width > 0 && maxSize.Width < htmlContainer.ActualSize.Width)
            {
                htmlContainer.MaxSize = new SizeF(maxSize.Width, 0);
                htmlContainer.PerformLayout(graphics);
            }

            var finalWidth = Math.Max(maxSize.Width > 0 ? Math.Min(maxSize.Width, (int)htmlContainer.ActualSize.Width) : (int)htmlContainer.ActualSize.Width, minSize.Width);

            if (finalWidth > htmlContainer.ActualSize.Width)
            {
                htmlContainer.MaxSize = new SizeF(finalWidth, 0);
                htmlContainer.PerformLayout(graphics);
            }

            var finalHeight = Math.Max(maxSize.Height > 0 ? Math.Min(maxSize.Height, (int)htmlContainer.ActualSize.Height) : (int)htmlContainer.ActualSize.Height, minSize.Height);

            return new Size(finalWidth, finalHeight);
        }

        public override void Engine(string html)
        {
            using (HtmlContainer htmlContainer = new HtmlContainer())
            {
                htmlContainer.AvoidAsyncImagesLoading = true;

                htmlContainer.ScrollOffset = new PointF(this.LeftMargin, this.TopMargin);

                using (PrintDocument printDocument = new PrintDocument())
                {
                    printDocument.PrinterSettings.Copies = Convert.ToInt16(this.Copies);
                    printDocument.PrinterSettings.PrinterName = this.Name;
                    printDocument.DefaultPageSettings.Margins = new Margins(this.LeftMargin, this.TopMargin, 0, 0);

                    printDocument.QueryPageSettings += (sender, e) =>
                    {
                        this.LimitPaperWidth = e.PageSettings.PaperSize.Width;
                        e.PageSettings.PaperSize = new PaperSize(e.PageSettings.PaperSize.PaperName, e.PageSettings.PaperSize.Width, 9999);
                    };

                    printDocument.PrintPage += (sender, e) =>
                    {
                        Graphics graphics = e.Graphics;

                        htmlContainer.SetHtml(html);

                        htmlContainer.MaxSize = this.MeasureHtmlByRestrictions(htmlContainer, graphics, Size.Empty, new Size(this.PaperWidth, 8888));

                        htmlContainer.PerformPaint(graphics);
                    };

                    printDocument.Print();
                }
            }
        }
    }
}
