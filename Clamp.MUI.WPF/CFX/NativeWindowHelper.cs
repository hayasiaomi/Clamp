using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.MUI.WPF.CFX
{
    public static class NativeWindowHelper
    {
        public const int SW_RESTORE = 9;
        public const int SW_SHOW = 5;
        public const int SW_SHOWDEFAULT = 10;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindowAsync(IntPtr windowHandle, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr windowHandle);

        public static void BringToFront(IntPtr windowHandle)
        {
            ShowWindowAsync(windowHandle, SW_SHOWDEFAULT);
            ShowWindowAsync(windowHandle, SW_SHOW);
            SetForegroundWindow(windowHandle);
        }
    }
}
