using ShanDian.AddIns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Doozer
{
    public class WebworkDoozer : IDoozer
    {
        public bool HandleConditions
        {
            get
            {
                return false;
            }
        }

        public object BuildItem(BuildItemArgs args)
        {
            Codon codon = args.Codon;

            return codon.AddIn.CreateObject(codon.Properties["class"]);
        }
    }

    
}
