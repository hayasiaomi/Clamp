﻿namespace Clamp.Linker.Extensions
{
    using System.IO;
    using System.Text;

    using Clamp.Linker.IO;

    /// <summary>
    /// Extensions for RequestStream.
    /// </summary>
    public static class RequestStreamExtensions
    {
        /// <summary>
        /// Gets the request body as a string.
        /// </summary>
        /// <param name="stream">The request body stream.</param>
        /// <param name="encoding">The encoding to use, <see cref="Encoding.UTF8"/> by default.</param>
        /// <returns>The request body as a <see cref="string"/>.</returns>
        public static string AsString(this RequestStream stream, Encoding encoding = null)
        {
            using (var reader = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}