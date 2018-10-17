using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Model
{
    public class UICommand
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public string Parameters { set; get; }

        public bool IsHandled { set; get; }

        public DateTime CreateDate { set; get; }
    }
}
