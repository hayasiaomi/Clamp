using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.UIShell.Brower
{
    public class ScriptedMethodsBoundObject
    {
        private static Dictionary<string, List<IJavascriptCallback>> events = new Dictionary<string, List<IJavascriptCallback>>();

        public void on(string eventName, IJavascriptCallback callback)
        {
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                if (!events.ContainsKey(eventName.ToLower()))
                {
                    events.Add(eventName.ToLower(), new List<IJavascriptCallback>() { callback });
                }
                else
                {
                    events[eventName.ToLower()].Add(callback);
                }
            }
        }

        public void emit(string eventName, object @params)
        {
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                if (events.ContainsKey(eventName.ToLower()))
                {
                    Task.Factory.StartNew(() =>
                    {
                        List<IJavascriptCallback> javascriptCallbacks = events[eventName.ToLower()];

                        for (int i = 0; i < javascriptCallbacks.Count; i++)
                        {
                            IJavascriptCallback callback = javascriptCallbacks[i];

                            if (callback.CanExecute)
                                callback.ExecuteAsync(@params);
                        }
                    });
                }
            }
        }

        public void off(string eventName, IJavascriptCallback callback)
        {
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                if (events.ContainsKey(eventName.ToLower()))
                {
                    if (callback == null)
                        events[eventName.ToLower()].Clear();
                    else
                    {
                        events[eventName.ToLower()].Remove(callback);
                    }
                }
            }
        }

        public void off(string eventName)
        {
            this.off(eventName, null);
        }

    }
}
