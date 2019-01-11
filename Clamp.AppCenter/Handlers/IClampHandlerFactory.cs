using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter.Handlers
{
    public interface IClampHandlerFactory
    {
        IClampHandler GetClampHandler();
    }
}
