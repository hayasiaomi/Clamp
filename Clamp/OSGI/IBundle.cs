﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    public interface IBundle
    {
        void Start();

        void Stop();
    }
}
