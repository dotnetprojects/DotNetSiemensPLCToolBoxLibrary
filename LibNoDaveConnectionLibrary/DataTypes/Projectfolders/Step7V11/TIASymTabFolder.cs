using System;
using System.Collections.Generic;

using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class TIASymTabFolder : TIAProjectFolder, ISymbolTable
    {
        public String Folder { get; set; }

        public SymbolTableEntry GetEntryFromOperand(string operand)
        {
            throw new NotImplementedException();
        }

        public SymbolTableEntry GetEntryFromSymbol(string symbol)
        {
            throw new NotImplementedException();
        }

        public List<SymbolTableEntry> SymbolTableEntrys { get; set; }

        public TIASymTabFolder(Step7ProjectV11 Project)
            : base(Project)
        {
            SymbolTableEntrys = new List<SymbolTableEntry>();
        }
    }
}
