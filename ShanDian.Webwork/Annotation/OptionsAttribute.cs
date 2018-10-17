using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Webwork.Annotation
{
    public class OptionsAttribute : RouteAttribute
    {
        public OptionsAttribute() : base("OPTIONS")
        { }
        public OptionsAttribute(string path) : base("OPTIONS", path)
        {

        }
    }
}
