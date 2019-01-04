using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data.Annotation
{
    /// <summary>
    /// Bundle的激活类标示
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleActivatorAttribute : Attribute
    {
        private string typeName;

        public BundleActivatorAttribute()
        {
        }

        public BundleActivatorAttribute(string typeName)
        {
            this.typeName = typeName;
        }

      
        public string TypeName
        {
            get { return this.typeName; }
            set { this.typeName = value; }
        }

    
    }
}
