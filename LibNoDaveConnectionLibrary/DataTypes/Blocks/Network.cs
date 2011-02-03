using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public abstract class Network
    {
        public Network()
        {
            AWLCode = new List<FunctionBlockRow>();
        }

        public virtual string Name { get; set; }
        public string Comment { get; set; }
        public List<FunctionBlockRow> AWLCode { get; set; }

        public Block Parent { get; set; }

        public string AWLCodeToString()
        {
            StringBuilder retVal = new StringBuilder();
            if (AWLCode != null)
                foreach (var plcFunctionBlockRow in AWLCode)
                {
                    //retVal.Append(/* "0x" + */ bytecnt.ToString(/* "X" */).PadLeft(4, '0') + "  :");
                    retVal.Append(plcFunctionBlockRow.ToString());
                    retVal.Append("\r\n");
                    //bytecnt += plcFunctionBlockRow.ByteSize;
                }
            return retVal.ToString();
        }

        public override string ToString()
        {       
            StringBuilder retVal = new StringBuilder();
            retVal.Append(AWLCodeToString());
            return retVal.ToString();
        }
    }
}
