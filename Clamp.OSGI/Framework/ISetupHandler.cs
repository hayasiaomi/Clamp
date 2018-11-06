using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    internal interface ISetupHandler
    {
        void Scan(IProgressStatus monitor, AddinRegistry registry, string scanFolder, string[] filesToIgnore);
        void GetAddinDescription(IProgressStatus monitor, AddinRegistry registry, string file, string outFile);
    }
}
