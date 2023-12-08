using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using System;

namespace DotNetSiemensPLCToolBoxLibrary.Communication
{
    public class PLCException : Exception
    {
        private int _ErrorCode;

        public int ErrorCode
        {
            get { return _ErrorCode; }
        }

        public PLCException(int errorCode) :
            base(String.Format("Operation failed due to error from PLC {0}: {1}", errorCode, libnodave.daveStrerror(errorCode)))
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