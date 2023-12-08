using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Network
    {
        public Network()
        {
            AWLCode = new List<FunctionBlockRow>();
        }

        [JsonProperty]
        public virtual string Name { get; set; }

        [JsonProperty]
        public string Comment { get; set; }

        [JsonProperty(Order = 10)]
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