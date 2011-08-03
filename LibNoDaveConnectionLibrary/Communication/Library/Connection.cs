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

        public delegate void PDURecievedDelegate(Pdu pdu);
        public event PDURecievedDelegate PDURecieved;

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

        public ResultSet ExecReadRequest(Pdu_ReadRequest myPdu)
        {
            Pdu wrt = ExchangePdu(myPdu);
            return new ResultSet(wrt.Param[1], wrt.Data.ToArray());
        }
        
        public void PLCStop()
        {
            Pdu pdu = new Pdu(1);
            byte[] para = new byte[] {0x29, 0, 0, 0, 0, 0, 9, (byte) 'P', (byte) '_', (byte) 'P', (byte) 'R', (byte) 'O', (byte) 'G', (byte) 'R', (byte) 'A', (byte) 'M'};
            pdu.Param.AddRange(para);

            Pdu rec = ExchangePdu(pdu);
        }

        public void PLCStart()
        {
            Pdu pdu = new Pdu(1);
            byte[] para = new byte[] {0x28, 0, 0, 0, 0, 0, 9, (byte) 'P', (byte) '_', (byte) 'P', (byte) 'R', (byte) 'O', (byte) 'G', (byte) 'R', (byte) 'A', (byte) 'M'};
            pdu.Param.AddRange(para);

            Pdu rec = ExchangePdu(pdu);
        }

        public void PLCCompress()
        {
            Pdu pdu = new Pdu(1);
            byte[] para = new byte[] {0x28, 0, 0, 0, 0, 0, 0, 0xFD, 0, 0, 5, (byte) '_', (byte) 'G', (byte) 'A', (byte) 'R', (byte) 'B'};
            pdu.Param.AddRange(para);

            Pdu rec = ExchangePdu(pdu);
        }

        public void PLCCopyRamToRom()
        {
            Pdu pdu = new Pdu(1);
            byte[] para = new byte[] {0x28, 0, 0, 0, 0, 0, 0, 0xFD, 0, 2, (byte) 'E', (byte) 'P', 5, (byte) '_', (byte) 'M', (byte) 'O', (byte) 'D', (byte) 'U'};
            pdu.Param.AddRange(para);

            Pdu rec = ExchangePdu(pdu);
        }

        public byte[] ReadSZL(int SZL_ID, int SZL_Index)
        {
            Pdu pdu = new Pdu(1);
            byte[] para_1 = new byte[] {0, 1, 18, 4, 17, 68, 1, 0};
            pdu.Param.AddRange(para_1);

            byte[] user = new byte[4];
            user[0] = (byte) (SZL_ID/0x100);
            user[1] = (byte) (SZL_ID%0x100);
            user[2] = (byte) (SZL_Index/0x100);
            user[3] = (byte) (SZL_Index%0x100);
            pdu.Data.AddRange(user);

            Pdu rec = ExchangePdu(pdu);

            byte[] para_2 = {0, 1, 18, 8, 18, 68, 1, 1, 0, 0, 0, 0};
            para_2[7] = rec.Param[7];
            pdu.Param.Clear();
            pdu.Param.AddRange(para_2);

            List<byte> retVal = new List<byte>();

            while (rec.Param[9] != 0)
            {
                retVal.AddRange(rec.UData);
                rec = ExchangePdu(pdu);
            }

            retVal.AddRange(rec.UData);

            return retVal.ToArray();
        }

        public void SendPdu(Pdu myPdu)
        {
            Interface.SendPdu(myPdu, this);
        }
        
        #region PDU/Data Exchange with Interface

        internal object RecievedData = null;

        private object lockpdu = new object();
        private ushort pduNr = 1;
        internal Dictionary<int, Pdu> RecievedPdus = new Dictionary<int, Pdu>();
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

                Interface.ExchangePdu(myPdu, this);
            }

            //Todo: maybe implement a Timeout here!
            while (RecievedPdus[pduNrInt] == null)
            { }

            Pdu retVal = RecievedPdus[pduNrInt];
            RecievedPdus.Remove(pduNrInt);
            return retVal;
        }

        internal void SetRecievedPdu(Pdu data)
        {
            if (PDURecieved != null)
                PDURecieved(data);

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
