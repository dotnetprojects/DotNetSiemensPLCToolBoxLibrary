using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Network
{
    public class Network : ProjectItem
    {
        public string SubnetId { get; set; }
        
        public NetworkType NetworkType { get; set; }
    }
}
