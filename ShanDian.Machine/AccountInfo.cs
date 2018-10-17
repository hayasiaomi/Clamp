using System;
using System.Collections.Generic;
using ShanDian.Machine.Model;

namespace ShanDian.Machine
{
    public class AccountInfo
    {

        //<phoneNum,key> 手机号、验证码验证, 密码重置修改授权码
        public static Dictionary<string, string> PwdKeyPhoneDic = new Dictionary<string, string>();
        //Tuple<phoneNum, DateTime, 验证码>
        public static List<Tuple<string, DateTime, string>> CodePhoneList = new List<Tuple<string, DateTime, string>>();


        //public static SecretKeyContent Key = null;

        public static List<LicenseCodePwd> LstLicenseCodePwd = new List<LicenseCodePwd>();

        public static List<LicenseCodeUrl> LstLicenseCodeUrl = new List<LicenseCodeUrl>();


    }
}
