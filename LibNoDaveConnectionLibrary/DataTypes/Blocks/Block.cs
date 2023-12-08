using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    /// <summary>
    /// Base Block for all Blocks, Subblocks are VATBlock, FunctionBlock, DataBlock, (DataBlockS5, FunctionBlockS5 maybe)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Block : IBlock
    {
        [JsonProperty(Order = -3)]
        public DataTypes.PLCBlockType BlockType { get; set; }

        public DataTypes.PLCLanguage BlockLanguage { get; set; }
        public virtual int BlockNumber { get; set; }

        [JsonProperty(Order = -1)]
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

        public virtual SymbolTableEntry SymbolTableEntry
        {
            get
            {
                return null;
            }
        }

        public ISymbolTable SymbolTable
        {
            get
            {
                if (ParentFolder != null && ParentFolder.Parent != null)
                {
                    return ((IProgrammFolder)ParentFolder.Parent).SymbolTable;
                }
                return null;
            }
        }

        [JsonProperty(Order = -4)]
        public string BlockName
        { get { return BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString(); } }
    }
}