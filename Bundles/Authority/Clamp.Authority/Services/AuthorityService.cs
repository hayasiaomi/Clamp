using Clamp.MUI.Framework.Security;
using Clamp.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Authority.Services
{
    [Extension]
    public class AuthorityService : IAuthority
    {
        public AuthorityInfo GetAuthorityInfo()
        {
            return new AuthorityInfo() { Name = "aomi", Username = "00000", Password = "1234" };
        }
    }
}
