using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    public enum OperationCode : byte
    {
        Write = 3,
        WriteAnswer = 4,
        Fetch = 5,
        FetchAnswer = 6,
    }
}
