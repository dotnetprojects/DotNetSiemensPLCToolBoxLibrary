using System;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    /// <summary>
    /// Base Block for all Blocks, Subblocks are VATBlock, FunctionBlock, DataBlock, (DataBlockS5, FunctionBlockS5 maybe)
    /// </summary>
    public abstract class Block
    {
        public DataTypes.PLCBlockType BlockType { get; set; }
        public DataTypes.PLCLanguage BlockLanguage { get; set; }
        public int BlockNumber { get; set; }
        public DotNetSiemensPLCToolBoxLibrary.Projectfiles.Project.Language ProjectLanguage {get; set;}
        //public DateTime LastModified { get; set; }

        public string Name { get; set; }

        public ProjectFolder ParentFolder { get; set; }


        public SymbolTableEntry SymbolTableEntry
        {
            get
            {
                if (ParentFolder != null)
                {
                    ISymbolTable tmp = ((IProgrammFolder) ParentFolder.Parent).SymbolTable;
                    if (tmp != null)
                        return tmp.GetEntryFromOperand(BlockName);
                }
                return null;
            }
        }

        public string BlockName { get { return BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString(); } }
    }
}
