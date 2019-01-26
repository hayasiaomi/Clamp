using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker.Annotation
{
    public class HeadAttribute : RouteAttribute
    {
        public HeadAttribute() : base("HEAD")
        { }
        public HeadAttribute(string path) : base("HEAD", path)
        {

        }
    }
}
