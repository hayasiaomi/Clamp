using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Webwork.Annotation
{
    public class PatchAttribute : RouteAttribute
    {
        public PatchAttribute() : base("PATCH")
        { }
        public PatchAttribute(string path) : base("PATCH", path)
        {

        }
    }
}
