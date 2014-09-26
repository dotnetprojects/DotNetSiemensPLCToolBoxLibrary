using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders.Step7V5;
using DotNetSiemensPLCToolBoxLibrary.PLCs.S7_xxx.MC7;

using System.Linq;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class S7FunctionBlock : S7Block, IFunctionBlock, INotifyPropertyChanged
    {
        public override IEnumerable<String> Dependencies
        {
            get
            {
                var retVal = new List<String>();

                if (AWLCode != null)
                    foreach (var row in AWLCode)
                    {
                        retVal.AddRange(((S7FunctionBlockRow) row).Dependencies);
                    }

                retVal.AddRange(Parameter.Dependencies);

                return retVal.Distinct().OrderBy(itm => itm);
            }
        }

        public override IEnumerable<String> CalledBlocks
        {
            get
            {
                var retVal = new List<String>();

                if (AWLCode!=null)
                    foreach (var row in AWLCode)
                    {
                        if (((S7FunctionBlockRow)row).CalledBlock != null) 
                            retVal.Add(((S7FunctionBlockRow)row).CalledBlock);
                    }

                return retVal;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private S7DataRow _parameter;
        public S7DataRow Parameter
        {
            get { return _parameter; }
            set { _parameter = value;
            NotifyPropertyChanged("Parameter");
            }
        }

        private S7DataRow _parameterWithoutTemp;
        internal S7DataRow ParameterWithoutTemp
        {
            get { return _parameterWithoutTemp; }
            set
            {
                _parameterWithoutTemp = value;
                NotifyPropertyChanged("ParameterWithoutTemp");
            }          
        }

        private List<FunctionBlockRow> _awlCode;
        public List<FunctionBlockRow> AWLCode
        {
            get { return _awlCode; }
            set { _awlCode = value;
            NotifyPropertyChanged("AWLCode");
            }
        }

        private List<Network> _networks;
        public List<Network> Networks
        {
            get { return _networks; }
            set { _networks = value;
            NotifyPropertyChanged("Networks");
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value;
            NotifyPropertyChanged("Description");
            }
        }

        private PLCConnection.DiagnosticData _diagnosticData;
        public PLCConnection.DiagnosticData DiagnosticData
        {
            get { return _diagnosticData; }
            set { _diagnosticData = value;
                NotifyPropertyChanged("DiagnosticData");
            }
        }

        //Todo: Implement "Rename Parameter" in S7FuntionBlock
        public void RenameParameter(string oldName, string newName)
        {
        }

        //Todo: Implement "Rename Label" in S7FuntionBlock
        public void RenameLabel(string oldName, string newName)
        {
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public override string GetSourceBlock(bool useSymbols = false)
        {
            StringBuilder retVal = new StringBuilder();
            string name = this.BlockName;
            if (useSymbols && SymbolTableEntry != null)
            {
                name = SymbolTableEntry.Symbol;
            }
            if (this.BlockType == PLCBlockType.FC)
                retVal.AppendLine("FUNCTION " + name + " : VOID");
            else
                retVal.AppendLine("FUNCTION_BLOCK " + this.BlockName);

            retVal.AppendLine("TITLE =" + this.Title);

            if (!string.IsNullOrEmpty(this.Description))
                retVal.AppendLine("//" + this.Description.Replace(Environment.NewLine, Environment.NewLine + "//"));
            if (!string.IsNullOrEmpty(this.Author))
                retVal.AppendLine("AUTHOR : " + this.Author);
            if (!string.IsNullOrEmpty(this.Name))
                retVal.AppendLine("NAME : " + this.Name);
            if (!string.IsNullOrEmpty(this.Version))
                retVal.AppendLine("VERSION : " + this.Version);
            retVal.Append(Environment.NewLine);
            retVal.Append(Environment.NewLine);


            if (this.Parameter.Children != null)
            {
                foreach (S7DataRow s7DataRow in this.Parameter.Children)
                {
                    string parnm = s7DataRow.Name;
                    string ber = "VAR_" + parnm;
                    if (parnm == "IN")
                        ber = "VAR_INPUT";
                    else if (parnm == "OUT")
                        ber = "VAR_OUTPUT";
                    else if (parnm == "STATIC")
                        ber = "VAR";
                    retVal.AppendLine(ber);
                    string structSource = AWLToSource.DataRowToSource(s7DataRow, "  ");
                    if (useSymbols)
                    {
                        Regex regex = new Regex(@"UDT[\s?]*(\d*)");
                        foreach (Match match in regex.Matches(structSource))
                        {
                            string operand = match.Value;
                            if (!match.Success || !structSource.Contains(operand)) continue;
                            string symbol = operand;
                            if (SymbolTable != null)
                            {
                                SymbolTableEntry symbolTableEntry = SymbolTable.GetEntryFromOperand("UDT" + match.Groups[1].Value);
                                if (symbolTableEntry != null) symbol = symbolTableEntry.Symbol;
                            }
                            structSource = structSource.Replace(operand, symbol);
                        }
                        regex = new Regex(@"FB[\s?]*(\d*)");
                        foreach (Match match in regex.Matches(structSource))
                        {
                            string operand = match.Value;
                            if (!match.Success || !structSource.Contains(operand)) continue;
                            string symbol = operand;
                            if (SymbolTable != null)
                            {
                                SymbolTableEntry symbolTableEntry = SymbolTable.GetEntryFromOperand("FB" + match.Groups[1].Value);
                                if (symbolTableEntry != null) symbol = symbolTableEntry.Symbol;
                            }
                            structSource = structSource.Replace(operand, symbol);
                        }
                    }
                    retVal.Append(structSource);
                    retVal.AppendLine("END_VAR");
                }

            }
            retVal.AppendLine("BEGIN");
            foreach (Network network in this.Networks)
            {
                retVal.AppendLine("NETWORK");
                retVal.AppendLine("TITLE = " + network.Name);
                if (!String.IsNullOrEmpty(network.Comment))
                    retVal.AppendLine("//" + network.Comment.Replace(Environment.NewLine, Environment.NewLine + "//") );
                else
                    retVal.Append(Environment.NewLine);
                StringBuilder sbAwl = new StringBuilder("");
                foreach (S7FunctionBlockRow functionBlockRow in network.AWLCode)
                {
                    string awlCode = functionBlockRow.ToString(useSymbols, true);
                    if (awlCode == "" || awlCode == ";")
                        sbAwl.Append(Environment.NewLine);
                    else
                    {
                        sbAwl.AppendLine(awlCode);
                    }
                }
                //Fix for Db access not merged for some lines temporary solution until the issue is found
                ByteBitAddress byteBitAddress = new ByteBitAddress(0,0);
                string awl = sbAwl.ToString();
                Regex regex = new Regex(@"AUF\s*(.*)[\s?]*;\s*(\S*)\s*(DB.*)[\s?]*;");
                foreach (Match match in regex.Matches(awl))
                {
                    string sMatch = match.Value;
                    if(!match.Success || !awl.Contains(sMatch)) continue;
                    string dbName = match.Groups[1].Value.Trim();
                    string awlCommand = match.Groups[2].Value;
                    string dbAddress = match.Groups[3].Value.Trim();
                    string dbAccess = dbName + "." + dbAddress;
                    if(useSymbols)
                    {
                        string symbolName = dbName.Replace("\"", "");
                        string address = dbAddress.Replace(" ", "");
                        int pointPosition = address.IndexOf('.');
                        byteBitAddress.ByteAddress = int.Parse(pointPosition < 0 ? address.Substring(3) : address.Substring(3,pointPosition-3));
                        byteBitAddress.BitAddress = int.Parse(pointPosition < 0 ? "0" : address.Substring(address.Length-1));
                        //dbAccess += "+"+byteBitAddress.ToString();
                        var dbNr = Helper.TryGetOperandFromSymbol(ParentFolder, symbolName) ?? symbolName;
                        dbAddress = Helper.TryGetStructuredName(ParentFolder, dbNr , dbAddress);
                        dbAccess = dbName + "." + dbAddress;
                        //BlocksOfflineFolder folder = ParentFolder as BlocksOfflineFolder;
                        //if (folder!=null)
                        //{
                        //    //dbAccess += "+folder not null+" + symbolName;
                        //    if (SymbolTable != null)
                        //    {
                        //        //dbAccess += "+symboltable ok!";
                        //        SymbolTableEntry ste = SymbolTable.GetEntryFromSymbol(symbolName);
                        //        if (ste != null)
                        //        {
                        //            //dbAccess += "+" + ste;
                        //            S7DataBlock block =
                        //                folder.GetBlock(ste.Operand.Replace(" ",""),
                        //                                new S7ConvertingOptions
                        //                                    {Mnemonic = folder.Project.ProjectLanguage})
                        //                as S7DataBlock;
                        //            //dbAccess += "+" + block;
                        //            if (block != null)
                        //            {
                        //                //dbAccess += "+block not null";
                        //                S7DataRow dbRow = block.GetDataRowWithAddress(byteBitAddress);
                        //                if (dbRow != null)
                        //                {
                        //                    if (dbRow.DataType != S7DataRowType.STRING)
                        //                        dbAccess = dbName + "." + dbRow.StructuredName;
                        //                    else if (pointPosition < 0) //if its not using a bit access to access the string variable
                        //                        dbAccess = dbName + "." + 
                        //                            dbRow.StructuredName +
                        //                            "[" + (byteBitAddress.ByteAddress - dbRow.BlockAddress.ByteAddress - 2) + "]";
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    string newAwl = awlCommand.PadRight(6) + dbAccess +";";
                    awl = awl.Replace(sMatch, newAwl);
                }
                //Fix for db variables used in parameters not transformed to symbols
                //TODO: implementation missing

                //Fix for Db access with anypointers
                //TODO: implementation missing
                retVal.AppendLine(awl);
            }
            retVal.Append("END_FUNCTION");

            return retVal.ToString();
        }

        public string ToString(bool WithParameter)
        {
            int bytecnt = 0;
            StringBuilder retVal = new StringBuilder();

            retVal.Append(BlockType.ToString());
            retVal.Append(BlockNumber.ToString());
            retVal.Append(" : ");
            if (Name != null)
                retVal.Append(Name);
            retVal.Append("\r\n\r\n");

            if (Description!=null)
            {
                retVal.Append("Description\r\n\t");
                retVal.Append(Description.Replace("\n", "\r\n\t"));
                retVal.Append("\r\n\r\n");
            }
            if (Parameter != null && WithParameter)
            {
                retVal.Append("Parameter\r\n");
                retVal.Append(Parameter.ToString());
                retVal.Append("\r\n\r\n");
            }

            retVal.Append("AWL-Code\r\n");

            if (AWLCode!=null)
                foreach (var plcFunctionBlockRow in AWLCode)
                {
                    plcFunctionBlockRow.MnemonicLanguage = MnemonicLanguage;
                    //retVal.Append(/* "0x" + */ bytecnt.ToString(/* "X" */).PadLeft(4, '0') + "  :");
                    retVal.Append(plcFunctionBlockRow.ToString());
                    retVal.AppendLine();
                    //bytecnt += plcFunctionBlockRow.ByteSize;
                }
            return retVal.ToString();
        }
    }
}
