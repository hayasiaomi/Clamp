using ShanDian.SDK.AddIns;
using ShanDian.Webwork;
using ShanDian.Webwork.Bootstrapper;
using ShanDian.Webwork.Conventions;
using ShanDian.Webwork.Diagnostics;
using ShanDian.Webwork.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK
{
    class SDBootstrapper : DefaultWebworkBootstrapper
    {
        public void InitializeAddIns()
        {
            this.ApplicationContainer.Resolve<IAddInGuideline>().Initialize();
        }

        protected override void ConfigureConventions(WebworkConventions webworkConventions)
        {
            webworkConventions.StaticContentsConventions.AddDirectory("docs", @"/Content/app");
            webworkConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("sd/upgrades", "/Upgrades"));
            webworkConventions.StaticContentsConventions.AddDirectory("os", "/UIShell");
            webworkConventions.StaticContentsConventions.AddDirectory("static", "/UIShell/static");

            base.ConfigureConventions(webworkConventions);
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.AfterRequest += (ctx) =>
            {
                if (ctx.Response.ContentType == "text/html")
                {
                    ctx.Response.ContentType = "text/html;charset=utf-8";
                }
            };
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                return new DiagnosticsConfiguration() { Password = "aomi" };
            }
        }

    }
}
