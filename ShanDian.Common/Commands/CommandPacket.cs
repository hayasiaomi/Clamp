using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Common.Commands
{
    public class CommandPacket
    {
        public string Id { set; get; }
        /// <summary>
        /// 1为业务包，2为确定包
        /// </summary>
        public CommandType CommandType { set; get; }
        /// <summary>
        /// 所有者。
        /// </summary>
        public string Possessor { set; get; }

        public string CommandName { set; get; }

        public string Data { set; get; }
    }
}
