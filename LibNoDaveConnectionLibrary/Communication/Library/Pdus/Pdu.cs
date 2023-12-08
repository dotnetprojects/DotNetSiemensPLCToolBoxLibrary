using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using DotNetSiemensPLCToolBoxLibrary.General;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus
{
    public class Pdu : IPDU
    {
        public Pdu() : this(1)
        { }

        public Pdu(int Type)
        {
            header = new PDUHeader();
            header.P = 0x32;
            header.type = (byte)Type;

            pduHeaderLen = (Type == 2 || Type == 3) ? 12 : 10;

            Param = new List<byte>();
            Data = new List<byte>();
            UData = new List<byte>();
        }

        protected byte PduVarCount = 0x00;

        public void initUData()
        {
            if (Data != null && Data.Count > 4)
            {
                UData = new List<byte>(Data);
                UData.RemoveRange(0, 4);
            }
        }

        public Pdu(byte[] recievedPdu)
        {
            int Type = recievedPdu[1];
            pduHeaderLen = (Type == 2 || Type == 3) ? 12 : 10;

            byte[] array = new byte[pduHeaderLen];
            Array.Copy(recievedPdu, 0, array, 0, pduHeaderLen);
            header = EndianessMarshaler.BytesToStruct<PDUHeader>(array);

            Param = new List<byte>();
            array = new byte[header.plen];
            Array.Copy(recievedPdu, pduHeaderLen, array, 0, header.plen);
            Param.AddRange(array);

            Data = new List<byte>();
            array = new byte[header.dlen];
            header.dlen = (ushort)Math.Min(header.dlen, recievedPdu.Length - pduHeaderLen - header.plen);//for OLD PLC: recievedPdu.Length = 400, header.dlen = 448

            Array.Copy(recievedPdu, pduHeaderLen + header.plen, array, 0, header.dlen);
            Data.AddRange(array);

            UData = new List<byte>();
            initUData();
        }

        public virtual byte[] ToBytes()
        {
            //Todo byteswap within the length of the pdu headers!
            header.plen = (ushort)Param.Count;
            header.dlen = (ushort)Data.Count;

            var retVal = new byte[pduHeaderLen + header.plen + header.dlen];

            byte[] bheader = EndianessMarshaler.StructToBytes<PDUHeader>(header);

            Array.Copy(bheader, 0, retVal, 0, pduHeaderLen);
            Array.Copy(Param.ToArray(), 0, retVal, pduHeaderLen, Param.Count);
            Array.Copy(Data.ToArray(), 0, retVal, pduHeaderLen + Param.Count, Data.Count);

            return retVal;
        }

        internal PDUHeader header;
        private int pduHeaderLen;

        public List<byte> Param { get; set; }
        public List<byte> Data { get; set; }
        public List<byte> UData { get; set; }

        protected void addData(byte[] pa, int len)
        {
            if (pa.Length >= len)
                for (int ii = 0; ii < len; ii++)
                    Data.Add(pa[ii]);
        }

        protected void addValue(byte[] data, int len)
        {
            ushort dCount;
            byte dtype;
            //dtype = p->data + p->dlen -4+1;			/* position of first byte in the 4 byte sequence */
            dtype = Data[Data.Count - 4 + 1];
            //dCount=* ((us *)(p->data+p->dlen-4+2));  /* changed for multiple write */
            dCount = BitConverter.ToUInt16(Data.ToArray(), Data.Count - 4 + 2);

            //            dCount=daveSwapIed_16(dCount);
            //	if (daveDebug & daveDebugPDU)
            //		LOG2("dCount: %d\n", dCount);
            if (dtype == 4)
            {	/* bit data, length is in bits */
                dCount += (ushort)(8 * len);
            }
            else if (dtype == 9)
            {	/* byte data, length is in bytes */
                dCount += (ushort)len;
            }
            else if (dtype == 3)
            {	/* bit data, length is in bits */
                dCount += (ushort)len;
                //	} else {
                //		if (daveDebug & daveDebugPDU)
                //			LOG2("unknown data type/length: %d\n", *dtype);
            }
            //if (p->udata==NULL) p->udata=p->data+4;
            //p->udlen+=len;
            //if (daveDebug & daveDebugPDU)
            //	LOG2("dCount: %d\n", dCount);
            //dCount=daveSwapIed_16(dCount);

            //*((us *)(p->data+p->dlen-4+2))=dCount;
            byte[] bCount = BitConverter.GetBytes(dCount);
            Data[Data.Count - 4 + 2] = bCount[1];
            Data[Data.Count - 4 + 3] = bCount[0];

            addData(data, len);
        }

        #region For Read Request

        private void addToReadRequest(int area, int DBnumber, int startByteAddress, int byteCount, bool isBit)
        {
            byte readSize = 2; /* 1=single bit, 2=byte, 4=word */
            int startByte = startByteAddress;

            if ((area == daveConst.daveAnaIn) || (area == daveConst.daveAnaOut))
            {
                readSize = 4; /* word */
                startByte *= 8; /* bits */
            }
            else if ((area == daveConst.daveTimer) || (area == daveConst.daveCounter) || (area == daveConst.daveTimer200) || (area == daveConst.daveCounter200))
                readSize = (byte)area;
            else
            {
                if (isBit)
                    area = 1;
                else
                    startByte *= 8; /* bit address of byte */
            }

            byte[] tag = {0x12, 0x0a, 0x10, /* Header */
                          readSize, /* 1=single bit, 2=byte, 4=word */
                          (byte) (byteCount/256), (byte) (byteCount & 0xff), /* length in bytes */
                          (byte) (DBnumber/256), (byte) (DBnumber & 0xff), /* DB number */
                          (byte) area, /* area code */
                          (byte) (startByte/0x10000), (byte) ((startByte/0x100) & 0xff), (byte) (startByte & 0xFF) /* start address in bits */};
            Param.AddRange(tag);
            PduVarCount++;
        }

        public void addVarToReadRequest(int area, int DBnumber, int startByteAddress, int byteCount)
        {
            addToReadRequest(area, DBnumber, startByteAddress, byteCount, false);
        }

        public IntPtr pointer
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void addBitVarToReadRequest(int area, int DBnumber, int startByteAddress, int bitNumber)
        {
            addToReadRequest(area, DBnumber, startByteAddress * 8 + bitNumber, 1, true);
        }

        public void addNCKToReadRequest(int area, int unit, int column, int line, int module, int linecount)
        {
            throw new NotImplementedException();
        }

        public void addNCKToWriteRequest(int area, int unit, int column, int line, int module, int linecount, int bytes, byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void daveAddFillByteToReadRequest()
        {
            byte[] pa = {
                0           /* fill byte */
            };
            //memcpy(p->param+p->plen, pa, 1);
            //p->plen+=1;
            Param.AddRange(pa);
        }

        public void addDbRead400ToReadRequest(int DBnum, int offset, int byteCount)
        {
            byte[] pa = {
                0x12, 0x07, 0xb0,
                0x01,       /*  */
                0,          /* length in bytes */
                0,0,        /* DB number */
                0,0,        /* start address in bits */
            };

            //int paSize = 0;

            pa[4] = (byte)byteCount;
            pa[5] = (byte)(DBnum / 256);
            pa[6] = (byte)(DBnum & 0xff);
            pa[7] = (byte)(offset / 256);
            pa[8] = (byte)(offset & 0xff);

            PduVarCount++;

            //memcpy(p->param+p->plen, pa, sizeof(pa));
            //p->plen+=sizeof(pa);
            Param.AddRange(pa);

            /*#ifdef ARM_FIX
                tmplen=daveSwapIed_16(p->plen);
                memcpy(&(((PDUHeader*)p->header)->plen), &tmplen, sizeof(us));
            #else
                ((PDUHeader*)p->header)->plen=daveSwapIed_16(p->plen);
            #endif    */
            //p->data=p->param+p->plen;
            //p->dlen=0;
            Data.Clear();
        }

        public void addSymbolVarToReadRequest(string completeSymbol)
        {
            addSymbolToReadRequest(completeSymbol);
        }

        private void addSymbolToReadRequest(string completeSymbol)
        {
            byte[] pa = { 0x12, 0x00, 0xb2, 0xff };

            pa[1] = (byte)(completeSymbol.Length + 4);

            PduVarCount++;
            //memcpy(p->param+p->plen, pa, sizeof(pa));
            Param.AddRange(pa);
            //memcpy(p->param+p->plen+4, completeSymbol, completeSymbolLength);
            Param.AddRange(completeSymbol.ToByteArray());
            //p->plen+= pa[1];

            /*#ifdef ARM_FIX
                tmplen=daveSwapIed_16(p->plen);
                memcpy(&(((PDUHeader*)p->header)->plen), &tmplen, sizeof(us));
            #else
                ((PDUHeader*)p->header)->plen=daveSwapIed_16(p->plen);
            #endif    */
            //p->data=p->param+p->plen;
            //p->dlen=0;
            Data.Clear();
            //	if (daveDebug & daveDebugPDU) {
            //		_daveDumpPDU(p);
            //	}
        }

        #endregion For Read Request

        #region For write request

        public void addBitVarToWriteRequest(int area, int DBnum, int start, int byteCount, byte[] buffer)
        {
            byte[] da = { 0, 3, 0, 0, };
            byte[] pa = {
                0x12, 0x0a, 0x10,
                0x01,       /* single bit */
                0,0,        /* insert length in bytes here */
                0,0,        /* insert DB number here */
                0,      /* change this to real area code */
                0,0,0       /* insert start address in bits */
            };

            addToWriteRequest(area, DBnum, start, byteCount, buffer, da, pa);
        }

        private void addToWriteRequest(int area, int DBnum, int start, int byteCount,
            byte[] buffer,
            byte[] da,
            byte[] pa
            )
        {
            //byte[] saveData = new byte[1024];
            if ((area == daveConst.daveTimer) || (area == daveConst.daveCounter) || (area == daveConst.daveTimer200) || (area == daveConst.daveCounter200))
            {
                pa[3] = (byte)area;
                pa[4] = (byte)(((byteCount + 1) / 2) / 0x100);
                pa[5] = (byte)(((byteCount + 1) / 2) & 0xff);
            }
            else if ((area == daveConst.daveAnaIn) || (area == daveConst.daveAnaOut))
            {
                pa[3] = 4;
                pa[4] = (byte)(((byteCount + 1) / 2) / 0x100);
                pa[5] = (byte)(((byteCount + 1) / 2) & 0xff);
            }
            else
            {
                pa[4] = (byte)(byteCount / 0x100);
                pa[5] = (byte)(byteCount & 0xff);
            }
            pa[6] = (byte)(DBnum / 256);
            pa[7] = (byte)(DBnum & 0xff);
            pa[8] = (byte)area;
            pa[11] = (byte)(start & 0xff);
            pa[10] = (byte)((start / 0x100) & 0xff);
            pa[9] = (byte)(start / 0x10000);
            if (Data.Count % 2 == 1)
            {
                addData(da, 1);
            }
            PduVarCount++;
            //if (p.Data.Count > 0)
            //{
            //memcpy(saveData, p->data, p->dlen);
            //memcpy(p->data+pasize, saveData, p->dlen);
            //}
            //memcpy(p->param+p->plen, pa, pasize);
            Param.AddRange(pa);
            //p->plen+=pasize;

            //p->data=p->param+p->plen;
            //_daveAddData(p, da, dasize);
            Data.AddRange(da);

            addValue(buffer, byteCount);
            //if (daveDebug & daveDebugPDU) {
            //	_daveDumpPDU(p);
            //}
        }

        public void addVarToWriteRequest(int area, int DBnum, int start, int byteCount, byte[] buffer)
        {
            byte[] da = {0, //Return Value
                4, //Transport-Size
                0, //Count of the following Data
                0, //Count of the following Data
            };

            byte[] pa = {
                0x12, 0x0a, 0x10,
                0x02,       /* unit (for count?, for consistency?) byte */
                0,0,        /* length in bytes */
                0,0,        /* DB number */
                0,      /* area code */
                0,0,0       /* start address in bits */
            };

            addToWriteRequest(area, DBnum, 8 * start, byteCount, buffer, da, pa);
        }

        #endregion For write request

        public int testWriteResult()
        {
            int res;/* =daveResCannotEvaluatePDU; */
            if (Param[0] != daveConst.daveFuncWrite) return Connection.daveResUnexpectedFunc;
            if ((Data[0] == 255))
            {
                res = Connection.daveResOK;
            }
            else
                res = Data[0];
            return res;
        }

        public int testReadResultMulti()
        {
            //if (p == null) return daveResNoBuffer;
            if (Param.Count == 0 || Param[0] != daveConst.daveFuncRead) return Connection.daveResUnexpectedFunc;
            return testResultDataMulti();
        }

        public int testReadResult()
        {
            if (Param[0] != daveConst.daveFuncRead) return Connection.daveResUnexpectedFunc;
            return testResultData();
        }

        public int testPGReadResult()
        {
            int pres = 0;
            if (Param[0] != 0) return Connection.daveResUnexpectedFunc;
            if (Param.Count == 12) pres = (256 * Param[10] + Param[11]);
            if (pres == 0) return testResultData(); else return pres;
        }

        public int testResultDataMulti()
        {
            int res; /*=daveResCannotEvaluatePDU;*/
            if ((Data[0] == 255) && (Data.Count > 4))
            {
                res = Connection.daveResOK;
                //p->udata=p->data+4;
                //p->udlen=p->data[2]*0x100+p->data[3];
                // !!!!!1 p.initUData();
                if (Data[1] == 4)
                {
                    //p->udlen>>=3;	/* len is in bits, adjust */
                }
                else if (Data[1] == 9)
                {
                    /* len is already in bytes, ok */
                }
                else if (Data[1] == 3)
                {
                    /* len is in bits, but there is a byte per result bit, ok */
                }
                else
                {
                    //			if (daveDebug & daveDebugPDU)
                    //				LOG2("fixme: what to do with data type %d?\n",p->data[1]);
                    res = Connection.daveResUnknownDataUnitSize;
                    //res = 0;
                }
            }
            else if (Data[0] == 10 || Data[0] == 5)
            {
                //This Section returns ok, even if nothing was read,
                //because with the multiple read we get the error in (daveUseResult)
                res = Connection.daveResOK;
            }
            else
            {
                res = Data[0];
            }
            return res;
        }

        public int testResultData()
        {
            int res; /*=daveResCannotEvaluatePDU;*/
            if ((Data[0] == 255) && (Data.Count > 4))
            {
                res = Connection.daveResOK;
                initUData();
                //p->udlen=p->data[2]*0x100+p->data[3];
                if (Data[1] == 4)
                {
                    //p.udlen>>=3;	/* len is in bits, adjust */ ?????????????????????
                }
                else if (Data[1] == 9)
                {
                    /* len is already in bytes, ok */
                }
                else if (Data[1] == 3)
                {
                    /* len is in bits, but there is a byte per result bit, ok */
                }
                else
                {
                    res = Connection.daveResUnknownDataUnitSize;
                }
            }
            else
            {
                res = Data[0];
            }
            return res;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        internal struct PDUHeader
        {
            public byte P; /* allways 0x32 */
            public byte type; /* Header type, one of 1,2,3 or 7. type 2 and 3 headers are two bytes longer. */
            public byte a, b; /* currently unknown. Maybe it can be used for long numbers? */

            [Endian(Endianness.BigEndian)]
            public ushort number; /* A number. This can be used to make sure a received answer */

            /* corresponds to the request with the same number. */

            [Endian(Endianness.BigEndian)]
            public ushort plen; /* length of parameters which follow this header */

            [Endian(Endianness.BigEndian)]
            public ushort dlen; /* length of data which follow the parameters */

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] result; /* only present in type 2 and 3 headers. This contains error information. */
        }
    }
}