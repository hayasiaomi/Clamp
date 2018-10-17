using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Win32
{
    internal class NativeConstant
    {
        public const int HWND_BROADCAST = 0xffff;

        public static readonly int WM_SHOWME = User32Dll.RegisterWindowMessage("WM_SHOWME");

        public const int SW_SHOWNORMAL = 1;

        public const Int32 WM_USER = 1024;
        public const Int32 WM_CSKEYBOARD = WM_USER + 192;
        public const Int32 WM_CSKEYBOARDMOVE = WM_USER + 193;
        public const Int32 WM_CSKEYBOARDRESIZE = WM_USER + 197;

        #region WindowsLong
        public const int GWL_WNDPROC = -4;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_HWNDPARENT = -8;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int GWL_USERDATA = -21;
        public const int GWL_ID = -12;
        #endregion

        #region UpdateLayerWindowParameter
        public const int ULW_COLORKEY = 0x00000001;
        public const int ULW_ALPHA = 0x00000002;
        public const int ULW_OPAQUE = 0x00000004;
        public const int ULW_EX_NORESIZE = 0x00000008;
        #endregion

        #region DIBColorTableIdentifier
        public const byte DIB_RGB_COLORS = 0;
        public const byte DIB_PAL_COLORS = 1;
        #endregion

        #region CompressionType
        public const byte BI_RGB = 0;
        public const byte BI_RLE8 = 1;
        public const byte BI_RLE4 = 2;
        public const byte BI_BITFIELDS = 3;
        public const byte BI_JPEG = 4;
        public const byte BI_PNG = 5;
        #endregion

        #region BlendOp
        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;
        #endregion

        #region WindowsMessage
        public const Int32 WM_LBUTTONDOWN = 0x0201;
        public const Int32 WM_NCLBUTTONDOWN = 0x00A1;
        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 WM_PAINT = 0x000F;
        public const Int32 WM_MOVE = 0x0003;
        public const Int32 WM_CTLCOLORMSGBOX = 0x0132;
        public const Int32 WM_CTLCOLOREDIT = 0x0133;
        public const Int32 WM_CTLCOLORLISTBOX = 0x0134;
        public const Int32 WM_CTLCOLORBTN = 0x0135;
        public const Int32 WM_CTLCOLORDLG = 0x0136;
        public const Int32 WM_CTLCOLORSCROLLBAR = 0x0137;
        public const Int32 WM_CTLCOLORSTATIC = 0x0138;
        public const Int32 WM_CAPTURECHANGED = 0x0215;
        #endregion


        #region WndClassType
        public const Int32 CS_VREDRAW = 0x0001;
        public const Int32 CS_HREDRAW = 0x0002;
        public const Int32 CS_DBLCLKS = 0x0008;
        public const Int32 CS_OWNDC = 0x0020;
        public const Int32 CS_CLASSDC = 0x0040;
        public const Int32 CS_PARENTDC = 0x0080;
        public const Int32 CS_NOCLOSE = 0x0200;
        public const Int32 CS_SAVEBITS = 0x0800;
        public const Int32 CS_BYTEALIGNCLIENT = 0x1000;
        public const Int32 CS_BYTEALIGNWINDOW = 0x2000;
        public const Int32 CS_GLOBALCLASS = 0x4000;
        public const Int32 CS_IME = 0x00010000;
        public const Int32 CS_DROPSHADOW = 0x00020000;
        #endregion



        #region WndStyle
        public const UInt32 WS_OVERLAPPED = 0x00000000;
        public const UInt32 WS_POPUP = 0x80000000;
        public const UInt32 WS_CHILD = 0x40000000;
        public const UInt32 WS_MINIMIZE = 0x20000000;
        public const UInt32 WS_VISIBLE = 0x10000000;
        public const UInt32 WS_DISABLED = 0x08000000;
        public const UInt32 WS_CLIPSIBLINGS = 0x04000000;
        public const UInt32 WS_CLIPCHILDREN = 0x02000000;
        public const UInt32 WS_MAXIMIZE = 0x01000000;
        public const UInt32 WS_CAPTION = 0x00C00000;
        public const UInt32 WS_BORDER = 0x00800000;
        public const UInt32 WS_DLGFRAME = 0x00400000;
        public const UInt32 WS_VSCROLL = 0x00200000;
        public const UInt32 WS_HSCROLL = 0x00100000;
        public const UInt32 WS_SYSMENU = 0x00080000;
        public const UInt32 WS_THICKFRAME = 0x00040000;
        public const UInt32 WS_GROUP = 0x00020000;
        public const UInt32 WS_TABSTOP = 0x00010000;
        public const UInt32 WS_MINIMIZEBOX = 0x00020000;
        public const UInt32 WS_MAXIMIZEBOX = 0x00010000;
        public const UInt32 WS_TILED = WS_OVERLAPPED;
        public const UInt32 WS_ICONIC = WS_MINIMIZE;
        public const UInt32 WS_SIZEBOX = WS_THICKFRAME;
        public const UInt32 WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;
        public const UInt32 WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const UInt32 WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);
        public const UInt32 WS_CHILDWINDOW = (WS_CHILD);
        #endregion


        #region ExtendedWndStyle
        public const UInt32 WS_EX_DLGMODALFRAME = 0x00000001;
        public const UInt32 WS_EX_NOPARENTNOTIFY = 0x00000004;
        public const UInt32 WS_EX_TOPMOST = 0x00000008;
        public const UInt32 WS_EX_ACCEPTFILES = 0x00000010;
        public const UInt32 WS_EX_TRANSPARENT = 0x00000020;
        public const UInt32 WS_EX_MDICHILD = 0x00000040;
        public const UInt32 WS_EX_TOOLWINDOW = 0x00000080;
        public const UInt32 WS_EX_WINDOWEDGE = 0x00000100;
        public const UInt32 WS_EX_CLIENTEDGE = 0x00000200;
        public const UInt32 WS_EX_CONTEXTHELP = 0x00000400;
        public const UInt32 WS_EX_RIGHT = 0x00001000;
        public const UInt32 WS_EX_LEFT = 0x00000000;
        public const UInt32 WS_EX_RTLREADING = 0x00002000;
        public const UInt32 WS_EX_LTRREADING = 0x00000000;
        public const UInt32 WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const UInt32 WS_EX_RIGHTSCROLLBAR = 0x00000000;
        public const UInt32 WS_EX_CONTROLPARENT = 0x00010000;
        public const UInt32 WS_EX_STATICEDGE = 0x00020000;
        public const UInt32 WS_EX_APPWINDOW = 0x00040000;
        public const UInt32 WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const UInt32 WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const UInt32 WS_EX_LAYERED = 0x00080000;
        public const UInt32 WS_EX_NOINHERITLAYOUT = 0x00100000;
        public const UInt32 WS_EX_LAYOUTRTL = 0x00400000;
        public const UInt32 WS_EX_COMPOSITED = 0x02000000;
        public const UInt32 WS_EX_NOACTIVATE = 0x08000000;
        #endregion

    }
}
