using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step5
{
    public class SymbolTable : Step5ProjectFolder, ISymbolTable
    {
        public String Folder { get; set; }

        private Dictionary<string, SymbolTableEntry> operandIndexList = new Dictionary<string, SymbolTableEntry>();
        private Dictionary<string, SymbolTableEntry> symbolIndexList = new Dictionary<string, SymbolTableEntry>();

        private List<SymbolTableEntry> _SymbolTableEntrys;

        public List<SymbolTableEntry> SymbolTableEntrys
        {
            get
            {
                return _SymbolTableEntrys;
            }
            set { _SymbolTableEntrys = value; }
        }

        internal bool showDeleted { get; set; }

        public SymbolTableEntry GetEntryFromOperand(string operand)
        {
            string tmpname = operand.Replace(" ", "");
            SymbolTableEntry retval = null;
            operandIndexList.TryGetValue(tmpname, out retval);
            return retval;
        }

        public SymbolTableEntry GetEntryFromSymbol(string symbol)
        {
            string tmpname = symbol.Trim().ToUpper();
            SymbolTableEntry retval = null;
            symbolIndexList.TryGetValue(tmpname, out retval);
            return retval;
        }

        public SymbolTable()
        {
            Name = "SymbolTable";
            //Step7SymbolTableEntrys = new List<SymbolTableEntry>();
        }

        internal void LoadSymboltable(Stream symTabStream)
        {
            _SymbolTableEntrys = new List<SymbolTableEntry>();

            StreamReader symTab = new StreamReader(symTabStream);

            while (!symTab.EndOfStream)
            {
                string line = symTab.ReadLine();
                if (line.Trim() != "" && !symTab.EndOfStream)
                {
                    string[] wrt = line.Split('\t');

                    SymbolTableEntry sym = new SymbolTableEntry();

                    if (wrt.Length > 2)
                    {
                        sym.Symbol = wrt[2];
                        sym.Operand = wrt[1];
                        sym.OperandIEC = wrt[1];
                        //sym.DataType =
                        if (wrt.Length > 3)
                            sym.Comment = wrt[3];
                        _SymbolTableEntrys.Add(sym);
                        try
                        {
                            operandIndexList.Add(sym.Operand.Replace(" ", ""), sym);
                            if (sym.Symbol.Trim() != "")
                                symbolIndexList.Add(sym.Symbol.ToUpper().Trim(), sym);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("1 SymbolTable.cs threw exception");
                        }
                    }
                }
            }
        }
    }
}