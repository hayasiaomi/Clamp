﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.Data
{
    class SingleFileAssemblyResolver
    {
        BundleScanResult scanResult;
        BundleScanner scanner;
        BundleRegistry registry;

        public SingleFileAssemblyResolver(BundleRegistry registry, BundleScanner scanner)
        {
            this.scanner = scanner;
            this.registry = registry;
        }

        public Assembly Resolve(object s, ResolveEventArgs args)
        {
            if (scanResult == null)
            {
                scanResult = new BundleScanResult();
                scanResult.LocateAssembliesOnly = true;

                if (registry.BasePath != null)
                    scanner.ScanFolder(registry.BasePath, null, scanResult);
                foreach (string dir in registry.GlobalBundleDirectories)
                    scanner.ScanFolderWithSubdirs(dir, BundleDatabase.GlobalDomain, scanResult);
            }

            string afile = scanResult.GetAssemblyLocation(args.Name);
            if (afile != null)
                return Util.LoadAssemblyForReflection(afile);
            else
                return null;
        }
    }
}
