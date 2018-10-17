using ShanDain.AIM.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanDain.AIM
{
    public interface IManagerGetter
    {
        AddInInfo GetAddInInfo(string addinId, string version);
    }
}
