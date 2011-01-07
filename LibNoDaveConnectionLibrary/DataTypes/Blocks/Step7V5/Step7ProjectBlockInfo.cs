using System;
using System.Collections.Generic;
using System.Text;
using LibNoDaveConnectionLibrary.DataTypes.Blocks;
using LibNoDaveConnectionLibrary.DataTypes.Step7Project;

namespace LibNoDaveConnectionLibrary.DataTypes
{
    public class Step7ProjectBlockInfo:ProjectBlockInfo
    {
        //internal int id;

        //public BlocksOfflineFolder ParentFolder { get; set; }

        //public bool Deleted { get; set; }
        //public int BlockNumber { get; set; }
        //public PLCBlockType BlockType { get; set; }
        //public string Symbol { get; set; }
        public bool KnowHowProtection { get; set; }

        public override string ToString()
        {
            string retVal = "";
            if (KnowHowProtection)
                retVal += "@";
            retVal += base.ToString();
            return retVal;
        }
    }
}
