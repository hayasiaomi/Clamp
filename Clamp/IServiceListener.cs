using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp
{
    public interface IServiceListener
    {
        void ServiceChanged(ServiceEvent serviceEvent);
    }
}
