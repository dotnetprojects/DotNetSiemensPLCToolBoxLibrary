using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    public interface ISymbolTable
    {
        String Folder { get; set; }
        
        SymbolTableEntry GetEntryFromOperand(string operand);
        SymbolTableEntry GetEntryFromSymbol(string symbol);
    }
}
