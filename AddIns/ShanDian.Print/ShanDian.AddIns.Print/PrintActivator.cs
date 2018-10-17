using ShanDian.AddIns.Print.Services.BUSHelper;
using ShanDian.AddIns.Print.Services.Module;
using ShanDian.AddIns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print
{
    public class PrintActivator : AddInActivator
    {
        public static AddIn AddIn;

        public override void Start(AddInContext addInContext)
        {
            AddIn = addInContext.AddIn;


            DatabaseServices.InitDatabase();
            DatabaseServices.LoadStandardVerSiteMethod();
            BusinessHelper.Instance.RestSystemMode();
        }

        public override void Stop(AddInContext addInContext)
        {

        }
    }
}
