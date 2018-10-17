using ShanDian.UIShell.Framework.DB;
using ShanDian.UIShell.Framework.Shortcut;
using ShanDian.UIShell.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShanDian.UIShell.Framework.Helpers
{
    class HotkeyHelper
    {
        private static Dictionary<string, Hotkey> ConflictHotkeys = new Dictionary<string, Hotkey>();
        public static readonly HotkeyBinder HotkeyBinder = new HotkeyBinder();
        public static Dictionary<string, string> DefaultHotkeys = new Dictionary<string, string>();
        public static DataHandler DBDataHandler = new DataHandler(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB", "shortcuts.db"), "LocalShortcutInfos");

        static HotkeyHelper()
        {
            DefaultHotkeys.Add("OpenShanDian", "Ctrl + O");
            DefaultHotkeys.Add("LockShanDian", "Ctrl + L");

            //检测是否是第一次初始化
            if (!DBDataHandler.Exist())
            {
                foreach (string key in DefaultHotkeys.Keys)
                {
                    DBDataHandler.Insert(key, DefaultHotkeys[key]);
                }
            }
        }

        public static void Store(string key, object value)
        {
            object dbResult = DBDataHandler.Get(key);

            if (dbResult == null)
            {
                DBDataHandler.Insert(key, value);
            }
            else
            {
                DBDataHandler.Update(key, value);
            }
        }

        public static List<DictionaryInfo> AcquireAll()
        {
            return DBDataHandler.GetAll();
        }

        public static DictionaryInfo Acquire(string key)
        {
            return DBDataHandler.Get(key);
        }

        public static object AcquireValue(string key)
        {
            return DBDataHandler.GetValue(key);
        }


        /// <summary>
        /// 获得是否开起快捷键
        /// </summary>
        /// <returns></returns>
        public static bool GetShortcutsEnabled()
        {
            object objKeyName = DBHelper.AcquireValue("ShortcutsEnabled");
            if (objKeyName != null)
                return Convert.ToBoolean(objKeyName);

            return false;
        }

        /// <summary>
        /// 设置是否开起快捷键
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetShortcutsEnabled(bool enabled)
        {
            DBHelper.Store("ShortcutsEnabled", enabled);
        }

        /// <summary>
        /// 注删快捷键
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="hotkeyvalue"></param>
        /// <param name="callback"></param>
        public static HotkeyRegisterResult RegisterHotKey(string keyName, string hotkeyvalue, Action<Hotkey> callback)
        {
            return RegisterHotKey(keyName, hotkeyvalue, false, callback);
        }

        public static HotkeyRegisterResult RegisterHotKey(string keyName, string hotkeyvalue, bool rewrite, Action<Hotkey> callback)
        {
            if (!IsOnlyModifiers(hotkeyvalue))
            {
                try
                {
                    HotkeyRegisterResult hotkeyRegisterResult = new HotkeyRegisterResult();

                    //去掉以前的绑定的快捷键
                    object oObjHotkeyValue = HotkeyHelper.AcquireValue(keyName);

                    string oHotkeyValue = null;

                    if (oObjHotkeyValue != null)
                        oHotkeyValue = Convert.ToString(oObjHotkeyValue);

                    if (oHotkeyValue != hotkeyvalue || rewrite)
                    {
                        Hotkey oHotkeyCombo = ConvertToHotkey(GetHotkeys(oHotkeyValue));

                        if (oHotkeyCombo != null)
                        {
                            if (oHotkeyCombo.Modifier != Modifiers.None || oHotkeyCombo.Key != Keys.None)
                            {
                                try
                                {
                                    HotkeyBinder.Unbind(oHotkeyCombo);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(hotkeyvalue))
                        {
                            Hotkey hotkeyCombo = ConvertToHotkey(GetHotkeys(hotkeyvalue));

                            if (hotkeyCombo != null && (hotkeyCombo.Modifier != Modifiers.None || hotkeyCombo.Key != Keys.None))
                            {

                                if (hotkeyCombo.Modifier != Modifiers.None || hotkeyCombo.Key != Keys.None)
                                {
                                    HotkeyCallback bindCallback = HotkeyBinder.Bind(keyName, hotkeyCombo);

                                    if (bindCallback.IsBind)
                                    {
                                        bindCallback.To(callback);

                                        if (ConflictHotkeys.ContainsKey(keyName))
                                            ConflictHotkeys.Remove(keyName);

                                        if (oHotkeyValue != hotkeyvalue)
                                            HotkeyHelper.Store(keyName, hotkeyvalue);
                                    }
                                    else
                                    {
                                        ConflictHotkeys.Add(keyName, hotkeyCombo);
                                    }

                                    hotkeyRegisterResult.IsSuccess = bindCallback.IsBind;
                                    hotkeyRegisterResult.ErrorMessage = bindCallback.Error;
                                }
                                else
                                {
                                    hotkeyRegisterResult.IsSuccess = true;
                                    hotkeyRegisterResult.ErrorMessage = "";
                                }

                                return hotkeyRegisterResult;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }

                //HotkeyHelper.Store(keyName, string.Empty);
            }

            return null;
        }


        public static bool IsBind(string keyName, string hotkeyvalue)
        {
            Hotkey hotkeyCombo = ConvertToHotkey(GetHotkeys(hotkeyvalue));

            return HotkeyBinder.IsHotkeyAlreadyBound(hotkeyCombo);
        }

        public static bool HaveConflict(string keyName)
        {
            return ConflictHotkeys.ContainsKey(keyName);
        }

        public static bool HaveConflict()
        {
            return ConflictHotkeys.Count > 0;
        }

        public static void RefreshConflictHotkey()
        {

        }


        /// <summary>
        /// 将字符串转成快捷键数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string[] GetHotkeys(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                List<string> hotkeys = new List<string>();

                int sIndex = 0;
                int eIndex = 0;

                while (eIndex < value.Length)
                {
                    sIndex = eIndex;

                    while (eIndex < value.Length && value[eIndex] != '+')
                        eIndex++;

                    if (sIndex < eIndex)
                        hotkeys.Add(value.Substring(sIndex, eIndex - sIndex));

                    eIndex++;
                }

                return hotkeys.ToArray();
            }

            return null;
        }

        /// <summary>
        /// 快捷数组转成快捷对象
        /// </summary>
        /// <param name="hotkeys"></param>
        /// <returns></returns>
        private static Hotkey ConvertToHotkey(string[] hotkeys)
        {
            if (hotkeys != null)
            {
                if (hotkeys.Length == 1)
                {
                    return new Hotkey(Modifiers.None, GetKeysByString(hotkeys[0]));
                }
                else if (hotkeys.Length == 2)
                {
                    return new Hotkey(GetModifiersByString(hotkeys[0]), GetKeysByString(hotkeys[1]));
                }
                else if (hotkeys.Length == 3)
                {
                    return new Hotkey(GetModifiersByString(hotkeys[0]) | GetModifiersByString(hotkeys[1]), GetKeysByString(hotkeys[2]));
                }

            }

            return null;
        }

        private static Modifiers GetModifiersByString(string value)
        {
            value = value.ToUpper().Trim();

            if ("CTRL" == value)
                return Modifiers.Control;
            else if ("SHIFT" == value)
                return Modifiers.Shift;
            else if ("ALT" == value)
                return Modifiers.Alt;
            else
                return Modifiers.None;
        }

        private static bool IsOnlyModifiers(string value)
        {
            return GetModifiersByString(value) != Modifiers.None;
        }

        private static Keys GetKeysByString(string value)
        {
            Keys keyValue = Keys.None;
            value = value.ToUpper().Trim();
            if (Enum.TryParse(value, out keyValue))
            {
            }

            return keyValue;
        }
    }


    public class HotkeyRegisterResult
    {
        public bool IsSuccess { set; get; }
        public Hotkey Hotkey { set; get; }

        public string ErrorMessage { set; get; }
    }
}
