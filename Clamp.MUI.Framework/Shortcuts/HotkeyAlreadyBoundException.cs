using System;
using System.ComponentModel;

namespace Clamp.MUI.Framework.Shortcuts
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