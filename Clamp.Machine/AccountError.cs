
using Clamp.Software.SDK;

namespace Clamp.Machine
{
    public class AccountError : SystemError<AccountError>
    {
        /// <summary>
        /// 账号组件异常
        /// </summary>
        public int ComponentError = 1001;

        /// <summary>
        /// 授权码错误
        /// </summary>
        public int KeyError = 1002;

        /// <summary>
        /// 授权码超时错误
        /// </summary>
        public int KeyOverTimeError = 1003;

        /// <summary>
        /// 用户没有权限
        /// </summary>
        public int NotPermissionError = 1004;


        /// <summary>
        /// 远程服务器不能访问
        /// </summary>
        public int RometeServerError = 1014;

        #region 旧code

        /// <summary>
        /// 账号不存在
        /// </summary>
        public int UserError = 1002;

        /// <summary>
        /// 密码错误
        /// </summary>
        public int PwdError = 1003;

        /// <summary>
        /// 操作异常
        /// </summary>
        public int ExecuteError = 1006;

        /// <summary>
        /// 账号被禁用
        /// </summary>
        public int UserDisableError = 1007;

        #endregion





    }
}
