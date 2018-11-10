using Aomi.Main;
using Clamp.OSGI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clamp
{
    public partial class FrmMain : Form
    {
        IClampBundle clampBundle;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (ICommand command in this.clampBundle.GetExtensionObjects<ICommand>())
                command.Run();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.clampBundle = ClampBundleFactory.GetClampBundle();

            this.clampBundle.Start();
        }
    }
}
