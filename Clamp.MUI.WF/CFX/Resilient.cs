using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clamp.MUI.WF.CFX
{
    internal class Resilient
    {
        private readonly Func<bool> _Do;
        private int _TimeOut = 100;

        public Resilient(Func<bool> @do)
        {
            _Do = @do;
        }

        public Resilient WithTimeOut(int timeOut)
        {
            _TimeOut = timeOut;
            return this;
        }

        public void StartIn(int firstTimeOut)
        {
            Thread.Sleep(firstTimeOut);

            while (!_Do())
            {
                Thread.Sleep(_TimeOut);
            }
        }
    }
}
