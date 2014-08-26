using System;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave
{
    public interface IresultSet
    {
        IntPtr pointer { get; set; }
        int getErrorOfResult(int number);
    }
}