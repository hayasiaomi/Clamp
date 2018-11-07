using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    enum BundleSearchFlagsInternal
    {
        IncludeAddins = 1,
        IncludeRoots = 1 << 1,
        IncludeAll = IncludeAddins | IncludeRoots,
        LatestVersionsOnly = 1 << 3,
        ExcludePendingUninstall = 1 << 4
    }
}
