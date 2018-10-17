using Newtonsoft.Json;
using Clamp.AddIns;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.Model;
using Clamp.SDK.Framework.Services;
using Clamp.Webwork;
using Clamp.Webwork.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aomi.Authority.Controllers
{
    public class AomiController : WebworkController
    {
        private IPrinterService printerService;

        public AomiController() : base("aomi")
        {
            this.printerService = AddInManager.GetEntityService<IPrinterService>("PrinterService");
        }

        [Get("Index")]
        public dynamic Index()
        {
            return View["Default"];
        }

        [Get("activate")]
        public dynamic Activate()
        {


            return View["Default"];
        }


        [Get("printers")]
        public dynamic Printers()
        {
            List<PrinterInfo> printerInfos = this.printerService.GetPrinters();

            return JsonConvert.SerializeObject(printerInfos);
        }


    }
}
