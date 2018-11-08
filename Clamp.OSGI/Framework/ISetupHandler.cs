using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    internal interface ISetupHandler
    {
        void Scan(BundleRegistry registry, string scanFolder, string[] filesToIgnore);
        void GetBundleDescription(BundleRegistry registry, string file, string outFile);
    }
}
