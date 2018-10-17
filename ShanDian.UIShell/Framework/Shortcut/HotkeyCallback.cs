using System;

namespace ShanDian.UIShell.Framework.Shortcut
{

    /// <summary>
    /// 热键回调类
    /// </summary>
    public class HotkeyCallback
    {
        private Action<Hotkey> callback;
        /// <summary>
        /// 是否分配过了
        /// </summary>
        public bool Assigned { get { return callback != null; } }

        public string KeyName { set; get; }

        public bool IsBind { set; get; }

        public string Error { set; get; }

        public void To(Action<Hotkey> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            this.callback = callback;
        }

        internal void Invoke(Hotkey hotkey)
        {
            callback.Invoke(hotkey);
        }
    }
}