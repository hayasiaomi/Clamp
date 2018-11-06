using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    [Flags]
    public enum AddinFlags
    {
        /// <summary>
        /// No flags
        /// </summary>
        None = 0,
        /// <summary>
        /// The add-in can't be uninstalled
        /// </summary>
        CantUninstall = 1,
        /// <summary>
        /// The add-in can't be disabled
        /// </summary>
        CantDisable = 2,
        /// <summary>
        /// The add-in is not visible to end users
        /// </summary>
        Hidden = 4
    }
}
