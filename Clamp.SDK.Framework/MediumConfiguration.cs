using Newtonsoft.Json;
using Clamp.Common.Initial;
using Clamp.SDK.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.SDK.Framework
{
    public class MediumConfiguration
    {
        public const string MediumFile = "medium.ini";

        public int[] Listeners { private set; get; }

        public int MainListener { private set; get; }

        public int SubListener { private set; get; }

        public string ConfigurationString { private set; get; }

        public string Env { set; get; }

        public string SDApiHost { set; get; }


        public MediumConfiguration() : this(Path.Combine(SDHelper.GetSDRootPath(), MediumFile))
        {

        }

        public MediumConfiguration(string configurationString)
        {
            this.ConfigurationString = configurationString;
        }

        public Uri[] GetUris()
        {
            Uri[] uris = new Uri[this.Listeners.Length + 1];

            uris[0] = new Uri(string.Format("http://localhost:{0}/", this.MainListener));

            for (int i = 1; i < uris.Length; i++)
            {
                uris[i] = new Uri(string.Format("http://localhost:{0}/", this.Listeners[i - 1]));
            }

            return uris;
        }

        public void Initialize()
        {
            FileInfo fileInfo = new FileInfo(this.ConfigurationString);

            InitialFile initials = InitialFile.LoadFromFile(this.ConfigurationString);

            if (!fileInfo.Exists || fileInfo.Length <= 0)
            {
                this.Listeners = new int[0];
                this.MainListener = 8899;
                this.Env = Environment.GetEnvironmentVariable("MikeToolHost", EnvironmentVariableTarget.Machine) ?? "";
                this.SDApiHost = $"https://@@Env@@sd-api.chidaoni.com";

                initials["Listeners"].IntValueArray = this.Listeners.ToArray();
                initials["MainListener"].IntValue = this.MainListener;
                initials["SubListener"].IntValue = this.SubListener;
                initials["Env"].StringValue = this.Env;
                initials["SDApiHost"].StringValue = this.SDApiHost;

                initials.SaveToFile(this.ConfigurationString);
            }
            else
            {
                this.MainListener = initials["MainListener"].IntValue;
                this.SubListener = initials["SubListener"].IntValue;
                this.Listeners = initials["Listeners"].IntValueArray;
                this.Env = initials["Env"].StringValue;
                this.SDApiHost = initials["SDApiHost"].StringValue;
            }

            if (string.IsNullOrWhiteSpace(this.Env))
                this.Env = Environment.GetEnvironmentVariable("MikeToolHost", EnvironmentVariableTarget.Machine) ?? "";

            if (!string.IsNullOrWhiteSpace(this.SDApiHost))
            {
                this.SDApiHost = this.SDApiHost.Replace("@@Env@@", this.Env);
            }

            this.Listeners = this.Listeners.ToArray();
        }
    }
}
