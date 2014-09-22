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
            
            if (this.BlockType == PLCBlockType.FC)
                retVal.Append("FUNCTION " + this.BlockName + " : VOID" + Environment.NewLine);
            else
                retVal.Append("FUNCTION_BLOCK " + this.BlockName + Environment.NewLine);

            retVal.Append("TITLE =" + this.Title + Environment.NewLine);

            if (!String.IsNullOrEmpty(this.Description))
                retVal.Append("//" + this.Description.Replace(Environment.NewLine, Environment.NewLine + "//") +
                              Environment.NewLine);
            if (!string.IsNullOrEmpty(this.Author))
                retVal.Append("AUTHOR : " + this.Author + Environment.NewLine);
            if (!string.IsNullOrEmpty(this.Name))
                retVal.Append("NAME : " + this.Name + Environment.NewLine);
            if (!string.IsNullOrEmpty(this.Version))
                retVal.Append("VERSION : " + this.Version + Environment.NewLine);
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
                    retVal.Append(ber + Environment.NewLine);
                    retVal.Append(AWLToSource.DataRowToSource(s7DataRow, "  "));
                    retVal.Append("END_VAR" + Environment.NewLine);
                }

            }
            retVal.Append("BEGIN" + Environment.NewLine);
            foreach (Network network in this.Networks)
            {
                retVal.Append("NETWORK" + Environment.NewLine);
                retVal.Append("TITLE = " + network.Name + Environment.NewLine);
                if (!String.IsNullOrEmpty(network.Comment))
                    retVal.Append("//" + network.Comment.Replace(Environment.NewLine, Environment.NewLine + "//") +
                                  Environment.NewLine);
                else
                    retVal.Append(Environment.NewLine);
                foreach (S7FunctionBlockRow functionBlockRow in network.AWLCode)
                {
                    string awlCode = functionBlockRow.ToString(useSymbols, true);
                    if (awlCode == "" || awlCode == ";")
                        retVal.Append(Environment.NewLine);
                    else
                    {
                        retVal.Append(awlCode + Environment.NewLine);
                    }
                }
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
                    retVal.Append("\r\n");
                    //bytecnt += plcFunctionBlockRow.ByteSize;
                }
            return retVal.ToString();
        }
    }
}
