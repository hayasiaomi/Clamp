using ShanDian.UIShell.Framework.Shortcut;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ShanDian.UIShell.Framework
{
  
    ///// <summary>
    ///// 系统API
    ///// </summary>
    //static class NativeMethods
    //{
    //    [DllImport("user32.dll", SetLastError = true)]
    //    internal static extern bool RegisterHotKey(IntPtr windowHandle, int hotkeyId, uint modifier, uint key);

    //    [DllImport("user32.dll", SetLastError = true)]
    //    internal static extern bool UnregisterHotKey(IntPtr windowHandle, int hotkeyId);

    //    [DllImport("user32.dll")]
    //    internal static extern bool HideCaret(IntPtr controlHandle);

    //    [DllImport("user32")]
    //    public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
    //    [DllImport("user32")]
    //    public static extern int RegisterWindowMessage(string message);

    //    [DllImport("user32.dll")]
    //    public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

    //    [DllImport("user32.dll")]
    //    public static extern bool SetForegroundWindow(IntPtr WindowHandle);

    //    [DllImport("user32.dll", SetLastError = true)]
    //    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    //    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
    //    public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

    //    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    //    public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    //    [DllImport("user32.dll")]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    public static extern bool ShowWindow(IntPtr hWnd, SW_WindowState flags);

    //    [DllImport("user32.dll")]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    public static extern bool GetWindowPlacement(IntPtr hWnd, ref Windowplacement lpwndpl);

    //    [DllImport("user32.dll")]
    //    public static extern bool IsIconic(IntPtr hWnd);

    //    [DllImport("user32.dll")]
    //    public static extern bool ShowWindowAsync(IntPtr hWnd, SW_WindowState flags);

    //    [DllImport("user32.dll")]
    //    internal static extern IntPtr SetActiveWindow(IntPtr hWnd);

    //    [DllImport("user32.dll")]
    //    public static extern bool EnumWindows(EnumWindowsProcDelegate lpEnumFunc, Int32 lParam);

    //    [DllImport("user32.dll")]
    //    public static extern int GetWindowThreadProcessId(IntPtr hWnd, ref Int32 lpdwProcessId);

    //    [DllImport("user32.dll")]
    //    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

    //    [DllImport("user32.dll")]
    //    public static extern bool ReleaseCapture();

    //}

    //public static class NativeConstant
    //{
    //    public const int HWND_BROADCAST = 0xffff;

    //    public static readonly int WM_SHOWME = NativeMethods.RegisterWindowMessage("WM_SHOWME");

    //    public const int SW_SHOWNORMAL = 1;

    //    public const Int32 WM_USER = 1024;
    //    public const Int32 WM_CSKEYBOARD = WM_USER + 192;
    //    public const Int32 WM_CSKEYBOARDMOVE = WM_USER + 193;
    //    public const Int32 WM_CSKEYBOARDRESIZE = WM_USER + 197;
    //}

    //public enum SW_WindowState : int
    //{
    //    Hide = 0,
    //    ShowNormal = 1,
    //    ShowMinimized = 2,
    //    ShowMaximized = 3,
    //    Maximize = 3,
    //    ShowNormalNoActivate = 4, Show = 5,
    //    Minimize = 6,
    //    ShowMinNoActivate = 7,
    //    ShowNoActivate = 8,
    //    Restore = 9,
    //    ShowDefault = 10,
    //    ForceMinimized = 11
    //};

    //public struct Windowplacement
    //{
    //    public int length;
    //    public int flags;
    //    public int showCmd;
    //    public System.Drawing.Point ptMinPosition;
    //    public System.Drawing.Point ptMaxPosition;
    //    public System.Drawing.Rectangle rcNormalPosition;
    //}


}