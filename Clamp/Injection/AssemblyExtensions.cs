using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.Injection
{
    public static class AssemblyExtensions
    {
        public static Type[] SafeGetTypes(this Assembly assembly)
        {
            Type[] assemblies;

            try
            {
                assemblies = assembly.GetTypes();
            }
            catch (System.IO.FileNotFoundException)
            {
                assemblies = new Type[] { };
            }
            catch (NotSupportedException)
            {
                assemblies = new Type[] { };
            }
            catch (ReflectionTypeLoadException e)
            {
                assemblies = e.Types.Where(t => t != null).ToArray();
            }
            return assemblies;
        }
    }
}
