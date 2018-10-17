using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Injection
{
    public enum DuplicateImplementationActions
    {
        RegisterSingle,
        RegisterMultiple,
        Fail
    }
}
