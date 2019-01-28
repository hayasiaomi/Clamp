namespace Clamp.Linker
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
        LinkerContext Create(Request request);
    }
}