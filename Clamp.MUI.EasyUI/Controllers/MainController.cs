using Clamp.MUI.Framework;
using Clamp.Linker;
using Clamp.Linker.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Clamp.MUI.EasyUI.ViewModel;

namespace Clamp.MUI.EasyUI.Controllers
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

                IMenuLink[] menuLinks = EasyUIActivator.BundleContext.GetExtensionObjects<IMenuLink>();

                if (menuLinks != null && menuLinks.Length > 0)
                {
                    indexVM.MenuLinks.AddRange(menuLinks);
                }

                return View["index", indexVM];
            };
        }
    }
}