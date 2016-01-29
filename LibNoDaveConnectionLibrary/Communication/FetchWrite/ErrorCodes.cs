using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.FetchWrite
{
    public enum ErrorCodes
    {
        Ok = 0,
        Error = -1,
        ErrorInvalidParam = -2,
        ErrorConnection = -3,
        ErrorTimeout = -4,
        ErrorCommunication = -5,
        ErrorBuffer = -6,
        ErrorSend = -7,
        ErrorReceive = -8,
    }
}
