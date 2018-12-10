using Clamp.OSGI.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    /// <summary>
    /// 特殊Bundle注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleFragmentAttribute : BundleAttribute
    {

    }
}
