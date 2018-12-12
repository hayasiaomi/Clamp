using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public interface IServiceListener
    {
        void ServiceChanged(ServiceEvent serviceEvent);
    }
}
