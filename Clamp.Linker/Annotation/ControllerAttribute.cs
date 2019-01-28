using Clamp.OSGI.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker.Annotation
{
    public class ControllerAttribute : ExtensionAttribute
    {
        public string ControllerName { set; get; }

        public ControllerAttribute() : this(string.Empty)
        {
        }

        public ControllerAttribute(string controllerName)
        {
            this.ControllerName = controllerName;
        }
    }
}
