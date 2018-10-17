using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.DB
{
    public class ConfigsDataHandler : DataHandler
    {
        public ConfigsDataHandler() : base(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB", "configs.db"), "LocalConfigInfos")
        {

        }
    }
}
