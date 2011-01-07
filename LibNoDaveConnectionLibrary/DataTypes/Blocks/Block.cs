using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Projects;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;

namespace LibNoDaveConnectionLibrary.DataTypes.Blocks
{
    /// <summary>
    /// Base Block for all Blocks, Subblocks are VATBlock, FunctionBlock, DataBlock, (DataBlockS5, FunctionBlockS5 maybe)
    /// </summary>
    public abstract class Block
    {
        public DataTypes.PLCBlockType BlockType { get; set; }
        public DataTypes.PLCLanguage BlockLanguage { get; set; }
        public int BlockNumber { get; set; }
        public DateTime LastModified { get; set; }

        public string Name { get; set; }

        public ProjectFolder ParentFolder { get; set; }
    }
}
