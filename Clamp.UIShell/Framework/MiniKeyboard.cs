using Clamp.UIShell.Framework.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Clamp.UIShell.Framework
{
    public sealed class MiniKeyboard
    {
        public static void Input(int value)
        {
            InputBuilder inputBuilder = new InputBuilder();

            switch (value)
            {
                case 0:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_0);
                    break;
                case 1:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_1);
                    break;
                case 2:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_2);
                    break;
                case 3:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_3);
                    break;
                case 4:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_4);
                    break;
                case 5:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_5);
                    break;
                case 6:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_6);
                    break;
                case 7:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_7);
                    break;
                case 8:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_8);
                    break;
                case 9:
                    inputBuilder.AddKeyDown(VirtualKeyCode.VK_9);
                    break;
                case 101:
                    inputBuilder.AddKeyDown(VirtualKeyCode.CLEAR);
                    break;
                case 100:
                    inputBuilder.AddKeyDown(VirtualKeyCode.BACK);
                    break;
                case 102:
                    inputBuilder.AddKeyDown(VirtualKeyCode.DECIMAL);
                    break;

            }

            INPUT[] inputs = inputBuilder.ToArray();

            NativeMethods.SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
