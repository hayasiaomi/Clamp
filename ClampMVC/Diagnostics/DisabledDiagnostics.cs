namespace ClampMVC.Diagnostics
{
    using ClampMVC.Bootstrapper;

    /// <summary>
    /// Implementation of the <see cref="IDiagnostics"/> interface that does nothing.
    /// </summary>
    public class DisabledDiagnostics : IDiagnostics
    {
        /// <summary>
        /// Initialise diagnostics
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            // Do nothing :-)
        }
    }
}