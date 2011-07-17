using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class Connection : IDisposable
    {
        delegate void AsynchronDataArrivedDelegate(ResultSet resultSet);
        private event AsynchronDataArrivedDelegate AsynchronDataArrived;

        internal int ConnectionNumber { get; set; }

        internal bool ConnectionEstablished { get; set; }

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

        public void Close()
        {
            Interface.DisconnectPlc(this);
        }

        //Data for S7Online!
        internal byte application_block_subsystem { get; set; }
        internal byte application_block_opcode { get; set; }
        
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

        public ResultSet ExecReadRequest(Pdu_ReadRequest myPdu)
        {
            Pdu wrt = ExchangePdu(myPdu);
            return null;
        }

        public void SendPdu(Pdu myPdu)
        {
            Interface.SendPdu(myPdu, this);
        }

        
        /* Vielleicht unnötig, da man gar nicht zuordnen könnte zu welcher Pdu die Antwort gehört!
        public Pdu RecievePdu()
        {
            throw new NotImplementedException();
        }
        */

        #region PDU/Data Exchange with Interface

        internal object RecievedData = null;

        private object lockpdu = new object();
        private ushort pduNr = 1;
        private Dictionary<int, Pdu> RecievedPdus = new Dictionary<int, Pdu>();
        public Pdu ExchangePdu(Pdu myPdu)
        {
            ushort pduNrInt;
            lock (lockpdu)
            {
                while (RecievedPdus.ContainsKey(pduNr))
                {
                    pduNr++;
                    if (pduNr >= ushort.MaxValue)
                        pduNr = 1;
                }

                pduNrInt = pduNr;

                myPdu.header.number = pduNr;

                RecievedPdus.Add(1, null);

                Interface.SendPdu(myPdu, this);
            }

            //Todo: maybe implement a Timeout here!
            while (RecievedPdus[pduNrInt] == null)
            {
                Application.DoEvents();
            }

            Pdu retVal = RecievedPdus[pduNrInt];
            RecievedPdus.Remove(pduNrInt);
            return retVal;
        }

        internal void SetRecievedPdu(Pdu data)
        {
            if (data.header.number != 0)
                RecievedPdus[data.header.number] = data;
            else
            {
                //Asynchronous Data arrived, call Event
                //Event should be used in PLC Connection!
            }
        }
        #endregion

        public void Dispose()
        {
            if (this._interface != null)
            {

                this.Close();
                this._interface = null;
            }
        }
    }
}
