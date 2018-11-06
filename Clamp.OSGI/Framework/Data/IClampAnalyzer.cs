using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    interface IClampAnalyzer
    {
        void CheckFolder();

        void Analyze(string[] folders);
    }
}
