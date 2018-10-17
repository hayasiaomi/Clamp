using System;
using System.ComponentModel;

namespace ShanDian.UIShell.Framework.Shortcut
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