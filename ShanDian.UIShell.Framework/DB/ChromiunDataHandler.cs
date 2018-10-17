using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.DB
{
    public class ChromiunDataHandler:DataHandler
    {
        public ChromiunDataHandler():base(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB", "chromiun.db"), "LocalChromiunInfo")
        {
        }
    }
}
