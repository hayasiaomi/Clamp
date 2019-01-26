using Clamp.Linker.Bootstrapper;

namespace Clamp.Linker.Diagnostics
{
    public interface IDiagnostics
    {
        /// <summary>
        /// Initialise diagnostics
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        void Initialize(IPipelines pipelines);
    }
}