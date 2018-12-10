using Clamp.MUI.Framework.Inputs;
using Clamp.MUI.Framework.UI;
using Clamp.MUI.Helpers;
using Clamp.MUI.Framework.Network;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Clamp.MUI.Properties;

namespace Clamp.MUI.Biz
{
    ///// <summary>
    ///// 根浏览器交动的事件
    ///// </summary>
    //internal class WinFormBridge
    //{
    //    private InputSimulator input = new InputSimulator();
    //    private IChromiumWinForm chromiumWinForm;

    //    public WinFormBridge(IChromiumWinForm chromiumForm)
    //    {
    //        this.chromiumWinForm = chromiumForm;
    //    }

    //    /// <summary>
    //    /// 是否开启动
    //    /// </summary>
    //    public bool GetRestarting()
    //    {
    //        bool launched = false;
    //        object startup;

    //        if (Environment.Is64BitOperatingSystem)
    //        {
    //            using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
    //            using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
    //            {
    //                startup = shanDianRegistry.GetValue("ShanDian");
    //            }
    //        }
    //        else
    //        {
    //            using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
    //            using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
    //            {
    //                startup = shanDianRegistry.GetValue("ShanDian");
    //            }
    //        }

    //        if (startup != null)
    //        {
    //            string startupPath = Convert.ToString(startup);

    //            if (!string.IsNullOrWhiteSpace(startupPath))
    //            {
    //                launched = true;
    //            }
    //        }

    //        return launched;

    //    }

    //    public void Authorize(string username, string password, bool rememberPassword, Action<bool, int, string> authorizeCallback)
    //    {
    //        Task.Factory.StartNew(() =>
    //        {
    //            username = username.Trim();

    //            HttpResult<TokenInfo> hrTokenInfo = HttpAccessor.Token(username, password);

    //            if (hrTokenInfo == null)
    //            {
    //                if (authorizeCallback != null)
    //                    authorizeCallback(false, 500, StringResources.WinFormBridge_BadData);
    //                return;
    //            }
    //            else if (!hrTokenInfo.Flag)
    //            {
    //                string errorMessage = StringResources.WinFormBridge_SystemBusy;

    //                if (ErrorHandler.Exist(hrTokenInfo.Code))
    //                    errorMessage = ErrorHandler.Get(hrTokenInfo.Code);

    //                if (authorizeCallback != null)
    //                    authorizeCallback(hrTokenInfo.Flag, hrTokenInfo.Code, errorMessage);
    //                return;
    //            }
    //            else if (hrTokenInfo.Result == null)
    //            {
    //                if (authorizeCallback != null)
    //                    authorizeCallback(false, hrTokenInfo.Code, StringResources.WinFormBridge_NotFoundTokenInfo);
    //                return;
    //            }

    //            HttpResult<UserPermissionInfo> hrUserPermissionInfo = HttpAccessor.UserPermissions(hrTokenInfo.Result.UserId);

    //            if (hrUserPermissionInfo == null)
    //            {
    //                if (authorizeCallback != null)
    //                    authorizeCallback(false, 500, StringResources.WinFormBridge_BadData);
    //                return;
    //            }
    //            else if (!hrUserPermissionInfo.Flag)
    //            {
    //                string errorMessage = StringResources.WinFormBridge_SystemBusy;

    //                if (ErrorHandler.Exist(hrTokenInfo.Code))
    //                    errorMessage = ErrorHandler.Get(hrTokenInfo.Code);

    //                if (authorizeCallback != null)
    //                    authorizeCallback(hrTokenInfo.Flag, hrTokenInfo.Code, errorMessage);

    //                return;
    //            }
    //            else if (hrUserPermissionInfo.Result == null)
    //            {
    //                if (authorizeCallback != null)
    //                    authorizeCallback(false, hrTokenInfo.Code, StringResources.WinFormBridge_NotFoundTokenInfo);

    //                return;
    //            }


