using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public interface IBlock
    {
        int BlockNumber { get; }
        string SymbolOrName { get; }
    }
}
