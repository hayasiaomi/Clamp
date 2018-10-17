namespace ShanDian.Webwork
{
    using ShanDian.Webwork.Bootstrapper;

    /// <summary>
    /// Enables JSONP support at application startup.
    /// </summary>
    public class JsonpApplicationStartup : IApplicationStartup
    {
        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize(IPipelines pipelines)
        {
            Jsonp.Enable(pipelines);
        }
    }
}
