using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Network.Service
{
    internal class PermissionInfo
    {
        public string Icon { get; set; }

        public string Code { get; set; }

        public bool IsInner { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }


        public string Url { get; set; }


        public int Sort { get; set; }

        public string KindCode { get; set; }

        public string CategoryCode { get; set; }
    }


    internal class PermissionInfoComparer : IEqualityComparer<PermissionInfo>
    {
        public bool Equals(PermissionInfo x, PermissionInfo y)
        {
            return x.Code == x.Code;
        }

        public int GetHashCode(PermissionInfo obj)
        {
            return obj.Code.GetHashCode();
        }
    }
}
