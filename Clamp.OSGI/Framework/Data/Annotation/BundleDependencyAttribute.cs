using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleDependencyAttribute : Attribute
    {
        public BundleDependencyAttribute(string id, string version)
        {
            this.Id = id;
            this.Version = version;
        }

        public string Id { private set; get; }

        public string Version { private set; get; }

    }
}
