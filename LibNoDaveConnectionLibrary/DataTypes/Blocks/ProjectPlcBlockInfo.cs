using System;
using System.Collections.Generic;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{

    public class ProjectPlcBlockInfo:ProjectBlockInfo
    {

        public int BlockNumber { get; set; }

        public string BlockName
        {
            get
            {
                string retVal = BlockType.ToString().Replace("S5_", "") + BlockNumber.ToString();
                if (Deleted)
                    retVal = "$$_" + retVal;
                return retVal;
            }
        }

        public SymbolTableEntry SymbolTabelEntry
        {
            get
            {
                if (ParentFolder.Parent is IProgrammFolder)
                {
                    ISymbolTable tmp = ((IProgrammFolder)ParentFolder.Parent).SymbolTable;
                    if (tmp != null)
                        return tmp.GetEntryFromOperand(BlockName);
                }
                return null;
            }
        }

        public override string ToString()
        {
            string retVal = "";
            if (Deleted)
                retVal += "$$_";
            if (SymbolTabelEntry != null)
                retVal += BlockName + " (" + SymbolTabelEntry.Symbol + ")";
            else
                retVal += BlockName;
            return retVal;
        }
    }
}
