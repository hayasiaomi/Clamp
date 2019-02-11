using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Description
{
    /// <summary>
    /// Type of the content of a string extension node attribute
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// Plain text
        /// </summary>
        Text,
        /// <summary>
        /// A class name
        /// </summary>
        Class,
        /// <summary>
        /// A resource name
        /// </summary>
        Resource,
        /// <summary>
        /// A file name
        /// </summary>
        File
    }
}
