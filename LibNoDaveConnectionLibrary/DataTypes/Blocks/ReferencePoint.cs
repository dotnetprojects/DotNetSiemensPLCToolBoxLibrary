namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public class ReferencePoint
    {
        public Block Block { get; set; }

        public FunctionBlockRow BlockRow { get; set; }

        public ReferenceDataAccessMode AccessMode { get; set; }

        public Network Network { get; set; }

        public int NetworkNumber { get; set; }

        public int LineNumber { get; set; }
    }
}
