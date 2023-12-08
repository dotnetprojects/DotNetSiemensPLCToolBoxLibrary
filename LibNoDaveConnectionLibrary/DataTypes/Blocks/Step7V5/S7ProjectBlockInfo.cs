namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7ProjectBlockInfo : ProjectPlcBlockInfo
    {
        public string Family { get; set; }

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