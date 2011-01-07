using System;
using System.Collections.Generic;
using System.Text;

namespace LibNoDaveConnectionLibrary.DataTypes.Blocks
{
    public interface IDataBlock
    {
        PLCDataRow Structure { get; set;}

        PLCDataRow GetArrayExpandedStructure(PLCDataBlockExpandOptions myExpOpt);
        PLCDataRow GetArrayExpandedStructure();        
    }
}
