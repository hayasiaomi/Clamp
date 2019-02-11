using Clamp.OSGI.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework
{
    [TypeExtensionPoint]
    public interface IMenuLink
    {
        string Id { get; }
        string BundleName { get; }

        string Path { get; }

        string Name { get; }

        string IconName { get; }

        IMenuLink[] SubMenuLinks { get; }
    }
}
