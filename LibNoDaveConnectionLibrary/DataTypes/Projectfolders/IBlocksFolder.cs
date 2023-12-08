using DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public interface IBlocksFolder
    {
        List<ProjectBlockInfo> readPlcBlocksList();

        List<ProjectBlockInfo> BlockInfos { get; }

        Block GetBlock(string BlockName);

        Block GetBlock(ProjectBlockInfo blkInfo);
    }
}