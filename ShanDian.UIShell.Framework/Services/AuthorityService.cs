using Newtonsoft.Json;
using ShanDian.Common.HTTP;
using ShanDian.UIShell.Framework.Brower;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.Model;
using ShanDian.UIShell.Framework.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Services
{
    public sealed class AuthorityService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="rememberPassword"></param>
        /// <param name="javascriptCallback"></param>
        public static bool Login(string username, string password, bool rememberPassword, out string errorMessage)
        {
            HttpResponse httpResponse = HttpRequest.Post(SDShellHelper.GetSDHost("Account", "users/login"), new { Username = username, Password = SDShellHelper.MD5Hash(password) });

            if (httpResponse == null)
            {
                errorMessage = UISResources.HttpAccessor_BadData;

                return false;
            }

            ServiceInfo<UserInfo> siUserInfo = httpResponse.AsDeserializeBody<ServiceInfo<UserInfo>>();

            if (siUserInfo == null)
            {
                errorMessage = UISResources.HttpAccessor_BadData;

                return false;
            }
            else if (!siUserInfo.Flag)
            {
                errorMessage = UISResources.HttpAccessor_SystemBusy;

                if (ErrorHelper.Exist(siUserInfo.Code))
                    errorMessage = ErrorHelper.Get(siUserInfo.Code);

                return false;
            }
            else if (siUserInfo.Result == null)
            {
                errorMessage = UISResources.HttpAccessor_NotFoundTokenInfo;

                return false;
            }

            UserInfo userInfo = siUserInfo.Result;

            List<CoreUserPermission> coreUserPermissions = new List<CoreUserPermission>();

            if (userInfo.Permissions != null && userInfo.Permissions.Count > 0)
            {
                foreach (PermissionInfo permissionInfo in userInfo.Permissions)
                {
                    CoreUserPermission coreUserPermission = new CoreUserPermission();

                    coreUserPermission.Code = permissionInfo.Code;
                    coreUserPermission.Icon = permissionInfo.Icon;
                    coreUserPermission.Name = permissionInfo.Name;
                    coreUserPermission.CategoryCode = permissionInfo.CategoryCode;
                    coreUserPermission.IsInner = permissionInfo.IsInner;
                    coreUserPermission.Sort = permissionInfo.Sort;
                    coreUserPermission.Token = permissionInfo.Token;
                    coreUserPermission.Url = permissionInfo.Url;
                    coreUserPermission.Icon = permissionInfo.Icon;
                    coreUserPermission.KindCode = permissionInfo.KindCode;

                    coreUserPermissions.Add(coreUserPermission);
                }
            }

            CoreUserInfo coreUserInfo = new CoreUserInfo();

            coreUserInfo.UserId = userInfo.UserId;
            coreUserInfo.UserName = userInfo.UserName;
            coreUserInfo.Token = userInfo.Token;
            coreUserInfo.Pwd = userInfo.Pwd;
            coreUserInfo.Status = userInfo.Status;
            coreUserInfo.UserName = userInfo.UserName;
            coreUserInfo.RoleName = userInfo.RoleName;
            coreUserInfo.Sex = userInfo.Sex;
            coreUserInfo.IsAdmin = userInfo.IsAdmin;
            coreUserInfo.Mobile = userInfo.Mobile;
            coreUserInfo.IsFirst = userInfo.IsFirstLogin;

            coreUserInfo.Permissions.AddRange(coreUserPermissions);

            string coreUserInfoValue = JsonConvert.SerializeObject(coreUserInfo);

            CDBHelper.Add("user_auth", coreUserInfoValue);

            string licenseNumberValues = CDBHelper.Get("license_number");


            if (!string.IsNullOrWhiteSpace(licenseNumberValues))
            {
                List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

                if (licensenumbers != null)
                {
                    LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

                    if (licenseNumber == null)
                    {
                        licensenumbers.Insert(0, new LicenseNumber()
                        {
                            Username = username,
                            Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
                            IsMemberkPassword = rememberPassword
                        });

                        if (licensenumbers.Count > 5)
                        {
                            licensenumbers.RemoveRange(5, licensenumbers.Count - 5);
                        }

                        CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
                    }
                    else
                    {
                        if (licensenumbers[0].Username != username)
                        {
                            licensenumbers.Remove(licenseNumber);
                            licensenumbers.Insert(0, licenseNumber);
                        }

                        licenseNumber.Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty;
                        licenseNumber.IsMemberkPassword = rememberPassword;

                        CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
                    }
                }
            }
            else
            {
                List<LicenseNumber> licensenumbers = new List<LicenseNumber>()
                        {
                            new LicenseNumber()
                            {
                                Username = username,
                                Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
                                IsMemberkPassword = rememberPassword
                            }
                        };

                CDBHelper.Add("license_number", JsonConvert.SerializeObject(licensenumbers));
            }

            errorMessage = "";

            return true;
        }

        public static bool Logout()
        {
            string coreUserInfoValue = CDBHelper.Get("user_auth");

            //if (!string.IsNullOrWhiteSpace(coreUserInfoValue))
            //{
            //    try
            //    {
            //        CoreUserInfo coreUserInfo = JsonConvert.DeserializeObject<CoreUserInfo>(coreUserInfoValue);

            //        if (coreUserInfo != null)
            //        {

            //            string url = string.Format(ServiceAccessor.HttpAccessTemplate, ChromiumSettings.Demand.Server, ChromiumSettings.Port, ServiceModule.Account, ServiceAccessor.Version, "users/token");

            //            DebugHelper.WriteLine("开始访问URL({0})", url);

            //            url = url + "?token=" + coreUserInfo.Token;

            //            var loader = new CloudLoader();
            //            var header = loader.CreateHeadDictionary("1.0.0.0", null, null);
            //            var client = new HttpSender(url, "", HttpMethod.Get, header);
            //            string result = client.GetResponse();

            //            DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");

            //            return true;

            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        DebugHelper.WriteException(ex);
            //    }
            //}

            return false;
        }


    }
}
