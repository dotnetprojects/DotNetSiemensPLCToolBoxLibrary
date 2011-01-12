using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Projects;

namespace LibNoDaveConnectionLibrary.DataTypes.Blocks
{
    class OnlineBlocksFolder : ProjectFolder, IBlocksFolder
    {

        private LibNoDaveConnectionConfiguration ConnectionConfig { get; set;}

        public OnlineBlocksFolder(string ConnectionName)
        {
            ConnectionConfig=new LibNoDaveConnectionConfiguration(ConnectionName);
        }

        public List<ProjectBlockInfo> readPlcBlocksList(bool useSymbolTable)
        {
            throw new NotImplementedException();
        }

        public Block GetBlock(string BlockName)
        {
            throw new NotImplementedException();
        }

        public Block GetBlock(ProjectBlockInfo blkInfo)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Online Blocks (" + ConnectionConfig.ConnectionName + ")";
        }
    }
}
