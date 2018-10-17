using ShanDian.Webwork.Bootstrapper;

namespace ShanDian.Webwork.Diagnostics
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