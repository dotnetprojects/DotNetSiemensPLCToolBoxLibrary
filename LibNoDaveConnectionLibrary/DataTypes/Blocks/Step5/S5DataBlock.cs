using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    public class S5DataBlock: S5Block,IDataBlock
    {
        public S7DataRow Structure { get; set; }
        public S7DataRow GetArrayExpandedStructure(S7DataBlockExpandOptions myExpOpt)
        {
            return Structure;
        }

        public S7DataRow GetArrayExpandedStructure()
        {
            return Structure;
        }
    }
}
