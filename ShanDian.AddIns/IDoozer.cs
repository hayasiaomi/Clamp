using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns
{
    public interface IDoozer
    {
        bool HandleConditions { get; }

        object BuildItem(BuildItemArgs args);
    }
}
