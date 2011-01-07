using System;
using System.Collections.Generic;
using System.Text;

namespace LibNoDaveConnectionLibrary.DataTypes.Blocks
{
    public class PLCDataBlockExpandOptions
    {
        //public bool ExpandUDTs { get; set; }
        //public bool ExpandFBs { get; set; }
        //public bool ExpandArrays { get; set; }
        public bool ExpandCharArrays { get; set; }

        public PLCDataBlockExpandOptions()
        {
            //ExpandUDTs = true;
            //ExpandFBs = true;
            //ExpandArrays = true;
            ExpandCharArrays = true;
        }
    }
}
