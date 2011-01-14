using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public interface IDataBlock
    {
        PLCDataRow Structure { get; set;}

        PLCDataRow GetArrayExpandedStructure(PLCDataBlockExpandOptions myExpOpt);
        PLCDataRow GetArrayExpandedStructure();        
    }
}
