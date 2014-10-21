using DotNetSiemensPLCToolBoxLibrary.DataTypes.Network;
using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class CPFolder : Step7ProjectFolder, IHardwareFolder
    {
        internal int UnitID;
        internal int TobjTyp;
        internal int TobjId;

        public int Rack { get; set; }
        public int Slot { get; set; }

        public List<NetworkInterface> NetworkInterfaces { get; set; }

        public override string ToString()
        {
            var retVal = base.ToString();
            retVal += "UnitID:" + UnitID + Environment.NewLine;
            retVal += "Id:" + ID + Environment.NewLine;
            retVal += "TobjTyp:" + TobjTyp + Environment.NewLine;
            retVal += "TobjId:" + TobjId + Environment.NewLine;

            if (NetworkInterfaces != null)
            {
                retVal += Environment.NewLine;
                retVal += string.Join(Environment.NewLine, NetworkInterfaces);
            }

            return retVal;
        }
    }
}
