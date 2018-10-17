using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.Platform
{
    /// <summary>
    /// 云平台返回Page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CloudOutPage<T>
    {
        /// <summary>
        /// 查询总数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<T> Data { get; set; }
    }
}
