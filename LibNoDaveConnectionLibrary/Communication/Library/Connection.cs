using System;
using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Interfaces;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    public class Connection : IDisposable
    {
        public const int daveResOK = 0;			/* means all ok */
        public const int daveResNoPeripheralAtAddress = 1;	/* CPU tells there is no peripheral at address */
        public const int daveResMultipleBitsNotSupported = 6; /* CPU tells it does not support to read a bit block with a */
        /* length other than 1 bit. */
        public const int daveResItemNotAvailable200 = 3;	/* means a a piece of data is not available in the CPU, e.g. */
        /* when trying to read a non existing DB or bit bloc of length<>1 */
        /* This code seems to be specific to 200 family. */

        public const int daveResItemNotAvailable = 10;	/* means a a piece of data is not available in the CPU, e.g. */
        /* when trying to read a non existing DB */

        public const int daveAddressOutOfRange = 5;		/* means the data address is beyond the CPUs address range */
        public const int daveWriteDataSizeMismatch = 7;	/* means the write data size doesn't fit item size */
        public const int daveResCannotEvaluatePDU = -123;     /* PDU is not understood by libnodave */
        public const int daveResCPUNoData = -124;
        public const int daveUnknownError = -125;
        public const int daveEmptyResultError = -126;
        public const int daveEmptyResultSetError = -127;
        public const int daveResUnexpectedFunc = -128;
        public const int daveResUnknownDataUnitSize = -129;
        public const int daveResNoBuffer = -130;
        public const int daveNotAvailableInS5 = -131;
        public const int daveResInvalidLength = -132;
        public const int daveResInvalidParam = -133;
        public const int daveResNotYetImplemented = -134;
        public const int daveResShortPacket = -1024;
        public const int daveResTimeout = -1025;

        public static string daveStrerror(int code)
        {
            switch (code)
            {
                case daveResOK: return "ok";
                case daveResMultipleBitsNotSupported: return "the CPU does not support reading a bit block of length<>1";
                case daveResItemNotAvailable: return "the desired item is not available in the PLC";
                case daveResItemNotAvailable200: return "the desired item is not available in the PLC (200 family)";
                case daveAddressOutOfRange: return "the desired address is beyond limit for this PLC";
                case daveResCPUNoData: return "the PLC returned a packet with no result data";
                case daveUnknownError: return "the PLC returned an error code not understood by this library";
                case daveEmptyResultError: return "this result contains no data";
                case daveEmptyResultSetError: return "cannot work with an undefined result set";
                case daveResCannotEvaluatePDU: return "cannot evaluate the received PDU";
                case daveWriteDataSizeMismatch: return "Write data size error";
                case daveResNoPeripheralAtAddress: return "No data from I/O module";
                case daveResUnexpectedFunc: return "Unexpected function code in answer";
                case daveResUnknownDataUnitSize: return "PLC responds with an unknown data type";

                case daveResShortPacket: return "Short packet from PLC";
                case daveResTimeout: return "Timeout when waiting for PLC response";
                case daveResNoBuffer: return "No buffer provided";
                case daveNotAvailableInS5: return "Function not supported for S5";

                case 0x8000: return "function already occupied.";
                case 0x8001: return "not allowed in current operating status.";
                case 0x8101: return "hardware fault.";
                case 0x8103: return "object access not allowed.";
                case 0x8104: return "context is not supported. Step7 says:Function not implemented or error in telgram.";
                case 0x8105: return "invalid address.";
                case 0x8106: return "data type not supported.";
                case 0x8107: return "data type not consistent.";
                case 0x810A: return "object does not exist.";
                case 0x8301: return "insufficient CPU memory ?";
                case 0x8402: return "CPU already in RUN or already in STOP ?";
                case 0x8404: return "severe error ?";
                case 0x8500: return "incorrect PDU size.";
                case 0x8702: return "address invalid."; ;
                case 0xd002: return "Step7:variant of command is illegal.";
                case 0xd004: return "Step7:status for this command is illegal.";
                case 0xd0A1: return "Step7:function is not allowed in the current protection level.";
                case 0xd201: return "block name syntax error.";
                case 0xd202: return "syntax error function parameter.";
                case 0xd203: return "syntax error block type.";
                case 0xd204: return "no linked block in storage medium.";
                case 0xd205: return "object already exists.";
                case 0xd206: return "object already exists.";
                case 0xd207: return "block exists in EPROM.";
                case 0xd209: return "block does not exist/could not be found.";
                case 0xd20e: return "no block present.";
                case 0xd210: return "block number too big.";
                //	case 0xd240: return "unfinished block transfer in progress?";  // my guess
                case 0xd240: return "Coordination rules were violated.";
                /*  Multiple functions tried to manipulate the same object.
                Example: a block could not be copied,because it is already present in the target system
                and
                */
                case 0xd241: return "Operation not permitted in current protection level.";
                /**/
                case 0xd242: return "protection violation while processing F-blocks. F-blocks can only be processed after password input.";
                case 0xd401: return "invalid SZL ID.";
                case 0xd402: return "invalid SZL index.";
                case 0xd406: return "diagnosis: info not available.";
                case 0xd409: return "diagnosis: DP error.";
                case 0xdc01: return "invalid BCD code or Invalid time format?";

                default: return "no message defined!";
            }
        }
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
