using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Clamp.UIShell.Framework.Native;

namespace Clamp.UIShell.Framework
{
    internal class InputBuilder : IEnumerable<INPUT>
    {
        private readonly List<INPUT> _inputList;

        public InputBuilder()
        {
            _inputList = new List<INPUT>();
        }

        public INPUT[] ToArray()
        {
            return _inputList.ToArray();
        }

        public IEnumerator<INPUT> GetEnumerator()
        {
            return _inputList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public INPUT this[int position]
        {
            get
            {
                return _inputList[position];
            }
        }

        public static bool IsExtendedKey(VirtualKeyCode keyCode)
        {
            if (keyCode == VirtualKeyCode.MENU ||
                keyCode == VirtualKeyCode.LMENU ||
                keyCode == VirtualKeyCode.RMENU ||
                keyCode == VirtualKeyCode.CONTROL ||
                keyCode == VirtualKeyCode.RCONTROL ||
                keyCode == VirtualKeyCode.INSERT ||
                keyCode == VirtualKeyCode.DELETE ||
                keyCode == VirtualKeyCode.HOME ||
                keyCode == VirtualKeyCode.END ||
                keyCode == VirtualKeyCode.PRIOR ||
                keyCode == VirtualKeyCode.NEXT ||
                keyCode == VirtualKeyCode.RIGHT ||
                keyCode == VirtualKeyCode.UP ||
                keyCode == VirtualKeyCode.LEFT ||
                keyCode == VirtualKeyCode.DOWN ||
                keyCode == VirtualKeyCode.NUMLOCK ||
                keyCode == VirtualKeyCode.CANCEL ||
                keyCode == VirtualKeyCode.SNAPSHOT ||
                keyCode == VirtualKeyCode.DIVIDE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public InputBuilder AddKeyDown(VirtualKeyCode keyCode)
        {
            var down =
                new INPUT
                {
                    Type = (UInt32)INPUTTYPE.Keyboard,
                    Data =
                    {
                        Keyboard =
                            new KEYBDINPUT
                            {
                                KeyCode = (UInt16)keyCode,
                                Scan = 0,
                                Flags = IsExtendedKey(keyCode) ? (UInt32)KEYBOARDFLAG.ExtendedKey : 0,
                                Time = 0,
                                ExtraInfo = IntPtr.Zero
                            }
                    }
                };

            _inputList.Add(down);
            return this;
        }


        public InputBuilder AddKeyUp(VirtualKeyCode keyCode)
        {
            var up =
                new INPUT
                {
                    Type = (UInt32)INPUTTYPE.Keyboard,
                    Data =
                    {
                        Keyboard =
                            new KEYBDINPUT
                            {
                                KeyCode = (UInt16)keyCode,
                                Scan = 0,
                                Flags = (UInt32)(IsExtendedKey(keyCode)
                                                      ? KEYBOARDFLAG.KeyUp | KEYBOARDFLAG.ExtendedKey
                                                      : KEYBOARDFLAG.KeyUp),
                                Time = 0,
                                ExtraInfo = IntPtr.Zero
                            }
                    }
                };

            _inputList.Add(up);
            return this;
        }

        public InputBuilder AddKeyPress(VirtualKeyCode keyCode)
        {
            AddKeyDown(keyCode);
            AddKeyUp(keyCode);
            return this;
        }

    }
}
