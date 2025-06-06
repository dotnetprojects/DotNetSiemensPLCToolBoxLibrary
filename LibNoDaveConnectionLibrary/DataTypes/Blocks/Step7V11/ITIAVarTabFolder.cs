using DotNetSiemensPLCToolBoxLibrary.DataTypes.Projectfolders;
using DotNetSiemensPLCToolBoxLibrary.Projectfiles.TIA.Openness;
using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    public interface ITIAVarTabFolder: ITIAOpennessProjectFolder
    {
        List<ITIAVarTab> TagTables { get; }
    }
}
