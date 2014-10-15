using DotNetSiemensPLCToolBoxLibrary.DataTypes.Network;
using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class CPUFolder : Step7ProjectFolder, IHardwareFolder
    {
        internal int UnitID;
        internal int TobjTyp;        
        public PLCType CpuType { get; set; }

        public string PasswdHard { get; set; }

        public int Rack { get; set; }
        public int Slot { get; set; }

        public List<NetworkInterface> NetworkInterfaces { get; set; }

        public override string ToString()
        {
            var retVal = base.ToString();

            if (NetworkInterfaces != null)
            {
                retVal += string.Join(Environment.NewLine, NetworkInterfaces);
            }

            return retVal;
        }
    }
}
