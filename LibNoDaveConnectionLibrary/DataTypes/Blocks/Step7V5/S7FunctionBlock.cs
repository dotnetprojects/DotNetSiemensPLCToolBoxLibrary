using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication;
using DotNetSiemensPLCToolBoxLibrary.DataTypes.AWL.Step7V5;
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
                name = "\"" + SymbolTableEntry.Symbol + "\"";
            }

            if (this.BlockType == PLCBlockType.FC)
                retVal.AppendLine("FUNCTION " + name + " : VOID");
            else if (this.BlockType == PLCBlockType.OB)
                retVal.AppendLine("ORGANIZATION_BLOCK " + name);
            else
                retVal.AppendLine("FUNCTION_BLOCK " + name);

            retVal.Append("TITLE =" + this.Title + Environment.NewLine);

            if (!String.IsNullOrEmpty(this.Description))
                retVal.AppendLine("//" + this.Description.Replace(Environment.NewLine, Environment.NewLine + "//"));
            if (!string.IsNullOrEmpty(this.Author))
                retVal.AppendLine("AUTHOR : " + this.Author);
            if (!string.IsNullOrEmpty(this.Name))
                retVal.AppendLine("NAME : " + this.Name);
            if (!string.IsNullOrEmpty(this.Version))
                retVal.AppendLine("VERSION : " + this.Version);
            retVal.AppendLine();
            retVal.AppendLine();


            if (this.Parameter.Children != null)
            {
                foreach (S7DataRow s7DataRow in this.Parameter.Children)
                {
                    if (s7DataRow.Children.Count > 0)
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
                        string vars = AWLToSource.DataRowToSource(s7DataRow, "  ", ((this.BlockType != PLCBlockType.FB && this.BlockType != PLCBlockType.SFB) || parnm == "TEMP"));
                        if (useSymbols) {
                            foreach (string dependency in Dependencies)
                            {
                                if (dependency.Contains("SFC") || dependency.Contains("SFB"))
                                    continue;
                                try
                                {
                                    string depSymbol = "\"" + SymbolTable.GetEntryFromOperand(dependency).Symbol + "\"";
                                    vars = vars.Replace(dependency, SymbolTable.GetEntryFromOperand(dependency).Symbol);
                                }
                                catch { }
                            }
                        }                        
                        retVal.Append(vars);
                        retVal.AppendLine("END_VAR");
                    }
                }

            }
            retVal.AppendLine("BEGIN");
            foreach (Network network in this.Networks)
            {
                retVal.AppendLine("NETWORK");
                retVal.AppendLine("TITLE = " + network.Name);
                if (!String.IsNullOrEmpty(network.Comment))
                    retVal.AppendLine("//" + network.Comment.Replace(Environment.NewLine, Environment.NewLine + "//"));
                else
                    retVal.AppendLine();
                foreach (S7FunctionBlockRow functionBlockRow in network.AWLCode)
                {
                    string awlCode = functionBlockRow.ToString(useSymbols, true);
                    if (awlCode == "" || awlCode == ";")
                        retVal.AppendLine();
                    else
                    {
                        retVal.AppendLine(awlCode);
                    }
                }
            }

            if (this.BlockType == PLCBlockType.FC)
                retVal.Append("END_FUNCTION");
            else if (this.BlockType == PLCBlockType.OB)
                retVal.AppendLine("END_ORGANIZATION_BLOCK");
            else
                retVal.Append("END_FUNCTION_BLOCK");
            //retVal.Append("END_FUNCTION");

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
                    retVal.Append("\r\n");
                    //bytecnt += plcFunctionBlockRow.ByteSize;
                }
            return retVal.ToString();
        }
    }
}
