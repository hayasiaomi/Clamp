using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Listeners
{
    class SDMqttListener : ISDListener<object, object>
    {
        public bool IsListening { get { return false; } }

        public Func<object, object> HandleMessage { set; get; }

        public void Shutdown()
        {
        }

        public void StartListen()
        {

        }
    }
}
