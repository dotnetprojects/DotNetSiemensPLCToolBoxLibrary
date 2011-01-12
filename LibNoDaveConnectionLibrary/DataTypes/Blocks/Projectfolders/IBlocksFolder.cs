using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;

namespace LibNoDaveConnectionLibrary.DataTypes.Projects
{
    public interface IBlocksFolder
    {
        List<ProjectBlockInfo> readPlcBlocksList(bool useSymbolTable);

        Block GetBlock(string BlockName);
        Block GetBlock(ProjectBlockInfo blkInfo);
    }
}
