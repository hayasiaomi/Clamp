namespace ClampMVC.Diagnostics.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using ClampMVC.Bootstrapper;
    using ClampMVC.ViewEngines;

    public class InfoController : DiagnosticController
    {
        public InfoController(IRootPathProvider rootPathProvider, ClampWebInternalConfiguration configuration)
            : base("/info")
        {
            Get["/"] = _ =>
            {
                return View["Info"];
            };

            Get["/data"] = _ =>
            {
                dynamic data = new ExpandoObject();

                data.Webwork = new ExpandoObject();
                data.Webwork.Version = string.Format("v{0}", this.GetType().Assembly.GetName().Version.ToString());
                data.Webwork.TracesDisabled = StaticConfiguration.DisableErrorTraces;
                data.Webwork.CaseSensitivity = StaticConfiguration.CaseSensitive ? "Sensitive" : "Insensitive";
                data.Webwork.RootPath = rootPathProvider.GetRootPath();
                data.Webwork.Hosting = GetHosting();
                data.Webwork.BootstrapperContainer = GetBootstrapperContainer();
                data.Webwork.LocatedBootstrapper = ClampWebBootstrapperLocator.Bootstrapper.GetType().ToString();
                data.Webwork.LoadedViewEngines = GetViewEngines();

                data.Configuration = new Dictionary<string, object>();
                foreach (var propertyInfo in configuration.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var value =
                        propertyInfo.GetValue(configuration, null);

                    data.Configuration[propertyInfo.Name] = (!typeof(IEnumerable).IsAssignableFrom(value.GetType())) ?
                        new[] { value.ToString() } :
                        ((IEnumerable<object>) value).Select(x => x.ToString());
                }

                return this.Response.AsJson((object)data);
            };
        }

        private static string[] GetViewEngines()
        {
            var engines =
                AppDomainAssemblyTypeScanner.TypesOf<IViewEngine>();

            return engines
                .Select(engine => engine.Name.Split(new [] { "ViewEngine" }, StringSplitOptions.None)[0])
                .ToArray();
        }

        private static string GetBootstrapperContainer()
        {
            var name = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(asm => asm.GetName())
                .FirstOrDefault(asmName => asmName.Name != null && asmName.Name.StartsWith("Webwork.Bootstrappers."));

            return (name == null) ?
                "TinyIoC" :
                string.Format("{0} (v{1})", name.Name.Split('.').Last(), name.Version);
        }

        private static string GetHosting()
        {
            var name = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(asm => asm.GetName())
                .FirstOrDefault(asmName => asmName.Name != null && asmName.Name.StartsWith("Webwork.Hosting."));

            return (name == null) ?
                "Unknown" :
                string.Format("{0} (v{1})", name.Name.Split('.').Last(), name.Version);
        }
    }
}