    //            HttpResult<UserInfo> hrUserInfo = HttpAccessor.UserInfo(hrTokenInfo.Result.UserId);
    //            if (hrUserInfo == null)
    //            {
    //                if (authorizeCallback != null)
    //                    authorizeCallback(false, 500, StringResources.WinFormBridge_BadData);
    //                return;
    //            }
    //            else if (!hrUserInfo.Flag)
    //            {
    //                string errorMessage = StringResources.WinFormBridge_SystemBusy;

    //                if (ErrorHandler.Exist(hrTokenInfo.Code))
    //                    errorMessage = ErrorHandler.Get(hrTokenInfo.Code);

    //                if (authorizeCallback != null)
    //                    authorizeCallback(hrTokenInfo.Flag, hrTokenInfo.Code, errorMessage);
    //                return;
    //            }
    //            else if (hrUserInfo.Result == null)
    //            {
    //                if (authorizeCallback != null)
    //                    authorizeCallback(false, hrTokenInfo.Code, StringResources.WinFormBridge_NotFoundUserInfo);
    //                return;
    //            }

    //            UserPermissionInfo userPermissionInfo = hrUserPermissionInfo.Result;

    //            List<PermissionInfo> permissionInfos = new List<PermissionInfo>();

    //            List<RolePermissionInfo> rolePermissionInfos = userPermissionInfo.RolePermissions;

    //            rolePermissionInfos.ForEach(rpi =>
    //            {
    //                permissionInfos = permissionInfos.Union(rpi.PermissionData, new PermissionInfoComparer()).ToList();
    //            });

    //            if (userPermissionInfo.RefuseData != null && userPermissionInfo.RefuseData.Count > 0)
    //            {
    //                foreach (RefuseInfo refuseInfo in userPermissionInfo.RefuseData)
    //                {
    //                    if (permissionInfos.Any(pi => pi.PermissionId == refuseInfo.RefusePermissionId))
    //                    {
    //                        permissionInfos.Remove(permissionInfos.First(pi => pi.PermissionId == refuseInfo.RefusePermissionId));
    //                    }
    //                }
    //            }

    //            List<CoreUserPermission> coreUserPermissions = new List<CoreUserPermission>();

    //            if (permissionInfos.Count > 0)
    //            {
    //                foreach (PermissionInfo permissionInfo in permissionInfos)
    //                {
    //                    CoreUserPermission coreUserPermission = new CoreUserPermission();

    //                    coreUserPermission.PermissionCode = permissionInfo.PermissionCode;
    //                    coreUserPermission.ParentId = permissionInfo.ParentId;
    //                    coreUserPermission.PermissionId = permissionInfo.PermissionId;
    //                    coreUserPermission.PermissionName = permissionInfo.PermissionName;

    //                    coreUserPermissions.Add(coreUserPermission);
    //                }
    //            }


    //            CoreUserInfo coreUserInfo = new CoreUserInfo();

    //            coreUserInfo.UserId = Convert.ToString(hrUserInfo.Result.UserId);
    //            coreUserInfo.UserName = hrUserInfo.Result.UserName;
    //            coreUserInfo.Token = hrTokenInfo.Result.Token;
    //            coreUserInfo.Pwd = hrUserInfo.Result.Pwd;
    //            coreUserInfo.permissions.AddRange(coreUserPermissions);

    //            string coreUserInfoValue = JsonConvert.SerializeObject(coreUserInfo);

    //            CDBHelper.Add("user_auth", coreUserInfoValue);

    //            string licenseNumberValues = CDBHelper.Get("license_number");

    //            if (!string.IsNullOrWhiteSpace(licenseNumberValues))
    //            {
    //                List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

    //                if (licensenumbers != null)
    //                {
    //                    LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

    //                    if (licenseNumber == null)
    //                    {
    //                        licensenumbers.Insert(0, new LicenseNumber()
    //                        {
    //                            Username = username,
    //                            Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
    //                            IsMemberkPassword = rememberPassword
    //                        });

    //                        if (licensenumbers.Count > 5)
    //                        {
    //                            licensenumbers.RemoveRange(5, licensenumbers.Count - 5);
    //                        }

