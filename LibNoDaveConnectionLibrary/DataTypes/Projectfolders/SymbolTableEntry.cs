using Newtonsoft.Json;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SymbolTableEntry
    {
        [JsonProperty("symbol", Order = 3)]
        public string Symbol { get; set; }

        [JsonProperty("description", Order = 4)]
        public string Comment { get; set; }

        [JsonProperty("address", Order = 1)]
        public string Address { get; set; }

        public string Operand { get; set; }
        public string OperandIEC { get; set; }

        [JsonProperty("datatype", Order = 2)]
        public string DataType { get; set; }

        public MemoryArea DataSource { get; set; }

        public override string ToString()
        {
            return Symbol + " (" + Operand + ")";
        }
    }
}