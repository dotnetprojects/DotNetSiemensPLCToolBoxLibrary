using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5
{
    public class SymbolTable : Step7ProjectFolder, ISymbolTable
    {
        public String Folder { get; set; }

        private Dictionary<string, SymbolTableEntry> operandIndexList = new Dictionary<string, SymbolTableEntry>();
        private Dictionary<string, SymbolTableEntry> symbolIndexList = new Dictionary<string, SymbolTableEntry>();

        private List<SymbolTableEntry> _step7SymbolTableEntrys;
        public List<SymbolTableEntry> SymbolTableEntrys
        {
            get
            {
                if (_step7SymbolTableEntrys == null)
                    LoadSymboltable();
                return _step7SymbolTableEntrys;
            }
            set { _step7SymbolTableEntrys = value; }
        }

        internal bool showDeleted { get; set; }

        public SymbolTableEntry GetEntryFromOperand(string operand)
        {
            if (_step7SymbolTableEntrys == null)
                LoadSymboltable();
            string tmpname = operand.Replace(" ", "");
            SymbolTableEntry retval = null;
            operandIndexList.TryGetValue(tmpname, out retval);
            return retval;
        }

        public SymbolTableEntry GetEntryFromSymbol(string symbol)
        {
            if (_step7SymbolTableEntrys == null)
                LoadSymboltable();
            string tmpname = symbol.Trim().ToUpper();
            SymbolTableEntry retval = null;
            symbolIndexList.TryGetValue(tmpname, out retval);
            return retval;
        }

        public SymbolTable()
        {
            //Step7SymbolTableEntrys = new List<SymbolTableEntry>();
        }

        internal void LoadSymboltable()
        {
            _step7SymbolTableEntrys = new List<SymbolTableEntry>();

            if (!string.IsNullOrEmpty(Folder))
            {
                try
                {
                    var dbfTbl = DBF.ParseDBF.ReadDBF(Folder + "SYMLIST.DBF", ((Step7ProjectV5)Project)._zipfile, ((Step7ProjectV5)Project)._DirSeperator);
                    foreach (DataRow row in dbfTbl.Rows)
                    {
                        if (!(bool)row["DELETED_FLAG"] || showDeleted)
                        {
                            SymbolTableEntry sym = new SymbolTableEntry();
                            sym.Symbol = (string)row["_SKZ"];
                            sym.Operand = (string)row["_OPHIST"];
                            sym.OperandIEC = (string)row["_OPIEC"];
                            sym.DataType = (string)row["_DATATYP"];
                            sym.Comment = (string)row["_COMMENT"];
                            if ((bool)row["DELETED_FLAG"]) sym.Comment = "(deleted) " + sym.Comment;
                            _step7SymbolTableEntrys.Add(sym);
                            string oper = sym.Operand.Replace(" ", "");
                            string addSym = sym.Symbol.ToUpper().Trim();

                            if (!string.IsNullOrEmpty(addSym))
                            {
                                if (!operandIndexList.ContainsKey(oper))
                                    operandIndexList.Add(oper, sym);
                                if (!symbolIndexList.ContainsKey(addSym))
                                    symbolIndexList.Add(addSym, sym);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }

}
        
    

