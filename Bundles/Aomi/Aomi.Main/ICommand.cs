using Clamp.OSGI.Framework.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aomi.Main
{
    [TypeExtensionPoint]
    public interface ICommand
    {
        void Run();
    }
}
