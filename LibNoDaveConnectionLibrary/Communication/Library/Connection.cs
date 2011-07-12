using System;
using System.Collections.Generic;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class Connection : IDisposable
    {
        delegate void AsynchronDataArrivedDelegate(ResultSet resultSet);
        private event AsynchronDataArrivedDelegate AsynchronDataArrived;

        internal int ConnectionNumber { get; set; }

        private ConnectionConfig _connectionConfig;
        public ConnectionConfig ConnectionConfig
        {
            get { return _connectionConfig; }            
        }

        private int _pduSize;
        public int PduSize
        {
            get { return _pduSize; }
            internal set { _pduSize = value; }            
        }

        //Data for S7Online!
        internal byte application_block_subsystem { get; set; }
        
        //End Data for S7Online

        internal Connection(Interface Interface, ConnectionConfig config, int PduSize)
        {
            _connectionConfig = config;
            _interface = Interface;
            _pduSize = PduSize;
        }

        private Interface _interface;
        public Interface Interface
        {
            get { return _interface; }          
        }

        public Pdu PrepareReadRequest()
        {
            return null;
        }

        public ResultSet ExecReadRequest(Pdu myPdu)
        {
            return null;
        }

        public void SendPdu(Pdu myPdu)
        { }        

        public void Dispose()
        { }
    }
}
