using System;
using System.ComponentModel;

namespace ShanDian.UIShell.Framework.Shortcut
{

    [Serializable]
    public sealed class HotkeyAlreadyBoundException : Win32Exception
    {
        internal HotkeyAlreadyBoundException(int error) : base(error)
        {
        }

        internal HotkeyAlreadyBoundException(string message) : base(message)
        {
        }
    }
}