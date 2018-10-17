using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Webwork.Annotation
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
