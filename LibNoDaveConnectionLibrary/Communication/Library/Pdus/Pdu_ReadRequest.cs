namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus
{
    public class Pdu_ReadRequest:Pdu
    {
        public Pdu_ReadRequest() : base(1)
        {
            byte[] bytes = new byte[] {0x04, 0}; //0x04 Function Read
            this.Param.AddRange(bytes);
        }
        
        private byte PduVarCount = 0x00;

        public override byte[] ToBytes()
        {
            Param[1] = PduVarCount;
            return base.ToBytes();
        }

        private void addToReadRequest(int area, int DBnumber, int startByteAddress, int byteCount, bool isBit)
        {
            byte readSize = 2; /* 1=single bit, 2=byte, 4=word */
            int startByte = startByteAddress;

            if ((area == 6 /*daveAnaIn*/) || (area == 7 /*daveAnaOut*/))
            {
                readSize = 4; /* word */
                startByte *= 8; /* bits */
            }
            else if ((area == 29 /*daveTimer*/) || (area == 28 /*daveCounter*/) || (area == 31 /*daveTimer200*/) || (area == 30 /*daveCounter200*/))
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

        public void addBitVarToReadRequest(int area, int DBnumber, int startByteAddress, int bitNumber)
        {
            addToReadRequest(area, DBnumber, startByteAddress*8 + bitNumber, 1, true);
        }
    }
}
