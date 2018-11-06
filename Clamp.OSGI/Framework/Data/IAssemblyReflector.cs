using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal interface IAssemblyReflector
    {
        void Initialize(IAssemblyLocator locator);

        object LoadAssembly(string file);

        string[] GetResourceNames(object asm);

        Stream GetResourceStream(object asm, string resourceName);

        object GetCustomAttribute(object obj, Type type, bool inherit);

        object[] GetCustomAttributes(object obj, Type type, bool inherit);
    }
}
