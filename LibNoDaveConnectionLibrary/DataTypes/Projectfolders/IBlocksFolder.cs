using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public interface IBlocksFolder
    {
        List<ProjectBlockInfo> readPlcBlocksList(bool useSymbolTable);

        Block GetBlock(string BlockName);
        Block GetBlock(ProjectBlockInfo blkInfo);
    }
}
