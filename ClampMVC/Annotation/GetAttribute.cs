using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClampMVC.Annotation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class GetAttribute : RouteAttribute
    {
        public GetAttribute():base("GET")
        { }
        public GetAttribute(string path) : base("GET", path)
        { }
    }
}
