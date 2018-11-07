using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    class SetupDomain : ISetupHandler
    {
        AppDomain domain;
        RemoteSetupDomain remoteSetupDomain;
        int useCount;

        public void Scan(BundleRegistry registry, string scanFolder, string[] filesToIgnore)
        {
            try
            {
                RemoteSetupDomain rsd = GetDomain();
                rsd.Scan(registry.BasePath, registry.DefaultAddinsFolder, registry.BundleCachePath, scanFolder, filesToIgnore);
            }
            catch (Exception ex)
            {
                throw new ProcessFailedException(null, ex);
            }
            finally
            {
                ReleaseDomain();
            }
        }

        public void GetAddinDescription(BundleRegistry registry, string file, string outFile)
        {
            try
            {
                RemoteSetupDomain rsd = GetDomain();
                rsd.GetAddinDescription(registry.BasePath, registry.DefaultAddinsFolder, registry.BundleCachePath, file, outFile);
            }
            catch (Exception ex)
            {
                throw new ProcessFailedException(null, ex);
            }
            finally
            {
                ReleaseDomain();
            }
        }

        // ensure types from this assembly returned to this domain from the remote domain can
        // be resolved even if we're in the LoadFrom context
        static System.Reflection.Assembly MonoAddinsAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asm = typeof(SetupDomain).Assembly;
            return args.Name == asm.FullName ? asm : null;
        }

        RemoteSetupDomain GetDomain()
        {
            lock (this)
            {
                if (useCount++ == 0)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += MonoAddinsAssemblyResolve;
                    domain = AppDomain.CreateDomain("SetupDomain", null, AppDomain.CurrentDomain.SetupInformation);
                    var type = typeof(RemoteSetupDomain);
                    remoteSetupDomain = (RemoteSetupDomain)domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);
                }
                return remoteSetupDomain;
            }
        }

        void ReleaseDomain()
        {
            lock (this)
            {
                if (--useCount == 0)
                {
                    AppDomain.Unload(domain);
                    domain = null;
                    remoteSetupDomain = null;
                    AppDomain.CurrentDomain.AssemblyResolve -= MonoAddinsAssemblyResolve;
                }
            }
        }
    }

    class RemoteSetupDomain : MarshalByRefObject
    {
        public RemoteSetupDomain()
        {
            // ensure types from this assembly passed to this domain from the main domain
            // can be resolved even though we're in the LoadFrom context
            AppDomain.CurrentDomain.AssemblyResolve += (o, a) =>
            {
                var asm = typeof(RemoteSetupDomain).Assembly;
                return a.Name == asm.FullName ? asm : null;
            };
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Scan(string basePath, string addinsDir, string databaseDir, string scanFolder, string[] filesToIgnore)
        {
            BundleDatabase.RunningSetupProcess = true;
            BundleRegistry reg = new BundleRegistry(basePath, addinsDir, databaseDir);
            List<string> files = new List<string>();
            for (int n = 0; n < filesToIgnore.Length; n++)
                files.Add(filesToIgnore[n]);
            reg.ScanFolders(scanFolder, files);
        }

        public void GetAddinDescription(string basePath, string addinsDir, string databaseDir, string file, string outFile)
        {
            BundleDatabase.RunningSetupProcess = true;

            BundleRegistry reg = new BundleRegistry(basePath, addinsDir, databaseDir);

            reg.ParseAddin(file, outFile);
        }
    }
}
