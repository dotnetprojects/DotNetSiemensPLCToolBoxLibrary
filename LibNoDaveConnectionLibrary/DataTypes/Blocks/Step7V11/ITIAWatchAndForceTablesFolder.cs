using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    public interface ITIAWatchAndForceTablesFolder : IProjectFolder
    {
        List<ITIAWatchTable> WatchTables { get; }

        List<ITIAForceTable> ForceTables { get; }
    }
}
