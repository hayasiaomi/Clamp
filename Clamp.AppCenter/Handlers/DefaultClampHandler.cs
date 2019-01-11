using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clamp.AppCenter.Handlers
{
    public abstract class DefaultClampHandler : IClampHandler
    {


        public DefaultClampHandler()
        {

        }

        public virtual object Handle(ClampHandlerContext context)
        {
            return null;
        }
    }
}
