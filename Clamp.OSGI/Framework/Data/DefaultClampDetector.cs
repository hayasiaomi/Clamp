using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class DefaultClampDetector : MarshalByRefObject
    {
        public DefaultClampDetector()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (o, a) =>
            {
                var asm = typeof(DefaultClampDetector).Assembly;
                return a.Name == asm.FullName ? asm : null;
            };
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Detect(string[] folders, string[] filesToIgnore)
        {
            BundleDatabase bundleDataBase = new BundleDatabase();

            foreach (string folder in folders)
            {
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder);

                    foreach (string file in files)
                    {
                        if (file.EndsWith(".bundle.xml", StringComparison.CurrentCultureIgnoreCase) || file.EndsWith(".bundle", StringComparison.CurrentCultureIgnoreCase))
                        {

                        }
                    }
                }
            }
        }
    }
}
