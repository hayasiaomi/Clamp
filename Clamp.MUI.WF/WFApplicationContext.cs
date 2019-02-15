using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clamp.MUI.WF
{
    public class WFApplicationContext : ApplicationContext
    {
        public WFApplicationContext(Form form) : base(form)
        {

        }

        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count > 0)
            {
                MainForm = Application.OpenForms[0];
            }
            else
            {
                base.OnMainFormClosed(sender, e);
            }
        }

    }
}
