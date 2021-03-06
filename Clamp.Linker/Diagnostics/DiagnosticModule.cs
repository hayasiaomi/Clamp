﻿namespace Clamp.Linker.Diagnostics
{
    public abstract class DiagnosticController : BaseController
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