    //                        CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
    //                    }
    //                    else
    //                    {
    //                        if (licensenumbers[0].Username != username)
    //                        {
    //                            licensenumbers.Remove(licenseNumber);
    //                            licensenumbers.Insert(0, licenseNumber);
    //                        }

    //                        licenseNumber.Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty;
    //                        licenseNumber.IsMemberkPassword = rememberPassword;

    //                        CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                List<LicenseNumber> licensenumbers = new List<LicenseNumber>()
    //                {
    //                    new LicenseNumber()
    //                    {
    //                        Username = username,
    //                        Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
    //                        IsMemberkPassword = rememberPassword
    //                    }
    //                };

    //                CDBHelper.Add("license_number", JsonConvert.SerializeObject(licensenumbers));
    //            }

    //            ChromiumSettings.FrmMainChromium.LoadUrl(ChromiumSettings.InitializeUrl);

    //        });
    //    }


    //    /// <summary>
    //    /// 获得用户信息
    //    /// </summary>
    //    /// <param name="username"></param>
    //    /// <returns></returns>
    //    public string GetLicenseNumberInfo(string username)
    //    {
    //        if (!string.IsNullOrWhiteSpace(username))
    //        {
    //            string licenseNumberValues = CDBHelper.Get("license_number");

    //            if (!string.IsNullOrWhiteSpace(licenseNumberValues))
    //            {
    //                List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);
    //                if (licensenumbers != null)
    //                {
    //                    LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

    //                    if (licenseNumber != null)
    //                    {
    //                        if (!string.IsNullOrWhiteSpace(licenseNumber.Password))
    //                            licenseNumber.Password = PasswordHelper.Encrypt(licenseNumber.Password);

    //                        return JsonConvert.SerializeObject(licenseNumber);
    //                    }
    //                }
    //            }
    //        }

    //        return string.Empty;
    //    }

    //    public void RemoveLicenseUsername(string username)
    //    {
    //        if (!string.IsNullOrWhiteSpace(username))
    //        {
    //            string licenseNumberValues = CDBHelper.Get("license_number");

    //            if (!string.IsNullOrWhiteSpace(licenseNumberValues))
    //            {
    //                List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);
    //                if (licensenumbers != null)
    //                {
    //                    LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

    //                    licensenumbers.Remove(licenseNumber);
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 获得所有的用户信息
    //    /// </summary>
    //    /// <param name="username"></param>
    //    /// <returns></returns>
    //    public string GetLicenseUsernames()
    //    {
    //        string licenseNumberValues = CDBHelper.Get("license_number");

    //        if (!string.IsNullOrWhiteSpace(licenseNumberValues))
    //        {
    //            try
    //            {
    //                string[] oldDatas = JsonConvert.DeserializeObject<string[]>(licenseNumberValues);

    //                if (oldDatas != null && oldDatas.Length > 0)
    //                {
    //                    List<LicenseNumber> oldLicenseNumbers = new List<LicenseNumber>();

    //                    foreach (string username in oldDatas)
    //                    {
    //                        oldLicenseNumbers.Add(new LicenseNumber()
    //                        {
    //                            Username = username
    //                        });
    //                    }

    //                    CDBHelper.Modify("license_number", JsonConvert.SerializeObject(oldLicenseNumbers));

    //                    licenseNumberValues = CDBHelper.Get("license_number");
    //                }
    //            }
    //            catch (Exception)
    //            {

    //            }

    //            List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

    //            if (licensenumbers != null)
    //            {
    //                string[] usernames = licensenumbers.Select(t => t.Username).ToArray();

    //                return JsonConvert.SerializeObject(usernames);
    //            }
    //        }

    //        return string.Empty;
    //    }


    //    /// <summary>
    //    /// 设置最小化
    //    /// </summary>
    //    public void WinFormMinimized()
    //    {
    //        this.chromiumWinForm.InvokeMinimized();
    //    }

