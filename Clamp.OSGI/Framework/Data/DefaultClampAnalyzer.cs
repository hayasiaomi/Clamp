using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class DefaultClampAnalyzer : IClampAnalyzer
    {
        private AppDomain domain;
        private DefaultClampDetector defaultClampDetector;

        public ClampBundle ClampBundle { set; get; }

        public string[] FilesToIgnore { set; get; }

        public DefaultClampAnalyzer(ClampBundle clampBundle, string[] filesToIgnore)
        {
            this.ClampBundle = clampBundle;
            this.FilesToIgnore = filesToIgnore;
        }

        public void Analyze(string[] folders)
        {
            try
            {
                DefaultClampDetector detector = this.GetDetector();

                detector.Detect(folders, this.FilesToIgnore);

            }
            catch (Exception ex)
            {
                throw new FrameworkException("分析插件的时候出现异常", ex);
            }
            finally
            {
                ReleaseDomain();
            }
        }

        public void CheckFolder()
        {
            string bundlesDirectory = this.ClampBundle.BundlesDirectory;

            if (Directory.Exists(bundlesDirectory))
                Directory.CreateDirectory(bundlesDirectory);
        }

        private DefaultClampDetector GetDetector()
        {
            this.domain = AppDomain.CreateDomain("ClampDetectorDomain", null, AppDomain.CurrentDomain.SetupInformation);
            var type = typeof(DefaultClampDetector);
            this.defaultClampDetector = (DefaultClampDetector)domain.CreateInstanceFromAndUnwrap(type.Assembly.Location, type.FullName);

            return this.defaultClampDetector;
        }

        private void ReleaseDomain()
        {
            AppDomain.Unload(domain);
            domain = null;
            defaultClampDetector = null;
        }
    }

   
}
