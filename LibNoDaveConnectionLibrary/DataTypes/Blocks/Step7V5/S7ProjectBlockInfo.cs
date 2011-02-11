namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7ProjectBlockInfo:ProjectPlcBlockInfo
    {
        //internal int id;

        //public BlocksOfflineFolder ParentFolder { get; set; }

        //public bool Deleted { get; set; }
        //public int BlockNumber { get; set; }
        //public PLCBlockType BlockType { get; set; }
        //public string Symbol { get; set; }
        public bool KnowHowProtection { get; set; }

        public override string ToString()
        {
            string retVal = "";
            if (KnowHowProtection)
                retVal += "@";
            retVal += base.ToString();
            return retVal;
        }
    }
}