    //    /// <summary>
    //    /// 设置最大化
    //    /// </summary>
    //    public void WinFormMaxmized()
    //    {
    //        this.chromiumWinForm.InvokeMaximized();
    //    }

    //    /// <summary>
    //    /// 关闭窗体
    //    /// </summary>
    //    public void WinFormClose()
    //    {
    //        this.chromiumWinForm.InvokeClose();
    //    }



    //    ///// <summary>
    //    ///// 解密密码
    //    ///// </summary>
    //    ///// <param name="strData"></param>
    //    ///// <returns></returns>
    //    //public string Encrypt(string strData)
    //    //{
    //    //    return PasswordHelper.Encrypt(strData);
    //    //}

    //    ///// <summary>
    //    ///// 加密密码
    //    ///// </summary>
    //    ///// <param name="strData"></param>
    //    ///// <returns></returns>
    //    //public string Decrypt(string strData)
    //    //{
    //    //    return PasswordHelper.Decrypt(strData);
    //    //}


    //    public void RedirectAuthorize()
    //    {
    //        ChromiumSettings.FrmMainChromium.LoadUrl(ChromiumSettings.AuthorizeUrl);
    //    }


    //    ///// <summary>
    //    ///// 设置开机重起
    //    ///// </summary>
    //    ///// <param name="start"></param>
    //    ///// <returns></returns>
    //    //public bool SetRestarting(bool start)
    //    //{
    //    //    if (start)
    //    //    {
    //    //        if (Environment.Is64BitOperatingSystem)
    //    //        {
    //    //            using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
    //    //            using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
    //    //            {
    //    //                string mainLaunch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShanDianAuto.exe");

    //    //                shanDianRegistry.SetValue("ShanDian", @"""" + mainLaunch + @"""");
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
    //    //            using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
    //    //            {
    //    //                string mainLaunch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShanDianAuto.exe");

    //    //                shanDianRegistry.SetValue("ShanDian", @"""" + mainLaunch + @"""");
    //    //            }
    //    //        }
    //    //    }
    //    //    else
    //    //    {

    //    //        if (Environment.Is64BitOperatingSystem)
    //    //        {
    //    //            using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
    //    //            using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
    //    //            {
    //    //                if (shanDianRegistry.GetValue("ShanDian") != null)
    //    //                {
    //    //                    shanDianRegistry.DeleteValue("ShanDian", true);
    //    //                }
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
    //    //            using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
    //    //            {
    //    //                if (shanDianRegistry.GetValue("ShanDian") != null)
    //    //                {
    //    //                    shanDianRegistry.DeleteValue("ShanDian", true);
    //    //                }
    //    //            }
    //    //        }
    //    //    }

    //    //    return this.getStartingup();
    //    //}



    //    ///// <summary>
    //    ///// 获得窗体的ID
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //public string getWindowId()
    //    //{
    //    //    return this.GetCurrent().Identity;
    //    //}


    //    /// <summary>
    //    /// 打开一个新窗体
    //    /// </summary>
    //    /// <param name="url"></param>
    //    /// <param name="width"></param>
    //    /// <param name="height"></param>
    //    /// <param name="left"></param>
    //    /// <param name="top"></param>
    //    /// <param name="javascriptCallback"></param>
    //    public void Open(string url, string title, int width, int height, double left, double top, Action<string> openCallback)
    //    {

    //        this.chromiumWinForm.InvokeMethod(new Action(() =>
    //        {
    //            FrmChromium frmChromium = ChromiumSettings.ChildrenChromiums.FirstOrDefault(cf => String.Compare(cf.Url, url, true) == 0);

    //            if (frmChromium == null)
    //            {
    //                frmChromium = new FrmChromium(url);
    //            }

    //            frmChromium.Show();

    //            if (openCallback != null)
    //                openCallback(frmChromium.Identity);

    //        }));
    //    }

    //    ///// <summary>
    //    ///// 调用软键盘
    //    ///// </summary>
    //    //public void openScreenKeyboard()
    //    //{
    //    //    this.GetCurrent().InvokeOpenScreenKeyboard();
    //    //}


