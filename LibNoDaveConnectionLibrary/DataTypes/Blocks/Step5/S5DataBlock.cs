using System;
using System.Collections.Generic;
using System.Text;

namespace LibNoDaveConnectionLibrary.DataTypes.Blocks.Step5
{
    public class S5DataBlock: S5Block,IDataBlock
    {
        public PLCDataRow Structure { get; set; }
        public PLCDataRow GetArrayExpandedStructure(PLCDataBlockExpandOptions myExpOpt)
        {
            return Structure;
        }

        public PLCDataRow GetArrayExpandedStructure()
        {
            return Structure;
        }
    }
}
