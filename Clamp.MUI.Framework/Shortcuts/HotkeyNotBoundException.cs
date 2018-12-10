using System;
using System.ComponentModel;

namespace Clamp.MUI.Framework.Shortcuts
{
    [Serializable]
    public sealed class HotkeyNotBoundException : Win32Exception
    {
        internal HotkeyNotBoundException(int errorCode) : base(errorCode)
        {
        }

        internal HotkeyNotBoundException(string message) : base(message)
        {
        }
    }
}