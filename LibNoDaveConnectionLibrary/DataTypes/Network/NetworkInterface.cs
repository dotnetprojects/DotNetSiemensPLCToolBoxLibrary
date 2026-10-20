using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Network
{
    public class NetworkInterface : ProjectItem
    {
        public Step7ObjectReference Reference { get; set; }
        public Network Network { get; set; }
        public NetworkNode Node { get; set; }
        public Step7ObjectReference StationReference { get; set; }
        public Projectfolders.Step7V5.CPUFolder Plc { get; set; }
        public Projectfolders.Step7V5.CPFolder CommunicationProcessor { get; set; }
        public Step7ObjectReference HardwareReference { get; set; }
        public NetworkType NetworkInterfaceType { get; set; }
    }
}
