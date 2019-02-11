using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Injection
{
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class ConstructorAttribute : Attribute
    {

    }
}
