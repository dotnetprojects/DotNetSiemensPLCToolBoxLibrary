namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    public class S5ProjectBlockInfo : ProjectPlcBlockInfo
    {
        public S5ProjectBlockInfo()
        {
        }

        public override string Name
        {
            get
            {
                if (BlockType == PLCBlockType.S5_FB || BlockType == PLCBlockType.S5_FX)
                    return System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(_blkByte, 12, 8).Trim();
                return null;
            }
        }

        public bool Assembler { get; internal set; }

        internal byte[] _blkByte;
        internal byte[] _blkHeaderByte;
    }
}
