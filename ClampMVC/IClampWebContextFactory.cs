namespace ClampMVC
{
    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public interface IClampWebContextFactory
    {
        /// <summary>
        /// Create a new NancyContext
        /// </summary>
        /// <returns>NancyContext instance</returns>
        ClampWebContext Create(Request request);
    }
}