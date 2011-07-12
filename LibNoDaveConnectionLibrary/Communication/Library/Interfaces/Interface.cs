using System;
using System.Collections.Generic;
using System.Net;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces
{
    public interface Interface : IDisposable
    {
        /// <summary>
        /// default 1,5s
        /// </summary>
        TimeSpan TimeOut { get; set;} 
               
        Connection ConnectPlc(ConnectionConfig config);

        List<int> ListReachablePartners();

        void SendPdu(Pdu pdu, Connection connection);
        //void SendData(byte[] data);
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
