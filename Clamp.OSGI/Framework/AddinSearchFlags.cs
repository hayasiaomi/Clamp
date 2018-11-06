using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    [Flags]
    public enum AddinSearchFlags
    {
        /// <summary>
        /// Add-ins are included in the search
        /// </summary>
        IncludeAddins = 1,
        /// <summary>
        /// Add-in roots are included in the search
        /// </summary>
        IncludeRoots = 1 << 1,
        /// <summary>
        /// Both add-in and add-in roots are included in the search
        /// </summary>
        IncludeAll = IncludeAddins | IncludeRoots,
        /// <summary>
        /// Only the latest version of every add-in or add-in root is included in the search
        /// </summary>
        LatestVersionsOnly = 1 << 3
    }
}
