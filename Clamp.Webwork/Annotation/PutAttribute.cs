using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Webwork.Annotation
{
    public class PutAttribute : RouteAttribute
    {
        public PutAttribute() : base("PUT")
        {

        }
        public PutAttribute(string path) : base("PUT", path)
        {

        }
    }
}
