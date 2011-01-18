using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step5
{
    [Serializable()]
    public class S5FunctionBlock : S5Block, IFunctionBlock
    {
        public List<S5Parameter> Parameter { get; set; }
        public List<FunctionBlockRow> AWLCode { get; set; }

        public List<Network> Networks { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            int bytecnt = 0;
            StringBuilder retVal = new StringBuilder();

            retVal.Append(BlockType.ToString());
            retVal.Append(BlockNumber.ToString());
            retVal.Append(" : ");
            if (Name != null)
                retVal.Append(Name);
            retVal.Append("\r\n\r\n");

            if (Description != null)
            {
                retVal.Append("Description\r\n\t");
                retVal.Append(Description.Replace("\n", "\r\n\t"));
                retVal.Append("\r\n\r\n");
            }
            
            if (Parameter != null)
            {
            	retVal.Append("Parameter\r\n");
                foreach(S5Parameter par in Parameter)
            	{
                	retVal.Append("\t" + par.ToString() + "\r\n");
                }
            	retVal.Append("\r\n");            	
            }    
            
            retVal.Append("AWL-Code\r\n");

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
    }
}
