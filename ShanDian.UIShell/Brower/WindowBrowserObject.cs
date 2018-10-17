using CefSharp;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.Model;
using ShanDian.UIShell.Framework.Vo;
using ShanDian.UIShell;
using ShanDian.UIShell.Framework;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.Network;
using ShanDian.UIShell.Framework.Network.Service;
using ShanDian.UIShell.Properties;
using ShanDian.UIShell.Views;
using LiteDB;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WindowsInput;
using WindowsInput.Native;

namespace ShanDian.UIShell.Brower
{
    /// <summary>
    /// 根浏览器交动的事件
    /// </summary>
    public class WindowBrowserObject
    {
        private InputSimulator input = new InputSimulator();
        private IChromiumWindow chromiumWindow;

        /// <summary>
        /// 终端ID
        /// </summary>
        public string PCID { get { return SDShell.Demand.PCID; } }

        public string clientID { get; set; }

        /// <summary>
        /// 店门ID
        /// </summary>
        public string orgExtCode
        {
            get { return SDShell.Demand.MikeRestId; }
        }

        /// <summary>
        /// 终端版本号
        /// </summary>
        public string finalVersion
        {
            get
            {
                return ServerHelper.GetServerVersion();
            }
        }



        /// <summary>
        /// 屏幕宽度
        /// </summary>
        public double screenWidth { get { return System.Windows.SystemParameters.PrimaryScreenWidth; } }

        /// <summary>
        /// 屏幕高度
        /// </summary>
        public double screenHeight { get { return System.Windows.SystemParameters.PrimaryScreenHeight; } }

      


        public ScriptedMethodsBoundObject events { set; get; }

        public DataBaseBoundObject database { set; get; }

        public WindowBrowserObject(IChromiumWindow chromiumWindow)
        {
            this.chromiumWindow = chromiumWindow;
            this.events = new ScriptedMethodsBoundObject();
            this.database = new DataBaseBoundObject();
        }

        /// <summary>
        /// 获得当前的对就应的WINDOW
        /// </summary>
        /// <returns></returns>
        private IChromiumWindow GetCurrent()
        {
            return chromiumWindow;
        }
        /// <summary>
        /// 判断是否为新店
        /// </summary>
        /// <returns></returns>
        public bool getIsNewRestId()
        {
            return true;
        }
        /// <summary>
        /// 发出声音
        /// </summary>
        /// <param name="name"></param>
        public void playSound(string name)
        {
            Task.Factory.StartNew(() =>
            {
                string[] mediaFiles = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media"), name + ".*", SearchOption.TopDirectoryOnly);

                if (mediaFiles != null && mediaFiles.Length > 0)
                {
                    using (SoundPlayer player = new SoundPlayer(mediaFiles[0]))
                    {
                        player.PlaySync();
                    }
                }
                else
                {
                    DebugHelper.WriteLine("没有找到{0}文件", name);
                }
            });
        }



        #region 开机启动业务

        /// <summary>
        /// 是否开启动
        /// </summary>
        public bool getStartingup()
        {
            bool launched = false;
            object startup;

            if (Environment.Is64BitOperatingSystem)
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
                {
                    startup = shanDianRegistry.GetValue("ShanDian");
                }
            }
            else
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"))
                {
                    startup = shanDianRegistry.GetValue("ShanDian");
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
        public bool setStartingup(bool start)
        {
            if (start)
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                    using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        string mainLaunch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShanDianAuto.exe");

                        shanDianRegistry.SetValue("ShanDian", @"""" + mainLaunch + @"""");
                    }
                }
                else
                {
                    using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                    using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        string mainLaunch = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShanDianAuto.exe");

                        shanDianRegistry.SetValue("ShanDian", @"""" + mainLaunch + @"""");
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
                        if (shanDianRegistry.GetValue("ShanDian") != null)
                        {
                            shanDianRegistry.DeleteValue("ShanDian", true);
                        }
                    }
                }
                else
                {
                    using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                    using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (shanDianRegistry.GetValue("ShanDian") != null)
                        {
                            shanDianRegistry.DeleteValue("ShanDian", true);
                        }
                    }
                }
            }

