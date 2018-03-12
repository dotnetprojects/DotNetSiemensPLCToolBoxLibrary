using DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave;
using System;

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
