using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DotNetSiemensPLCToolBoxLibrary.General;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus
{
    public class Pdu
    {
        public Pdu(int Type)
        {            
            header=new PDUHeader();
            header.P = 0x32;
            header.type = (byte) Type;

            pduHeaderLen = (Type == 2 || Type == 3) ? 12 : 10;

            Param = new List<byte>();
            Data = new List<byte>();
            UData = new List<byte>();
        }

        public Pdu(byte[] recievedPdu)
        {
            int Type = recievedPdu[1];
            pduHeaderLen = (Type == 2 || Type == 3) ? 12 : 10;
            
            byte[] array=new byte[pduHeaderLen];
            Array.Copy(recievedPdu, 0, array, 0, pduHeaderLen);
            header = EndianessMarshaler.BytesToStruct<PDUHeader>(array);
            
            Param = new List<byte>();
            array = new byte[header.plen];
            Array.Copy(recievedPdu, pduHeaderLen, array, 0, header.plen);
            Param.AddRange(array);

            Data = new List<byte>();
            array = new byte[header.dlen];
            Array.Copy(recievedPdu, pduHeaderLen + header.plen, array, 0, header.dlen);
            Data.AddRange(array);

            UData = new List<byte>();            
        }

        public virtual byte[] ToBytes()
        {
            //Todo byteswap within the length of the pdu headers!
            header.plen = (byte) Param.Count;
            header.dlen = (byte) Data.Count;

            var retVal = new byte[pduHeaderLen + header.plen + header.dlen];

            byte[] bheader = EndianessMarshaler.StructToBytes<PDUHeader>(header);

            Array.Copy(bheader, 0, retVal, 0, bheader.Length);
            Array.Copy(Param.ToArray(), 0, retVal, pduHeaderLen, Param.Count);
            Array.Copy(Data.ToArray(), 0, retVal, pduHeaderLen + Param.Count, Data.Count);

            return retVal;
        }

        internal PDUHeader header;
        private int pduHeaderLen;

        public List<byte> Param { get; set; }
        public List<byte> Data { get; set; }
        public List<byte> UData { get; set; }
      
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
