using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using System.Collections.Generic;

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

        private List<ReferencePoint> _referencePoints = new List<ReferencePoint>();

        public List<ReferencePoint> ReferencePoints
        {
            get { return _referencePoints; }
            set { _referencePoints = value; }
        }
    }
}