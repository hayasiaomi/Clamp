using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data
{
    /// <summary>
    /// 在AppDomain域中进行安装
    /// </summary>
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
                rsd.Scan(registry.BasePath, registry.DefaultBundlesFolder, registry.BundleCachePath, scanFolder, filesToIgnore);
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

        public void GetBundleDescription(BundleRegistry registry, string file, string outFile)
        {
            try
            {
                RemoteSetupDomain rsd = GetDomain();
                rsd.GetBundleDescription(registry.BasePath, registry.DefaultBundlesFolder, registry.BundleCachePath, file, outFile);
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
        static System.Reflection.Assembly MonoBundlesAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var asm = typeof(SetupDomain).Assembly;
            return args.Name == asm.FullName ? asm : null;
        }

        /// <summary>
        /// 获得一个新的AppDomain域的
        /// </summary>
        /// <returns></returns>
        private RemoteSetupDomain GetDomain()
        {
            lock (this)
            {
                if (useCount++ == 0)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += MonoBundlesAssemblyResolve;
                    domain = AppDomain.CreateDomain("SetupDomain", null, AppDomain.CurrentDomain.SetupInformation);
                    var type = typeof(RemoteSetupDomain);
                    remoteSetupDomain = (RemoteSetupDomain)domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);
                }
                return remoteSetupDomain;
            }
        }

        /// <summary>
        /// 卸载当前的AppDomain域
        /// </summary>
        private void ReleaseDomain()
        {
            lock (this)
            {
                if (--useCount == 0)
                {
                    AppDomain.Unload(domain);
                    domain = null;
                    remoteSetupDomain = null;
                    AppDomain.CurrentDomain.AssemblyResolve -= MonoBundlesAssemblyResolve;
                }
            }
        }
    }

    /// <summary>
    /// 用于跨AppDomain域所用的对象
    /// </summary>
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

        public void Scan(string basePath, string bundlesDir, string databaseDir, string scanFolder, string[] filesToIgnore)
        {
            BundleDatabase.RunningSetupProcess = true;

            BundleRegistry reg = new BundleRegistry(basePath, bundlesDir, databaseDir);

            List<string> files = new List<string>();

            for (int n = 0; n < filesToIgnore.Length; n++)
                files.Add(filesToIgnore[n]);

            reg.ScanFolders(scanFolder, files);
        }

        public void GetBundleDescription(string basePath, string addinsDir, string databaseDir, string file, string outFile)
        {
            BundleDatabase.RunningSetupProcess = true;

            BundleRegistry reg = new BundleRegistry(basePath, addinsDir, databaseDir);

            reg.ParseBundle(file, outFile);
        }
    }
}
