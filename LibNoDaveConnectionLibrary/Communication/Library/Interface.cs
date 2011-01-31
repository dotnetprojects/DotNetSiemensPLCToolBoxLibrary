using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public abstract class Interface : IDisposable
    {
        public void Dispose()
        { }

        public Connection NewConnection(int TargetMPI, string CpuMpi, bool IPConnection, int CpuRack, int CpuSlot)
        {
            return null;
        }

        public Connection NewRoutingConnection(int TargetMPI, string CpuMpi, bool IPConnection, int CpuRack, int CpuSlot, bool Routing, string RoutingSubnet1, string RoutingSubnet2, int RoutingDestinationRack, int RoutingDestinationSlot, string RoutingDestination)
        {
            return null;
        }
     
        public List<int> ListReachablePartners()
        {
            return null;
        }
    }
}
