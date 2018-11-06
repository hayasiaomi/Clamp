using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    /// <summary>
    /// Delegate to be used in add-in engine events
    /// </summary>
    public delegate void AddinEventHandler(object sender, AddinEventArgs args);

    /// <summary>
    /// Provides information about an add-in engine event.
    /// </summary>
    public class AddinEventArgs : EventArgs
    {
        string addinId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.AddinEventArgs"/> class.
        /// </summary>
        /// <param name='addinId'>
        /// Add-in identifier.
        /// </param>
        public AddinEventArgs(string addinId)
        {
            this.addinId = addinId;
        }

        /// <summary>
        /// Identifier of the add-in that generated the event.
        /// </summary>
        public string AddinId
        {
            get { return addinId; }
        }
    }
}
