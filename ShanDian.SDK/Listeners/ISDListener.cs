using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Listeners
{
    interface ISDListener<TRequest, TContext> : ISDListener
    {
        Func<TRequest, TContext> HandleMessage { set; get; }
    }

    interface ISDListener
    {
        bool IsListening { get; }

        void StartListen();

        void Shutdown();
    }
}
