using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Net.NetworkInformation;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Network
{
    public class MpiProfiBusNetworkInterface : NetworkInterface
    {
        public int Address { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}) Address: {2}", Name, this.NetworkInterfaceType == NetworkType.Profibus ? "ProfiBus" : "MPI", this.Address);
        }
    }
}
