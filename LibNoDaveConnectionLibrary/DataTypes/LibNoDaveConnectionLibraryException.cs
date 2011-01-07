using System;
using System.Collections.Generic;
using System.Text;

namespace LibNoDaveConnectionLibrary.DataTypes
{
    class LibNoDaveConnectionLibraryException:Exception
    {
        public LibNoDaveConnectionLibraryExceptionType ExceptionType;

        public LibNoDaveConnectionLibraryException(LibNoDaveConnectionLibraryExceptionType ExceptionType)
        {
            this.ExceptionType = ExceptionType;
        }

        public override string ToString()
        {            
            return base.ToString();
        }
    }
}
