using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClampMVC.Annotation
{
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// The method for the route
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The path for the route
        /// </summary>
        public string Path { get; set; }

        public RouteAttribute(string method) : this(method, null)
        {

        }

        public RouteAttribute(string method, string path)
        {
            this.Method = method;
            this.Path = path;
        }
    }
}
