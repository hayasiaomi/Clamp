using Clamp.Common;
using Clamp.SDK;
using Clamp.SDK.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clamp.Test
{
    public partial class Form1 : Form
    {
        private readonly SDPipelineClient pipelineClient;
        public Form1()
        {
            this.pipelineClient = new SDPipelineClient("UIShell");
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.pipelineClient.HandleMessage = packet =>
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show(packet.Data);
                }));

                return "success";
            };

            this.pipelineClient.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
