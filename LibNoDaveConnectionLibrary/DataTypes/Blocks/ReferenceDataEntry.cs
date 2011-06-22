using System.Collections.Generic;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public class ReferenceDataEntry
    {
        public string Operand { get; set; }

        private SymbolTableEntry _SymbolTableEntry;
        public SymbolTableEntry SymbolTableEntry
        {
            get { return _SymbolTableEntry; }
            set { _SymbolTableEntry = value; }
        }

        public List<ReferencePoint> ReferencePoints { get; set; }
    }
}
