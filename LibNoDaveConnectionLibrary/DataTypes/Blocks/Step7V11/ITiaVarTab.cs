using System.Collections.Generic;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks.Step7V11
{
    public interface ITIAVarTab
    {
        string Name { get; set; }

        List<ITIAConstant> Constants { get; }

        List<ITIATag> Tags { get; }

        string Export();
    }
}
