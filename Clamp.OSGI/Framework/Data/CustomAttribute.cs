using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    public class CustomAttribute : Dictionary<string, string>
    {
        private string typeName;

        /// <summary>
        /// Full name of the type of the custom attribute
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }
    }
}
