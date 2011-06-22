namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    public class S5ProjectBlockInfo:ProjectPlcBlockInfo
    {
        //internal int id;

        //public BlocksOfflineFolder ParentFolder { get; set; }

        //public bool Deleted { get; set; }
        //public int BlockNumber { get; set; }
        //public PLCBlockType BlockType { get; set; }
        //public string Symbol { get; set; }

        public override string Name
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
        
    }
}
