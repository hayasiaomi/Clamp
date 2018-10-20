using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public interface IServiceListener
    {
        void ServiceChanged(ServiceEvent serviceEvent);
    }
}
