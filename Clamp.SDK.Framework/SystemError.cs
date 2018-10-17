using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Software.SDK
{
    public abstract class SystemError<T> where T : SystemError<T>, new()
    {
        public static T Code { get; } = new T();

        /// <summary>
        /// 服务器异常
        /// </summary>
        public int ServerError = 403;

        /// <summary>
        /// 端口不存在
        /// </summary>
        public int PointNull = 404;

        /// <summary>
        /// 数据库异常
        /// </summary>
        public int DatabaseError = 405;

        /// <summary>
        /// 参数异常
        /// </summary>
        public int ParamsError = 406;

        /// <summary>
        /// 网络异常
        /// </summary>
        public int NetworkError = 407;

        /// <summary>
        /// 权限异常
        /// </summary>
        public int CheckpointError = 408;

        /// <summary>
        /// 执行目标错误
        /// </summary>
        /// 
        public int TargetNotFoundError = 409;
    }

}
