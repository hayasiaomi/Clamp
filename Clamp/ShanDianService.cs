using Clamp.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Clamp
{
    public partial class ShanDianService : ServiceBase
    {
        public ShanDianService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SD.Start();
        }

        protected override void OnStop()
        {
            SD.Stop();
        }
    }
}
