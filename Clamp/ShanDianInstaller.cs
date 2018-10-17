using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace Clamp
{
    [RunInstaller(true)]
    public partial class ShanDianInstaller : System.Configuration.Install.Installer
    {
        public ShanDianInstaller()
        {
            InitializeComponent();
        }
    }
}
