using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public delegate void AddinErrorEventHandler(object sender, BundleErrorEventArgs args);

    /// <summary>
    /// Provides information about an add-in loading error.
    /// </summary>
    public class BundleErrorEventArgs : BundleEventArgs
    {
        Exception exception;
        string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.AddinErrorEventArgs"/> class.
        /// </summary>
        /// <param name='message'>
        /// Error message
        /// </param>
        /// <param name='addinId'>
        /// Add-in identifier.
        /// </param>
        /// <param name='exception'>
        /// Exception that caused the error.
        /// </param>
        public BundleErrorEventArgs(string message, string addinId, Exception exception) : base(addinId)
        {
            this.message = message;
            this.exception = exception;
        }

        /// <summary>
        /// Exception that caused the error.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message
        {
            get { return message; }
        }
    }
}
