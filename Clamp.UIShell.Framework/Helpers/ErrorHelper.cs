using Clamp.UIShell.Framework.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework
{
    public class ErrorHelper
    {
        private static Dictionary<int, string> ErrorDicionarys = new Dictionary<int, string>();

        static ErrorHelper()
        {
            ErrorDicionarys.Add(1002, UISResources.Error_InValidUsername);
            ErrorDicionarys.Add(1003, UISResources.Error_InValidUsername);
            ErrorDicionarys.Add(1007, UISResources.Error_InValidAuth);
            ErrorDicionarys.Add(0, UISResources.Error_OtherError);
            ErrorDicionarys.Add(510303, UISResources.Error_InValidUsername);
            ErrorDicionarys.Add(510302, UISResources.Error_InValidUsername);
            ErrorDicionarys.Add(510305, UISResources.Error_InValidAuth);
            ErrorDicionarys.Add(519201, UISResources.Error_NoAction);
            ErrorDicionarys.Add(510304, UISResources.Error_NoAction);
            ErrorDicionarys.Add(510309, UISResources.Error_NoAction);
            ErrorDicionarys.Add(510003, UISResources.Error_ExistDriver);

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
