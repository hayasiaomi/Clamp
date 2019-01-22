namespace ClampMVC.ErrorHandling
{
    /// <summary>
    /// Provides informative responses for particular HTTP status codes
    /// </summary>
    public interface IStatusCodeHandler
    {
        /// <summary>
        /// Check if the error handler can handle errors of the provided status code.
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">The <see cref="ClampWebContext"/> instance of the current request.</param>
        /// <returns>True if handled, false otherwise</returns>
        bool HandlesStatusCode(HttpStatusCode statusCode, ClampWebContext context);

        /// <summary>
        /// Handle the error code
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">Current context</param>
        void Handle(HttpStatusCode statusCode, ClampWebContext context);
    }
}
