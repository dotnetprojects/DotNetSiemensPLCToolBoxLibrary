using Newtonsoft.Json;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class FunctionBlockRow
    {
        internal virtual void resetByte()
        { }

        [JsonProperty(Order = 3)]
        public string Label { get; set; }

        public MnemonicLanguage MnemonicLanguage { get; set; }

        public Block Parent { get; set; }

        private string _command;

        [JsonProperty(Order = -10)]
        public string Command
        {
            get { return _command.Trim().ToUpper(); }
            set
            {
                _command = value.Trim().ToUpper();
                resetByte();
            }
        }

        [JsonProperty(Order = 5)]
        public string Comment { get; set; }
    }
}