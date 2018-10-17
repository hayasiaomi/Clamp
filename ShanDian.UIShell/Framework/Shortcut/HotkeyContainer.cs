using System.Collections.Generic;

namespace ShanDian.UIShell.Framework.Shortcut
{
    /// <summary>
    /// 热键的容器类
    /// </summary>
    internal class HotkeyContainer
    {
        private readonly IDictionary<Hotkey, HotkeyCallback> container;

        internal HotkeyContainer()
        {
            container = new Dictionary<Hotkey, HotkeyCallback>();
        }

        internal void AddAndUpdate(Hotkey hotkey, HotkeyCallback callback)
        {
            if (container.ContainsKey(hotkey))
            {
                container[hotkey] = callback;
            }
            else
            {
                container.Add(hotkey, callback);
            }
        }

        internal void Remove(Hotkey hotkey)
        {
            if (container.ContainsKey(hotkey))
            {
                container.Remove(hotkey);
            }
        }

        internal HotkeyCallback Find(Hotkey hotkey)
        {
            return container[hotkey];
        }
    }
}