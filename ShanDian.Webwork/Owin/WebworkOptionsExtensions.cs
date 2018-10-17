namespace ShanDian.Webwork.Owin
{
    using System.Linq;

    /// <summary>
    /// Extensions for the NancyOptions class.
    /// </summary>
    public static class WebworkOptionsExtensions
    {
        /// <summary>
        /// Tells the NancyMiddleware to pass through when
        /// response has one of the given status codes.
        /// </summary>
        /// <param name="webworkOptions">The Nancy options.</param>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        public static void PassThroughWhenStatusCodesAre(this WebworkOptions webworkOptions, params HttpStatusCode[] httpStatusCode)
        {
            webworkOptions.PerformPassThrough = context => httpStatusCode.Any(code => context.Response.StatusCode == code);
        }
    }
}