using System;

namespace Clamp.MUI.Framework.Shortcuts
{

    /// <summary>
    /// 热键回调类
    /// </summary>
    public class HotkeyCallback
    {
        private Action callback;
        /// <summary>
        /// 是否分配过了
        /// </summary>
        public bool Assigned { get { return callback != null; } }

        public void To(Action callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            this.callback = callback;
        }

        internal void Invoke()
        {
            callback.Invoke();
        }
    }
}