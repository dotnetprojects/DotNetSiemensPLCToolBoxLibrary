using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public interface IDataBlock
    {
        S7DataRow Structure { get; set;}

        S7DataRow GetArrayExpandedStructure(S7DataBlockExpandOptions myExpOpt);
        S7DataRow GetArrayExpandedStructure();        
    }
}
