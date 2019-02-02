using Clamp.MUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clamp.MUI.AdminUI.ViewModel
{
    public class IndexVM
    {
        public IndexVM()
        {
            this.MenuLinks = new List<IMenuLink>();
        }

        public List<IMenuLink> MenuLinks { set; get; }
    }
}