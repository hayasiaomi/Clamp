using Chromium;
using Chromium.Remote;
using Chromium.Remote.Event;
using Chromium.WebBrowser;
using Clamp.MUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Clamp.MUI.ChromiumCore
{
    internal class JsEventsObject : JSObject
    {
        public JsEventsObject()
        {
            this.AddFunction("on").Execute += Execute_On;
            this.AddFunction("emit").Execute += Execute_Emit;
            this.AddFunction("off").Execute += Execute_Off;
        }

        private void Execute_Off(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                string eventName = e.Arguments[0].StringValue;

                CfrV8Value callback = null;

                if (e.Arguments.Length > 1)
                {
                    callback = e.Arguments[1];
                }

                CfrV8Context context = CfrV8Context.GetCurrentContext();

                EventsHelper.Off(eventName, new ScriptsMethod(context, callback));
            }
        }

        private void Execute_Emit(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                string eventName = e.Arguments[0].StringValue;
                string arg = e.Arguments[0].StringValue;

                var script = @"(function ()
                {
                     var evt = document.createEvent('Event');
                     evt.initEvent('##EVENTNAME##', true, true); 
                     evt.argments = '##ARGMENTS##';
                     document.dispatchEvent(evt);
                })();";

                script = Regex.Replace(script, "##EVENTNAME##", eventName);
                script = Regex.Replace(script, "##ARGMENTS##", arg);

                this.Browser.ExecuteJavascript(script);
            }
        }

        private void Execute_On(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 1)
            {
                string eventName = e.Arguments[0].StringValue;

                CfrV8Value function = e.Arguments[1];

                var script = @"(function ()
                {
                    document.addEventListener('##EVENTNAME##',##EVENT##,false);
                })();";

                script = Regex.Replace(script, "##EVENTNAME##", eventName);

                script = Regex.Replace(script, "##EVENT##", function.FunctionName);

                this.Browser.ExecuteJavascript(script);
            }
        }

    }
}
