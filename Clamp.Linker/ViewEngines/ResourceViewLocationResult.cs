using Clamp.OSGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.Linker.ViewEngines
{
    public class ResourceViewLocationResult : ViewLocationResult
    {
        private readonly RuntimeBundle runtimeBundle;
        private readonly string resourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewLocationResult"/> class.
        /// </summary>
        /// <param name="location">The location of where the view was found.</param>
        /// <param name="name">The name of the view.</param>
        /// <param name="extension">The file extension of the located view.</param>
        /// <param name="contents">A <see cref="TextReader"/> that can be used to read the contents of the located view.</param>
        /// <param name="fullFilename">Full filename of the file</param>
        /// <param name="fileSystem">An <see cref="IFileSystemReader"/> instance that should be used when retrieving view information from the file system.</param>
        public ResourceViewLocationResult(string resourceName, string name, string extension, RuntimeBundle runtimeBundle)
        {
            this.resourceName = resourceName;
            this.runtimeBundle = runtimeBundle;
            this.Location = resourceName;
            this.Name = name;
            this.BundleName = runtimeBundle.Name;
            this.Extension = extension;
            this.Contents = this.GetContents;
        }

        /// <summary>
        /// Gets a value indicating whether the current item is stale
        /// </summary>
        /// <returns>True if stale, false otherwise</returns>
        public override bool IsStale()
        {
            return false;
        }

        /// <summary>
        /// Wraps the real contents delegate to set the last modified date first
        /// </summary>
        /// <returns>TextReader to read the file</returns>
        private StreamReader GetContents()
        {
            return new StreamReader(runtimeBundle.GetResource(this.resourceName));
        }
    }
}
