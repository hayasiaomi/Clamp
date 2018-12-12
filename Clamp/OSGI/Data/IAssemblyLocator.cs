using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data
{
    public interface IAssemblyLocator
    {
        /// <summary>
        /// Locates an assembly
        /// </summary>
        /// <returns>
        /// The full path to the assembly, or null if not found
        /// </returns>
        /// <param name='fullName'>
        /// Full name of the assembly
        /// </param>
        string GetAssemblyLocation(string fullName);
    }
}
