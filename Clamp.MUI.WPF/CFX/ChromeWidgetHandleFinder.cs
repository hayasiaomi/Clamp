﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.MUI.WPF.CFX
{
    internal static class ChromeWidgetHandleFinder
    {
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private class ClassDetails
        {
            public IntPtr DescendantFound { get; set; }
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            const string chromeWidgetHostClassName = "Chrome_RenderWidgetHostHWND";

            var buffer = new StringBuilder(128);
            GetClassName(hWnd, buffer, buffer.Capacity);

            if (buffer.ToString() == chromeWidgetHostClassName)
            {
                var gcHandle = GCHandle.FromIntPtr(lParam);

                var classDetails = (ClassDetails)gcHandle.Target;

                classDetails.DescendantFound = hWnd;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Chrome's message-loop Window isn't created synchronously, so this may not find it.
        /// If so, you need to wait and try again later.
        /// </summary>
        public static bool TryFindHandle(IntPtr browserHandle, out IntPtr chromeWidgetHostHandle)
        {
            var classDetails = new ClassDetails();
            var gcHandle = GCHandle.Alloc(classDetails);

            var childProc = new EnumWindowProc(EnumWindow);

            EnumChildWindows(browserHandle, childProc, GCHandle.ToIntPtr(gcHandle));

            chromeWidgetHostHandle = classDetails.DescendantFound;

            gcHandle.Free();

            return classDetails.DescendantFound != IntPtr.Zero;
        }
    }
}
