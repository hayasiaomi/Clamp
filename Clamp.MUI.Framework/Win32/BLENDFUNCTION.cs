using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.MUI.Framework.Win32
{
    /// <summary>
    /// BLENDFUNCTION
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BLENDFUNCTION
    {
        byte BlendOp;
        byte BlendFlags;
        byte SourceConstantAlpha;
        byte AlphaFormat;

        public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
        {
            BlendOp = op;
            BlendFlags = flags;
            SourceConstantAlpha = alpha;
            AlphaFormat = format;
        }
    }

}
