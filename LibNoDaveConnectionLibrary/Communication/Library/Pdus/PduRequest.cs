namespace DotNetSiemensPLCToolBoxLibrary.Communication.Library.Pdus
{
    public class Pdu_ReadRequest : Pdu
    {
        public Pdu_ReadRequest() : base(1)
        {
            //byte[] bytes = new byte[] { (byte)daveFuncRead, 0 }; //0x04 Function Read
            this.Param.AddRange(new byte[] { (byte)daveConst.daveFuncRead, 0 });
        }

        public override byte[] ToBytes()
        {
            Param[1] = PduVarCount;
            return base.ToBytes();
        }
    }

    public class Pdu_WriteRequest : Pdu
    {
        public Pdu_WriteRequest() : base(1)
        {
            //byte[] bytes = new byte[] { (byte)daveFuncWrite, 0 }; //daveFuncWrite = 0x04
            this.Param.AddRange(new byte[] { (byte)daveConst.daveFuncWrite, 0 });
        }

        public override byte[] ToBytes()
        {
            Param[1] = PduVarCount;
            return base.ToBytes();
        }
    }
}