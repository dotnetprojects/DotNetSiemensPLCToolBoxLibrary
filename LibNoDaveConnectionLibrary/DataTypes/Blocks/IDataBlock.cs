using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public interface IDataBlock : IBlock
    {
        IDataRow Structure { get; set; }

        IDataRow GetArrayExpandedStructure(S7DataBlockExpandOptions myExpOpt);
    }
}