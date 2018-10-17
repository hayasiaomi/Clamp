using ShanDian.Webwork;
using ShanDian.Webwork.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK
{
    public class AomiModule : WebworkController
    {
        public AomiModule() : base("aomi")
        {

        }

        [Get]
        public dynamic Login(Aaa aaa)
        {
            return aaa.username;
        }
    }

    public class Aaa
    {
        public string username { set; get; }
    }
}
