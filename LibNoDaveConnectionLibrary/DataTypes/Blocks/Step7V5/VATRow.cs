using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class VATRow
    {
        public LibNoDaveValue LibNoDaveValue { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return LibNoDaveValue.S7FormatAddress;
        }
    }
}
