using Clamp.AIM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.AIM
{
    public interface IManagerGetter
    {
        AddInInfo GetAddInInfo(string addinId, string version);
    }
}
