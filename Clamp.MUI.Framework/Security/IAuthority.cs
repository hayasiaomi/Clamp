using Clamp.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.Security
{
    [TypeExtensionPoint]
    public interface IAuthority
    {
        AuthorityInfo GetAuthorityInfo();
    }
}
