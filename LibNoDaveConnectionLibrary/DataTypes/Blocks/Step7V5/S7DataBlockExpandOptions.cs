namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    public class S7DataBlockExpandOptions
    {
        //public bool ExpandUDTs { get; set; }
        //public bool ExpandFBs { get; set; }
        //public bool ExpandArrays { get; set; }
        public bool ExpandCharArrays { get; set; }

        public S7DataBlockExpandOptions()
        {
            //ExpandUDTs = true;
            //ExpandFBs = true;
            //ExpandArrays = true;
            ExpandCharArrays = true;
        }
    }
}
