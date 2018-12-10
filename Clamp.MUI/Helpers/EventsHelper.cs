using Chromium;
using Chromium.Remote;
using Chromium.WebBrowser;
using Clamp.MUI.ChromiumCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Clamp.MUI.Helpers
{
    class EventsHelper
    {
        private static Dictionary<string, List<ScriptsMethod>> events = new Dictionary<string, List<ScriptsMethod>>();

        public static void Off(string eventName, ScriptsMethod method)
        {
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                if (events.ContainsKey(eventName.ToLower()))
                {
                    if (method == null)
                        events[eventName.ToLower()].Clear();
                    else
                    {
                        events[eventName.ToLower()].Remove(method);
                    }
                }
            }
        }

        public static void Emit(string eventName, string arg)
        {
            if (events.ContainsKey(eventName.ToLower()))
            {
                List<ScriptsMethod> functionList = events[eventName.ToLower()];

                if (functionList != null && functionList.Count > 0)
                {
                    foreach (ScriptsMethod function in functionList)
                    {
                        CfrV8Context context = function.V8Context;

                        CfxRemoteCallContext rc = function.Method.CreateRemoteCallContext();
                        try
                        {
                            rc.Enter();

                            CfrTask cfrTask = new CfrTask();

                            cfrTask.Execute += delegate (object src, CfrEventArgs evt)
                            {
                                CfrV8Value[] argments = new CfrV8Value[] { arg };

                                bool valid = function.Method.IsValid;

                                function.Method.ExecuteFunctionWithContext(context, null, argments);
                            };

                            context.TaskRunner.PostTask(cfrTask);
                        }
                        finally
                        {
                            rc.Exit();
                        }
                    }
                }
            }
        }

        public static void Emit(ChromiumWebBrowser browser, string eventName, string arg)
        {
            var script = @"(function ()
                {
                     var evt = new CustomEvent('##EVENTNAME##', {  argments:'##ARGMENTS##' });
                     document.dispatchEvent(evt);
                })();";

            script = Regex.Replace(script, "##EVENTNAME##", eventName);
            script = Regex.Replace(script, "##ARGMENTS##", arg);

            browser.ExecuteJavascript(script);
        }

        public static void On(string eventName, ScriptsMethod method)
        {
            if (!events.ContainsKey(eventName.ToLower()))
            {
                events.Add(eventName.ToLower(), new List<ScriptsMethod>() { method });
            }
            else
            {
                events[eventName.ToLower()].Add(method);
            }
        }
    }
}
