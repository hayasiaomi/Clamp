﻿using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Clamp.MUI.Framework.Shortcuts
{
    /// <summary>
    /// 热键信息类
    ///   [Editor(typeof(HotkeyEditor), typeof(UITypeEditor))]
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(HotkeyConverter))]
    public class Hotkey : IEquatable<Hotkey>
    {
        /// <summary>
        /// 特殊键的值
        /// </summary>
        public Modifiers Modifier { get; private set; }

        /// <summary>
        /// 按键的值 
        /// </summary>
        public Keys Key { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="key"></param>
        public Hotkey(Modifiers modifier, Keys key)
        {
            this.Key = key;
            this.Modifier = modifier;
        }

        public bool Equals(Hotkey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Modifier.Equals(other.Modifier) && Key.Equals(other.Key);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((Hotkey)other);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Modifier.GetHashCode() * 397) ^ Key.GetHashCode();
            }
        }

        public override string ToString()
        {
            return this.Modifier + ", " + Key;
        }


        public static bool operator ==(Hotkey left, Hotkey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Hotkey left, Hotkey right)
        {
            return !Equals(left, right);
        }

    }
}