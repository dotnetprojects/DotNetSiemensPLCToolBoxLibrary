using Newtonsoft.Json;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [JsonObject(MemberSerialization.OptIn)]
    public class S7SourceBlock : S7Block
    {
        [JsonProperty(Order = 50)]
        public string Text { get; set; }

        [JsonProperty(Order = 49)]
        public string Filename { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}