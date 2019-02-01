using Clamp.Linker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamp.Linker.Annotation;

namespace Clamp.ShanDian.Controllers
{
    [Controller]
    public class HomeController : DefaultController
    {
        public HomeController()
        {
            Get["/index"] = _ =>
            {
                return View["Index"];
            };
        }
    }
}
