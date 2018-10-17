using Aomi.Common.Services;
using ShanDian.AddIns;
using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Services;
using ShanDian.Webwork;
using ShanDian.Webwork.Annotation;
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
