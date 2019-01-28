using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker.Annotation
{
    public class PostAttribute : RouteAttribute
    {
        public PostAttribute() : base("POST")
        { }
        public PostAttribute(string path) : base("POST", path)
        {
        }
    }
}
