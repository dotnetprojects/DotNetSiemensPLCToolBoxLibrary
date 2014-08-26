using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolBoxLibUnitTests
{
    public class ResultSetWrapper : IresultSet
    {
        public IntPtr pointer { get; set; }

        public int getErrorOfResult(int number)
        {
            throw new NotImplementedException();
        }
    }
}
