using Clamp.Linker;
using Clamp.Linker.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clamp.MUI.Pages.Controllers
{
    [Controller]
    public class MainController : BaseController
    {
        public MainController()
        {
            Get["/login"] = _ =>
            {
                return View["login"];
            };
        }
    }
}