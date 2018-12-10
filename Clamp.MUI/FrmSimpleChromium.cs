using Clamp.MUI.Biz;
using Clamp.MUI.ChromiumCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clamp.MUI
{
    internal partial class FrmSimpleChromium : FrmChromium
    {
        public FrmSimpleChromium() : this(null)
        {

        }

        public FrmSimpleChromium(string initialUrl) : base(initialUrl)
        {
            this.WinFormBridge = new WinFormBridge(this);
            this.ChromiumWebBrowser.GlobalObject.Add("SD", new JsWinformObject(this.WinFormBridge));

            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            ChromiumSettings.ChildrenChromiums.Add(this);
            base.OnShown(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ChromiumSettings.ChildrenChromiums.Remove(this);
            base.OnClosing(e);
        }
    }
}
