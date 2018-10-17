using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aomi.Common.Services
{
    public interface ICommonService : IService
    {
        string Hello(string name);
    }
}
