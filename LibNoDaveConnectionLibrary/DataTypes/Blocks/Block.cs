using System;
using System.Collections.Generic;

using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    /// <summary>
    /// Base Block for all Blocks, Subblocks are VATBlock, FunctionBlock, DataBlock, (DataBlockS5, FunctionBlockS5 maybe)
    /// </summary>
    public abstract class Block : IBlock
    {
        public DataTypes.PLCBlockType BlockType { get; set; }
        public DataTypes.PLCLanguage BlockLanguage { get; set; }
        public virtual int BlockNumber { get; set; }
        public MnemonicLanguage MnemonicLanguage { get; set; }
        //public DateTime LastModified { get; set; }

        public virtual string Name { get; set; }

        public virtual string SymbolOrName
        {
            get
            {
                if (SymbolTableEntry != null)
                    return SymbolTableEntry.Symbol;
                return Name;
            }
        }

        public ProjectFolder ParentFolder { get; set; }

        public virtual IEnumerable<String> Dependencies
        {
            get
            {
                return new List<String>();
            }
        }

        public virtual IEnumerable<String> CalledBlocks
        {
            get
            {
                return new List<String>();
            }
        }

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

        public ISymbolTable SymbolTable
        {
            get
            {
                if (ParentFolder != null)
                {
                    return ((IProgrammFolder)ParentFolder.Parent).SymbolTable;                    
                }
                return null;
            }
        }

        public string BlockName { get { return BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString(); } }
    }
}
