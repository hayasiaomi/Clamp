﻿namespace Clamp.Linker
{
    /// <summary>
    /// Defines the functionality of a <see cref="IResponseFormatter"/> factory.
    /// </summary>
    public interface IResponseFormatterFactory
    {
        /// <summary>
        /// Creates a new <see cref="IResponseFormatter"/> instance.
        /// </summary>
        /// <param name="context">The <see cref="LinkerContext"/> instance that should be used by the response formatter.</param>
        /// <returns>An <see cref="IResponseFormatter"/> instance.</returns>
        IResponseFormatter Create(LinkerContext context);
    }
}