            return this.getStartingup();
        }
        #endregion

        #region 登录页面的业务

        ///// <summary>
        ///// 用户登录
        ///// </summary>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <param name="rememberPassword"></param>
        ///// <param name="javascriptCallback"></param>
        //public void authorize(string username, string password, bool rememberPassword, IJavascriptCallback javascriptCallback)
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        username = username.Trim();

        //        if (UIS.Demand.RunMode == "sub")
        //        {
        //            ServiceResult<InitConfigInfo> srInitConfigInfo = ServiceAccessor.GetInitConfig();

        //            if (srInitConfigInfo == null)
        //            {
        //                if (javascriptCallback != null)
        //                    javascriptCallback.ExecuteAsync(false, 500, SDResources.HttpAccessor_BadData);
        //                return;
        //            }
        //            else if (!srInitConfigInfo.Flag)
        //            {
        //                DebugHelper.WriteLine("获得不到主机的店信息");

        //                if (javascriptCallback != null)
        //                    javascriptCallback.ExecuteAsync(false, 500, SDResources.HttpAccessor_BadData);
        //                return;
        //            }
        //            else if (srInitConfigInfo.Result == null)
        //            {
        //                DebugHelper.WriteLine("获得不到主机的店信息");

        //                if (javascriptCallback != null)
        //                    javascriptCallback.ExecuteAsync(false, 500, SDResources.HttpAccessor_BadData);
        //                return;
        //            }

        //            InitConfigInfo initConfigInfo = srInitConfigInfo.Result;

        //            if (string.IsNullOrWhiteSpace(initConfigInfo.AppId) || string.IsNullOrWhiteSpace(initConfigInfo.OrgExtCode))
        //            {
        //                DebugHelper.WriteLine("获得不到主机的店信息");

        //                if (javascriptCallback != null)
        //                    javascriptCallback.ExecuteAsync(false, 500, SDResources.HttpAccessor_BadData);
        //                return;
        //            }

        //            UIS.Demand.AppId = initConfigInfo.AppId;
        //            UIS.Demand.SecureKey = initConfigInfo.SecureKey;
        //            UIS.IsNewRestId = initConfigInfo.IsNewRestId;
        //            UIS.Demand.MikeRestId = initConfigInfo.OrgExtCode;
        //        }


        //        ServiceResult<UserInfo> hrUserInfo = ServiceAccessor.Authorized(username, password);

        //        if (hrUserInfo == null)
        //        {
        //            if (javascriptCallback != null)
        //                javascriptCallback.ExecuteAsync(false, 500, SDResources.HttpAccessor_BadData);
        //            return;
        //        }
        //        else if (!hrUserInfo.Flag)
        //        {
        //            string errorMessage = SDResources.HttpAccessor_SystemBusy;

        //            if (ErrorHandler.Exist(hrUserInfo.Code))
        //                errorMessage = ErrorHandler.Get(hrUserInfo.Code);

        //            if (javascriptCallback != null)
        //                javascriptCallback.ExecuteAsync(hrUserInfo.Flag, hrUserInfo.Code, errorMessage);
        //            return;
        //        }
        //        else if (hrUserInfo.Result == null)
        //        {
        //            if (javascriptCallback != null)
        //                javascriptCallback.ExecuteAsync(false, hrUserInfo.Code, SDResources.HttpAccessor_NotFoundTokenInfo);
        //            return;
        //        }

        //        UserInfo userInfo = hrUserInfo.Result;

        //        List<CoreUserPermission> coreUserPermissions = new List<CoreUserPermission>();

        //        if (userInfo.Permissions != null && userInfo.Permissions.Count > 0)
        //        {
        //            foreach (PermissionInfo permissionInfo in userInfo.Permissions)
        //            {
        //                CoreUserPermission coreUserPermission = new CoreUserPermission();

        //                coreUserPermission.Code = permissionInfo.Code;
        //                coreUserPermission.Icon = permissionInfo.Icon;
        //                coreUserPermission.Name = permissionInfo.Name;
        //                coreUserPermission.CategoryCode = permissionInfo.CategoryCode;
        //                coreUserPermission.IsInner = permissionInfo.IsInner;
        //                coreUserPermission.Sort = permissionInfo.Sort;
        //                coreUserPermission.Token = permissionInfo.Token;
        //                coreUserPermission.Url = permissionInfo.Url;
        //                coreUserPermission.Icon = permissionInfo.Icon;
        //                coreUserPermission.KindCode = permissionInfo.KindCode;

        //                coreUserPermissions.Add(coreUserPermission);
        //            }
        //        }


        //        CoreUserInfo coreUserInfo = new CoreUserInfo();

        //        coreUserInfo.UserId = userInfo.UserId;
        //        coreUserInfo.UserName = userInfo.UserName;
        //        coreUserInfo.Token = userInfo.Token;
        //        coreUserInfo.Pwd = userInfo.Pwd;
        //        coreUserInfo.Status = userInfo.Status;
        //        coreUserInfo.UserName = userInfo.UserName;
        //        coreUserInfo.RoleName = userInfo.RoleName;
        //        coreUserInfo.Sex = userInfo.Sex;
        //        coreUserInfo.IsAdmin = userInfo.IsAdmin;
        //        coreUserInfo.Mobile = userInfo.Mobile;
        //        coreUserInfo.IsFirst = userInfo.IsFirstLogin;

        //        coreUserInfo.Permissions.AddRange(coreUserPermissions);

        //        string coreUserInfoValue = JsonConvert.SerializeObject(coreUserInfo);

        //        CDBHelper.Add("user_auth", coreUserInfoValue);

        //        string licenseNumberValues = CDBHelper.Get("license_number");

        //        if (!string.IsNullOrWhiteSpace(licenseNumberValues))
        //        {
        //            List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

        //            if (licensenumbers != null)
        //            {
        //                LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

        //                if (licenseNumber == null)
        //                {
        //                    licensenumbers.Insert(0, new LicenseNumber()
        //                    {
        //                        Username = username,
        //                        Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
        //                        IsMemberkPassword = rememberPassword
        //                    });

        //                    if (licensenumbers.Count > 5)
        //                    {
        //                        licensenumbers.RemoveRange(5, licensenumbers.Count - 5);
        //                    }

        //                    CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
        //                }
        //                else
        //                {
        //                    if (licensenumbers[0].Username != username)
        //                    {
        //                        licensenumbers.Remove(licenseNumber);
        //                        licensenumbers.Insert(0, licenseNumber);
        //                    }

        //                    licenseNumber.Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty;
        //                    licenseNumber.IsMemberkPassword = rememberPassword;

        //                    CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
        //                }
        //            }
        //        }
        //        else
        //        {
        //            List<LicenseNumber> licensenumbers = new List<LicenseNumber>()
        //            {
        //                new LicenseNumber()
        //                {
        //                    Username = username,
        //                    Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
        //                    IsMemberkPassword = rememberPassword
        //                }
        //            };

        //            CDBHelper.Add("license_number", JsonConvert.SerializeObject(licensenumbers));
        //        }

        //        //ChromiumSettings.ChromiumWindow.Redirect(ChromiumSettings.InitializeUrl, (w, v) =>
        //        // {
        //        //     if (javascriptCallback != null && javascriptCallback.CanExecute)
        //        //         javascriptCallback.ExecuteAsync(true, 200, "");


        //        //     Task.Factory.StartNew(() =>
        //        //     {
        //        //         try
        //        //         {
        //        //             Logistics.UploadSoftware();
        //        //         }
        //        //         catch (Exception ex)
        //        //         {
        //        //             DebugHelper.WriteException(ex);
        //        //         }

        //        //     });

        //        //     Task.Factory.StartNew(() =>
        //        //     {
        //        //         try
        //        //         {
        //        //             Logistics.LoginNoticeUpdate();
        //        //         }
        //        //         catch (Exception ex)
        //        //         {
        //        //             DebugHelper.WriteException(ex);
        //        //         }

        //        //     });

        //        // });

        //    });
        //}

        ///// <summary>
        ///// 获得用户信息
        ///// </summary>
        ///// <param name="username"></param>
        ///// <returns></returns>
        //public string getLicenseNumberInfo(string username)
        //{
        //    if (!string.IsNullOrWhiteSpace(username))
        //    {
        //        string licenseNumberValues = CDBHelper.Get("license_number");

        //        if (!string.IsNullOrWhiteSpace(licenseNumberValues))
        //        {
        //            List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);
        //            if (licensenumbers != null)
        //            {
        //                LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

        //                if (licenseNumber != null)
        //                {
        //                    if (!string.IsNullOrWhiteSpace(licenseNumber.Password))
        //                        licenseNumber.Password = PasswordHelper.Encrypt(licenseNumber.Password);

        //                    return JsonConvert.SerializeObject(licenseNumber);
        //                }
        //            }
        //        }
        //    }

        //    return string.Empty;
        //}
        ///// <summary>
        ///// 移除用户记录
        ///// </summary>
        ///// <param name="username"></param>
        //public void removeLicenseUsername(string username)
        //{
        //    if (!string.IsNullOrWhiteSpace(username))
        //    {
        //        string licenseNumberValues = CDBHelper.Get("license_number");

        //        if (!string.IsNullOrWhiteSpace(licenseNumberValues))
        //        {
        //            List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);
        //            if (licensenumbers != null)
        //            {
        //                LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

        //                licensenumbers.Remove(licenseNumber);
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获得所有的用户信息
        ///// </summary>
        ///// <param name="username"></param>
        ///// <returns></returns>
        //public string getLicenseUsernames()
        //{
        //    string licenseNumberValues = CDBHelper.Get("license_number");

        //    if (!string.IsNullOrWhiteSpace(licenseNumberValues))
        //    {
        //        try
        //        {
        //            string[] oldDatas = JsonConvert.DeserializeObject<string[]>(licenseNumberValues);

        //            if (oldDatas != null && oldDatas.Length > 0)
        //            {
        //                List<LicenseNumber> oldLicenseNumbers = new List<LicenseNumber>();

        //                foreach (string username in oldDatas)
        //                {
        //                    oldLicenseNumbers.Add(new LicenseNumber()
        //                    {
        //                        Username = username
        //                    });
        //                }

        //                CDBHelper.Modify("license_number", JsonConvert.SerializeObject(oldLicenseNumbers));

        //                licenseNumberValues = CDBHelper.Get("license_number");
        //            }
        //        }
        //        catch (Exception)
        //        {

        //        }

        //        List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

        //        if (licensenumbers != null)
        //        {
        //            string[] usernames = licensenumbers.Select(t => t.Username).ToArray();

        //            return JsonConvert.SerializeObject(usernames);
        //        }
        //    }

        //    return string.Empty;
        //}

        #endregion

        #region 快捷键
        /// <summary>
        /// 获得是否开起快捷键
        /// </summary>
        /// <returns></returns>
        public bool getShortcutsEnabled()
        {
            return HotkeyHelper.GetShortcutsEnabled();
        }

        /// <summary>
        /// 设置是否开起快捷键
        /// </summary>
        /// <param name="enabled"></param>
        public void setShortcutsEnabled(bool enabled)
        {
            HotkeyHelper.SetShortcutsEnabled(enabled);
        }

        /// <summary>
        /// 获得设置快捷键的值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string getShortcutsText(string keyName)
        {
            object objKeyName = HotkeyHelper.AcquireValue(keyName);

            if (objKeyName != null)
            {
                if (!HotkeyHelper.HaveConflict(keyName))
                {
                    return JsonConvert.SerializeObject(new { isSuccess = true, hotkeyValue = Convert.ToString(objKeyName), error = "" });
                }
                else
                {
                    HotkeyRegisterResult hrr = null;

                    SDShell.ChromiumWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        hrr = SDShell.ChromiumWindow.RegisterHotkey(keyName, Convert.ToString(objKeyName));
                    }));

                    if (hrr != null && hrr.IsSuccess)
                        return JsonConvert.SerializeObject(new { isSuccess = true, hotkeyValue = Convert.ToString(objKeyName), error = "" });

                    return JsonConvert.SerializeObject(new { isSuccess = false, hotkeyValue = Convert.ToString(objKeyName), error = SDResources.Hotkey_AlreadyExist });
                }
            }

            //if (objKeyName != null)
            //{
            //    return Convert.ToString(objKeyName);
            //}

            return string.Empty;
        }

        /// <summary>
        /// 设置快捷键的值 
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        public string setShortcutsText(string keyName, string value)
        {
            string error = null;

            SDShell.ChromiumWindow.Dispatcher.Invoke(new Action(() =>
            {
                HotkeyRegisterResult hrr = SDShell.ChromiumWindow.RegisterHotkey(keyName, value);

                if (hrr != null && !hrr.IsSuccess)
                {
                    error = hrr.ErrorMessage;
                }
            }));

            return error;
        }

        /// <summary>
        /// 恢复快捷键的值
        /// </summary>
        public void recoverShortcuts()
        {
            foreach (string key in HotkeyHelper.DefaultHotkeys.Keys)
            {
                SDShell.ChromiumWindow.Dispatcher.Invoke(new Action(() =>
                {
                    SDShell.ChromiumWindow.RegisterHotkey(key, HotkeyHelper.DefaultHotkeys[key]);
                }));
            }
        }


        #endregion


        public void pageCompleted()
        {
            SDShell.ChromiumWindow.ValidateAdvicesMessage();
        }

        /// <summary>
        /// 打开匹配进程
        /// </summary>
        /// <param name="path"></param>
        //public void openMatchingProcess(string path)
        //{
        //    try
        //    {
        //        Process process = new Process();


        //        ProcessStartInfo psi = new ProcessStartInfo(path);

        //        psi.UseShellExecute = false;
        //        psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

        //        process.StartInfo = psi;
        //        process.Start();

        //        process.WaitForInputIdle(1000);
        //    }
        //    catch (Exception ex)
        //    {
        //        DebugHelper.WriteException(ex);
        //    }
        //}



        /// <summary>
        /// 打开匹配进程
        /// </summary>
        /// <param name="path"></param>
        public void openMatchingProject(string path, string project, string restPluginCollect)
        {
            try
            {
                DebugHelper.WriteLine("path:{0}", path);

                RegistryHelper.DeployRestPluginCollectShortcutAndregedit(path, project, restPluginCollect);

                Process process = new Process();

                ProcessStartInfo psi = new ProcessStartInfo(path);

                psi.UseShellExecute = false;
                psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

                process.StartInfo = psi;
                process.Start();

                process.WaitForInputIdle(1000);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
        }


        /// <summary>
        /// 验证是否存在快捷键冲突
        /// </summary>
        public void validateConflictHotkeys()
        {
            SDShell.ChromiumWindow.ValidateConflictHotkeys();
        }


        ///// <summary>
        ///// 解密密码
        ///// </summary>
        ///// <param name="strData"></param>
        ///// <returns></returns>
        //public string Encrypt(string strData)
        //{
        //    return PasswordHelper.Encrypt(strData);
        //}

        ///// <summary>
        ///// 加密密码
        ///// </summary>
        ///// <param name="strData"></param>
        ///// <returns></returns>
        //public string Decrypt(string strData)
        //{
        //    return PasswordHelper.Decrypt(strData);
        //}



        /// <summary>
        /// 获得Sign
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public string getSign(string userId, string userName, string phone, string currentTime)
        {
            return Logistics.GetSign(userId, userName, phone, currentTime);
        }

        /// <summary>
        /// 重定向登录页
        /// </summary>
        public void redirectAuthorize()
        {
            SDShell.RedirectAuthorize();
        }


        public bool isFirstRendered()
        {
            RegistryKey localKey32;
            RegistryKey shanDianRegistry;

            if (Environment.Is64BitOperatingSystem)
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }
            else
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }

            if (shanDianRegistry == null)
            {
                shanDianRegistry = localKey32.CreateSubKey(@"SOFTWARE\ShanDian");

                shanDianRegistry.SetValue("Rendered", "true");

                return true;
            }
            else
            {
                object rendered = shanDianRegistry.GetValue("Rendered");

                if (rendered == null)
                {
                    shanDianRegistry.SetValue("Rendered", "true");
                    return true;
                }
            }

            if (localKey32 != null)
                localKey32.Dispose();

            if (shanDianRegistry != null)
                shanDianRegistry.Dispose();

            return false;
        }





        /// <summary>
        /// 获得窗体的ID
        /// </summary>
        /// <returns></returns>
        public string getWindowId()
        {
            return this.GetCurrent().Identity;
        }


        /// <summary>
        /// 打开一个新窗体
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="javascriptCallback"></param>
        public void open(string url, string title, int width, int height, double left, double top, IJavascriptCallback javascriptCallback)
        {
            SDShell.CreateChildrenChromium(url, title, width, height, left, top, new Action<string>(s =>
            {
                if (javascriptCallback != null)
                    javascriptCallback.ExecuteAsync(s);
            }));
        }

        /// <summary>
        /// 调用软键盘
        /// </summary>
        public void openScreenKeyboard()
        {
            this.GetCurrent().InvokeOpenScreenKeyboard();
        }


        /// <summary>
        /// 输入按键的字符串
        /// </summary>
        /// <param name="value"></param>
        public void keyboard(string value)
        {
            //this.mBrowser.Dispatcher.Invoke(new Action(() =>
            //{
            if (!string.IsNullOrWhiteSpace(value))
            {
                string[] cmd = value.Split('+');
                if (cmd != null && cmd.Length > 0)
                {
                    if (cmd.Length > 1)
                    {
                        VirtualKeyCode keyDownCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[0]);

                        input.Keyboard
                            .KeyDown(keyDownCode)
                            .KeyPress((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[1]))
                            .KeyUp(keyDownCode);

                    }
                    else
                    {
                        input.Keyboard.KeyPress((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), cmd[0]));
                    }
                }
            }
            //}),DispatcherPriority.Input);


        }

        ///// <summary>
        ///// 关闭窗体
        ///// </summary>
        public void close()
        {
            this.GetCurrent().InvokeClose();
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="windowid"></param>
        public void closeById(string windowid)
        {
            WindowSimpleChromium closeBrowser = SDShell.GetChromiumByIdentity(windowid);

            if (closeBrowser != null)
                closeBrowser.InvokeClose();
        }

        /// <summary>
        /// 退出
        /// </summary>
        public void quit()
        {
            this.GetCurrent().InvokeQuit();
        }


        /// <summary>
        /// 隐藏窗体
        /// </summary>
        public void hide()
        {
            this.hide(null);
        }

        /// <summary>
        ///  隐藏窗体
        /// </summary>
        /// <param name="windowid"></param>
        public void hide(string windowid)
        {
            WindowSimpleChromium hideBrowser = SDShell.GetChromiumByIdentity(windowid);

            if (hideBrowser != null)
                hideBrowser.Hide();
        }

        /// <summary>
        /// 显示窗体
        /// </summary>
        public void show()
        {
            this.show(null);
        }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="windowid"></param>
        public void show(string windowid)
        {

            WindowSimpleChromium showBrowser = SDShell.GetChromiumByIdentity(windowid);

            if (showBrowser != null)
                showBrowser.Show();
        }

        /// <summary>
        /// 设置最小化
        /// </summary>
        public void setWindowMinimize()
        {
            this.GetCurrent().InvokeMinimized();
        }

        /// <summary>
        /// 设置最大化
        /// </summary>
        public void setWindowMaxmize()
        {
            this.GetCurrent().InvokeMaximized();
        }
        /// <summary>
        /// 移动窗体
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void move(int width, int height, double left, double top)
        {
            this.GetCurrent().InvokeMove(width, height, left, top);
        }

        /// <summary>
        /// 移动窗体
        /// </summary>
        /// <param name="windowid"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void move(string windowid, int width, int height, double left, double top)
        {
            WindowSimpleChromium dBrowser = SDShell.GetChromiumByIdentity(windowid);

            if (dBrowser != null)
                dBrowser.InvokeMove(width, height, left, top);
        }

        /// <summary>
        /// 更新门店信息
        /// </summary>
        public void updateSoftwareInfo()
        {
            Task.Factory.StartNew(() =>
            {
                Logistics.UploadSoftware();
            });

        }
        #region 悬浮窗开关

        /// <summary>
        /// 获得悬浮窗开关的值
        /// </summary>
        /// <returns></returns>
        public bool getFloatSwitch()
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
        public void setFloatSwitch(bool state)
        {
            SDShell.ChromiumWindow.SwitchFloatWindow(state, false);
        }

        #endregion

        #region 消息弹框的接口


        /// <summary>
        /// 获得启用消息订阅的状态
        /// </summary>
        /// <returns></returns>
        public bool getNoticeSwitch()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeSwitch");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置启用消息订阅的状态
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeSwitch(bool state)
        {
            DBHelper.Store("NoticeSwitch", state);
        }

        /// <summary>
        /// 获得下单消息的状态
        /// </summary>
        /// <returns></returns>
        public bool getNoticeOrder()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeOrder");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置下单消息的状态
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeOrder(bool state)
        {
            DBHelper.Store("NoticeOrder", state);
        }

        /// <summary>
        /// 获得支付消息的状态
        /// </summary>
        /// <returns></returns>
        public bool getNoticePayment()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticePayment");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置支付消息的状态
        /// </summary>
        /// <param name="state"></param>
        public void setNoticePayment(bool state)
        {
            DBHelper.Store("NoticePayment", state);
        }

        /// <summary>
        /// 获得扫码提醒的状态
        /// </summary>
        /// <returns></returns>
        public bool getNoticeScanCodeHint()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeScanCodeHint");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置扫码提醒的状态
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeScanCodeHint(bool state)
        {
            DBHelper.Store("NoticeScanCodeHint", state);
        }

        /// <summary>
        /// 获得服务提醒的状态
        /// </summary>
        /// <returns></returns>
        public bool getNoticeServiceHint()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeServiceHint");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置服务提醒的状态
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeServiceHint(bool state)
        {
            DBHelper.Store("NoticeServiceHint", state);
        }

        /// <summary>
        /// 获得外卖消息的状态
        /// </summary>
        /// <returns></returns>
        public bool getNoticeTakeAway()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeTakeAway");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置外卖消息的状态
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeTakeAway(bool state)
        {
            DBHelper.Store("NoticeTakeAway", state);
        }

        /// <summary>
        ///获得提示音总开关
        /// </summary>
        /// <returns></returns>
        public bool getNoticeSoundSwitch()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeSoundSwitch");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置提示音总开关
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeSoundSwitch(bool state)
        {
            DBHelper.Store("NoticeSoundSwitch", state);
        }

        /// <summary>
        /// 获得下单的提示音
        /// </summary>
        /// <returns></returns>
        public bool getNoticeSoundOrder()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeSoundOrder");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置下单的提示音
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeSoundOrder(bool state)
        {
            DBHelper.Store("NoticeSoundOrder", state);
        }

        /// <summary>
        /// 获得支付的提示音
        /// </summary>
        /// <returns></returns>
        public bool getNoticeSoundPayment()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeSoundPayment");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置支付的提示音
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeSoundPayment(bool state)
        {
            DBHelper.Store("NoticeSoundPayment", state);
        }

        /// <summary>
        /// 根据编号获得提示音的文件路径
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public string getNoticeSound(string serialNumber)
        {
            string soundFileName = null;

            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                string serialCode = serialNumber.ToUpper();

                if (serialCode.StartsWith("MSG_SO"))
                {
                    if (serialCode != "MSG_SO_0005")
                    {
                        soundFileName = this.getSoundFileName("order-tohes");
                    }
                    else
                    {
                        soundFileName = this.getSoundFileName("order-failure-tohes");
                    }
                }
                else if (serialCode.StartsWith("MSG_FI"))
                {

                    if (serialCode != "MSG_FI_0003")
                    {
                        soundFileName = this.getSoundFileName("proceeds-tohes");
                    }
                }
                else
                {
                    soundFileName = this.getSoundFileName("unify");
                }
            }

            return soundFileName;
        }

        public string getSoundFileName(string name)
        {
            string[] mediaFiles = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media"), name + ".*", SearchOption.TopDirectoryOnly);

            if (mediaFiles != null && mediaFiles.Length > 0)
                return mediaFiles[0];
            return null;
        }
        #endregion

        #region 消息中心的处理
        /// <summary>
        /// 获得所有的消息
        /// </summary>
        /// <returns></returns>
        public string getNoticesAll()
        {
            List<NoticeVo> tdNoticeVos = new List<NoticeVo>();
            List<NoticeVo> ydNoticeVos = new List<NoticeVo>();

            string advicesPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Advices");

            if (!Directory.Exists(advicesPath))
            {
                Directory.CreateDirectory(advicesPath);
            }

            using (var db = new LiteDB.LiteDatabase(System.IO.Path.Combine(advicesPath, NotcieHelper.GetFileName())))
            {
                LiteCollection<Notice> noticeCollection = db.GetCollection<Notice>();

                List<Notice> tdNotices = noticeCollection.FindAll().ToList();

                foreach (Notice notice in tdNotices)
                {
                    NoticeVo noticeVo = new NoticeVo();

                    noticeVo.Content = notice.Content;
                    noticeVo.NoticeCategory = (int)notice.Category;
                    noticeVo.CreateDate = notice.CreateDate.ToString("HH:mm");
                    noticeVo.Title = notice.Title;
                    noticeVo.ShutCount = notice.ShutCount;
                    noticeVo.SerialNumber = notice.SerialNumber;
                    noticeVo.IconName = notice.IconName;

                    tdNoticeVos.Add(noticeVo);

                    notice.IsDisplay = true;
                }

                noticeCollection.Update(tdNotices);
            }

            string prevFileName = System.IO.Path.Combine(advicesPath, NotcieHelper.GetPrevFileName());

            if (File.Exists(prevFileName))
            {
                using (var db = new LiteDB.LiteDatabase(System.IO.Path.Combine(advicesPath, NotcieHelper.GetPrevFileName())))
                {
                    LiteCollection<Notice> noticeCollection = db.GetCollection<Notice>();

                    List<Notice> ydNotices = noticeCollection.FindAll().ToList();

                    foreach (Notice notice in ydNotices)
                    {
                        NoticeVo noticeVo = new NoticeVo();

                        noticeVo.Content = notice.Content;
                        noticeVo.NoticeCategory = (int)notice.Category;
                        noticeVo.CreateDate = notice.CreateDate.ToString("HH:mm");
                        noticeVo.Title = notice.Title;
                        noticeVo.ShutCount = notice.ShutCount;
                        noticeVo.SerialNumber = notice.SerialNumber;
                        noticeVo.IconName = notice.IconName;

                        ydNoticeVos.Add(noticeVo);

                        notice.IsDisplay = true;
                    }

                }

            }

            return JsonConvert.SerializeObject(new { today = tdNoticeVos, yesterday = ydNoticeVos });
        }

        /// <summary>
        /// 获得是否有未读取的消息
        /// </summary>
        /// <returns></returns>
        public bool getHaveUnDisplayNotice()
        {
            bool haveUnDisplayNotice = false;
            string advicesPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "Advices");

            if (!Directory.Exists(advicesPath))
            {
                Directory.CreateDirectory(advicesPath);
            }

            using (var db = new LiteDB.LiteDatabase(System.IO.Path.Combine(advicesPath, NotcieHelper.GetFileName())))
            {
                LiteCollection<Notice> noticeCollection = db.GetCollection<Notice>();

                if (noticeCollection.Count(n => n.IsDisplay == false) > 0)
                    haveUnDisplayNotice = true;
                else
                    haveUnDisplayNotice = false;
            }

            return haveUnDisplayNotice;
        }


        #endregion

        #region 设置服务区


        /// <summary>
        /// 保存选中的餐桌
        /// </summary>
        /// <param name="value"></param>
        public void saveSelectTables(string value)
        {
            DBHelper.Store("NoticeSelectTables", value);
        }

        /// <summary>
        /// 获得选中的餐桌
        /// </summary>
        /// <returns></returns>
        public string getSelectTables()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeSelectTables");

            if (floatSwitchValue == null)
                return null;

            return Convert.ToString(floatSwitchValue);
        }


        /// <summary>
        ///获得设置服务区总开关
        /// </summary>
        /// <returns></returns>
        public bool getNoticeTableSwitch()
        {
            object floatSwitchValue = DBHelper.AcquireValue("NoticeTableSwitch");

            if (floatSwitchValue == null)
                return false;

            return Convert.ToBoolean(floatSwitchValue);
        }

        /// <summary>
        /// 设置设置服务区总开关
        /// </summary>
        /// <param name="state"></param>
        public void setNoticeTableSwitch(bool state)
        {
            DBHelper.Store("NoticeTableSwitch", state);
        }

        #endregion
    }
}



