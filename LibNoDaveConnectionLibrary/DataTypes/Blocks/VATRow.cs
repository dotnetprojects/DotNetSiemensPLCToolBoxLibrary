using System;
using System.Collections.Generic;
using System.Text;

namespace LibNoDaveConnectionLibrary.DataTypes.Blocks
{
    [Serializable()]
    public class VATRow
    {
        public LibNoDaveValue LibNoDaveValue { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            return LibNoDaveValue.GetS7FormatAddress();
        }
    }
}
