using Clamp.Linker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamp.Linker.Annotation;

namespace Clamp.Authority.Controllers
{
    [Controller]
    public class AuthorityController : DefaultController
    {
        public AuthorityController()
        {
            Get["/index"] = _ =>
            {
                return View["Authority"];
            };
        }
    }
}
