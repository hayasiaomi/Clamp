using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.WPF.CFX
{
    public class ContextMenuItem
    {
        public Action Command { get; }
        public string Name { get; }
        public bool Enabled { get; set; }

        public ContextMenuItem(Action command, string name, bool enabled = true)
        {
            Command = command;
            Name = name;
            Enabled = enabled;
        }
    }
}
