using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Brower
{
    internal class MenuHandler : IContextMenuHandler
    {
        private const int ShowDevTools = 26501;
        private const int CloseDevTools = 26502;

        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            if (SDShell.SDShellSettings.ShowDevTools)
            {
                model.AddItem((CefMenuCommand)ShowDevTools, "Show DevTools");
                model.AddItem((CefMenuCommand)CloseDevTools, "Close DevTools");
            }
            else
            {
                model.Clear();
            }
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if (SDShell.SDShellSettings.ShowDevTools)
            {
                if ((int)commandId == ShowDevTools)
                {
                    browser.ShowDevTools();
                }
                if ((int)commandId == CloseDevTools)
                {
                    browser.CloseDevTools();
                }
            }
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}
