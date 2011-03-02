using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    public enum PLCReadTriggerVarTab
    {
        BeginOfCycle = 0x02,
        EndOfCycle = 0x03,
        TransitFromStartToStop = 0x04,
    }
}
