using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class S7FunctionBlock : S7Block, IFunctionBlock, INotifyPropertyChanged
    {
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
