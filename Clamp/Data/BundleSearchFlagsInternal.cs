using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data
{
    enum BundleSearchFlagsInternal
    {
        IncludeBundles = 1,
        IncludeRoots = 1 << 1,
        IncludeAll = IncludeBundles | IncludeRoots,
        LatestVersionsOnly = 1 << 3,
        ExcludePendingUninstall = 1 << 4
    }
}
