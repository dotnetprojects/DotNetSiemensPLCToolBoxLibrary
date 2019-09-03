using System;

namespace BackupS7
{
    [Flags]
    public enum BackupType
    {
        UseList = 0,
        Datablocks = 1,
        SystemDataBlocks = 2,
        Functions = 4,
        FunctionBlocks = 8,
        All = 15
    }
}
