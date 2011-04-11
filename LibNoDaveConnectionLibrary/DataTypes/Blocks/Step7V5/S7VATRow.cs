using System;
using DotNetSiemensPLCToolBoxLibrary.Communication;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V5
{
    [Serializable()]
    public class S7VATRow
    {
        private PLCTag _libNoDaveValue;
        public PLCTag LibNoDaveValue
        {
            get { return _libNoDaveValue; }
            set { _libNoDaveValue = value; }
        }

        public string Comment { get; set; }

        public override string ToString()
        {
            return LibNoDaveValue.S7FormatAddress;
        }
    }
}
