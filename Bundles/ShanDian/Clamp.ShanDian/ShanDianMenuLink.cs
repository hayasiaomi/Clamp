using Clamp.MUI.Framework;
using Clamp.OSGI.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.ShanDian
{
    [Extension]
    public class ShanDianMenuLink : IMenuLink
    {
        public string Id
        {
            get
            {
                return "Clamp.ShanDian";
            }
        }
        public string BundleName
        {
            get
            {
                return "ShanDian";
            }
        }
        public string Path
        {
            get
            {
                return "Home/index";
            }
        }
        public string Name
        {
            get
            {
                return "善点管理";
            }
        }

        public string IconName
        {
            get
            {
                return "fa fa-suitcase fa-fw";

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
