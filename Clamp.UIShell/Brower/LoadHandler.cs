using CefSharp;
using Clamp.UIShell.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Brower
{
    public class LoadHandler : ILoadHandler
    {

        public void OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs)
        {
        }

        public void OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs)
        {

        }

        public void OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs)
        {
        }

        public void OnLoadingStateChange(IWebBrowser browserControl, LoadingStateChangedEventArgs loadingStateChangedArgs)
        {
        }
    }
}
