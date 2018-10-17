using Newtonsoft.Json;
using ShanDian.AddIns;
using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Model;
using ShanDian.SDK.Framework.Services;
using ShanDian.Webwork;
using ShanDian.Webwork.Annotation;
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
