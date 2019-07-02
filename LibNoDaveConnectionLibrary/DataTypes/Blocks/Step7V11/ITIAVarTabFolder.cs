using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    public interface ITIAVarTabFolder: IProjectFolder
    {
        List<ITIAVarTab> TagTables { get; }
    }
}
