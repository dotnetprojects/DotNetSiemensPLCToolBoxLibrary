using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    public interface ITIAWatchTable
    {
        string Name { get; set; }

        string Export();
    }
}
