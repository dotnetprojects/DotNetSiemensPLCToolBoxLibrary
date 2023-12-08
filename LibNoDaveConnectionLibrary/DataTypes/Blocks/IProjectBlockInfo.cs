using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    /// <summary>
    /// This can be Information about a PLC Block (DB, FC,...) or a Block in the Source Folder
    /// </summary>
    public interface IProjectBlockInfo
    {
        ProjectFolder ParentFolder { get; set; }

        string Name { get; set; }

        PLCBlockType BlockType { get; set; }

        Block GetBlock();
    }
}