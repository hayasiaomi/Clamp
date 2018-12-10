using System;

namespace Clamp.MUI.Framework.Shortcuts
{
    internal class HotkeyPressedEventArgs : EventArgs
    {
        internal Hotkey Hotkey { get; private set; }

        internal HotkeyPressedEventArgs(Hotkey hotkey)
        {
            Hotkey = hotkey;
        }
    }
}