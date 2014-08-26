using System;

namespace DotNetSiemensPLCToolBoxLibrary.Communication.LibNoDave
{
    public interface IPDU
    {
        IntPtr pointer { get; set; }
        void addBitVarToReadRequest(int area, int DBnum, int start, int bytes);
        void addBitVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer);
        void addDbRead400ToReadRequest(int DBnum, int offset, int byteCount);
        void addSymbolVarToReadRequest(string completeSymbol);
        void addVarToReadRequest(int area, int DBnum, int start, int bytes);
        void addVarToWriteRequest(int area, int DBnum, int start, int bytes, byte[] buffer);
        void daveAddFillByteToReadRequest();
    }
}