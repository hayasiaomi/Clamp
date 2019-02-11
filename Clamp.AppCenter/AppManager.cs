using Clamp.AppCenter.Initial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter
{
    public abstract class AppManager : IAppManager
    {
       

        public virtual void Initialize()
        {
        }

        public virtual void Run(params string[] commandLines)
        {

        }
    }
}
