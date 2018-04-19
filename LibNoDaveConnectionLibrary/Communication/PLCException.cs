using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public class PLCException:Exception
    {
        int _ErrorCode;
        public int ErrorCode
        {
            get { return _ErrorCode; }
        }

        public PLCException (int errorCode): 
            base(String.Format("Operation failed due to error from PLC {0}: {1}",errorCode, libnodave.daveStrerror(errorCode)))
        {
            _ErrorCode = errorCode; 
        }

        public PLCException(string msg, int errorCode) :
            base(msg)
        {
            _ErrorCode = errorCode;
        }
    }
}
