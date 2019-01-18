namespace ClampMVC.Localization
{
    /// <summary>
    /// Used to return string values
    /// </summary>
    public interface ITextResource
    {
        /// <summary>
        /// Gets a translation based on the provided key.
        /// </summary>
        /// <param name="key">The key to look up the translation for.</param>
        /// <param name="context">The current <see cref="WebworkContext"/> instance.</param>
        string this[string key, WebworkContext context] { get; }
    }
}