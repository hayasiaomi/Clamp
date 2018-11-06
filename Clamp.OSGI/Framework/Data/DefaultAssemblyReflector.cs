using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    class DefaultAssemblyReflector : IAssemblyReflector
    {
      

        public void Initialize(IAssemblyLocator locator)
        {
        }

        public object LoadAssembly(string file)
        {
            return Util.LoadAssemblyForReflection(file);
        }

        public string[] GetResourceNames(object asm)
        {
            return ((Assembly)asm).GetManifestResourceNames();
        }

        public System.IO.Stream GetResourceStream(object asm, string resourceName)
        {
            return ((Assembly)asm).GetManifestResourceStream(resourceName);
        }

        public object GetCustomAttribute(object obj, Type type, bool inherit)
        {
            foreach (object att in GetCustomAttributes(obj, type, inherit))
                if (type.IsInstanceOfType(att))
                    return att;
            return null;
        }

        public object[] GetCustomAttributes(object obj, Type type, bool inherit)
        {
            ICustomAttributeProvider aprov = obj as ICustomAttributeProvider;
            if (aprov != null)
                return aprov.GetCustomAttributes(type, inherit);
            else
                return new object[0];
        }
    }
}
