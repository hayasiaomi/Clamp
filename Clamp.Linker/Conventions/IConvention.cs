namespace Clamp.Linker.Conventions
{
    using System;

    /// <summary>
    /// Provides Nancy convention defaults and validation
    /// </summary>
    public interface IConvention
    {
        /// <summary>
        /// Initialise any conventions this class "owns"
        /// </summary>
        /// <param name="conventions">Convention object instance</param>
        void Initialise(LinkerConventions conventions);

        /// <summary>
        /// Asserts that the conventions that this class "owns" are valid
        /// </summary>
        /// <param name="conventions">Conventions object instance</param>
        /// <returns>Tuple containing true/false for valid/invalid, and any error messages</returns>
        Tuple<bool, string> Validate(LinkerConventions conventions);
    }
}