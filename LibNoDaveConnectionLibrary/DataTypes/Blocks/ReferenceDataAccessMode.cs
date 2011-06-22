using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes.Blocks
{
    public enum ReferenceDataAccessMode
    {
        None = 0,
        Read = 1,
        Write = 2,
        Call = 3,
        Open = 4,
        Create = 5,
    }
}
