namespace Clamp.Linker.Diagnostics.Controllers
{
    public class MainController : DiagnosticController
    {
        public MainController()
        {
            Get["/"] = _ =>
            {
                return View["Dashboard"];
            };

            Post["/"] = _ => this.Response.AsRedirect("~/");
        }
    }
}
