namespace Clamp.Webwork.Diagnostics
{
    public abstract class DiagnosticController : Controller
    {
        protected DiagnosticController()
        {
        }

        protected DiagnosticController(string basePath)
            : base(basePath)
        {
        }

        public new DiagnosticsViewRenderer View
        {
            get { return new DiagnosticsViewRenderer(this.Context); }
        }
    }
}