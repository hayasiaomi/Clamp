using Clamp.MUI.Framework;
using Clamp.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Authority
{
    [Extension]
    public class AuthorityMenuLink : IMenuLink
    {
        public string Id
        {
            get
            {
                return "Clamp.Authority";
            }
        }
        public string BundleName
        {
            get
            {
                return "Authority";
            }
        }
        public string Path
        {
            get
            {
                return "Authority/index";
            }
        }
        public string Name
        {
            get
            {
                return "用户授权";
            }
        }

        public string IconName
        {
            get
            {
                return "fa fa-dashboard fa-fw";

            }
        }
        public IMenuLink[] SubMenuLinks
        {
            get
            {
                return null;
            }
        }
    }
}
