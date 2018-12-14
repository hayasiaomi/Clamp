using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter
{
    public interface IAppManager
    {
        void Initialize();

        void Run(params string[] commandLines);
    }
}
