using Aomi.Common.Services;
using Clamp.AddIns;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.Services;
using Clamp.Webwork;
using Clamp.Webwork.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aomi.Crm.Controllers
{
    public class HomeController : WebworkController
    {
        private ICommonService commonService;

        public HomeController() : base("crm")
        {
            this.commonService = AddInManager.GetEntityService<ICommonService>("CommonService");
        }

        [Get("Index")]
        public dynamic Index()
        {
            return View["Default"];
        }

        [Get("common")]
        public dynamic Common()
        {
            return this.commonService.Hello("aomi"); ;
        }
    }
}
