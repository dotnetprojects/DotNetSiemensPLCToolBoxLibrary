using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public class PLCException:Exception
    {
        int ErrorCode {get;}

        public PLCException (int errorCode): 
            base(String.Format("Operation failed due to error from PLC {0}: {1}",errorCode, libnodave.daveStrerror(errorCode)))
        {
            ErrorCode = errorCode; 
        }

        public PLCException(string msg, int errorCode) :
            base(msg)
        {
            ErrorCode = errorCode;
        }
    }
}
