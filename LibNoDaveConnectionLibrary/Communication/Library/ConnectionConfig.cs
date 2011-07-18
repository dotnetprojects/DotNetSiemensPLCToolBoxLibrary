using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class ConnectionConfig
    {
        //public string EntryPoint { get; set; }
        public IPAddress IPAddress { get; set; }
        public bool ConnectionToEthernet { get; set; }
        public int MPIAddress { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }
        /// <summary>
        /// 1 = PG Connection, 2 = OP Connection
        /// </summary>
        public int ConnectionType { get; set; }

        public bool Routing { get; set; }
        public IPAddress RoutingIPAddress { get; set; }
        public bool RoutingToEthernet { get; set; }
        public int RoutingMPIAddres { get; set; }
        public Int32 RoutingSubnet1 { get; set; }
        public Int32 RoutingSubnet2 { get; set; }
        public int RoutingRack { get; set; }
        public int RoutingSlot { get; set; }
        public int RoutingConnectionType { get; set; }

        public ConnectionConfig()
        {

        }

        public ConnectionConfig(int MPIAddress, int Rack, int Slot)
        {
            this.MPIAddress = MPIAddress;
            this.Rack = Rack;
            this.Slot = Slot;

            this.ConnectionType = 1;
            this.ConnectionToEthernet = false;
        }

        public ConnectionConfig(IPAddress IPAddress, int Rack, int Slot)
        {
            this.IPAddress = IPAddress;
            this.Rack = Rack;
            this.Slot = Slot;

            this.ConnectionType = 1;
            this.ConnectionToEthernet = true;
        }

    }
}
