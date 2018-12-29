using Clamp.AppCenter.Initial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.AppCenter
{
    public abstract class AppManager : IAppManager
    {
        private static AppManager appManager;

        private Dictionary<string, string> clampConfs;

        public static AppManager Current
        {
            get
            {
                return appManager;
            }
        }

        public Dictionary<string, string> ClampConfs { get { return this.clampConfs; } }

        public virtual void Initialize()
        {
            if (appManager == null)
                appManager = this;

            this.clampConfs = this.GetClampConfiguration();
        }

        public virtual void Run(params string[] commandLines)
        {

        }

        protected virtual Dictionary<string, string> GetClampConfiguration()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            string clampConfFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Clamp.conf");

            if (File.Exists(clampConfFile))
            {
                InitialFile initialProperties = InitialFile.LoadFromFile(clampConfFile);

                foreach (InitialProperty initialProperty in initialProperties)
                {
                    keyValuePairs.Add(initialProperty.Name, initialProperty.StringValueTrimmed);
                }
            }

            return keyValuePairs;
        }
    }
}
