using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Annotation
{

    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : Attribute
    {
     
        public BundleAttribute()
        {
        }

        public BundleAttribute(string id)
        {
            this.Id = id;
        }

        public BundleAttribute(string id, string version)
        {
            this.Id = id;
            this.Version = version;
        }

        public string Id { set; get; }

        public string Version { set; get; }
   
        public string CompatVersion { set; get; }

        public string Namespace { set; get; }

        public string Category { set; get; }

        public string Url { set; get; }

        public bool EnabledByDefault { set; get; }
    }
}
