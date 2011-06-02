using System;
using System.Collections.Generic;
using System.Net;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces
{
    public abstract class Interface : IDisposable
    {

        /// <summary>
        /// default 1,5s
        /// </summary>
        public TimeSpan TimeOut { get; set;} 

        public Interface()
        {
            TimeOut = new TimeSpan(0, 0, 0, 1, 500);
        }

        public void Dispose()
        { }

        protected abstract byte[] ConnectPlc(byte cpuMpi, byte cpuRack, byte cpuSlot, byte connType, bool routing, bool routingDestIsIp, IPAddress routingIp, byte routingMpi, byte routingRack, byte routingSlot, byte routingConnType);
         
        public Connection NewConnection(int TargetMPI, string CpuMpi, bool IPConnection, int CpuRack, int CpuSlot)
        {
            return null;
        }

        public Connection NewRoutingConnection(int TargetMPI, string CpuMpi, bool IPConnection, int CpuRack, int CpuSlot, bool Routing, string RoutingSubnet1, string RoutingSubnet2, int RoutingDestinationRack, int RoutingDestinationSlot, string RoutingDestination)
        {
            return new Connection(ConnectPlc(0, 0, 0, 0, false, false, new IPAddress(new byte[] { 192, 168, 1, 210 }), 0, 0, 0, 0));
        }
     
        public List<int> ListReachablePartners()
        {
            return null;
        }

        //event AsynchronousDataRecieved

        /*
        protected Pdu NegotiatePduLengthRequest()
        {
            Pdu myPdu=new Pdu();
            myPdu.Param = new byte[] {0xF0, 0, 0, 1, 0, 1, 3, 0xC0,};
        }
        */
    }
}
