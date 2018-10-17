using CefSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Brower
{
    public class DragHandler : IDragHandler, IDisposable
    {
        public event Action<Region> RegionsChanged;

        bool IDragHandler.OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            return false;
        }

        void IDragHandler.OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions)
        {
            if (browser.IsPopup == false)
            {
                Region draggableRegion = null;

                if (regions != null && regions.Count > 0)
                {
                    foreach (var region in regions)
                    {
                        var rect = new Rectangle(region.X, region.Y, region.Width, region.Height);

                        if (draggableRegion == null)
                        {
                            draggableRegion = new Region(rect);
                        }
                        else
                        {
                            if (region.Draggable)
                            {
                                draggableRegion.Union(rect);
                            }
                            else
                            {
                                draggableRegion.Exclude(rect);
                            }
                        }
                    }
                }

                var handler = RegionsChanged;

                if (handler != null)
                {
                    handler(draggableRegion);
                }
            }
        }

        public void Dispose()
        {
            RegionsChanged = null;
        }
    }
}
