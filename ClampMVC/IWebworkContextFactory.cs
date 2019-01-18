namespace ClampMVC
{
    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public interface IWebworkContextFactory
    {
        /// <summary>
        /// Create a new NancyContext
        /// </summary>
        /// <returns>NancyContext instance</returns>
        WebworkContext Create(Request request);
    }
}