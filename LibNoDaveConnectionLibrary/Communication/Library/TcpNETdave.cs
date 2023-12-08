using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library
{
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct daveBlockEntry
    {
        private ushort number;
        private byte type1;
        private byte type2;
    }

    public class resultN
    {
        public int error;
        public byte[] bytes;
    }

    public class daveResultN : IresultSet
    {
        public List<resultN> allResults;

        public daveResultN()
        {
            allResults = new List<resultN>();
        }

        public IntPtr pointer
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public int getErrorOfResult(int number)
        {
            throw new NotImplementedException();
        }
    }

    public class TcpNETdave : IDaveConnection, IDisposable
    {
        private string plc_ip;
        private int rack, slot;
        private int connection_port;
        private TcpClient tcpClient;
        private int maxPDUlength;
        private PLCConnectionConfiguration config;
        private int AnswLen;	/* length of last message */

        //public delegate void TegramRecievedEventHandler(byte[] telegramm);
        //public event TegramRecievedEventHandler TelegrammRecievedSend;
        //private SynchronizationContext context;
        private object lockpdu = new object();

        public TcpNETdave(PLCConnectionConfiguration conf)//string ip, int port, int con_rack, int con_slot)
        {
            //msgIn = new byte[daveMaxRawLen];
            config = conf;
            connection_port = conf.Port;
            rack = conf.CpuRack;
            slot = conf.CpuSlot;
            plc_ip = conf.CpuIP;
            //context = new SynchronizationContext();
        }

        public int connectPLC()
        {
            //IPEndPoint ipLocalEndPoint = new IPEndPoint(plc_ip, connection_port);
            maxPDUlength = 0;
            tcpClient = new TcpClient(plc_ip.ToString(), connection_port);// (ipLocalEndPoint);
            tcpClient.SendTimeout = 5000;
            tcpClient.ReceiveTimeout = 5000;
            //send Setup Communication
            int success, retries;
            byte[] b4 ={
                0x11,		//Length
        		0xE0,		// TDPU Type CR = Connection Request (see RFC1006/ISO8073)
        		0x00, 0x00, // TPDU Destination Reference (unknown)
        		0x00, 0x01, // TPDU Source-Reference (my own reference, should not be zero)
        		0x00,		// TPDU Class 0 and no Option
        		0xC1,		// Parameter Source-TSAP
        		2,			// Length of this parameter
        		1, 			// Function (1=PG,2=OP,3=Step7Basic)
        		0,			// Rack (Bit 7-5) and Slot (Bit 4-0)
        		0xC2,		// Parameter Destination-TSAP
        		2,			// Length of this parameter
        		1,//dc->ConnectionType, 			// Function (1=PG,2=OP,3=Step7Basic)
        		(byte)(slot + rack * 32),			// Rack (Bit 7-5) and Slot (Bit 4-0)
        		0xC0,		// Parameter requested TPDU-Size
        		1,			// Length of this parameter
        		9			// requested TPDU-Size 8=256 Bytes, 9=512 Bytes , a=1024 Bytes
	        };

            /*   byte[] b4R ={			// for routing
           6 + 30 + 30 + 3,	// Length over all without this byte (fixed
           // Data 6 Bytes + size of Parameters (3 for C0h,30 for C1h+C2h)

           0xE0,		// TDPU Type CR = Connection Request (see RFC1006/ISO8073)
           0x00,0x00,	// TPDU Destination Reference (unknown)
           0x00,0x01,	// TPDU Source-Reference (my own reference, should not be zero)
           0x00,		// TPDU Class 0 and no Option

           0xC1,		// Parameter Source-TSAP
           28,		// Length of this parameter
           1,		// one block of data (???)
           0,		// Length for S7-Subnet-ID
           0,		// Length of PLC-Number
           2,		// Length of Function/Rack/Slot
           0,0,0,0,0,0,0,0,	// empty Data
           0,0,0,0,0,0,0,0,
           0,0,0,0,0,0,
           (byte)config.ConnectionType,		// Function (1=PG,2=OP,3=Step7Basic)
           (byte)(slot + rack * 32),		// Rack (Bit 7-5) and Slot (Bit 4-0)

           0xC2,		// Parameter Destination-TSAP
           28,		// Length of this parameter
           1,		// one block of data (???)
           6,		// Length for S7-Subnet-ID
            dc->_routingDestinationSize,		// Length of PLC-Number - 04 if you use a IP as Destination!
           2,		// Length of Function/Rack/Slot

           0,//(unsigned char) (dc->routingSubnetFirst >> 8), (unsigned char) dc->routingSubnetFirst,	// first part of S7-Subnet-ID
           // (look into the S7Project/Network configuration)
           0x00,0x00,		// fix always 0000 (reserved for later use ?)
           0,//(unsigned char) (dc->routingSubnetSecond >> 8), (unsigned char) dc->routingSubnetSecond,		// second part of S7-Subnet-ID
           // (see S7Project/Network configuration)

           0,//dc->_routingDestination1,			// PLC-Number (0-126) or IP Adress (then 4 Bytes are used)
           0,//dc->_routingDestination2,
           0,//dc->_routingDestination3,
           0,//dc->_routingDestination4,

           0,0,0,0,0,	// empty
           0,0,0,0,0,0,0,

           1,//dc->routingConnectionType,		// Function (1=PG,2=OP,3=Step7Basic)
           0,//(dc->routingSlot + dc->routingRack*32),		// Rack (Bit 7-5) and Slot (Bit 4-0)
           // 0 for slot = let select the plc itself the correct slotnumber

           0xC0,		// Parameter requested TPDU-Size
           1,		// Length of this parameter
           9		// requested TPDU-Size 8=256 Bytes, 9=512 Bytes , a=1024 Bytes
       };

         /*      byte[] b243 ={
           0x11,0xE0,0x00,
           0x00,0x00,0x01,0x00,
           0xC1,2,(byte)'M',(byte)'W',
           0xC2,2,(byte)'M',(byte)'W',
           0xC0,1,9
       };*/

            //Pdu p1 = new Pdu(1);
            success = 0;
            retries = 0;
            //p1.Param = new List<byte>(b4);
            //memcpy(dc->msgOut+4, b4, sizeof(b4));

            if (!config.Routing)
                sendISOPacket(b4);
            //            else
            //                sendISOPacket(b4R);

            byte[] ret;
            do
            {
                ret = ReceiveData();
                if (ret != null && ret.Length == 22)
                {
                    success = 1;
                }
                retries++;
            } while ((success == 0) && (retries < 3));
            if (success == 0) return -1;// throw new Exception("not connected");

            retries = 0;
            do
            {
                maxPDUlength = NegPDUlengthRequest();
                retries++;
            } while (maxPDUlength == 0 && retries < 3);
            if (maxPDUlength == 0) return -1;// throw new Exception("PDU length not found");
            return Connection.daveResOK;
        }

        public int daveBuildAndSendPDU(IPDU myPDU, byte[] Parameter, byte[] Data)
        {
            throw new NotImplementedException();
        }

        public int daveGetPDUData(IPDU myPDU, out byte[] data, out byte[] param)
        {
            throw new NotImplementedException();
        }

        public int disconnectPLC()
        {
            Dispose();
            return 0;
        }

        public bool Connected()
        {
            return tcpClient != null ? tcpClient.Connected : false;
        }

        public void SendData(byte[] telegramm)
        {
            if (tcpClient == null)
                throw new Exception("Send not possible, not connected");
            NetworkStream stream = tcpClient.GetStream();
            if (stream.CanWrite)
                stream.Write(telegramm, 0, telegramm.Length);
            else throw new Exception("can not send");
        }

        public byte[] ReceiveData()
        {
            byte[] bytes = new byte[4];
            NetworkStream stream = null;
            int len = 0;
            try
            {
                if (stream == null)
                    stream = tcpClient.GetStream();
                if (stream.CanRead)
                    len = stream.Read(bytes, 0, 4);

                if (len > 3)
                {
                    int size = bytes[3] + 0x100 * bytes[2];
                    byte[] gesbytes = new byte[size];
                    if (stream.CanRead)
                    {
                        len = stream.Read(gesbytes, 4, size - 4);
                        Array.Copy(bytes, 0, gesbytes, 0, 4);
                    }

                    //if (TelegrammRecievedSend != null && len > 0)
                    //    context.Post(delegate { TelegrammRecievedSend(gesbytes); }, null);
                    if (len > 0)
                        return gesbytes;
                }
                //stream.Dispose();
            }
            catch (Exception)
            {
                Console.WriteLine("1 TcpNetDave.cs threw exception");
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
            return null;
        }

        private ushort pduNr = 1;

        private Pdu ExchangePdu(Pdu myPdu)
        {
            ushort pduNrInt;
            byte[] ret;
            lock (lockpdu)
            {
                pduNr++;
                if (pduNr >= ushort.MaxValue)
                    pduNr = 1;

                pduNrInt = pduNr;

                myPdu.header.number = pduNr;

                //int res;
                //*(dc->msgOut + 6) = 0x80;
                //*(dc->msgOut + 5) = 0xf0;
                //*(dc->msgOut + 4) = 0x02;
                //_daveSendISOPacket(dc, 3 + p->hlen + p->plen + p->dlen);
                byte[] pdu_b = myPdu.ToBytes();
                byte[] _message = new byte[pdu_b.Length + 3];
                _message[0] = 0x02;
                _message[1] = 0xf0;
                _message[2] = 0x80;
                Array.Copy(pdu_b, 0, _message, 3, pdu_b.Length);
                sendISOPacket(_message);
                ret = ReceiveData();
                if (ret != null && ret.Length == 7)
                {
                    //if (daveDebug & daveDebugByte)
                    //    LOG1("CPU sends funny 7 byte packets.\n");
                    ret = ReceiveData();
                }
                if (ret == null) return new Pdu(); //daveResTimeout;
                /*if (daveDebug & daveDebugExchange)
                {
                    LOG3("%s _daveExchangeTCP res from read %d\n", dc->iface->name, res);
                }*/
                if (ret.Length <= daveConst.ISOTCPminPacketLength) return new Pdu(); //daveResShortPacket;

                //Interface.ExchangePdu(myPdu, this);
            }

            //Todo: maybe implement a Timeout here!
            //while (RecievedPdus[pduNrInt] == null)
            //{ }
            byte[] res = new byte[ret.Length - 7];
            Array.Copy(ret, 7, res, 0, ret.Length - 7);
            Pdu retVal = new Pdu(res);
            //RecievedPdus.Remove(pduNrInt);
            return retVal;
        }

        private void sendISOPacket(byte[] message)
        {
            byte[] _message = new byte[message.Length + 4];
            _message[0] = 0x03;
            _message[1] = 0x0;
            _message[2] = (byte)(_message.Length / 0x100);
            _message[3] = (byte)(_message.Length % 0x100);

            Array.Copy(message, 0, _message, 4, message.Length);

            SendData(_message);
        }

        /*private byte[] readISOPacket()
        {
            //byte[] res = readPacket();
            return ReceiveData();
        }*/

        public int resetIBH()
        {
            throw new NotImplementedException();
        }

        public int start()
        {
            Pdu pdu = new Pdu(1);
            byte[] para = new byte[] { 0x28, 0, 0, 0, 0, 0, 0, 0xFD, 0, 0x00, 9, (byte)'P', (byte)'_', (byte)'P', (byte)'R', (byte)'O', (byte)'G', (byte)'R', (byte)'A', (byte)'M' };
            pdu.Param.AddRange(para);

            Pdu rec = ExchangePdu(pdu);

            return 0;
        }

        public int stop()
        {
            Pdu pdu = new Pdu(1);
            byte[] para = new byte[] { 0x29, 0, 0, 0, 0, 0, 9, (byte)'P', (byte)'_', (byte)'P', (byte)'R', (byte)'O', (byte)'G', (byte)'R', (byte)'A', (byte)'M' };
            pdu.Param.AddRange(para);

            Pdu rec = ExchangePdu(pdu);

            return 0;
        }

        private int NegPDUlengthRequest()
        {
            byte[] pa = { 0xF0, 0, 0, 1, 0, 1, 3, 0xC0 };
            int CpuPduLimit = 0;
            Pdu p1 = new Pdu(1);

            p1.Param.AddRange(pa);

            var p2 = ExchangePdu(p1);
            if (p2 != null)
                CpuPduLimit = ByteFunctions.getU16from(p2.Param.ToArray(), 6);

            // daveGetU16from(p2.param+6);
            return CpuPduLimit;
        }

        public int daveReadPLCTime(out DateTime dateTime)
        {
            //	int res, len;
            Pdu p2 = new Pdu(7);
            byte[] pa = { 0, 1, 18, 4, 17, (byte)'G', 1, 0 };
            //	len=0;
            var ret = _daveBuildAndSendPDU(p2, pa, null, true);
            /*	if (res==daveResOK) {
                    dc->resultPointer=p2.udata;
                    dc->_resultPointer=p2.udata;
                    len=p2.udlen;
                } else {
                    if(daveDebug & daveDebugPrintErrors)
                        LOG3("daveGetTime: %04X=%s\n",res, daveStrerror(res));
                }
                dc->AnswLen=len;*/
            AnswLen = p2.Data.Count - 4;
            int year, month, day, hour, minute, second, millisecond;
            //getU8();
            //getU8();
            //byte[] tmp = new byte[1];
            //tmp[0] = Convert.ToByte(getU8());
            byte[] res = p2.UData.ToArray();
            year = ByteFunctions.getBCD8from(res, 2);
            year += year >= 90 ? 1900 : 2000;
            //tmp[0] = Convert.ToByte(getU8());
            //month = getBCD8from(tmp, 0);
            month = ByteFunctions.getBCD8from(res, 3);
            //tmp[0] = Convert.ToByte(getU8());
            //day = getBCD8from(tmp, 0);
            day = ByteFunctions.getBCD8from(res, 4);
            //tmp[0] = Convert.ToByte(getU8());
            //hour = getBCD8from(tmp, 0);
            hour = ByteFunctions.getBCD8from(res, 5);
            //tmp[0] = Convert.ToByte(getU8());
            //minute = getBCD8from(tmp, 0);
            minute = ByteFunctions.getBCD8from(res, 6);
            //tmp[0] = Convert.ToByte(getU8());
            //second = getBCD8from(tmp, 0);
            second = ByteFunctions.getBCD8from(res, 7);
            //tmp[0] = Convert.ToByte(getU8());
            //millisecond = getBCD8from(tmp, 0) * 10;
            millisecond = ByteFunctions.getBCD8from(res, 8);
            //tmp[0] = Convert.ToByte(getU8());
            //tmp[0] = Convert.ToByte(tmp[0] >> 4);
            //millisecond += getBCD8from(tmp, 0);
            millisecond += ByteFunctions.getBCD8from(res, 9) >> 4;
            dateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
            return ret;
        }

        public int daveSetPLCTime(DateTime tm)
        {
            byte[] buffer = new byte[] { 0x00, 0x19, 0x05, 0x08, 0x23, 0x04, 0x10, 0x23, 0x67, 0x83, };
            ByteFunctions.putBCD8at(buffer, 2, tm.Year % 100);
            ByteFunctions.putBCD8at(buffer, 3, tm.Month);
            ByteFunctions.putBCD8at(buffer, 4, tm.Day);
            ByteFunctions.putBCD8at(buffer, 5, tm.Hour);
            ByteFunctions.putBCD8at(buffer, 6, tm.Minute);
            ByteFunctions.putBCD8at(buffer, 7, tm.Second);
            ByteFunctions.putBCD8at(buffer, 8, tm.Millisecond / 10);
            ByteFunctions.putBCD8at(buffer, 9, (tm.Millisecond % 10) << 4);

            int res;
            Pdu p2 = new Pdu(1);
            byte[] pa = { 0, 1, 18, 4, 17, (byte)'G', 2, 0 };
            //len=0;
            res = daveBuildAndSendPDU(p2, pa, buffer);
            AnswLen = p2.UData.Count;
            return res;
        }

        public int daveBuildAndSendPDU(Pdu myPDU, byte[] para, byte[] data)
        {
            return _daveBuildAndSendPDU(myPDU, para, data, false);
        }

        public int _daveBuildAndSendPDU(Pdu myPDU, byte[] para, byte[] data, bool nullD)
        {
            //int res;
            Pdu p = new Pdu(7);
            byte[] nullData = { 0x0a, 0, 0, 0 };
            //p.header=dc->msgOut+dc->PDUstartO;
            //_daveInitPDUheader(&p, 7);

            //_daveAddParam(&p, pa, psize);

            p.Param.AddRange(para);

            if (data != null) p.UData.AddRange(data);
            else
                if (nullD) p.Data.AddRange(nullData);//_daveAddData(&p, nullData, 4);

            Pdu rez = ExchangePdu(p);
            if (rez != null)
            {
                myPDU.Data = rez.Data;
                myPDU.Param = rez.Param;
                myPDU.initUData();

                return myPDU.testPGReadResult();
            }
            return -1;
        }

        private int BuildAndSendPDU(Pdu p2, byte[] pa, byte[] ud, byte[] ud2)
        {
            //int res;
            Pdu p = new Pdu(7);
            byte[] dn;
            //p.header=dc->msgOut+dc->PDUstartO;
            //_daveInitPDUheader(&p, 7);
            //_daveAddParam(&p, pa, psize);
            p.Param.AddRange(pa);

            //_daveAddUserData(&p, ud, usize);
            p.UData.AddRange(ud);
            //    LOG2("*** here we are: %d\n",p.dlen);
            //p3=&p;
            //dn= p3->data+p3->dlen;
            //p3->dlen+=usize2;
            //memcpy(dn, ud2, usize2);
            p.UData.AddRange(ud2);

            //((PDUHeader*)p3->header)->dlen=daveSwapIed_16(p3->dlen);

            Pdu rez = ExchangePdu(p);
            if (rez != null)
            {
                p2.Data = rez.Data;
                p2.Param = rez.Param;
                p2.initUData();

                return p2.testPGReadResult();
            }
            return -1;
        }

        public int daveRecieveData(out byte[] rdata, out byte[] rparam)
        {
            //int res;
            byte[] msgBuffer;
            getGetResponse(out msgBuffer);

            Pdu myPDU = new Pdu(msgBuffer);
            //if (IntPtr.Size == 8)
            //    _daveSetupReceivedPDU64(pointer, myPDU.pointer);
            //else
            //    _daveSetupReceivedPDU32(pointer, myPDU.pointer);

            //byte[] tmp1;// = new byte[65536];
            //byte[] tmp2;// = new byte[65536];
            //int ltmp1 = 0;
            //int ltmp2 = 0;

            //if (IntPtr.Size == 8)
            //    res = daveGetPDUData64(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);
            //else
            //    res = daveGetPDUData32(pointer, myPDU.pointer, tmp1, ref ltmp1, tmp2, ref ltmp2);
            return /*res =*/ daveGetPDUData(myPDU, out rdata, out rparam);

            //rdata = new byte[ltmp1];
            //rparam = new byte[ltmp2];
            //Array.Copy(tmp1, data, ltmp1);
            //Array.Copy(tmp2, param, ltmp2);
            //return res;
        }

        private int getGetResponse(out byte[] buffer)
        {
            byte[] msgIn = ReceiveData();
            buffer = new byte[10];
            if (msgIn != null && msgIn.Length == 7)
            {
                msgIn = ReceiveData();
            }
            if (msgIn == null) return daveConst.daveResTimeout;
            Array.Copy(buffer, msgIn, msgIn.Length);
            if (msgIn.Length < daveConst.ISOTCPminPacketLength) return daveConst.daveResShortPacket;
            return 0;
        }

        public int daveGetPDUData(Pdu mPDU, out byte[] rdata, out byte[] rparam)
        {
            Pdu myPDU = mPDU as Pdu;
            //int res = 0;
            //memcpy(data, p2->data, p2->dlen);
            //*ldata = p2->dlen;
            rdata = myPDU.Data.ToArray();
            //Array.Copy(myPDU.Data.ToArray(), rdata, myPDU.Data.Count);
            //memcpy(param, p2->param, p2->plen);
            //*lparam = p2->plen;
            rparam = myPDU.Param.ToArray();

            return 0;
        }

        public int readSZL(int id, int index, byte[] buffer)
        {
            //int  daveReadSZL(daveConnection * dc, int ID, int index, void * buffer, int buflen) {
            int res, len, cpylen;
            byte pa7;
            //    int pa6;
            Pdu p2 = new Pdu();
            byte[] pa = { 0, 1, 18, 4, 17, 68, 1, 0 };
            byte[] da = { 1, 17, 0, 1 };

            byte[] pam = { 0, 1, 18, 8, 18, 68, 1, 1, 0, 0, 0, 0 };
            //    uc dam[]={10,0,0,0};

            da[0] = (byte)(id / 0x100);
            da[1] = (byte)(id % 0x100);
            da[2] = (byte)(index / 0x100);
            da[3] = (byte)(index % 0x100);
            res = daveBuildAndSendPDU(p2, pa, da);
            if (res != Connection.daveResOK) return res; 	// bugfix from Natalie Kather

            len = 0;
            pa7 = p2.Param[7];
            //    pa6=p2.param[6];
            while (p2.Param[9] != 0)
            {
                if (buffer != null)
                {
                    cpylen = p2.UData.Count;
                    if (len + cpylen > buffer.Length) cpylen = buffer.Length - len;
                    if (cpylen > 0) Array.Copy(p2.UData.ToArray(), 0, buffer, len, cpylen);// memcpy((uc *)buffer+len,p2.udata,cpylen);
                }
                //dc->resultPointer=p2.udata;
                //dc->_resultPointer=p2.udata;
                len += p2.UData.Count;
                pam[7] = pa7;
                ////		res=daveBuildAndSendPDU(dc, &p2,pam, sizeof(pam), NULL, sizeof(dam));
                res = _daveBuildAndSendPDU(p2, pam, null, true);
                if (res != Connection.daveResOK) return res; 	// bugfix from Natalie Kather
            }

            if (buffer != null)
            {
                cpylen = p2.UData.Count;
                if (len + cpylen > buffer.Length) cpylen = buffer.Length - len;
                if (cpylen > 0) Array.Copy(p2.UData.ToArray(), 0, buffer, len, cpylen);//memcpy((uc *)buffer+len,p2.udata,cpylen);
            }
            //dc->resultPointer=p2.udata;
            //dc->_resultPointer=p2.udata;
            len += p2.UData.Count;
            AnswLen = len;
            return 0;
        }

        public int getU8()
        {
            throw new NotImplementedException();
        }

        public int ListBlocksOfType(int blockType, byte[] buffer)
        {
            //int DECL2 daveListBlocksOfType(daveConnection * dc,uc type,daveBlockEntry * buf) {
            int res, i, len;
            Pdu p2 = new Pdu();
            //uc * buffer=(uc*)buf;
            byte[] pa = { 0, 1, 18, 4, 17, 67, 2, 0 };
            byte[] da = { (byte)'0', (byte)'0' };
            byte[] pam = { 0, 1, 18, 8, 0x12, 0x43, 2, 1, 0, 0, 0, 0 };
            da[1] = (byte)blockType;
            res = daveBuildAndSendPDU(p2, pa, da);
            if (res != Connection.daveResOK) return -res;
            len = 0;
            while (p2.Param[9] != 0)
            {
                if (buffer != null) Array.Copy(p2.UData.ToArray(), 0, buffer, len, p2.UData.Count);// memcpy(buffer + len, p2.udata, p2.udlen);
                //dc->resultPointer=p2.udata;
                //dc->_resultPointer=p2.udata;
                len += p2.UData.Count;
                //printf("more data\n");
                pam[7] = p2.Param[7];
                res = _daveBuildAndSendPDU(p2, pam, null, true);
                if (res == 0xa) break;
                if (res != Connection.daveResOK) return res;
            }

            //if (res==daveResOK) {
            if (buffer != null) Array.Copy(p2.UData.ToArray(), 0, buffer, len, p2.UData.Count); //memcpy(buffer + len, p2.udata, p2.udlen);
            //dc->resultPointer=p2.udata;
            //dc->_resultPointer=p2.udata;
            len += p2.UData.Count;
            //} else {
            //	if(daveDebug & daveDebugPrintErrors)
            //		LOG3("daveListBlocksOfType: %d=%s\n",res, daveStrerror(res));
            //}
            AnswLen = len;
            res = len / System.Runtime.InteropServices.Marshal.SizeOf(typeof(daveBlockEntry));

            //for (i = 0; i < res; i++)
            //{
            //    buf[i].number=daveSwapIed_16(buf[i].number);
            // }
            return res;
        }

        private int initUpload(byte blockType, int blockNr, ref int uploadID)
        {
            Pdu p1 = new Pdu(); ;
            //p1.header=dc->msgOut+dc->PDUstartO;
            _daveConstructUpload(p1, blockType, blockNr);
            Pdu ret = ExchangePdu(p1);

            //if(res!=daveResOK) return res;
            //res=daveSetupReceivedPDU(dc, &p2);
            //if(res!=daveResOK) return res;
            uploadID = ret.Param[7];
            return 0;
        }

        /*
        Functions to load blocks from PLC:
        */

        private void _daveConstructUpload(Pdu p, byte blockType, int blockNr)
        {
            byte[] pa = {0x1d,
                 0,0,0,0,0,0,0,9,0x5f,0x30,0x41,48,48,48,48,49,65};
            pa[11] = blockType;
            //sprintf((char*)(pa+12),"%05d",blockNr);
            sprintf(pa, 12, 5, blockNr);
            pa[17] = (byte)'A';
            //_daveInitPDUheader(p,1);
            //_daveAddParam(p, pa, sizeof(pa));
            p.Param.AddRange(pa);
        }

        private void _daveConstructDoUpload(Pdu p, int uploadID)
        {
            byte[] pa = { 0x1e, 0, 0, 0, 0, 0, 0, 1 };
            pa[7] = (byte)uploadID;
            //_daveInitPDUheader(p,1);
            //_daveAddParam(p, pa, sizeof(pa));
            p.Param.AddRange(pa);
        }

        private void _daveConstructEndUpload(Pdu p, int uploadID)
        {
            byte[] pa = { 0x1f, 0, 0, 0, 0, 0, 0, 1 };
            pa[7] = (byte)uploadID;
            //_daveInitPDUheader(p,1);
            //_daveAddParam(p, pa, sizeof(pa));
            p.Param.AddRange(pa);
        }

        private int doUpload(ref int more, byte[] buffer, ref int len, int uploadID)
        {
            Pdu p1 = new Pdu();
            int netLen;
            _daveConstructDoUpload(p1, uploadID);
            Pdu ret = ExchangePdu(p1);
            //res=_daveExchange(dc, &p1);
            more = 0;

            //	if(res!=daveResOK) return res;
            //res=_daveSetupReceivedPDU(dc, &p2);
            more = ret.Param[1];
            //if(res!=daveResOK) return res;
            //    netLen=p2.data[1] /* +256*p2.data[0]; */ /* for long PDUs, I guess it is so */;
            netLen = ret.Data[1] + 256 * ret.Data[0]; /* some user confirmed my guess... */;
            if (buffer != null)
            {
                Array.Copy(ret.UData.ToArray(), 0, buffer, len, netLen);
                //memcpy(*buffer,p2.data+4,netLen);
                //*buffer+=netLen;?????????????????????????????????
            }
            len += netLen;
            return 0;
        }

        private int endUpload(int uploadID)
        {
            Pdu p1 = new Pdu();
            int res;

            //p1.header=dc->msgOut+dc->PDUstartO;
            _daveConstructEndUpload(p1, uploadID);
            Pdu ret = ExchangePdu(p1);
            //res=_daveExchange(dc, &p1);
            //if(res!=daveResOK) return res;
            //res=_daveSetupReceivedPDU(dc, &p2);
            return 0;
        }

        public int getMessage(IPDU p)
        {
            throw new NotImplementedException();
        }

        public int getProgramBlock(int blockType, int number, byte[] buffer, ref int length)
        {
            //int DECL2 daveGetProgramBlock(daveConnection * dc, int blockType, int number, char* buffer, int * length) {
            int res, uploadID, more, totlen;
            //uc *bb=(uc*)buffer;	//cs: is this right?
            totlen = 0;
            uploadID = 0;
            more = 0;

            res = initUpload((byte)blockType, number, ref uploadID);
            if (res != 0) return res;
            do
            {
                res = doUpload(ref more, buffer, ref totlen, uploadID);
                //totlen+=len;
                //if (res!=0) return res;
            } while (more != 0);
            res = endUpload(uploadID);
            length = totlen;
            return res;
        }

        private int daveGetPDUerror(Pdu p)
        {
            if (p.header.type == 2 || p.header.type == 3)
            {
                return ByteFunctions.getU16from(p.header.result, 0);
            }
            else
                return 0;
        }

        public IPDU createPDU()
        {
            return new Pdu();
        }

        public int putProgramBlock(int blockType, int blknumber, byte[] buffer, int length)
        {
            //int DECL2 davePutProgramBlock(daveConnection * dc, int blockType, int blknumber, char* buffer, int * length) {
            int maxPBlockLen = 0xDe;	// real maximum 222 bytes

            int res = 0;
            int cnt = 0;
            int size = 0;
            int blockNumber, rawLen, netLen, blockCont;
            int number = 0;

            byte[] pup = {			// Load request
            		0x1A,0,1,0,0,0,0,0,9,
                    0x5F,0x30,0x42,0x30,0x30,0x30,0x30,0x34,0x50, // block type code and number
            		//     _    0    B   0     0    0    0    4    P
            		//		SDB
            		0x0D,
                    0x31,0x30,0x30,0x30,0x32,0x30,0x38,0x30,0x30,0x30,0x31,0x31,0x30,0	// file length and netto length
            		//     1   0     0    0    2    0    8    0    0    0    1    1    0
            	};
            byte[] paInsert = {		// sended after transmission of a complete block,
            	// I guess this makes the CPU link the block into a program.
            	0x28,0,0,0,0,0,0,0xFD,0,0x0A,1,0,0x30,0x42,0x30,0x30,0x30,0x30,0x34,0x50, // block type code and number
            	0x05,(byte)'_',(byte)'I',(byte)'N',(byte)'S',(byte)'E'
            };

            Pdu p = new Pdu();

            byte[] pablock = {	// parameters for parts of a block
            		0x1B,0
                };

            byte[] progBlock = new byte[maxPBlockLen + 4];
            progBlock[0] = 0;
            progBlock[1] = (byte)maxPBlockLen;
            progBlock[2] = 0;
            progBlock[3] = 0xFB;
            //{		0,maxPBlockLen,0,0xFB,	// This seems to be a fix prefix for program blocks	};

            pup[11] = (byte)blockType;
            paInsert[13] = (byte)blockType;
            /*pup[12] = number / (10*10*10*10);
            pup[13] = (number - (pup[12] * 10*10*10*10 )) / (10*10*10);
            pup[14] = (number - (pup[13] * 10*10*10)) / (10*10);
            pup[15] = (number - (pup[14] * 10*10)) / (10);
            pup[16] = (number - (pup[15] * 10));

            pup[12] = pup[12] + 0x30;
            pup[13] = pup[13] + 0x30;
            pup[14] = pup[14] + 0x30;
            pup[15] = pup[15] + 0x30;
            pup[16] = pup[16] + 0x30;*/

            //memcpy(progBlock+4,buffer,maxPBlockLen);
            Array.Copy(buffer, 0, progBlock, 4, maxPBlockLen);

            progBlock[9] = (byte)(blockType + 0x0A - 'A'); //Convert 'A' to 0x0A
            if (blockType == '8') progBlock[9] = 0x08;

            progBlock[10] = (byte)(blknumber / 0x100);
            progBlock[11] = (byte)(blknumber - (progBlock[10] * 0x100));

            rawLen = ByteFunctions.getU16from(progBlock, 14);
            netLen = ByteFunctions.getU16from(progBlock, 38);

            //sprintf((char*)pup+19,"1%06d%06d",rawLen,netLen);
            pup[19] = (byte)'1';
            sprintf(pup, 20, 6, rawLen);
            sprintf(pup, 26, 6, netLen);

            //sprintf((char*)pup+12,"%05d",blknumber);
            sprintf(pup, 12, 5, blknumber);
            //sprintf((char*)paInsert+14,"%05d",blknumber);
            sprintf(paInsert, 14, 5, blknumber);

            pup[17] = (byte)'P';
            paInsert[19] = (byte)'P';

            //p.header=dc->msgOut+dc->PDUstartO;
            //_daveInitPDUheader(&p, 1);
            //_daveAddParam(&p, pup, sizeof(pup)-1);
            p.Param.AddRange(pup);

            var p2 = ExchangePdu(p);
            //	if (res==daveResOK) {
            //res=_daveSetupReceivedPDU(dc, &p2);
            res = daveGetPDUerror(p2);

            if (res == 0)
            {
                blockCont = 1;
                byte[] msgBuffer;

                res = getGetResponse(out msgBuffer);
                //res=_daveSetupReceivedPDU(dc, &p2);
                p2 = new Pdu(msgBuffer);

                cnt = 0;

                do
                {
                    res = 0;
                    //res=_daveSetupReceivedPDU(dc, &p2);
                    //p2 = new Pdu(msgIn);

                    number = p2.header.number;
                    if (p2.Param[0] == 0x1B)
                    {
                        //READFILE
                        Array.Copy(buffer, cnt * maxPBlockLen, progBlock, 4, maxPBlockLen);
                        //memcpy(progBlock+4,buffer+(cnt*maxPBlockLen),maxPBlockLen);

                        if (cnt == 0)
                        {
                            progBlock[9] = (byte)(blockType + 0x0A - 'A'); //Convert 'A' to 0x0A
                            if (blockType == '8') progBlock[9] = 0x08;

                            progBlock[10] = (byte)(blknumber / 0x100);
                            progBlock[11] = (byte)(blknumber - (progBlock[10] * 0x100));
                        }

                        //p.header=dc->msgOut+dc->PDUstartO;
                        //_daveInitPDUheader(&p, 3);
                        p = new Pdu(3);
                        size = maxPBlockLen;

                        if (length > ((cnt + 1) * maxPBlockLen))
                            pablock[1] = 1;
                        else
                        {
                            size = length - (cnt * maxPBlockLen);
                            pablock[1] = 0;	//last block
                            blockCont = 0;
                        }

                        progBlock[1] = (byte)size;
                        //_daveAddParam(&p, pablock, sizeof(pablock));
                        p.Param.AddRange(pablock);
                        //_daveAddData(&p, progBlock, size + 4 /* size of block) */);
                        byte[] dataA = new byte[size + 4];
                        Array.Copy(progBlock, dataA, size + 4);
                        p.Data.AddRange(dataA);

                        p.header.number = (ushort)number;
                        //_daveExchange(dc,&p);
                        p2 = ExchangePdu(p);
                    }
                    cnt++;
                } while (blockCont != 0);

                //res=_daveSetupReceivedPDU(dc, &p2);
                number = p2.header.number;
                if (p2.Param[0] == 0x1C)
                {
                    //p.header=dc->msgOut+dc->PDUstartO;

                    //_daveInitPDUheader(&p, 3);
                    p = new Pdu(3);
                    //_daveAddParam(&p, p2.param,1);
                    p.Param.AddRange(p2.Param);
                    p.header.number = (ushort)number;
                    //_daveExchange(dc,&p);
                    ExchangePdu(p);

                    //p.header=dc->msgOut+dc->PDUstartO;
                    //_daveInitPDUheader(&p, 1);
                    p = new Pdu(1);
                    //_daveAddParam(&p, paInsert, sizeof(paInsert));
                    p.Param.AddRange(paInsert);
                    //res=_daveExchange(dc, &p);
                    //res=_daveSetupReceivedPDU(dc, &p2);
                    p2 = ExchangePdu(p);
                    res = daveGetPDUerror(p2);
                }
            }
            else
            {
                throw new Exception(string.Format("CPU doesn't accept load request:{0:4X}\n", res));
            }
            return res;
        }

        public int readBits(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int deleteProgramBlock(int blockType, int number)
        {
            //int DECL2 daveDeleteProgramBlock(daveConnection*dc, int blockType, int number) {
            Pdu p = new Pdu();
            byte[] paDelete = {
                    0x28,0,0,0,0,0,0,0xFD,0,
                    0x0a,0x01,0x00,
                    (byte)'0',(byte)'C', //Block type in ASCII (0C = FC)
                	(byte)'0',(byte)'0',(byte)'0',(byte)'0',(byte)'1', //Block Number in ASCII
                	(byte)'B', //Direction?
                	0x05, //Length of Command
                	(byte)'_',(byte)'D',(byte)'E',(byte)'L',(byte)'E' //Command Delete
                	};

            paDelete[13] = (byte)blockType;
            //sprintf((char*)(paDelete+14),"%05d",number);
            sprintf(paDelete, 14, 5, number);
            paDelete[19] = (byte)'B'; //This is overriden by sprintf via 0x00 as String seperator!

            //p.header=dc->msgOut+dc->PDUstartO;
            //_daveInitPDUheader(&p, 1);
            //_daveAddParam(&p, paDelete, sizeof(paDelete));
            p.Param.AddRange(paDelete);
            //res = _daveExchange(dc, &p);
            var p2 = ExchangePdu(p);
            //    if (res == daveResOK)
            //    {
            //		res=_daveSetupReceivedPDU(dc, &p2);
            //		if (daveDebug & daveDebugPDU) {
            //			_daveDumpPDU(&p2);
            //		}
            //	}

            //Retval of 0x28 in Recieved PDU Parameter Part means delete was sucessfull.
            //This needs to be implemneted and also error Codes Like Block does not exist or block is locked and so on...
            return 0;
        }

        public int getGetResponse()
        {
            throw new NotImplementedException();
        }

        public int getMaxPDULen()
        {
            return maxPDUlength == 0 ? (maxPDUlength = NegPDUlengthRequest()) : maxPDUlength;
        }

        public IPDU prepareReadRequest()
        {
            return new Pdu_ReadRequest();
        }

        /*public IresultSet getResultSet()
        {
            return new libnodave.resultSet();
        }*/

        public IresultSet getResultSet()
        {
            return new daveResultN();
        }

        /*public int execReadRequest(IPDU p, IresultSet rl)//!!!!!!!!!!!!!!!!!! for delete
        {
            return 0;
        }*/

        public int execReadRequest(IPDU p, IresultSet rl)
        {
            //int DECL2 daveExecReadRequest(daveConnection * dc, PDU *p, daveResultSet* rl){
            Pdu p2;
            byte[] q, qU;
            resultN cr;//, c2;
            int res, i, len, rlen;

            AnswLen = 0;	// 03/12/05
            //dc->resultPointer=NULL;
            //dc->_resultPointer=NULL;
            //res=_daveExchange(dc, p);
            p2 = ExchangePdu((Pdu)p);
            //if (res!=daveResOK) return res;
            //res=_daveSetupReceivedPDU(dc, &p2);
            //if (res!=daveResOK) return res;
            res = p2.testReadResultMulti();
            if (res != Connection.daveResOK) return res;
            i = 0;
            if (rl != null)
            {
                //cr=(daveResult*)calloc(p2.param[1], sizeof(daveResult));
                int numResults = p2.Param[1];
                //rl->results=cr;
                //c2=cr;
                q = p2.Data.ToArray();
                //qU = p2.UData.ToArray();
                int countQ = 0;

                rlen = p2.Data.Count;
                while (i < p2.Param[1])
                {
                    cr = new resultN();
                    /*	    printf("result %d: %d  %d %d %d\n",i, *q,q[1],q[2],q[3]); */
                    if ((q[countQ] == 255) && (rlen > 4))
                    {
                        len = q[countQ + 2] * 0x100 + q[countQ + 3];
                        if (q[countQ + 1] == 4)
                        {
                            len >>= 3;	/* len is in bits, adjust */
                        }
                        else if (q[countQ + 1] == 5)
                        {			/* Fehlenden Size-Type INTEGER ergдnzt */
                            len >>= 3;	/* len is in bits, adjust */
                        }
                        else if (q[countQ + 1] == 7)
                        {			/* Fehlenden Size-Type REAL ergдnzt */
                            /* len is already in bytes, ok */
                        }
                        else if (q[countQ + 1] == 9)
                        {
                            /* len is already in bytes, ok */
                        }
                        else if (q[countQ + 1] == 3)
                        {
                            /* len is in bits, but there is a byte per result bit, ok */
                        }
                        else
                        {
                            //if (daveDebug & daveDebugPDU)
                            //	LOG2("fixme: what to do with data type %d?\n",q[1]);
                        }
                    }
                    else
                    {
                        len = 0;
                    }
                    /*	    printf("Store result %d length:%d\n", i, len); */
                    //c2->length=len;
                    if (len > 0)
                    {
                        //c2.bytes=(uc*)malloc(len);
                        //memcpy(c2->bytes, q+4, len);
                        cr.bytes = new byte[len];
                        Array.Copy(q, countQ + 4, cr.bytes, 0, len);
                    }
                    cr.error = Connection.daveUnknownError;

                    if (q[countQ] == 0xFF)
                    {
                        cr.error = Connection.daveResOK;
                    }
                    else
                        cr.error = q[countQ];

                    /*	    printf("Error %d\n", c2->error); */
                    //q+=len+4;
                    countQ += len + 4;
                    rlen -= len;
                    if ((len % 2) == 1)
                    {
                        countQ++;
                        rlen--;
                    }
                    //c2++;
                    ((daveResultN)rl).allResults.Add(cr);
                    i++;
                }
            }
            return res;
        }

        public int useResult(IresultSet irs, int number, byte[] buffer)
        {
            var rs = irs as daveResultN;
            //int DECL2 daveUseResult(daveConnection * dc, daveResultSet * rl, int n, void * buffer){
            resultN dr;
            if (rs == null)
            {
                return Connection.daveEmptyResultSetError;
            }
            if (rs.allResults.Count == 0) return Connection.daveEmptyResultSetError;
            if (number >= rs.allResults.Count) return Connection.daveEmptyResultSetError;
            dr = rs.allResults[number];
            if (dr.error != 0) return dr.error;
            if (dr.bytes.Length <= 0) return Connection.daveEmptyResultError;

            if (buffer != null) Array.Copy(dr.bytes, buffer, dr.bytes.Length);// memcpy(buffer, dr->bytes, dr->length);
            //dc->resultPointer=dr->bytes;
            //dc->_resultPointer=dr->bytes;
            return 0;
        }

        public int useResultBuffer(IresultSet rs, int number, byte[] buffer)
        {
            return useResult(rs, number, buffer);
        }

        public int readManyBytes(int area, int DBnumber, int start, int len, ref byte[] buffer)
        {
            int res, readLen;
            int pos = 0;

            int pdulen = getMaxPDULen();

            while (len > 0)
            {
                if (len > pdulen - 18) readLen = pdulen - 18; else readLen = len;

                byte[] tmp = new byte[readLen];

                res = readBytes(area, DBnumber, start + pos, readLen, tmp);
                if (res != 0) return res;

                tmp.CopyTo(buffer, pos);

                len -= readLen;
                pos += readLen;
            }

            return 0;
        }

        public int writeBits(int area, int DB, int start, int len, byte[] buffer)
        {
            var p1 = new Pdu_WriteRequest();
            Pdu p2;
            //p1.header = dc->msgOut + dc->PDUstartO;
            //davePrepareWriteRequest(ref p1);
            p1.addBitVarToWriteRequest(area, DB, start, len, buffer);
            //res = _daveExchange(dc, &p1);
            p2 = ExchangePdu(p1);
            //if (res != daveResOK) return res;
            //res = _daveSetupReceivedPDU(dc, &p2);
            //if (res != 0) return res;
            return p2.testWriteResult();
        }

        public int writeBytes(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            var p1 = new Pdu_WriteRequest();
            Pdu p2;
            int res;
            /*if (dc->iface->protocol == daveProtoAS511)
            {
                return daveWriteS5Bytes(dc, area, DB, start, len, buffer);
            }*/
            //p1.header = dc->msgOut + dc->PDUstartO;
            //davePrepareWriteRequest(ref p1);
            p1.addVarToWriteRequest(area, DBnumber, start, len, buffer);
            //res = _daveExchange(dc, &p1);
            //if (res != daveResOK) return res;
            p2 = ExchangePdu(p1);
            //res = _daveSetupReceivedPDU(dc, &p2);
            //if (res != daveResOK) return res;
            return p2.testWriteResult();
        }

        public int writeManyBytes(int area, int DBnumber, int start, int len, byte[] buffer)
        {
            int res, pos, writeLen;
            byte[] pbuf = new byte[maxPDUlength];
            pos = 0;
            if (buffer == null) return Connection.daveResNoBuffer;
            //pbuf = buffer;
            res = Connection.daveResInvalidLength; //the only chance to return this is when len<=0
            while (len > 0)
            {
                if (len > maxPDUlength - 28) writeLen = maxPDUlength - 28; else writeLen = len;
                Array.Copy(buffer, pos, pbuf, 0, writeLen);
                res = writeBytes(area, DBnumber, start, writeLen, pbuf);
                if (res != 0) return res;
                len -= writeLen;
                start += writeLen;
                //pbuf += writeLen;
            }
            return res;
        }

        public IPDU prepareWriteRequest()
        {
            return new Pdu_WriteRequest();
        }

        public int execWriteRequest(IPDU p, IresultSet rl)//not checked
        {
            Pdu p2;
            byte[] q;
            resultN cr;//, c2;
            int res, i;
            p2 = ExchangePdu((Pdu)p);
            //if (res != daveResOK) return res;
            //res=_daveSetupReceivedPDU(dc, &p2);
            //if(res!=daveResOK) return res;
            res = p2.testReadResultMulti();
            if (res != Connection.daveResOK) return res;
            if (rl != null)
            {
                //cr=(daveResult*)calloc(p2.param[1], sizeof(daveResult));
                int numResults = p2.Param[1];
                //rl->results=cr;
                //c2=cr;
                //q=p2.data;
                q = p2.Data.ToArray();
                i = 0;
                int countQ = 0;
                while (i < p2.Param[1])
                {
                    cr = new resultN();
                    /*		printf("result %d: %d  %d %d %d\n",i, *q,q[1],q[2],q[3]); */
                    cr.error = Connection.daveUnknownError;
                    if (q[countQ] == 0x0A)
                    {	/* 300 and 400 families */
                        cr.error = Connection.daveResItemNotAvailable;
                    }
                    else if (q[countQ] == 0x03)
                    {	/* 200 family */
                        cr.error = Connection.daveResItemNotAvailable;
                    }
                    else if (q[countQ] == 0x05)
                    {
                        cr.error = Connection.daveAddressOutOfRange;
                    }
                    else if (q[countQ] == 0xFF)
                    {
                        cr.error = Connection.daveResOK;
                    }
                    else if (q[countQ] == 0x07)
                    {
                        cr.error = Connection.daveWriteDataSizeMismatch;
                    }
                    /*		    printf("Error %d\n", c2->error); */
                    countQ++;
                    //c2++;
                    i++;
                    ((daveResultN)rl).allResults.Add(cr);
                }
            }
            return res;
        }

        public void Dispose()
        {
            if (tcpClient != null)
            {
                try
                {
                    tcpClient.Client.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("2 TcpNetDave.cs threw exception");
                }
                try
                {
                    tcpClient.Client.Disconnect(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("3 TcpNetDave.cs threw exception");
                }
                try
                {
                    tcpClient.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("4 TcpNetDave.cs threw exception");
                }
            }
        }

        public int getAnswLen()
        {
            return AnswLen;
        }

        public static ushort daveSwapIed_16(short ff)
        {
            return (ushort)ff;
        }

        private void sprintf(byte[] bytes, int position, int len, int vol)
        {
            string sss = string.Format("{0:D" + len + "}", vol);
            System.Buffer.BlockCopy(Encoding.ASCII.GetBytes(sss), 0, bytes, position, sss.Length);
        }

        public int force200(int area, int start, int val)
        {
            //int res;
            Pdu p2 = new Pdu();
            //    uc pa[]={0,1,18,4,17,67,2,0};
            //    uc da[]={'0','0'};

            //32,7,0,0,0,0,0,c,0,16,

            byte[] pa = { 0, 1, 18, 8, 18, 72, 14, 0, 0, 0, 0, 0 };
            byte[] da ={0,1,0x10,2,
                        0,1,
                        0,0,
                        0,		// area
                		0,0,0,		// start
                    	};
            byte[] da2 = { 0, 4, 0, 8, 0, 0, };
            //    uc da2[]={0,4,0,8,7,0,};

            if ((area == daveConst.daveAnaIn) || (area == daveConst.daveAnaOut) /*|| (area==daveP)*/)
            {
                da[3] = 4;
                start *= 8;			/* bits */
            }
            else if ((area == daveConst.daveTimer) || (area == daveConst.daveCounter) || (area == daveConst.daveTimer200) || (area == daveConst.daveCounter200))
            {
                da[3] = (byte)area;
            }
            else
            {
                start *= 8;
            }
            /*    else {
            if(isBit) {
            pa[3]=1;
            } else {
            start*=8;
            }
            }
            */
            da[8] = (byte)area;
            da[9] = (byte)(start / 0x10000);
            da[10] = (byte)((start / 0x100) & 0xff);
            da[11] = (byte)(start & 0xff);

            da2[4] = (byte)(val % 0x100);
            da2[5] = (byte)(val / 0x100);
            return /*res =*/ BuildAndSendPDU(p2, pa, da, da2);
            //return res;
        }

        public int forceDisconnectIBH(int src, int dest, int mpi)
        {// INTERFACE
            byte[] b = new byte[daveConst.daveMaxRawLen];

            byte[] chal31 = { 0x07, 0xff, 0x06, 0x08, 0x00, 0x00, 0x82, 0x00, 0x14, 0x14, 0x02, 0x00, 0x01, 0x80, };

            chal31[8] = (byte)src;
            chal31[9] = (byte)dest;
            chal31[10] = (byte)mpi;
            /*	_daveWriteIBH(di, chal31, sizeof(chal31));
                _daveReadIBHPacket(di, b);
            #ifdef BCCWIN
            #else
                _daveReadIBHPacket(di, b);
            #endif    */
            return 0;
        }

        /*public int getMessage(IPDU p)
        {
            //Console.WriteLine(p);
            //return dc->iface->sendMessage(dc, p);
            return 0;
        }*/
        /*public int getU8()//not realy - delete
        {
            return 0;
        }*/
        //public int readBits(int area, int DBnum, int start, int len, byte[] buffer)
        // {
        //   Pdu p1, p2;
        /*	int res;
        #ifdef DEBUG_CALLS
            LOG7("daveReadBits(dc:%p area:%s area number:%d start address:%d byte count:%d buffer:%p)\n",
                dc, daveAreaName(area), DBnum, start,len,buffer);
            FLUSH;
        #endif
            dc->resultPointer=NULL;
            dc->_resultPointer=NULL;
            dc->AnswLen=0;
            p1.header=dc->msgOut+dc->PDUstartO;
            davePrepareReadRequest(dc, &p1);
            daveAddBitVarToReadRequest(&p1, area, DBnum, start, len);

            res=_daveExchange(dc, &p1);
            if (res!=daveResOK) return res;
            res=_daveSetupReceivedPDU(dc, &p2);
            if (daveDebug & daveDebugPDU)
                LOG3("_daveSetupReceivedPDU() returned: %d=%s\n", res,daveStrerror(res));
            if (res!=daveResOK) return res;

            res=_daveTestReadResult(&p2);
            if (daveDebug & daveDebugPDU)
                LOG3("_daveTestReadResult() returned: %d=%s\n", res,daveStrerror(res));
            if (res!=daveResOK) return res;
            if (daveDebug & daveDebugPDU)
                LOG2("got %d bytes of data\n", p2.udlen);
            if (p2.udlen==0) {
                return daveResCPUNoData;
            }
            if (buffer!=NULL) {
                if (daveDebug & daveDebugPDU)
                    LOG2("copy %d bytes to buffer\n", p2.udlen);
                memcpy(buffer,p2.udata,p2.udlen);
            }
            dc->resultPointer=p2.udata;
            dc->_resultPointer=p2.udata;
            dc->AnswLen=p2.udlen;
            return res;*/

        // return 0;
        //}
        public int readBytes(int area, int DBnum, int start, int len, byte[] buffer)
        {
            var p1 = new Pdu_ReadRequest();
            Pdu p2;
            int res;
            AnswLen = 0;	// 03/12/05
            //dc->resultPointer=NULL;
            //dc->_resultPointer=NULL;
            //p1.header=dc->msgOut+dc->PDUstartO;
            //davePrepareReadRequest(ref p1);
            p1.addVarToReadRequest(area, DBnum, start, len);
            //res = _daveExchange(dc, &p1);
            p2 = ExchangePdu(p1);

            //if (res != daveResOK) return res;
            //res = _daveSetupReceivedPDU(dc, &p2);
            //if (res != daveResOK) return res;

            res = p2.testReadResult();
            if (res != Connection.daveResOK) return res;

            if (p2.UData.Count == 0)
            {
                return Connection.daveResCPUNoData;
            }
            if (buffer != null) Array.Copy(p2.UData.ToArray(), 0, buffer, 0, p2.UData.Count);// memcpy(buffer, p2.udata, p2.udlen);
            //dc->resultPointer = p2.udata;
            //dc->_resultPointer = p2.udata;
            AnswLen = p2.UData.Count;
            return res;
        }

        /*public int resetIBH()//??????
        {
            byte[] chalReset = { 0x00, 0xff, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01 };
            byte[] b = new byte[daveMaxRawLen];
            //_daveWriteIBH(di, chalReset, sizeof(chalReset));
            //_daveReadIBHPacket(di, b);
            return 0;
        }*/

        /*void DECL2 daveAddFillByteToReadRequest(PDU *p) {
            uc pa[]=	{
                0			// fill byte
            };

            memcpy(p->param+p->plen, pa, 1);
            p->plen+=1;
        }*/

        public int PI_StartNC(string piservice, string[] param, int paramCount)
        {
            throw new NotImplementedException();
        }

        public int initUploadNC(string file, ref byte[] uploadID)
        {
            Pdu p1 = new Pdu(); ;
            //p1.header=dc->msgOut+dc->PDUstartO;
            _daveConstructUploadNC(p1, file);
            Pdu ret = ExchangePdu(p1);

            //if(res!=daveResOK) return res;
            //res=daveSetupReceivedPDU(dc, &p2);
            //if(res!=daveResOK) return res;
            uploadID[0] = ret.Param[4];
            uploadID[1] = ret.Param[5];
            uploadID[2] = ret.Param[6];
            uploadID[3] = ret.Param[7];
            return 0;
        }

        private void _daveConstructUploadNC(Pdu p, string file)
        {
            //byte[] pa =	new byte[1024];
            List<byte> pa = new List<byte>();
            pa.AddRange(new byte[] { 0x1d, 0, 0, 0, 0, 0, 0, 0, 0x11 });
            foreach (var item in file)
            {
                pa.Add((byte)item);
            }

            var xx = file.Select(c => (byte)c).ToArray();

            //pa.AddRange(new byte[] { () });

            //_daveInitPDUheader(p,1);
            //_daveAddParam(p, pa, sizeof(pa));

            p.Param.AddRange(pa);
        }

        public int doUploadNC(out int more, byte[] buffer, out int len, byte[] uploadID)
        {
            throw new NotImplementedException();
        }

        public int endUploadNC(byte[] uploadID)
        {
            throw new NotImplementedException();
        }

        public int daveGetNCProgram(string filename, byte[] buffer, ref int length)
        {
            throw new NotImplementedException();
        }

        public int daveGetNcFile(string filename, byte[] buffer, ref int length)
        {
            throw new NotImplementedException();
        }

        public int daveGetNcFileSize(string filename, ref int length)
        {
            throw new NotImplementedException();
        }

        public int davePutNCProgram(string filename, string path, string ts, byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        public int alarmQueryAlarm_S(byte[] buffer, int length, ref int alarmCount)
        {
            throw new NotImplementedException();
        }
    }
}