    //    ///// <summary>
    //    ///// 输入按键的字符串
    //    ///// </summary>
    //    ///// <param name="value"></param>
    //    //public void keyboard(string value)
    //    //{
    //    //    //this.mBrowser.Dispatcher.Invoke(new Action(() =>
    //    //    //{
    //    //    if (!string.IsNullOrWhiteSpace(value))
    //    //    {
    //    //        string[] cmd = value.Split('+');
    //    //        if (cmd != null && cmd.Length > 0)
    //    //        {
    //    //            if (cmd.Length > 1)
    //    //            {
    //    //                VirtualKeyCode keyDownCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[0]);

    //    //                input.Keyboard
    //    //                    .KeyDown(keyDownCode)
    //    //                    .KeyPress((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[1]))
    //    //                    .KeyUp(keyDownCode);

    //    //            }
    //    //            else
    //    //            {
    //    //                input.Keyboard.KeyPress((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[0]));
    //    //            }
    //    //        }
    //    //    }
    //    //    //}),DispatcherPriority.Input);


    //    //}



    //    ///// <summary>
    //    ///// 关闭窗体
    //    ///// </summary>
    //    ///// <param name="windowid"></param>
    //    //public void closeById(string windowid)
    //    //{
    //    //    WindowBrowser closeBrowser = ChromiumSettings.GetChromiumByIdentity(windowid);

    //    //    if (closeBrowser != null)
    //    //        closeBrowser.InvokeClose();
    //    //}

    //    ///// <summary>
    //    ///// 退出
    //    ///// </summary>
    //    //public void quit()
    //    //{
    //    //    this.GetCurrent().InvokeQuit();
    //    //}


    //    ///// <summary>
    //    ///// 隐藏窗体
    //    ///// </summary>
    //    //public void hide()
    //    //{
    //    //    this.hide(null);
    //    //}

    //    ///// <summary>
    //    /////  隐藏窗体
    //    ///// </summary>
    //    ///// <param name="windowid"></param>
    //    //public void hide(string windowid)
    //    //{
    //    //    WindowBrowser hideBrowser = ChromiumSettings.GetChromiumByIdentity(windowid);

    //    //    if (hideBrowser != null)
    //    //        hideBrowser.Hide();
    //    //}

    //    ///// <summary>
    //    ///// 显示窗体
    //    ///// </summary>
    //    //public void show()
    //    //{
    //    //    this.show(null);
    //    //}

    //    ///// <summary>
    //    ///// 显示窗体
    //    ///// </summary>
    //    ///// <param name="windowid"></param>
    //    //public void show(string windowid)
    //    //{

    //    //    WindowBrowser showBrowser = ChromiumSettings.GetChromiumByIdentity(windowid);

    //    //    if (showBrowser != null)
    //    //        showBrowser.Show();
    //    //}

    //    ///// <summary>
    //    ///// 移动窗体
    //    ///// </summary>
    //    ///// <param name="width"></param>
    //    ///// <param name="height"></param>
    //    ///// <param name="left"></param>
    //    ///// <param name="top"></param>
    //    //public void move(int width, int height, double left, double top)
    //    //{
    //    //    this.GetCurrent().InvokeMove(width, height, left, top);
    //    //}

    //    ///// <summary>
    //    ///// 移动窗体
    //    ///// </summary>
    //    ///// <param name="windowid"></param>
    //    ///// <param name="width"></param>
    //    ///// <param name="height"></param>
    //    ///// <param name="left"></param>
    //    ///// <param name="top"></param>
    //    //public void move(string windowid, int width, int height, double left, double top)
    //    //{
    //    //    WindowBrowser dBrowser = ChromiumSettings.GetChromiumByIdentity(windowid);

    //    //    if (dBrowser != null)
    //    //        dBrowser.InvokeMove(width, height, left, top);
    //    //}

