using System;

namespace DotNetSiemensPLCToolBoxLibrary.DataTypes
{
    internal class WPFToolboxForSiemensPLCsException : Exception
    {
        public WPFToolboxForSiemensPLCsExceptionType ExceptionType;

        public WPFToolboxForSiemensPLCsException(WPFToolboxForSiemensPLCsExceptionType ExceptionType)
        {
            this.ExceptionType = ExceptionType;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}