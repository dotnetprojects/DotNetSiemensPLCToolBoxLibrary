using System;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public interface ISymbolTable : IProjectFolder
    {
        String Folder { get; set; }

        SymbolTableEntry GetEntryFromOperand(string operand);

        SymbolTableEntry GetEntryFromSymbol(string symbol);

        List<SymbolTableEntry> SymbolTableEntrys { get; set; }
    }
}