using Clamp.OSGI.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter
{
    [TypeExtensionPoint]
    public interface IAppManager
    {
        void Initialize();

        void Run(params string[] commandLines);
    }
}
