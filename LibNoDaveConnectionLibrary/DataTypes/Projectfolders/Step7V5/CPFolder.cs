using DotNetSiemensPLCToolBoxLibrary.DataTypes.Network;
using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class CPFolder : Step7ProjectFolder, IHardwareFolder
    {
        internal int UnitID;
        internal int TobjTyp;
        internal List<int> IdTobjId;
        internal List<int> TobjId;
        internal int SubModulNumber;
        internal CPFolder SubModul;

        public int Rack { get; set; }
        public int Slot { get; set; }

        public List<NetworkInterface> NetworkInterfaces { get; set; }

        public override string ToString()
        {
            var retVal = base.ToString();

            if (NetworkInterfaces != null)
            {
                retVal += Environment.NewLine;
                retVal += string.Join(Environment.NewLine, NetworkInterfaces);
            }

            return retVal;
        }
    }
}