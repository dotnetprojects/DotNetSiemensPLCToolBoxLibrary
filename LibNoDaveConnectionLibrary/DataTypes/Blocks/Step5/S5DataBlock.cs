using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    public class S5DataBlock : S5Block, IDataBlock
    {
        public IDataRow Structure { get; set; }

        public IDataRow GetArrayExpandedStructure(S7DataBlockExpandOptions myExpOpt)
        {
            return Structure;
        }

        public IDataRow GetArrayExpandedStructure()
        {
            return Structure;
        }
    }
}