    //    /// <summary>
    //    /// 获得悬浮窗开关的值
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool GetFloatSwitch()
    //    {
    //        object floatSwitchValue = DBHelper.AcquireValue("FloatSwitch");

    //        if (floatSwitchValue == null)
    //            return false;

    //        return Convert.ToBoolean(floatSwitchValue);
    //    }

    //    /// <summary>
    //    /// 设置悬浮窗开关的值 
    //    /// </summary>
    //    /// <param name="state"></param>
    //    public void SetFloatSwitch(bool state)
    //    {
    //        ChromiumSettings.FrmMainChromium.SwitchFloatWindow(state, false);
    //    }

    //}


    /// <summary>
    /// 根浏览器交动的事件
    /// </summary>
    internal class WinFormBridge
    {
        private InputSimulator input = new InputSimulator();
        private IChromiumWinForm chromiumWinForm;

        public WinFormBridge(IChromiumWinForm chromiumForm)
        {
            this.chromiumWinForm = chromiumForm;
        }

        /// <summary>
        /// 是否开启动
        /// </summary>
        public bool GetRestarting()
        {
            bool launched = false;
            object startup;

            if (Environment.Is64BitOperatingSystem)
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
                {
                    startup = shanDianRegistry.GetValue("ClampExplorer");
                }
            }
            else
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
                {
                    startup = shanDianRegistry.GetValue("ClampExplorer");
                }
            }

            if (startup != null)
            {
                string startupPath = Convert.ToString(startup);

                if (!string.IsNullOrWhiteSpace(startupPath))
                {
                    launched = true;
                }
            }

