using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class BundleStartLevelAttribute : Attribute
    {

        public BundleStartLevelAttribute(int startLevel)
        {
            this.StartLevel = startLevel;
        }

        /// <summary>
        /// 启动等级
        /// </summary>
        public int StartLevel { get; set; }
    }
}
