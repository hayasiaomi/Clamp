using Clamp.MUI.WF.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Clamp.MUI.WF.Controls
{
    internal static class CONSTS
    {
        internal const string CLASS_NAME = "NetDimensionFormShadowWindow";
        internal const int SHOW_BORDER_DELAY = 250;

    }

    internal enum FormShadowDockPositon
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }

    internal delegate void FormShadowResizeEventHandler(object sender, FormShadowResizeArgs args);

    internal class FormShadowResizeArgs : EventArgs
    {
        private readonly FormShadowDockPositon _side;
        private readonly HitTest _mode;

        public FormShadowDockPositon Side
        {
            get { return _side; }
        }

        public HitTest Mode
        {
            get { return _mode; }
        }

        internal FormShadowResizeArgs(FormShadowDockPositon side, HitTest mode)
        {
            _side = side;
            _mode = mode;
        }
    }

    public interface IFormShadow : IDisposable
    {
        Color ActiveColor { get; set; }
        Color InactiveColor { get; set; }
        bool IsInitialized { get; }
        bool IsEnabled { get; }
        void InitializeShadows();
        void Enable(bool enable);
        void SetOwner(IntPtr owner);
        void SetFocus();
        void KillFocus();

    }
}
