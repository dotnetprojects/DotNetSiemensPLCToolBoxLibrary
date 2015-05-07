using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    public enum OrgTypes : byte
    {
        DB = 1,    //DB
        Flags = 2, //M
        Inputs = 3, //I
        Outputs = 4, //O
        AnalogIo = 5, //PB
        Counters = 6, //T
        Timer = 7,  //Z
        SystemData = 8, //BS
        MemoryCells = 9, //AS
        DBx = 10,  //DX
        AnalogIox = 11, //QB
        SpecialFlags = 16, //SM


    }
}
