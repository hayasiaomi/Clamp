using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    class SetupLocal : ISetupHandler
    {
        public void Scan(BundleRegistry registry, string scanFolder, string[] filesToIgnore)
        {
            BundleRegistry reg = new BundleRegistry(registry.RegistryPath, registry.StartupDirectory, registry.DefaultAddinsFolder, registry.AddinCachePath);

            reg.CopyExtensionsFrom(registry);

            List<string> files = new List<string>();

            for (int n = 0; n < filesToIgnore.Length; n++)
                files.Add(filesToIgnore[n]);

            reg.ScanFolders(scanFolder, files);
        }

        public void GetAddinDescription(BundleRegistry registry, string file, string outFile)
        {
            registry.ParseAddin(file, outFile);
        }
    }
}