            return launched;

        }

        /// <summary>
        /// 设置开机重起
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public bool SetRestarting(bool start)
        {
            if (start)
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                    using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        string mainLaunch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShanDianAuto.exe");

                        shanDianRegistry.SetValue("ClampExplorer", @"""" + mainLaunch + @"""");
                    }
                }
                else
                {
                    using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                    using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        string mainLaunch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShanDianAuto.exe");

                        shanDianRegistry.SetValue("ClampExplorer", @"""" + mainLaunch + @"""");
                    }
                }
            }
            else
            {

                if (Environment.Is64BitOperatingSystem)
                {
                    using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                    using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (shanDianRegistry.GetValue("ClampExplorer") != null)
                        {
                            shanDianRegistry.DeleteValue("ClampExplorer", true);
                        }
                    }
                }
                else
                {
                    using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                    using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (shanDianRegistry.GetValue("ClampExplorer") != null)
                        {
                            shanDianRegistry.DeleteValue("ClampExplorer", true);
                        }
                    }
                }
            }

            return this.GetRestarting();
        }

        public void Authorize(string username, string password, bool rememberPassword, Action<bool, int, string> authorizeCallback)
        {
            Task.Factory.StartNew(() =>
            {
                username = username.Trim();

                HttpResult<TokenInfo> hrTokenInfo = HttpAccessor.Token(username, password);

                if (hrTokenInfo == null)
                {
                    if (authorizeCallback != null)
                        authorizeCallback(false, 500, StringResources.WinFormBridge_BadData);
                    return;
                }
                else if (!hrTokenInfo.Flag)
                {
                    string errorMessage = StringResources.WinFormBridge_SystemBusy;

                    if (ErrorHandler.Exist(hrTokenInfo.Code))
                        errorMessage = ErrorHandler.Get(hrTokenInfo.Code);

                    if (authorizeCallback != null)
                        authorizeCallback(hrTokenInfo.Flag, hrTokenInfo.Code, errorMessage);
                    return;
                }
                else if (hrTokenInfo.Result == null)
                {
                    if (authorizeCallback != null)
                        authorizeCallback(false, hrTokenInfo.Code, StringResources.WinFormBridge_NotFoundTokenInfo);
                    return;
                }

                HttpResult<UserPermissionInfo> hrUserPermissionInfo = HttpAccessor.UserPermissions(hrTokenInfo.Result.UserId);

                if (hrUserPermissionInfo == null)
                {
                    if (authorizeCallback != null)
                        authorizeCallback(false, 500, StringResources.WinFormBridge_BadData);
                    return;
                }
                else if (!hrUserPermissionInfo.Flag)
                {
                    string errorMessage = StringResources.WinFormBridge_SystemBusy;

                    if (ErrorHandler.Exist(hrTokenInfo.Code))
                        errorMessage = ErrorHandler.Get(hrTokenInfo.Code);

                    if (authorizeCallback != null)
                        authorizeCallback(hrTokenInfo.Flag, hrTokenInfo.Code, errorMessage);

                    return;
                }
                else if (hrUserPermissionInfo.Result == null)
                {
                    if (authorizeCallback != null)
                        authorizeCallback(false, hrTokenInfo.Code, StringResources.WinFormBridge_NotFoundTokenInfo);

                    return;
                }


                HttpResult<UserInfo> hrUserInfo = HttpAccessor.UserInfo(hrTokenInfo.Result.UserId);
                if (hrUserInfo == null)
                {
                    if (authorizeCallback != null)
                        authorizeCallback(false, 500, StringResources.WinFormBridge_BadData);
                    return;
                }
                else if (!hrUserInfo.Flag)
                {
                    string errorMessage = StringResources.WinFormBridge_SystemBusy;

                    if (ErrorHandler.Exist(hrTokenInfo.Code))
                        errorMessage = ErrorHandler.Get(hrTokenInfo.Code);

                    if (authorizeCallback != null)
                        authorizeCallback(hrTokenInfo.Flag, hrTokenInfo.Code, errorMessage);
                    return;
                }
                else if (hrUserInfo.Result == null)
                {
                    if (authorizeCallback != null)
                        authorizeCallback(false, hrTokenInfo.Code, StringResources.WinFormBridge_NotFoundUserInfo);
                    return;
                }

                UserPermissionInfo userPermissionInfo = hrUserPermissionInfo.Result;

                List<PermissionInfo> permissionInfos = new List<PermissionInfo>();

                List<RolePermissionInfo> rolePermissionInfos = userPermissionInfo.RolePermissions;

                rolePermissionInfos.ForEach(rpi =>
                {
                    permissionInfos = permissionInfos.Union(rpi.PermissionData, new PermissionInfoComparer()).ToList();
                });

                if (userPermissionInfo.RefuseData != null && userPermissionInfo.RefuseData.Count > 0)
                {
                    foreach (RefuseInfo refuseInfo in userPermissionInfo.RefuseData)
                    {
                        if (permissionInfos.Any(pi => pi.PermissionId == refuseInfo.RefusePermissionId))
                        {
                            permissionInfos.Remove(permissionInfos.First(pi => pi.PermissionId == refuseInfo.RefusePermissionId));
                        }
                    }
                }

                List<CoreUserPermission> coreUserPermissions = new List<CoreUserPermission>();

                if (permissionInfos.Count > 0)
                {
                    foreach (PermissionInfo permissionInfo in permissionInfos)
                    {
                        CoreUserPermission coreUserPermission = new CoreUserPermission();

                        coreUserPermission.PermissionCode = permissionInfo.PermissionCode;
                        coreUserPermission.ParentId = permissionInfo.ParentId;
                        coreUserPermission.PermissionId = permissionInfo.PermissionId;
                        coreUserPermission.PermissionName = permissionInfo.PermissionName;

                        coreUserPermissions.Add(coreUserPermission);
                    }
                }


                CoreUserInfo coreUserInfo = new CoreUserInfo();

                coreUserInfo.UserId = Convert.ToString(hrUserInfo.Result.UserId);
                coreUserInfo.UserName = hrUserInfo.Result.UserName;
                coreUserInfo.Token = hrTokenInfo.Result.Token;
                coreUserInfo.Pwd = hrUserInfo.Result.Pwd;
                coreUserInfo.permissions.AddRange(coreUserPermissions);

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

                ChromiumSettings.FrmMainChromium.LoadUrl(ChromiumSettings.InitializeUrl);

            });
        }


        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetLicenseNumberInfo(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                string licenseNumberValues = CDBHelper.Get("license_number");

                if (!string.IsNullOrWhiteSpace(licenseNumberValues))
                {
                    List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);
                    if (licensenumbers != null)
                    {
                        LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

                        if (licenseNumber != null)
                        {
                            if (!string.IsNullOrWhiteSpace(licenseNumber.Password))
                                licenseNumber.Password = PasswordHelper.Encrypt(licenseNumber.Password);

                            return JsonConvert.SerializeObject(licenseNumber);
                        }
                    }
                }
            }

            return string.Empty;
        }

        public void RemoveLicenseUsername(string username)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                string licenseNumberValues = CDBHelper.Get("license_number");

                if (!string.IsNullOrWhiteSpace(licenseNumberValues))
                {
                    List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);
                    if (licensenumbers != null)
                    {
                        LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

                        licensenumbers.Remove(licenseNumber);
                    }
                }
            }
        }

        /// <summary>
        /// 获得所有的用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetLicenseUsernames()
        {
            string licenseNumberValues = CDBHelper.Get("license_number");

            if (!string.IsNullOrWhiteSpace(licenseNumberValues))
            {
                try
                {
                    string[] oldDatas = JsonConvert.DeserializeObject<string[]>(licenseNumberValues);

                    if (oldDatas != null && oldDatas.Length > 0)
                    {
                        List<LicenseNumber> oldLicenseNumbers = new List<LicenseNumber>();

                        foreach (string username in oldDatas)
                        {
                            oldLicenseNumbers.Add(new LicenseNumber()
                            {
                                Username = username
                            });
                        }

                        CDBHelper.Modify("license_number", JsonConvert.SerializeObject(oldLicenseNumbers));

                        licenseNumberValues = CDBHelper.Get("license_number");
                    }
                }
                catch (Exception)
                {

                }

                List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

                if (licensenumbers != null)
                {
                    string[] usernames = licensenumbers.Select(t => t.Username).ToArray();

                    return JsonConvert.SerializeObject(usernames);
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// 设置最小化
        /// </summary>
        public void WinFormMinimized()
        {
            this.chromiumWinForm.InvokeMinimized();
        }

        /// <summary>
        /// 设置最大化
        /// </summary>
        public void WinFormMaxmized()
        {
            this.chromiumWinForm.InvokeMaximized();
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        public void WinFormClose()
        {
            this.chromiumWinForm.InvokeClose();
        }


        public void RedirectAuthorize()
        {
            ChromiumSettings.FrmMainChromium.LoadUrl(ChromiumSettings.AuthorizeUrl);
        }






        ///// <summary>
        ///// 获得窗体的ID
        ///// </summary>
        ///// <returns></returns>
        //public string getWindowId()
        //{
        //    return this.GetCurrent().Identity;
        //}


        /// <summary>
        /// 打开一个新窗体
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="javascriptCallback"></param>
        public void Open(string url, string title, int width, int height, double left, double top, Action<string> openCallback)
        {

            this.chromiumWinForm.InvokeMethod(new Action(() =>
            {
                FrmChromium frmChromium = ChromiumSettings.ChildrenChromiums.FirstOrDefault(cf => String.Compare(cf.Url, url, true) == 0);

                if (frmChromium == null)
                {
                    frmChromium = new FrmSimpleChromium(url);
                }

                frmChromium.Show();

                if (openCallback != null)
                    openCallback(frmChromium.Identity);

            }));
        }

        /// <summary>
        /// 获得悬浮窗开关的值
        /// </summary>
        /// <returns></returns>
        public bool GetFloatSwitch()
        {
            object floatSwitchValue = DBHelper.AcquireValue("FloatSwitch");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置悬浮窗开关的值 
        /// </summary>
        /// <param name="state"></param>
        public void SetFloatSwitch(bool state)
        {
            ChromiumSettings.FrmMainChromium.SwitchFloatWindow(state, false);
        }
    }
}
