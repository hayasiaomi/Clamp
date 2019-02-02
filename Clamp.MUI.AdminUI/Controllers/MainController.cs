using Clamp.MUI.Framework;
using Clamp.Linker;
using Clamp.Linker.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Clamp.MUI.AdminUI.ViewModel;

namespace Clamp.MUI.AdminUI.Controllers
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

            Get["/index"] = _ =>
            {
                IndexVM indexVM = new IndexVM();

                IMenuLink[] menuLinks = AdminUIActivator.BundleContext.GetExtensionObjects<IMenuLink>();

                if (menuLinks != null && menuLinks.Length > 0)
                {
                    indexVM.MenuLinks.AddRange(menuLinks);
                }

                return View["index", indexVM];
            };
        }
    }
}