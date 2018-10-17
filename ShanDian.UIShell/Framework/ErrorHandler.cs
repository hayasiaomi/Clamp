using ShanDian.UIShell.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework
{
    internal class ErrorHandler
    {
        private static Dictionary<int, string> ErrorDicionarys = new Dictionary<int, string>();

        static ErrorHandler()
        {
            ErrorDicionarys.Add(1002, SDResources.Error_InValidUsername);
            ErrorDicionarys.Add(1003, SDResources.Error_InValidUsername);
            ErrorDicionarys.Add(1007, SDResources.Error_InValidAuth);
            ErrorDicionarys.Add(0, SDResources.Error_OtherError);
            ErrorDicionarys.Add(510303, SDResources.Error_InValidUsername);
            ErrorDicionarys.Add(510302, SDResources.Error_InValidUsername);
            ErrorDicionarys.Add(510305, SDResources.Error_InValidAuth);
            ErrorDicionarys.Add(519201, SDResources.Error_NoAction);
            ErrorDicionarys.Add(510304, SDResources.Error_NoAction);
            ErrorDicionarys.Add(510309, SDResources.Error_NoAction);
            ErrorDicionarys.Add(510003, SDResources.Error_ExistDriver);

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
