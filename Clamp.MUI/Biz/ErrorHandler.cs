using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Biz
{
    internal class ErrorHandler
    {
        private static Dictionary<int, string> ErrorDicionarys = new Dictionary<int, string>();

        static ErrorHandler()
        {
            ErrorDicionarys.Add(1002, "账号或密码错误");
            ErrorDicionarys.Add(1003, "账号或密码错误");
            ErrorDicionarys.Add(1007, "账号未授权，请联系门店管理员");
            ErrorDicionarys.Add(0, "系统繁忙, 请稍后重试");
        }

        public static string Get(int code)
        {
            if (ErrorDicionarys.ContainsKey(code))
                return ErrorDicionarys[code];
            return null;
        }

        public static bool Exist(int code)
        {
            return ErrorDicionarys.ContainsKey(code);
        }
    }
}
