using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker.Annotation
{
    public class DeleteAttribute : RouteAttribute
    {
        public DeleteAttribute() : base("DELETE")
        { }
        public DeleteAttribute(string path) : base("DELETE", path)
        { }
    }
}
