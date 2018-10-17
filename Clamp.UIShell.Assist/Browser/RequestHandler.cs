using CefSharp;
using Clamp.UIShell.Assist.Helpers;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Assist.Brower
{
    public class RequestHandler : IRequestHandler
    {
        public const string SchemeName = "custom";
        public const string RenderProcessCrashedUrl = "http://processcrashed";

        public static readonly string VersionNumberString = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}", Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {
            return false;
        }

        bool IRequestHandler.OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return OnOpenUrlFromTab(browserControl, browser, frame, targetUrl, targetDisposition, userGesture);
        }

        protected virtual bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        bool IRequestHandler.OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        void IRequestHandler.OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
        }

        CefReturnValue IRequestHandler.OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {

                    NLogService.Info(request.Method + ":" + request.Url);

                    Uri callUri;

                    if (Uri.TryCreate(request.Url, UriKind.Absolute, out callUri))
                    {
                        if (callUri.DnsSafeHost.EndsWith("SD-proxy.chidaoni.com", StringComparison.OrdinalIgnoreCase))
                        {
                            var headers = request.Headers;

                            headers["Access-Control-Allow-Origin"] = "*";
                            headers["Access-Control-Allow-Methods"] = "POST, GET, OPTIONS";
                            headers["Access-Control-Max-Age"] = "30000";
                            headers["Access-Control-Allow-Headers"] = "*";
                            headers["Cache-Control"] = "no-cache, no-store";
                            request.Headers = headers;

                            request.Url = $"{callUri.Scheme}://{SDShellHelper.GetSDShellSettings().ShanDianHost}:{SDShellHelper.GetSDShellSettings().ShanDianPort}{ callUri.PathAndQuery + callUri.Fragment}";

                            NLogService.Info("To " + request.Method + ":" + request.Url);
                        }

                    }
                    else
                    {
                        NLogService.Info("无效URL :" + request.Url);
                    }
                }
            }

            return CefReturnValue.Continue;
        }

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            if (!callback.IsDisposed)
                callback.Dispose();
            return false;
        }

        void IRequestHandler.OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {
            browserControl.Load(RenderProcessCrashedUrl);
        }

        bool IRequestHandler.OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            if (!callback.IsDisposed)
            {
                callback.Dispose();
            }
            return false;
        }

        void IRequestHandler.OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl)
        {
        }

        bool IRequestHandler.OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return url.StartsWith("mailto");
        }

        void IRequestHandler.OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {

        }

        bool IRequestHandler.OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var headers = response.ResponseHeaders;

            headers["Cache-Control"] = "no-cache, no-store";

            response.ResponseHeaders = headers;

            return false;
        }

        IResponseFilter IRequestHandler.GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        void IRequestHandler.OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {

        }
    }
}
