namespace ClampMVC.Bootstrapper
{
    /// <summary>
    /// Determines which set of types that the <see cref="AppDomainAssemblyTypeScanner"/> should scan in.
    /// </summary>
    public enum ScanMode
    {
        /// <summary>
        /// All available types.
        /// </summary>
        All,

        /// <summary>
        /// Only in types that are defined in the Nancy assembly.
        /// </summary>
        OnlyWebwork,

        /// <summary>
        /// Only types that are defined outside the Nancy assembly.
        /// </summary>
        ExcludeWebwork,

        /// <summary>
        /// Only Namespaces that starts with 'Nancy'
        /// </summary>
        OnlyWebworkNamespace,

        /// <summary>
        /// Only Namespaces that does not start with Nancy
        /// </summary>
        ExcludeWebworkNamespace
    }
}