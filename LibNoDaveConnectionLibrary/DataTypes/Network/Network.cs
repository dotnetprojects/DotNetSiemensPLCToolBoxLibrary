using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Network
{
    public class Network : ProjectItem
    {
        public Step7ObjectReference Reference { get; set; }
        public NetworkLayout Layout { get; set; }
        public List<NetworkInterface> Members { get; } = new List<NetworkInterface>();
        public string SubnetId { get; set; }
        
        public NetworkType NetworkType { get; set; }
    }
}
