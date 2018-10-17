using ShanDian.UIShell.Properties;
using ShanDian.UIShell.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShanDian.UIShell.Framework.Shortcut
{
    /// <summary>
    /// 热键绑定类
    /// </summary>
    public class HotkeyBinder : IDisposable
    {
        private readonly HotkeyContainer container = new HotkeyContainer();
        private readonly HotkeyWindow hotkeyWindow = new HotkeyWindow();

        public event Action<Hotkey, HotkeyAlreadyBoundException> OnHotkeyAlreadyRegistered;

        public HotkeyBinder()
        {
            this.hotkeyWindow.HotkeyPressed += OnHotkeyPressed;
        }

        /// <summary>
        /// 确定是否绑定过热键
        /// </summary>
        /// <param name="hotkeyCombo"></param>
        /// <returns></returns>
        public bool IsHotkeyAlreadyBound(Hotkey hotkeyCombo)
        {
            bool successful = User32Dll.RegisterHotKey(this.hotkeyWindow.Handle, hotkeyCombo.GetHashCode(), (uint)hotkeyCombo.Modifier, (uint)hotkeyCombo.Key);

            if (!successful)
                return true;

            User32Dll.UnregisterHotKey(this.hotkeyWindow.Handle, hotkeyCombo.GetHashCode());

            return false;
        }


        /// <summary>
        /// 绑定热键
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public HotkeyCallback Bind(string keyName, Modifiers modifiers, Keys keys)
        {
            return Bind(keyName, new Hotkey(modifiers, keys));
        }

        /// <summary>
        /// 绑定热键
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public HotkeyCallback Bind(Modifiers modifiers, Keys keys)
        {
            return Bind(null, new Hotkey(modifiers, keys));
        }

        /// <summary>
        /// 绑定热键
        /// </summary>
        /// <param name="hotkeyCombo"></param>
        /// <returns></returns>
        public HotkeyCallback Bind(string keyName, Hotkey hotkeyCombo)
        {
            if (hotkeyCombo == null)
                throw new ArgumentNullException("hotkeyCombo");

            HotkeyCallback callback = new HotkeyCallback();

            callback.KeyName = keyName;

            this.container.AddAndUpdate(hotkeyCombo, callback);

            callback.IsBind = User32Dll.RegisterHotKey(hotkeyWindow.Handle, hotkeyCombo.GetHashCode(), (uint)hotkeyCombo.Modifier, (uint)hotkeyCombo.Key);
            string errorMessage = string.Empty;
            if (!callback.IsBind)
            {
                var hr = Marshal.GetHRForLastWin32Error();
                var ex = Marshal.GetExceptionForHR(hr);

                if ((uint)hr == 0x80070581)
                {
                    if (this.OnHotkeyAlreadyRegistered != null)
                        this.OnHotkeyAlreadyRegistered(hotkeyCombo, new HotkeyAlreadyBoundException(Marshal.GetLastWin32Error()));

                    callback.Error = SDResources.Hotkey_AlreadyExist;
                }
                else
                {
                    callback.Error = ex.Message;
                }
            }

            return callback;
        }

        /// <summary>
        /// 注册一个热键
        /// </summary>
        /// <param name="hotkeyCombo"></param>
        //private bool RegisterHotkey(Hotkey hotkeyCombo)
        //{

        //}

        /// <summary>
        /// 解绑热键
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="keys"></param>
        public void Unbind(Modifiers modifiers, Keys keys)
        {
            Unbind(new Hotkey(modifiers, keys));
        }

        /// <summary>
        ///  解绑热键
        /// </summary>
        /// <param name="hotkeyCombo"></param>
        public void Unbind(Hotkey hotkeyCombo)
        {
            this.container.Remove(hotkeyCombo);
            this.UnregisterHotkey(hotkeyCombo);
        }

        /// <summary>
        /// 取消一个热键
        /// </summary>
        /// <param name="hotkeyCombo"></param>
        private void UnregisterHotkey(Hotkey hotkeyCombo)
        {
            bool successful = User32Dll.UnregisterHotKey(hotkeyWindow.Handle, hotkeyCombo.GetHashCode());

            if (!successful)
                throw new HotkeyNotBoundException(Marshal.GetLastWin32Error());
        }

        private void OnHotkeyPressed(object sender, HotkeyPressedEventArgs e)
        {
            HotkeyCallback callback = container.Find(e.Hotkey);

            if (callback.Assigned)
            {
                e.Hotkey.KeyName = callback.KeyName;

                callback.Invoke(e.Hotkey);
            }
        }

        public void Dispose()
        {
            hotkeyWindow.Dispose();
        }
    }
}