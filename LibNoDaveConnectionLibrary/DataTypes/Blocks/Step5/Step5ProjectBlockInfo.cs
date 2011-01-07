namespace LibNoDaveConnectionLibrary.DataTypes.Blocks.Step5
{
    public class Step5ProjectBlockInfo:ProjectBlockInfo
    {
        //internal int id;

        //public BlocksOfflineFolder ParentFolder { get; set; }

        //public bool Deleted { get; set; }
        //public int BlockNumber { get; set; }
        //public PLCBlockType BlockType { get; set; }
        //public string Symbol { get; set; }

        public override string BlockName
        {
            get
            {
                if (BlockType == PLCBlockType.S5_FB || BlockType == PLCBlockType.S5_FX)
                    return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(_blkByte, 12, 8).Trim();
                return null;
            }            
        }

        internal byte[] _blkByte;
        internal byte[] _blkHeaderByte;
        //internal S5Block _myblk;

        public override string ToString()
        {
            string retVal = "";

            string blknm = "";
            if (!string.IsNullOrEmpty(BlockName))
                blknm = " (" + BlockName + ")";
            if (Deleted)
                retVal += "$$_";
            if (!string.IsNullOrEmpty(Symbol))
                retVal += Symbol + " (" + BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString() + ")" + blknm;
            else
                retVal += BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString() + blknm;
            return retVal;
        }
    }
}
